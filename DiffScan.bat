@echo off
title Differential Scanner - Finding ALT/HDG
echo ============================================
echo   Auto Differential Scanner
echo   Runs ~5 min unattended
echo   Watch the PFD when it says "WATCH"
echo ============================================
echo.
cd /d "%~dp0"
python diff_scan_auto.py
pause
