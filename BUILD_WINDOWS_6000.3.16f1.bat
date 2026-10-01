@echo off
setlocal
set UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe
if not exist "%UNITY%" (
  echo [ERROR] No se encontro Unity 6000.3.16f1 en:
  echo %UNITY%
  echo Instala esa version desde Unity Hub y vuelve a ejecutar este archivo.
  pause
  exit /b 1
)

echo Construyendo Ping-Pong_Game para Windows x64...
"%UNITY%" -batchmode -quit -projectPath "%~dp0" -executeMethod PingPongBuildPipeline.BuildWindowsCommandLine -logFile "%~dp0build.log"
if errorlevel 1 (
  echo [ERROR] Build fallido. Revisa build.log
  pause
  exit /b 1
)

echo [OK] Ejecutable generado en Build\Windows\Ping-Pong_Game.exe
pause
