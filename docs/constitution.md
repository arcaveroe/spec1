# Constitución del proyecto

1. Ningún requisito sin test; ningún test sin requisito.
2. TDD: primero el test en rojo, luego el código mínimo, luego refactor. Un commit por ciclo.
3. Sin lógica de negocio en `Program.cs` ni en consola: ubicarla en componentes apropiados.
4. Errores de dominio explícitos, con mensajes en español; nunca `null` silencioso.
5. Sin `DateTime.Now` ni `DateTimeOffset.Now` en la lógica: el instante se inyecta.
6. Un cambio = una spec + un plan + tareas trazables. Sin spec no hay implementación.
7. El agente propone; el equipo aprueba cada artefacto antes del siguiente.
