@echo off
setlocal
cd /d "%~dp0"
echo ============================================================
echo Ping-Pong_Game - Reparacion Unity 6000.3.16f1
echo ============================================================
echo.
echo IMPORTANTE: cierra Unity antes de continuar.
echo Este script elimina SOLO carpetas regenerables de Unity:
echo Library, Temp y Obj.
echo.
pause
if exist Library rmdir /s /q Library
if exist Temp rmdir /s /q Temp
if exist Obj rmdir /s /q Obj
echo.
echo Limpieza terminada. Abre este proyecto otra vez desde Unity Hub
echo con Unity 6000.3.16f1 y espera a que termine la importacion.
echo.
pause
