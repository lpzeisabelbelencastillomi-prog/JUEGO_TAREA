# Git Flow - Ping-Pong_Game

## Ramas permanentes
- `main`: solo versiones estables.
- `develop`: integracion de trabajo terminado.

## Ramas temporales
- `feature/unity-6000.3.16f1`
- `feature/gameplay-polish`
- `feature/visual-audio-polish`
- `feature/build-pipeline`
- `release/v1.1.0`
- `hotfix/*` solo si una version estable falla.

## Flujo correcto para una feature
```bash
git checkout develop
git pull
git checkout -b feature/nombre

# trabajar
git add .
git commit -m "feat: descripcion"
git push -u origin feature/nombre

# despues de revisar
git checkout develop
git merge --no-ff feature/nombre
git push origin develop
git branch -d feature/nombre
```

## Release
```bash
git checkout develop
git checkout -b release/v1.1.0
# pruebas, README y build final

git checkout main
git merge --no-ff release/v1.1.0
git tag -a v1.1.0 -m "Ping-Pong_Game v1.1.0"
git push origin main --tags

git checkout develop
git merge --no-ff release/v1.1.0
git push origin develop
git branch -d release/v1.1.0
```

## Hotfix
```bash
git checkout main
git checkout -b hotfix/nombre
# corregir

git checkout main
git merge --no-ff hotfix/nombre
git tag -a v1.1.1 -m "hotfix"

git checkout develop
git merge --no-ff hotfix/nombre
```

## Convencion de commits
- `feat:` funcionalidad nueva
- `fix:` correccion
- `chore:` configuracion / mantenimiento
- `docs:` documentacion
- `refactor:` cambio interno sin alterar comportamiento
- `release:` cierre de version
