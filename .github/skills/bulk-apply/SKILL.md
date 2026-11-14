---
name: bulk-apply
description: Ejecuta /opsx-apply para un cambio dado en el worktree indicado, con git-discipline (entorno limpio, commit por tarea).
argument-hint: <cambio> <ruta-del-worktree>
---
1. Verifica `git status` limpio en la ruta del worktree; si no, detente.
2. Ejecuta las tareas del cambio con /test-driven-development; commit por tarea en la rama del worktree.
3. Al terminar, `dotnet test` en el worktree y resume: tareas completadas, commits, tests.
