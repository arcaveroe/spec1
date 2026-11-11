# Arquitectura
- **Capas:** consola → dominio → repositorios.
- La consola delega los casos de uso; el dominio contiene las reglas de negocio.
- El dominio controla versiones, turnos y reglas de aprobación y rechazo.
- Los repositorios son la frontera prevista para almacenar y recuperar documentos.
- Los repositorios aíslan el acceso a datos; hoy no hay persistencia implementada.
- Cada capacidad tiene su propia carpeta, por ejemplo `FirmaDocs/Circuito/`.
- `Program.cs` solo compone e inicia la aplicación; no contiene lógica de negocio.
- Los errores de dominio se expresan mediante `CircuitoFirmaException`.
- El instante de cada operación se recibe como `DateTimeOffset` inyectado.
