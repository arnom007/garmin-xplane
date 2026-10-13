// dllmain.cpp — GarminHook V8.0 — IOP Register Probe
//
// CLEAN REWRITE: Abandons message-buffer hacking (V3-V7).
// Instead, uses the Garmin IOP register system — the same path
// that real AHRS (GRS 56) and ADC (GAD 29B) hardware use.
//
// Phase 1: PROBE MODE
//   - Resolves named register tags to numeric IDs via REG_pvg_lookup_tag_by_name
//   - Reads current demo values via reg_read
//   - Logs everything for analysis
//
// Phase 2 (next): Write X-Plane telemetry to these registers via reg_write

#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <stdio.h>
#include <iostream>
#include <fstream>
#include <Windows.h>
#include <Shlobj.h>
#include <ctime>
#include <cmath>
#include <easyhook.h>

#include "garmin_bridge_shm.h"

// ============================================================
// Function pointer types
// ============================================================

typedef unsigned long (__cdecl* reg_read_t)(unsigned long tag_id, void* buf, unsigned long size, unsigned long flags);
typedef unsigned long (__cdecl* reg_write_t)(unsigned long tag_id, void* buf, unsigned long size, unsigned long flags);
typedef unsigned long (__cdecl* reg_lookup_t)(const char* name, unsigned long* out_tag_id);
typedef unsigned long (__cdecl* TSK_pvg_send_msg_ex_t)(unsigned long p1, unsigned long p2);
typedef unsigned long (__cdecl* hsdb_set_sim_lrus_t)(unsigned long port, unsigned long lru_mask);
typedef unsigned long (__cdecl* hsdb_get_lrus_t)(unsigned long port, unsigned long* out_mask);

// EasyHook bypass pointer
PVOID* original_send_msg_ex;

// ============================================================
// Resolved function pointers (loaded at init from krnl.dll)
// ============================================================

static reg_read_t    fn_reg_read = NULL;
static reg_write_t   fn_reg_write = NULL;
static reg_lookup_t  fn_reg_lookup = NULL;
static hsdb_set_sim_lrus_t fn_hsdb_set_sim_lrus = NULL;
static hsdb_get_lrus_t     fn_hsdb_get_lrus = NULL;

// ============================================================
// Shared memory
// ============================================================

HANDLE g_hMapXPToGarmin = NULL;
XPToGarminData* g_pXPData = NULL;
volatile bool g_shmConnected = false;

// ============================================================
// Logging
// ============================================================

std::ofstream g_logFile;

void initLog()
{
    char appDataPath[MAX_PATH];
    DWORD pid = GetCurrentProcessId();
    if (SUCCEEDED(SHGetFolderPathA(NULL, CSIDL_APPDATA, NULL, 0, appDataPath)))
    {
        std::string dir = std::string(appDataPath) + "\\GarminBridge";
        CreateDirectoryA(dir.c_str(), NULL);
        char logName[MAX_PATH];
        sprintf_s(logName, "GarminHook_V8_%lu.log", pid);
        std::string logPath = dir + "\\" + logName;
        g_logFile.open(logPath, std::ios_base::trunc);
    }
}

void hookLog(const char* msg)
{
    if (g_logFile.is_open())
    {
        auto now = std::time(nullptr);
        char timeBuf[32];
        std::strftime(timeBuf, sizeof(timeBuf), "%H:%M:%S", std::localtime(&now));
        g_logFile << "[" << timeBuf << "] " << msg << std::endl;
        g_logFile.flush();
    }
}

void hookLogf(const char* fmt, ...)
{
    char buf[1024];
    va_list args;
    va_start(args, fmt);
    vsnprintf(buf, sizeof(buf), fmt, args);
    va_end(args);
    hookLog(buf);
}

// ============================================================
// Register Tag Database
// ============================================================

struct RegTagEntry {
    const char*   name;
    unsigned long id;
    unsigned long dataSize;
    const char*   description;
};

static RegTagEntry g_regTags[] = {
    // AHRS Attitude
    { "IOP_AHRS_PITCH",           0, 4, "Pitch angle" },
    { "IOP_AHRS_ROLL",            0, 4, "Roll angle" },
    { "IOP_AHRS_M_HDG",           0, 4, "Magnetic heading" },
    { "IOP_AHRS_T_HDG",           0, 4, "True heading" },
    { "IOP_AHRS_BKUP_M_HDG",      0, 4, "Backup magnetic heading" },
    { "IOP_AHRS_BKUP_T_HDG",      0, 4, "Backup true heading" },
    { "IOP_AHRS_MAG_VAR",         0, 4, "Magnetic variation" },
    // AHRS Rates
    { "IOP_AHRS_BODY_HDG_RATE",   0, 4, "Body heading rate" },
    { "IOP_AHRS_PITCH_ATT_RATE",  0, 4, "Pitch attitude rate" },
    { "IOP_AHRS_ROLL_ATT_RATE",   0, 4, "Roll attitude rate" },
    { "IOP_AHRS_PITCH_BDY_RATE",  0, 4, "Pitch body rate" },
    { "IOP_AHRS_ROLL_BDY_RATE",   0, 4, "Roll body rate" },
    { "IOP_AHRS_YAW_BDY_RATE",    0, 4, "Yaw body rate" },
    { "IOP_AHRS_HDG_ATT_RATE",    0, 4, "Heading attitude rate" },
    // AHRS Accelerations
    { "IOP_AHRS_X_ACCEL",         0, 4, "X acceleration" },
    { "IOP_AHRS_Y_ACCEL",         0, 4, "Y acceleration" },
    { "IOP_AHRS_Z_ACCEL",         0, 4, "Z acceleration" },
    { "IOP_AHRS_VERT_ACCEL",      0, 4, "Vertical acceleration" },
    { "IOP_AHRS_CROSS_HDG_ACCEL", 0, 4, "Cross-heading acceleration" },
    // AHRS Vertical Speed
    { "IOP_AHRS_VSPD",            0, 4, "Vertical speed (AHRS)" },
    { "IOP_AHRS_FLTR_VSPD",       0, 4, "Filtered vertical speed" },
    // AHRS ARINC Labels
    { "IOP_AHRS_LBL_324",         0, 4, "ARINC 429 Label 324 (pitch)" },
    { "IOP_AHRS_LBL_325",         0, 4, "ARINC 429 Label 325 (roll)" },
    // AHRS Status
    { "IOP_AHRS_DIS",             0, 4, "AHRS discrete status" },
    { "IOP_AHRS_HPM_DATA",        0, 4, "AHRS HPM data" },
    { "IOP_AHRS_MON_SMPHR",       0, 4, "AHRS monitor semaphore" },
    // ADC Air Data
    { "IOP_ADC_IAS",              0, 4, "Indicated airspeed" },
    { "IOP_ADC_TAS",              0, 4, "True airspeed" },
    { "IOP_ADC_PRESS_ALT",        0, 4, "Pressure altitude" },
    { "IOP_ADC_DNSTY_ALT",        0, 4, "Density altitude" },
    { "IOP_ADC_VSPD",             0, 4, "Vertical speed (ADC)" },
    { "IOP_ADC_STAT_PRESS",       0, 4, "Static pressure" },
    { "IOP_ADC_DIFF_PRESS",       0, 4, "Differential pressure" },
    { "IOP_ADC_SAT",              0, 4, "Static air temperature" },
    { "IOP_ADC_TTL_AT",           0, 4, "Total air temperature" },
    { "IOP_ADC_MACH_NMBR",        0, 4, "Mach number" },
    { "IOP_ADC_MMO_DELTA",        0, 4, "MMO delta" },
    { "IOP_ADC_VMO_DELTA",        0, 4, "VMO delta" },
    // ADC Filtered
    { "IOP_ADC_FLTR_IAS",         0, 4, "Filtered IAS" },
    { "IOP_ADC_FLTR_TAS",         0, 4, "Filtered TAS" },
    { "IOP_ADC_FLTR_VSPD",        0, 4, "Filtered vertical speed" },
    { "IOP_ADC_FLTR_MACH_NMBR",   0, 4, "Filtered Mach" },
    { "IOP_ADC_FLTR_AS_ACCEL",    0, 4, "Filtered airspeed accel" },
    { "IOP_ADC_FLTR_AS_TREND",    0, 4, "Filtered airspeed trend" },
    // ADC Stall/Limits
    { "IOP_ADC_STALL_LSAP_RATIO",   0, 4, "Stall LSAP ratio" },
    { "IOP_ADC_STALL_LSAW_RATIO",   0, 4, "Stall LSAW ratio" },
    { "IOP_ADC_GREEN_CIRCLE_RATIO",  0, 4, "Green circle ratio" },
    { "IOP_ADC_YELLOW_BAND_ASPD",    0, 4, "Yellow band airspeed" },
    { "IOP_ADC_YELLOW_BAND_RATIO",   0, 4, "Yellow band ratio" },
    { "IOP_ADC_YELLOW_BAND_OFFSET",  0, 4, "Yellow band offset" },
    // GPS
    { "IOP_GPS_PVT_MSG",          0, 128, "GPS PVT message" },
    { "IOP_GPS_STATUS",           0, 4, "GPS status" },
    { "IOP_GPS_STATE",            0, 4, "GPS state" },
    { "IOP_GPS_RAIM_RQST",       0, 4, "GPS RAIM request" },
    { "IOP_GPS_RAIM_RSLT",       0, 4, "GPS RAIM result" },
    { "IOP_GPS_SYC_EVNT",        0, 4, "GPS sync event" },
    { "IOP_GPS_PVT_RAIM_PERF",   0, 4, "GPS PVT RAIM performance" },
    // Sentinel
    { NULL, 0, 0, NULL }
};

#define NUM_REG_TAGS (sizeof(g_regTags)/sizeof(g_regTags[0]) - 1)

// ============================================================
// Shared memory init
// ============================================================

bool initSharedMemory()
{
    g_hMapXPToGarmin = OpenFileMappingW(FILE_MAP_READ, FALSE, SHM_NAME_XP_TO_GARMIN);
    if (!g_hMapXPToGarmin) return false;
    g_pXPData = (XPToGarminData*)MapViewOfFile(g_hMapXPToGarmin, FILE_MAP_READ, 0, 0, sizeof(XPToGarminData));
    if (!g_pXPData) { CloseHandle(g_hMapXPToGarmin); g_hMapXPToGarmin = NULL; return false; }
    g_shmConnected = true;
    return true;
}

// ============================================================
// Resolve kernel functions
// ============================================================

#include <Psapi.h>
#pragma comment(lib, "Psapi.lib")

void logAllModules()
{
    HMODULE hMods[256];
    DWORD cbNeeded;
    if (EnumProcessModules(GetCurrentProcess(), hMods, sizeof(hMods), &cbNeeded)) {
        int count = cbNeeded / sizeof(HMODULE);
        hookLogf("Loaded modules (%d):", count);
        for (int i = 0; i < count && i < 256; i++) {
            char modName[MAX_PATH];
            if (GetModuleFileNameA(hMods[i], modName, MAX_PATH)) {
                // Extract just filename
                char* slash = strrchr(modName, '\\');
                char* name = slash ? slash + 1 : modName;
                hookLogf("  [%3d] 0x%08X %s", i, (unsigned long)hMods[i], name);
            }
        }
    }
}

HMODULE findKrnlModule()
{
    // Try direct GetModuleHandle with various case combinations
    const char* names[] = { "krnl.dll", "KRNL.dll", "krnl_sim.dll", "KRNL_SIM.dll", "Krnl_sim.dll", "krnl", "KRNL", NULL };
    for (int i = 0; names[i]; i++) {
        HMODULE h = GetModuleHandleA(names[i]);
        if (h) {
            hookLogf("Found krnl via GetModuleHandle('%s') = 0x%08X", names[i], (unsigned long)h);
            return h;
        }
    }

    // Enumerate all modules and search by name
    HMODULE hMods[512];
    DWORD cbNeeded;
    if (EnumProcessModules(GetCurrentProcess(), hMods, sizeof(hMods), &cbNeeded)) {
        int count = cbNeeded / sizeof(HMODULE);
        for (int i = 0; i < count; i++) {
            char modName[MAX_PATH];
            if (GetModuleFileNameA(hMods[i], modName, MAX_PATH)) {
                char* slash = strrchr(modName, '\\');
                char* name = slash ? slash + 1 : modName;
                // Case-insensitive comparison — check both krnl.dll and krnl_sim.dll
                if (_stricmp(name, "krnl.dll") == 0 || _stricmp(name, "krnl_sim.dll") == 0) {
                    hookLogf("Found krnl via EnumProcessModules: 0x%08X (%s)", (unsigned long)hMods[i], modName);
                    return hMods[i];
                }
            }
        }
    }

    return NULL;
}

bool resolveKernelFunctions()
{
    // Log all modules first for diagnostics
    logAllModules();

    // Try finding krnl.dll with retries (it may load later)
    HMODULE hKrnl = NULL;
    for (int attempt = 0; attempt < 15; attempt++) {
        hKrnl = findKrnlModule();
        if (hKrnl) break;

        if (attempt == 0) {
            hookLog("krnl.dll not loaded yet, waiting...");
        }
        hookLogf("  Retry %d/15...", attempt + 1);
        Sleep(2000);  // Wait 2 seconds between retries

        // Re-log modules on retry 5 and 10
        if (attempt == 4 || attempt == 9) {
            logAllModules();
        }
    }

    // Last resort: try LoadLibrary with full path
    if (!hKrnl) {
        hookLog("Trying LoadLibraryA as last resort...");
        const char* paths[] = {
            "krnl_sim.dll",
            "krnl.dll",
            ".\\krnl_sim.dll",
            ".\\krnl.dll",
            "C:\\ProgramData\\Garmin\\Trainers\\GTI\\Production\\GTI__TXi__3_50\\app-3.50.20\\bin\\krnl_sim.dll",
            "C:\\ProgramData\\Garmin\\Trainers\\GTI\\Production\\GTI__TXi__3_50\\app-3.50.20\\bin\\krnl.dll",
            NULL
        };
        for (int i = 0; paths[i]; i++) {
            hKrnl = LoadLibraryA(paths[i]);
            if (hKrnl) {
                hookLogf("Loaded krnl via LoadLibrary('%s') = 0x%08X", paths[i], (unsigned long)hKrnl);
                break;
            }
        }
    }

    if (!hKrnl) {
        hookLog("ERROR: krnl.dll not found after all attempts!");
        logAllModules();
        return false;
    }

    hookLogf("krnl.dll base: 0x%08X", (unsigned long)hKrnl);

    fn_reg_read  = (reg_read_t)GetProcAddress(hKrnl, "reg_read");
    fn_reg_write = (reg_write_t)GetProcAddress(hKrnl, "reg_write");
    fn_reg_lookup = (reg_lookup_t)GetProcAddress(hKrnl, "REG_pvg_lookup_tag_by_name");
    fn_hsdb_set_sim_lrus = (hsdb_set_sim_lrus_t)GetProcAddress(hKrnl, "HWM_pvg_hsdb_set_sim_lrus_online");
    fn_hsdb_get_lrus = (hsdb_get_lrus_t)GetProcAddress(hKrnl, "HWM_pvg_hsdb_get_lrus_online");

    hookLogf("  reg_read:       0x%08X %s", (unsigned long)fn_reg_read,  fn_reg_read  ? "OK" : "FAIL");
    hookLogf("  reg_write:      0x%08X %s", (unsigned long)fn_reg_write, fn_reg_write ? "OK" : "FAIL");
    hookLogf("  REG_lookup:     0x%08X %s", (unsigned long)fn_reg_lookup, fn_reg_lookup ? "OK" : "FAIL");
    hookLogf("  hsdb_set_sim:   0x%08X %s", (unsigned long)fn_hsdb_set_sim_lrus, fn_hsdb_set_sim_lrus ? "OK" : "FAIL");
    hookLogf("  hsdb_get_lrus:  0x%08X %s", (unsigned long)fn_hsdb_get_lrus, fn_hsdb_get_lrus ? "OK" : "FAIL");

    return (fn_reg_read && fn_reg_write);
}

// ============================================================
// Resolve register tag IDs
// ============================================================

void resolveRegisterTags()
{
    hookLog("========================================");
    hookLog("  REGISTER TAG RESOLUTION");
    hookLog("========================================");

    int resolved = 0, failed = 0;
    for (int i = 0; g_regTags[i].name != NULL; i++)
    {
        unsigned long tagId = 0;
        if (fn_reg_lookup)
        {
            __try {
                unsigned long result = fn_reg_lookup(g_regTags[i].name, &tagId);
                if (result == 0 && tagId != 0) {
                    g_regTags[i].id = tagId;
                    resolved++;
                    hookLogf("  [OK]   %-35s -> ID %lu (0x%08X)", g_regTags[i].name, tagId, tagId);
                } else {
                    failed++;
                    hookLogf("  [FAIL] %-35s -> ret=%lu tagId=%lu", g_regTags[i].name, result, tagId);
                }
            } __except(EXCEPTION_EXECUTE_HANDLER) {
                failed++;
                hookLogf("  [CRASH] %-35s -> exception!", g_regTags[i].name);
            }
        } else {
            failed++;
            hookLogf("  [SKIP] %-35s -> no lookup function", g_regTags[i].name);
        }
    }
    hookLogf("Resolution: %d OK, %d FAILED / %d total", resolved, failed, (int)NUM_REG_TAGS);
}

// ============================================================
// Read and log current register values
// ============================================================

void readAndLogRegisterValues()
{
    hookLog("========================================");
    hookLog("  CURRENT REGISTER VALUES (Demo Mode)");
    hookLog("========================================");

    for (int i = 0; g_regTags[i].name != NULL; i++)
    {
        if (g_regTags[i].id == 0) continue;
        unsigned char buf[256] = {0};
        unsigned long readSize = g_regTags[i].dataSize;
        if (readSize > sizeof(buf)) readSize = sizeof(buf);

        __try {
            unsigned long result = fn_reg_read(g_regTags[i].id, buf, readSize, 0);
            if (readSize == 4) {
                float fval = *(float*)buf;
                unsigned long ival = *(unsigned long*)buf;
                hookLogf("  %-35s = Float(%.6f)  Hex(0x%08X)  ret=%lu",
                    g_regTags[i].name, fval, ival, result);
            } else if (readSize == 8) {
                double dval = *(double*)buf;
                hookLogf("  %-35s = Double(%.10f)  ret=%lu",
                    g_regTags[i].name, dval, result);
            } else {
                char hexStr[256] = {0};
                int n = 0;
                for (unsigned long b = 0; b < readSize && b < 32; b++)
                    n += snprintf(hexStr + n, sizeof(hexStr) - n, "%02X ", buf[b]);
                hookLogf("  %-35s = [%s]  ret=%lu", g_regTags[i].name, hexStr, result);
            }
        } __except(EXCEPTION_EXECUTE_HANDLER) {
            hookLogf("  %-35s = EXCEPTION!", g_regTags[i].name);
        }
    }
}

// ============================================================
// Log HSDB LRU online status
// ============================================================

void logHsdbLruStatus()
{
    hookLog("========================================");
    hookLog("  HSDB LRU ONLINE STATUS");
    hookLog("========================================");

    if (!fn_hsdb_get_lrus) { hookLog("  function not available"); return; }

    for (unsigned long port = 0; port < 4; port++)
    {
        unsigned long mask = 0;
        __try {
            unsigned long result = fn_hsdb_get_lrus(port, &mask);
            hookLogf("  HSDB Port %lu: LRU mask = 0x%08X (ret=%lu)", port, mask, result);
            if (mask != 0) {
                for (int bit = 0; bit < 32; bit++)
                    if (mask & (1 << bit))
                        hookLogf("    -> LRU bit %d ONLINE", bit);
            }
        } __except(EXCEPTION_EXECUTE_HANDLER) {
            hookLogf("  HSDB Port %lu: EXCEPTION!", port);
        }
    }
}

// ============================================================
// Minimal hook for stats
// ============================================================

volatile long g_msgCount = 0;
volatile long g_lastLogTime = 0;

unsigned long __cdecl my_send_msg_ex(unsigned long p1, unsigned long p2)
{
    InterlockedIncrement(&g_msgCount);

    DWORD now = GetTickCount();
    DWORD last = (DWORD)g_lastLogTime;
    if (now - last > 10000) {
        if (InterlockedCompareExchange(&g_lastLogTime, now, last) == (long)last) {
            long count = InterlockedExchange(&g_msgCount, 0);
            if (!g_shmConnected) initSharedMemory();
            hookLogf("STATS: msgs/10s=%ld SHM=%s", count, g_shmConnected ? "YES" : "NO");

            if (g_shmConnected && g_pXPData && g_pXPData->magic == MAGIC_XP_TO_GARMIN) {
                hookLogf("  XP-DATA: hdg=%.1f pitch=%.1f roll=%.1f alt=%.0f ias=%.0f seq=%u",
                    g_pXPData->true_heading_deg, g_pXPData->pitch_deg, g_pXPData->roll_deg,
                    g_pXPData->altitude_msl_m, g_pXPData->ias_kts, g_pXPData->sequence);
            }

            // Read key registers for comparison
            if (fn_reg_read) {
                float pitch=0, roll=0, hdg=0, ias=0, alt=0;
                for (int i = 0; g_regTags[i].name != NULL; i++) {
                    if (g_regTags[i].id == 0) continue;
                    float val = 0;
                    __try { fn_reg_read(g_regTags[i].id, &val, 4, 0); } __except(1) {}
                    if      (strcmp(g_regTags[i].name, "IOP_AHRS_PITCH") == 0) pitch = val;
                    else if (strcmp(g_regTags[i].name, "IOP_AHRS_ROLL") == 0) roll = val;
                    else if (strcmp(g_regTags[i].name, "IOP_AHRS_M_HDG") == 0) hdg = val;
                    else if (strcmp(g_regTags[i].name, "IOP_ADC_IAS") == 0) ias = val;
                    else if (strcmp(g_regTags[i].name, "IOP_ADC_PRESS_ALT") == 0) alt = val;
                }
                hookLogf("  GARMIN-REGS: hdg=%.1f pitch=%.2f roll=%.2f ias=%.1f alt=%.0f",
                    hdg, pitch, roll, ias, alt);
            }
        }
    }

    TSK_pvg_send_msg_ex_t original = (TSK_pvg_send_msg_ex_t)*original_send_msg_ex;
    return original(p1, p2);
}

// ============================================================
// DLL Entry Point
// ============================================================

DWORD WINAPI probeThread(LPVOID param)
{
    // Wait for the kernel to fully initialize before probing registers
    hookLog("Probe thread started. Waiting for kernel to initialize...");
    
    // Step 1: Wait for registry system to be ready (retry register lookup)
    int maxAttempts = 30;  // up to 150 seconds (5s x 30)
    bool resolved = false;
    
    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
        Sleep(5000);  // Wait 5 seconds between attempts
        
        // Try resolving one test tag
        unsigned long testTagId = 0;
        unsigned long result = 0;
        
        if (fn_reg_lookup) {
            __try {
                result = fn_reg_lookup("IOP_AHRS_PITCH", &testTagId);
            } __except(EXCEPTION_EXECUTE_HANDLER) {
                hookLogf("  [%d/%d] Exception in REG_lookup", attempt+1, maxAttempts);
                continue;
            }
        }
        
        hookLogf("  [%d/%d] REG_lookup('IOP_AHRS_PITCH') -> ret=%lu tagId=%lu", 
            attempt+1, maxAttempts, result, testTagId);
        
        if (result == 0 && testTagId != 0) {
            hookLog("  Registry system is READY!");
            resolved = true;
            break;
        }
        
        // Log module count to see if more DLLs are loading
        HMODULE hMods[256];
        DWORD cbNeeded;
        if (EnumProcessModules(GetCurrentProcess(), hMods, sizeof(hMods), &cbNeeded)) {
            hookLogf("  Modules loaded: %d", cbNeeded / sizeof(HMODULE));
        }
    }
    
    if (!resolved) {
        hookLog("Registry system did NOT initialize after max attempts.");
        hookLog("Trying with lowercase names...");
        
        // Try lowercase variant
        if (fn_reg_lookup) {
            unsigned long tagId = 0;
            const char* testNames[] = {
                "iop_ahrs_pitch", "IOP_AHRS_PITCH",
                "iop_ahrs_pitch_1_3", "IOP_AHRS_PITCH_1_3",
                "ahrs_pitch", "AHRS_PITCH",
                "reg_tag_iop_ahrs_pitch", "REG_TAG_IOP_AHRS_PITCH",
                NULL
            };
            for (int i = 0; testNames[i]; i++) {
                unsigned long r = 0;
                __try { r = fn_reg_lookup(testNames[i], &tagId); } __except(1) {}
                hookLogf("  '%s' -> ret=%lu tagId=%lu", testNames[i], r, tagId);
                if (r == 0 && tagId != 0) {
                    hookLogf("  FOUND working name format: '%s' = %lu", testNames[i], tagId);
                    break;
                }
            }
        }
        
        // Also try brute-force reading some tag IDs directly
        hookLog("Brute-force reading tag IDs 1..200:");
        if (fn_reg_read) {
            for (unsigned long id = 1; id <= 200; id++) {
                float val = 0;
                unsigned long r = 0;
                __try { r = fn_reg_read(id, &val, 4, 0); } __except(1) { continue; }
                if (r == 0 && val != 0.0f) {
                    hookLogf("  tag[%lu] = %.6f", id, val);
                }
            }
        }
        return 1;
    }
    
    // Step 2: Resolve all register tags
    resolveRegisterTags();
    
    // Step 3: Read current demo values
    readAndLogRegisterValues();
    
    // Step 4: Log HSDB LRU status
    logHsdbLruStatus();
    
    hookLog("========================================");
    hookLog("V8 Probe COMPLETE. Register data logged.");
    hookLog("========================================");
    
    return 0;
}

extern "C" __declspec(dllexport) void __stdcall NativeInjectionEntryPoint(REMOTE_ENTRY_INFO* inRemoteInfo)
{
    initLog();
    hookLog("========================================");
    hookLog("GarminHook V8.1 — IOP Register Probe");
    hookLog("  (NO hooks — probe only)");
    hookLog("========================================");
    hookLogf("Process ID: %lu", GetCurrentProcessId());

    // Step 1: Resolve kernel functions (with retries for delayed loading)
    if (!resolveKernelFunctions()) {
        hookLog("FATAL: Could not resolve kernel functions.");
        return;
    }

    // Step 2: Connect shared memory
    if (initSharedMemory()) hookLog("SHM connected");
    else hookLog("SHM not available (will retry)");

    // Step 3: Spawn background thread for register probing
    // (the registry system needs time to initialize after boot)
    hookLog("Spawning probe thread (will retry register resolution)...");
    HANDLE hThread = CreateThread(NULL, 0, probeThread, NULL, 0, NULL);
    if (hThread) {
        CloseHandle(hThread);
        hookLog("Probe thread started. Results will appear below.");
    } else {
        hookLog("ERROR: Failed to create probe thread!");
    }

    hookLog("Entry point complete. Returning control to trainer.");
}

