// DllInjector_v2.cpp — Multi-process DLL Injector for GarminBridge
// 
// Usage:
//   DllInjector.exe                    - Auto-detect and inject into all Garmin processes
//   DllInjector.exe gdu_7xx_simulator  - Inject into GDU TXi only
//   DllInjector.exe gtn_simulator      - Inject into GTN Xi only

#include <iostream>
#include <string>
#include <cstring>
#include <vector>
#include "tchar.h"
#include "easyhook.h"
#pragma comment(lib, "EasyHook32.lib")
#include <tlhelp32.h>
#include <Windows.h>

using namespace std;

struct ProcessInfo {
    DWORD pid;
    wstring name;
};

vector<ProcessInfo> FindProcesses(const wchar_t* processName)
{
    vector<ProcessInfo> results;
    PROCESSENTRY32W pt;
    HANDLE hsnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    pt.dwSize = sizeof(PROCESSENTRY32W);

    if (Process32FirstW(hsnap, &pt)) {
        do {
            wstring exeName(pt.szExeFile);
            wstring searchName(processName);

            // Case-insensitive comparison
            wstring exeLower = exeName;
            wstring searchLower = searchName;
            for (auto& c : exeLower) c = towlower(c);
            for (auto& c : searchLower) c = towlower(c);

            if (exeLower.find(searchLower) != wstring::npos) {
                ProcessInfo pi;
                pi.pid = pt.th32ProcessID;
                pi.name = exeName;
                results.push_back(pi);
            }
        } while (Process32NextW(hsnap, &pt));
    }
    CloseHandle(hsnap);
    return results;
}

bool InjectDll(DWORD processId, const wchar_t* dllPath)
{
    wprintf(L"\nInjecting into PID %lu...\n", processId);
    wprintf(L"  DLL: %s\n", dllPath);

    NTSTATUS nt = RhInjectLibrary(
        processId,      // Target process
        0,              // ThreadId
        EASYHOOK_INJECT_DEFAULT,
        (WCHAR*)dllPath,  // 32-bit DLL path (cast needed for EasyHook API)
        NULL,           // 64-bit (not used)
        NULL,           // User data
        0               // User data size
    );

    if (nt != 0)
    {
        wprintf(L"  FAILED! Error code: %d\n", nt);
        PWCHAR err = RtlGetLastErrorString();
        wcout << L"  Error: " << err << endl;
        return false;
    }
    else
    {
        wprintf(L"  SUCCESS! Library injected.\n");
        return true;
    }
}

int wmain(int argc, wchar_t* argv[])
{
    wprintf(L"========================================\n");
    wprintf(L"  GarminBridge DLL Injector v2.0\n");
    wprintf(L"  T27 EMB-312 Integration\n");
    wprintf(L"========================================\n\n");

    // Determine DLL path (same directory as injector)
    wchar_t exePath[MAX_PATH];
    GetModuleFileNameW(NULL, exePath, MAX_PATH);
    wstring exeDir(exePath);
    exeDir = exeDir.substr(0, exeDir.find_last_of(L"\\/") + 1);
    wstring dllPath = exeDir + L"GarminHook.dll";

    // Check DLL exists
    if (GetFileAttributesW(dllPath.c_str()) == INVALID_FILE_ATTRIBUTES)
    {
        wprintf(L"ERROR: GarminHook.dll not found at:\n  %s\n", dllPath.c_str());
        wprintf(L"\nPlace GarminHook.dll in the same folder as this exe.\n");
        return 1;
    }

    // Determine target processes
    vector<wstring> targets;

    if (argc > 1) {
        targets.push_back(wstring(argv[1]));
    }
    else {
        // Auto-detect all Garmin trainer processes
        targets.push_back(L"gdu_7xx_simulator");
        targets.push_back(L"gtn_simulator");
    }

    int successCount = 0;
    int totalFound = 0;

    for (const auto& target : targets)
    {
        wprintf(L"Searching for: %s...\n", target.c_str());
        auto procs = FindProcesses(target.c_str());

        if (procs.empty())
        {
            wprintf(L"  Not found.\n");
            continue;
        }

        for (const auto& proc : procs)
        {
            totalFound++;
            wprintf(L"  Found: %s (PID: %lu)\n", proc.name.c_str(), proc.pid);

            if (InjectDll(proc.pid, dllPath.c_str()))
            {
                successCount++;
            }
        }
    }

    wprintf(L"\n========================================\n");
    wprintf(L"Results: %d/%d processes injected successfully.\n", successCount, totalFound);
    wprintf(L"========================================\n");

    if (totalFound == 0)
    {
        wprintf(L"\nNo Garmin Trainer processes found!\n");
        wprintf(L"Make sure the Garmin Aviation Trainer is running.\n");
        wprintf(L"  - GDU TXi: gdu_7xx_simulator.exe\n");
        wprintf(L"  - GTN Xi:  gtn_simulator.exe\n");
    }

    return (successCount > 0) ? 0 : 1;
}
