@echo off
cls
Tools\FAKE\FAKE.exe build.fsx %*
echo %time% %date%
pause