@echo off
setlocal
set MSVC_ROOT=C:\Program Files\Microsoft Visual Studio\18\Community\VC\Tools\MSVC\14.50.35717
set WINSDK_VER=10.0.26100.0
set WINSDK_ROOT=C:\Program Files (x86)\Windows Kits\10
set EASYHOOK_PKG=C:\Users\Arnom\Desktop\X-Plane 11.5 CSAV\Aircraft\CSAV\emb312M\garminDll\GarminHook\packages\EasyHookNativePackage.2.7.7097
set CL_EXE="%MSVC_ROOT%\bin\Hostx64\x86\cl.exe"
set LINK_EXE="%MSVC_ROOT%\bin\Hostx64\x86\link.exe"
set INC=/I"%MSVC_ROOT%\include" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\ucrt" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\um" /I"%WINSDK_ROOT%\Include\%WINSDK_VER%\shared" /I"%EASYHOOK_PKG%\build\native\include"
set LIBP=/LIBPATH:"%MSVC_ROOT%\lib\x86" /LIBPATH:"%WINSDK_ROOT%\Lib\%WINSDK_VER%\ucrt\x86" /LIBPATH:"%WINSDK_ROOT%\Lib\%WINSDK_VER%\um\x86" /LIBPATH:"%EASYHOOK_PKG%\build\native\lib\Win32\v141\Release"
set OUT=%~dp0build
if not exist "%OUT%" mkdir "%OUT%"
pushd "%~dp0"

%CL_EXE% /nologo /W3 /EHsc /MT /O2 /D WIN32 /D NDEBUG /D _CRT_SECURE_NO_WARNINGS %INC% /c sniffer.cpp /Fo"%OUT%\sniffer.obj" || goto :err
%LINK_EXE% /nologo /DLL /OUT:"%OUT%\sniffer.dll" "%OUT%\sniffer.obj" %LIBP% EasyHook32.lib ws2_32.lib shell32.lib kernel32.lib user32.lib advapi32.lib || goto :err

%CL_EXE% /nologo /W3 /EHsc /MT /O2 /D WIN32 /D NDEBUG /D _CRT_SECURE_NO_WARNINGS %INC% /c inject.cpp /Fo"%OUT%\inject.obj" || goto :err
%LINK_EXE% /nologo /OUT:"%OUT%\inject.exe" "%OUT%\inject.obj" %LIBP% EasyHook32.lib kernel32.lib user32.lib advapi32.lib /SUBSYSTEM:CONSOLE || goto :err

copy /Y "C:\Users\Arnom\Documents\projeto garminbridge\GarminHook_v2\build\EasyHook32.dll" "%OUT%\" >nul
popd
echo BUILD OK
exit /b 0
:err
popd
echo BUILD FAILED
exit /b 1
