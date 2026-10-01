# Ping-Pong_Game

Proyecto académico de Pong 2D desarrollado en Unity con estética pixel-fantasy.

## Incluye
- 2 escenas: Menu y Game
- Dos jugadores locales
- Player 1: W / S
- Player 2: Flecha arriba / Flecha abajo
- Colisiones y rebotes
- Marcador y condición de victoria a 5 puntos
- Pausa con ESC
- Sonidos de rebote, punto, click y victoria
- Menú, controles, pausa y pantalla final
- Assets originales incluidos en `Assets/_Project/Art`
- Historial Git Flow local incluido

## Abrir el proyecto
1. Descomprime la carpeta completa.
2. Unity Hub > Add > selecciona `Ping-Pong_Game`.
3. Ábrelo con Unity 6.3 LTS o una versión Unity 6 compatible.
4. Espera a que Unity compile.
5. El script `PingPongProjectBuilder` genera automáticamente las escenas la primera vez.
6. Si no ocurre, usa: **Tools > Ping Pong > Rebuild Complete Project**.
7. Abre `Assets/_Project/Scenes/Menu.unity` y pulsa Play.

## Build
- File > Build Profiles / Build Settings
- Windows
- Scenes: Menu y Game
- Build

## Git
El ZIP incluye `.git` con una historia de ramas/merges local. Para conectarlo a tu GitHub:

```bash
git remote add origin https://github.com/Developer-vic1/Ping-Pong_Game.git
git push -u origin main
git push -u origin develop
git push origin --tags
```

Si el repositorio remoto aún no existe y tienes GitHub CLI:

```bash
gh repo create Developer-vic1/Ping-Pong_Game --public --source=. --remote=origin --push
git push -u origin develop
git push origin --tags
```
