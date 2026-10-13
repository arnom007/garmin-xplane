// sniffer.cpp - Passive Winsock sniffer for Garmin trainer ROAR traffic.
// Hooks ws2_32!send/sendto/recv (stdcall, well-known signatures) and records
// every datagram to %APPDATA%\GarminBridge\cap_<pid>.bin for offline analysis.
// It does NOT modify any data.

#define WIN32_LEAN_AND_MEAN
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>
#include <shlobj.h>
#include <stdio.h>
#include <share.h>
#include <stdint.h>
#include "easyhook.h"

#pragma comment(lib, "ws2_32.lib")

static CRITICAL_SECTION g_cs;
static FILE* g_cap = NULL;
static FILE* g_log = NULL;
static uint64_t g_bytesWritten = 0;
static const uint64_t MAX_CAP_BYTES = 300ull * 1024 * 1024;
static DWORD g_t0 = 0;

static HOOK_TRACE_INFO g_hSend = { NULL }, g_hSendTo = { NULL }, g_hRecv = { NULL };

#pragma pack(push, 1)
struct CapRec {
    uint32_t t_ms;
    uint8_t  dir;      // 0 = send/sendto, 1 = recv
    uint8_t  api;      // 0 = send, 1 = sendto, 2 = recv
    uint16_t lport;
    uint16_t rport;
    uint16_t rsv;
    uint32_t sock;
    uint32_t len;
};
#pragma pack(pop)

static void logf(const char* fmt, ...)
{
    if (!g_log) return;
    va_list ap; va_start(ap, fmt);
    fprintf(g_log, "[%8lu] ", GetTickCount() - g_t0);
    vfprintf(g_log, fmt, ap);
    fputc('\n', g_log);
    fflush(g_log);
    va_end(ap);
}

static void getPorts(SOCKET s, const sockaddr* to, uint16_t* lp, uint16_t* rp)
{
    sockaddr_in a; int al = sizeof(a);
    *lp = 0; *rp = 0;
    if (getsockname(s, (sockaddr*)&a, &al) == 0 && a.sin_family == AF_INET) *lp = ntohs(a.sin_port);
    if (to && ((const sockaddr_in*)to)->sin_family == AF_INET) {
        *rp = ntohs(((const sockaddr_in*)to)->sin_port);
    } else {
        al = sizeof(a);
        if (getpeername(s, (sockaddr*)&a, &al) == 0 && a.sin_family == AF_INET) *rp = ntohs(a.sin_port);
    }
}

static void record(uint8_t dir, uint8_t api, SOCKET s, const sockaddr* to, const char* buf, int len)
{
    if (len <= 0 || !buf) return;
    CapRec r;
    r.t_ms = GetTickCount() - g_t0;
    r.dir = dir; r.api = api; r.rsv = 0;
    r.sock = (uint32_t)s;
    r.len = (uint32_t)len;
    getPorts(s, to, &r.lport, &r.rport);

    EnterCriticalSection(&g_cs);
    if (g_cap && g_bytesWritten < MAX_CAP_BYTES) {
        fwrite(&r, sizeof(r), 1, g_cap);
        fwrite(buf, 1, len, g_cap);
        g_bytesWritten += sizeof(r) + len;
        static unsigned n = 0;
        if ((++n % 200) == 0) fflush(g_cap);
    }
    LeaveCriticalSection(&g_cs);
}

// ---- Hook handlers. Calling the API inside the handler goes to the original
// ---- function (EasyHook thread barrier prevents re-entry).
int WSAAPI my_send(SOCKET s, const char* buf, int len, int flags)
{
    int r = send(s, buf, len, flags);
    if (r > 0) record(0, 0, s, NULL, buf, r);
    return r;
}

int WSAAPI my_sendto(SOCKET s, const char* buf, int len, int flags, const sockaddr* to, int tolen)
{
    int r = sendto(s, buf, len, flags, to, tolen);
    if (r > 0) record(0, 1, s, to, buf, r);
    return r;
}

int WSAAPI my_recv(SOCKET s, char* buf, int len, int flags)
{
    int r = recv(s, buf, len, flags);
    if (r > 0) record(1, 2, s, NULL, buf, r);
    return r;
}

static bool installHook(const char* name, void* handler, HOOK_TRACE_INFO* hi)
{
    HMODULE ws = GetModuleHandleA("ws2_32.dll");
    void* target = ws ? (void*)GetProcAddress(ws, name) : NULL;
    if (!target) { logf("hook %s: target not found", name); return false; }
    NTSTATUS st = LhInstallHook(target, handler, NULL, hi);
    if (FAILED(st)) { logf("hook %s: LhInstallHook failed 0x%08X (%S)", name, st, RtlGetLastErrorString()); return false; }
    ULONG acl[1] = { 0 };
    LhSetExclusiveACL(acl, 0, hi);   // exclusive of nobody => active on all threads
    logf("hook %s: OK @ %p", name, target);
    return true;
}

static DWORD WINAPI flusher(LPVOID)
{
    for (;;) {
        Sleep(2000);
        EnterCriticalSection(&g_cs);
        if (g_cap) fflush(g_cap);
        uint64_t b = g_bytesWritten;
        LeaveCriticalSection(&g_cs);
        logf("captured bytes: %llu", b);
    }
}

extern "C" __declspec(dllexport) void __stdcall NativeInjectionEntryPoint(REMOTE_ENTRY_INFO* info)
{
    g_t0 = GetTickCount();
    InitializeCriticalSection(&g_cs);

    char dir[MAX_PATH];
    SHGetFolderPathA(NULL, CSIDL_APPDATA, NULL, 0, dir);
    strcat_s(dir, "\\GarminBridge");
    CreateDirectoryA(dir, NULL);

    char path[MAX_PATH];
    DWORD pid = GetCurrentProcessId();
    sprintf_s(path, "%s\\sniff_%lu.log", dir, pid);
    g_log = _fsopen(path, "w", _SH_DENYNO);
    sprintf_s(path, "%s\\cap_%lu.bin", dir, pid);
    g_cap = _fsopen(path, "wb", _SH_DENYNO);

    char exe[MAX_PATH]; GetModuleFileNameA(NULL, exe, MAX_PATH);
    logf("Garmin sniffer attached. pid=%lu exe=%s", pid, exe);
    logf("capture file: %s", path);

    installHook("send", (void*)my_send, &g_hSend);
    installHook("sendto", (void*)my_sendto, &g_hSendTo);
    installHook("recv", (void*)my_recv, &g_hRecv);

    HANDLE h = CreateThread(NULL, 0, flusher, NULL, 0, NULL);
    if (h) CloseHandle(h);
    logf("entry point done");
}

BOOL APIENTRY DllMain(HMODULE, DWORD, LPVOID) { return TRUE; }
