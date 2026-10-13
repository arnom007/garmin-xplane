@echo off
REM ============================================================
REM  Build script for GarminHook v2 + DllInjector v2
REM  Requires: Visual Studio with C++ tools
REM ============================================================

echo ============================================
echo   GarminBridge Build Script
echo   GarminHook v2 + DllInjector v2
echo ============================================
echo.

REM --- Paths ---
set MSVC_VER=14.50.35717
set MSVC_ROOT=C:\Program Files\Microsoft Visual Studio\18\Community\VC\Tools\MSVC\%MSVC_VER%
set WINSDK_VER=10.0.26100.0
set WINSDK_ROOT=C:\Program Files (x86)\Windows Kits\10

REM EasyHook from existing NuGet packages
set EASYHOOK_PKG=C:\Users\Arnom\Desktop\X-Plane 11.5 CSAV\Aircraft\CSAV\emb312M\garminDll\GarminHook\packages\EasyHookNativePackage.2.7.7097
set EASYHOOK_INCLUDE=%EASYHOOK_PKG%\build\native\include
set EASYHOOK_LIB=%EASYHOOK_PKG%\build\native\lib\Win32\v141\Release

REM EasyHook runtime DLLs
set EASYHOOK_REDIST=C:\Users\Arnom\Desktop\X-Plane 11.5 CSAV\Aircraft\CSAV\emb312M\garminDll\GarminHook\packages\EasyHookNativePackage.redist.2.7.7097

REM Compiler and linker
set CL_EXE="%MSVC_ROOT%\bin\Hostx64\x86\cl.exe"
set LINK_EXE="%MSVC_ROOT%\bin\Hostx64\x86\link.exe"

REM Include paths
set INCLUDES=/I"%MSVC_ROOT%\include" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\ucrt" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\um" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\shared" /I"%EASYHOOK_INCLUDE%" /I"."

REM Lib paths
set LIBPATHS=/LIBPATH:"%MSVC_ROOT%\lib\x86" /LIBPATH:"%WINSDK_ROOT%\Lib\%WINSDK_VER%\ucrt\x86" /LIBPATH:"%WINSDK_ROOT%\Lib\%WINSDK_VER%\um\x86" /LIBPATH:"%EASYHOOK_LIB%"

REM Output directory
set OUT_DIR=build
if not exist %OUT_DIR% mkdir %OUT_DIR%

echo --- Compiling GarminHook.dll (32-bit) ---
echo.

%CL_EXE% /nologo /W3 /EHa /MD /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_USRDLL" /D "_CRT_SECURE_NO_WARNINGS" %INCLUDES% /c dllmain.cpp /Fo%OUT_DIR%\dllmain.obj

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Compilation of dllmain.cpp failed!
    pause
    exit /b 1
)

%LINK_EXE% /nologo /DLL /OUT:%OUT_DIR%\GarminHook.dll %OUT_DIR%\dllmain.obj %LIBPATHS% EasyHook32.lib ws2_32.lib shell32.lib kernel32.lib user32.lib advapi32.lib

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Linking GarminHook.dll failed!
    pause
    exit /b 1
)

echo.
echo --- GarminHook.dll compiled successfully! ---
echo.

echo --- Compiling DllInjector.exe (32-bit) ---
echo.

%CL_EXE% /nologo /W3 /EHsc /MD /O2 /D "WIN32" /D "NDEBUG" /D "_CONSOLE" /D "_CRT_SECURE_NO_WARNINGS" %INCLUDES% /c DllInjector.cpp /Fo%OUT_DIR%\DllInjector.obj

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Compilation of DllInjector.cpp failed!
    pause
    exit /b 1
)

%LINK_EXE% /nologo /OUT:%OUT_DIR%\DllInjector.exe %OUT_DIR%\DllInjector.obj %LIBPATHS% EasyHook32.lib kernel32.lib user32.lib advapi32.lib /SUBSYSTEM:CONSOLE

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Linking DllInjector.exe failed!
    pause
    exit /b 1
)

echo.
echo --- DllInjector.exe compiled successfully! ---
echo.

REM Copy EasyHook runtime DLLs
echo --- Copying EasyHook runtime ---
for %%f in ("%EASYHOOK_REDIST%\build\native\bin\Win32\*.dll") do (
    copy /Y "%%f" %OUT_DIR%\ >nul 2>&1
)
REM Also try the Release folder for EasyHook32.dll
copy /Y "C:\Users\Arnom\Desktop\X-Plane 11.5 CSAV\Aircraft\CSAV\emb312M\garminDll\DllInjector\Release\EasyHook32.dll" %OUT_DIR%\ >nul 2>&1

echo.
echo ============================================
echo   BUILD COMPLETE!
echo   Output: %CD%\%OUT_DIR%\
echo     - GarminHook.dll
echo     - DllInjector.exe
echo     - EasyHook32.dll
echo ============================================
echo.
REM pause
