@echo off
cls
Tools\FAKE\FAKE.exe build.fsx target=Test %*
echo %time% %date%
pause