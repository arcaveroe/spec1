## Contexto

La consulta debe separar la selección de solicitudes de cualquier transición
de estado. El instante de referencia debe recibirse desde una abstracción
inyectada para que la API y las pruebas sean deterministas.

## Diseño propuesto

- `FirmaDocs.Api/`: contrato HTTP, validación del filtro y respuesta.
- Capa de consulta existente: predicado de vencimiento y orden estable.
- `FirmaDocs.Tests/`: pruebas de límites, solicitudes no vencidas y consulta
  sin efectos secundarios.

La forma exacta del endpoint y del contrato de respuesta se decidirá al
resolver las preguntas abiertas; no se deben inventar rutas en la fase de
implementación.

## Alternativas descartadas

- **Filtrar en el cliente:** se descarta porque transfiere datos innecesarios y
  produce resultados inconsistentes entre consumidores.
- **Marcar la solicitud como vencida al consultarla:** se descarta porque una
  consulta de solo lectura no debe mutar el estado.
- **Usar la hora local del proceso sin inyección:** se descarta porque dificulta
  reproducibilidad y puede cambiar el resultado según el servidor.
