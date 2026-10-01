# Ping-Pong_Game

Juego 2D local para dos jugadores, creado para **Unity 6000.3.16f1** y organizado con un flujo Git Flow simplificado.

## Estado
- Version estable: **v1.1.0**
- Unity: **6000.3.16f1**
- Plataforma objetivo: **Windows x64**
- Estilo: **Fantasy / pixel-art 2D**

## Caracteristicas
- 2 escenas: `Menu` y `Game`
- 2 jugadores locales
- Player 1: `W / S`
- Player 2: `Flecha Arriba / Flecha Abajo`
- `ESC` para pausar
- Fisica 2D con `Rigidbody2D` y `Collider2D`
- Rebote segun punto de impacto en la paleta
- Incremento progresivo de velocidad con limite
- Proteccion contra trayectorias casi verticales
- Marcador visible
- Cuenta regresiva antes de cada saque
- Primero en llegar a 5 puntos gana
- Pantalla de victoria y revancha
- Menu de pausa
- Efectos de sonido y ambiente musical original
- Camara con pequeno shake en impactos/goles
- Assets pixel-art originales incluidos
- Configuracion Pixel Perfect 2D compatible con Unity 6.3
- Builder automatico de escenas
- Validador del proyecto
- Build automatico para Windows x64

## Abrir el proyecto
1. Instala Unity **6000.3.16f1** en Unity Hub.
2. Descomprime este proyecto.
3. Unity Hub > Add/Open > selecciona la carpeta del proyecto.
4. Espera a que Unity importe los paquetes.
5. La primera vez el builder crea automaticamente `Menu.unity` y `Game.unity`.
6. Si no aparecen: `Tools > Ping Pong > 1 - Rebuild Complete Project`.
7. Abre `Assets/_Project/Scenes/Menu.unity` y pulsa Play.

## Validar
En Unity ejecuta:

`Tools > Ping Pong > 2 - Validate Project`

La consola debe mostrar `PING-PONG_GAME VALIDATION: OK.`

## Generar ejecutable
Opcion A:

`Tools > Ping Pong > 3 - Build Windows x64`

Opcion B: ejecuta `BUILD_WINDOWS_6000.3.16f1.bat`.

Salida:

`Build/Windows/Ping-Pong_Game.exe`

## Estructura
```text
Assets/
├── Editor/
│   ├── PingPongProjectBuilder.cs
│   └── PingPongBuildPipeline.cs
└── _Project/
    ├── Art/
    │   ├── Backgrounds/
    │   ├── Sprites/
    │   └── UI/
    ├── Audio/
    │   ├── Music/
    │   └── SFX/
    ├── Materials/
    ├── Scenes/
    ├── Scripts/
    │   ├── Audio/
    │   ├── Core/
    │   ├── Gameplay/
    │   └── UI/
    └── Settings/
```

## Git Flow
`main` = estable / entrega.  
`develop` = integracion.  
`feature/*` = funcionalidades.  
`release/*` = preparacion de version.  
`hotfix/*` = correcciones urgentes desde `main`.

Consulta `GITFLOW.md` para el flujo completo.

## Nota tecnica
Las escenas se generan desde un script Editor para que el ZIP sea portable y pueda reconstruirse de forma reproducible al abrirse en Unity 6000.3.16f1.
