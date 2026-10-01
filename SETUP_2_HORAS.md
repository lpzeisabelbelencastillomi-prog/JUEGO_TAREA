# Ruta de 2 horas

## 0-10 min - Abrir y generar
1. Abre con Unity 6000.3.16f1.
2. Espera la importacion.
3. Ejecuta `Tools > Ping Pong > 1 - Rebuild Complete Project`.
4. Abre `Menu.unity`.

## 10-30 min - Probar jugabilidad
- JUGAR abre Game.
- P1: W/S.
- P2: flechas.
- Verifica rebotes, goles y marcador.
- Llega a 5 para comprobar victoria.
- Prueba ESC, continuar, reiniciar y menu.

## 30-45 min - Validacion
- `Tools > Ping Pong > 2 - Validate Project`.
- Consola sin errores rojos.
- Revisa que Menu y Game esten en Build Settings.

## 45-65 min - Ajustes visuales opcionales
Solo si todo funciona:
- cambia textos o volumenes;
- mueve UI;
- ajusta `startSpeed`, `maxSpeed` o `maxScore` desde Inspector.

## 65-85 min - GitHub
Si el repo remoto aun no existe:
```bash
gh auth login
gh repo create Developer-vic1/Ping-Pong_Game --public --source=. --remote=origin --push
git push -u origin develop
git push origin --tags
```

## 85-105 min - Ejecutable
- `Tools > Ping Pong > 3 - Build Windows x64`
- o `BUILD_WINDOWS_6000.3.16f1.bat`.
- Prueba el `.exe` fuera de Unity.

## 105-120 min - Exposicion
Muestra:
1. Menu.
2. Juego y controles.
3. Colisiones y sonidos.
4. Marcador y victoria.
5. Git graph: `git log --oneline --graph --decorate --all`.
6. Ejecutable y repo GitHub.
