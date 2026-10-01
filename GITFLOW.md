# Git Flow utilizado

```text
main
└── develop
    ├── feature/project-setup
    ├── feature/assets-import
    ├── feature/level-design
    ├── feature/gameplay-core
    ├── feature/scoring-endgame
    ├── feature/ui-menu
    ├── feature/audio-polish
    └── release/v1.0.0
```

## Regla
- `main`: estable / entrega.
- `develop`: integración.
- `feature/*`: una función concreta.
- `release/*`: preparación final.
- `hotfix/*`: correcciones urgentes posteriores.

## Ver historial
```bash
git log --oneline --graph --decorate --all
```
