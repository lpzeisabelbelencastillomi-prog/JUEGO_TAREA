@echo off
setlocal
cd /d "%~dp0"
where gh >nul 2>&1
if errorlevel 1 (
  echo Instala GitHub CLI antes de ejecutar este archivo.
  echo https://cli.github.com/
  pause
  exit /b 1
)

gh auth status >nul 2>&1
if errorlevel 1 gh auth login

gh repo create Developer-vic1/Ping-Pong_Game --public --source=. --remote=origin --push
if errorlevel 1 (
  echo Puede que el repositorio ya exista. Verifica el remoto con: git remote -v
) else (
  git push -u origin develop
  git push origin --tags
)
pause
