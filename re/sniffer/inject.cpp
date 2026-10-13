// inject.cpp - inject <dll> into all processes whose exe name contains <name>
// Usage: inject.exe <process-substring> <dll-path>
#include <windows.h>
#include <tlhelp32.h>
#include <stdio.h>
#include <wchar.h>
#include "easyhook.h"

int wmain(int argc, wchar_t** argv)
{
    if (argc < 3) { wprintf(L"usage: inject <process-substring> <dll-path>\n"); return 2; }
    wchar_t full[MAX_PATH];
    GetFullPathNameW(argv[2], MAX_PATH, full, NULL);
    if (GetFileAttributesW(full) == INVALID_FILE_ATTRIBUTES) { wprintf(L"dll not found: %s\n", full); return 2; }

    wchar_t want[260]; wcsncpy_s(want, argv[1], _TRUNCATE); _wcslwr_s(want);
    HANDLE snap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    PROCESSENTRY32W pe = { sizeof(pe) };
    int ok = 0, found = 0;
    for (BOOL b = Process32FirstW(snap, &pe); b; b = Process32NextW(snap, &pe)) {
        wchar_t n[260]; wcsncpy_s(n, pe.szExeFile, _TRUNCATE); _wcslwr_s(n);
        if (!wcsstr(n, want)) continue;
        found++;
        NTSTATUS st = RhInjectLibrary(pe.th32ProcessID, 0, EASYHOOK_INJECT_DEFAULT, full, NULL, NULL, 0);
        if (st == 0) { ok++; wprintf(L"OK   %s pid=%lu\n", pe.szExeFile, pe.th32ProcessID); }
        else wprintf(L"FAIL %s pid=%lu st=0x%08X %s\n", pe.szExeFile, pe.th32ProcessID, st, RtlGetLastErrorString());
    }
    CloseHandle(snap);
    wprintf(L"%d/%d injected\n", ok, found);
    return ok ? 0 : 1;
}
