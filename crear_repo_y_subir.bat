@echo off
cd /d "%~dp0"
echo === Estado Git ===
git status

echo === Creando repo remoto con GitHub CLI ===
gh repo create Developer-vic1/Ping-Pong_Game --public --source=. --remote=origin --push
if errorlevel 1 (
  echo No se pudo crear el repo automaticamente. Verifica gh auth login o si el repo ya existe.
)

echo === Subiendo develop y tags ===
git push -u origin develop
git push origin --tags
pause
