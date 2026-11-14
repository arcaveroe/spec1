---
name: test-driven-development
description: TDD estricto con slicing vertical para implementar tareas de OpenSpec. Úsala en cualquier /opsx-apply.
---
# Reglas (no negociables)
1. Un ciclo = UN test nuevo → `dotnet test` en rojo (muestra la salida) → commit `test(...)` → código mínimo → `dotnet test` en verde → commit `feat(...)` → refactor opcional (commit `refactor(...)`, sin tocar tests).
2. Prohibido modificar tests y código de producción en el mismo commit. Un cambio de test es su propio commit y explica por qué.
3. Prohibido modificar un test existente para que pase. Si falla, corrige el código o pregunta.
4. Variantes del mismo caso → `[Theory]` con `[InlineData]`; incluye siempre los valores límite.
5. Toda aserción sobre errores verifica tipo Y mensaje/motivo. Toda interacción con un sustituto se verifica con `Received()` / `DidNotReceive()` y sus argumentos exactos.
6. Objetivo: 100 % de cobertura de ramas del código nuevo. Nada de código sin un test que lo haya exigido.
7. Mensaje de commit: `test|feat|refactor(<alcance>): Tn (RF-xx) <qué>`. Cada ciclo deja en el log un `test` seguido de su `feat`.
