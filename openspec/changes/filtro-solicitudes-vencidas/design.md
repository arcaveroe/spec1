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
de estado. El instante de referencia llega como parámetro explícito; la capa
de consulta no lee el reloj del sistema.

## Decisiones

- `GET /api/solicitudes/vencidas` requiere `instante` ISO-8601 con offset.
- `page` comienza en 1 y `pageSize` en 50; ambos deben ser positivos.
- El límite es estricto (`vence_en < instante`). Los vencimientos nulos no
  coinciden.
- La respuesta contiene `items`, `page`, `pageSize` y `totalCount`. Los
  elementos se ordenan por `vence_en` y luego por `id`, ambos ascendentes.
- Se añade `vence_en` como texto ISO-8601 nullable. La migración conserva las
  filas existentes sin asignarles una fecha.
- El endpoint usa el repositorio SQLite existente. Los conteos y resultados de
  una página se leen dentro de la misma transacción de lectura.
- La API obtiene `ConnectionStrings:Solicitudes` de la configuración estándar
  de ASP.NET Core; la variable de entorno equivalente es
  `ConnectionStrings__Solicitudes`.

## Módulos y tipos afectados

- `FirmaDocs.Api`: composición de la API y mapeo del endpoint.
- `FirmaDocs.Legado.SolicitudFirma`: propiedad opcional `VenceEn`.
- `FirmaDocs.Legado.SolicitudFirmaRepository`: consulta paginada de solo lectura.
- `db/schema.sql` y `db/migrations`: columna nullable y reversión.
- `FirmaDocs.Tests`: límites temporales, paginación, HTTP, migración y ausencia
  de efectos secundarios.

## Alternativas descartadas

- **Filtrar en el cliente:** transfiere datos innecesarios y puede producir
  resultados inconsistentes.
- **Marcar como expirada al consultar:** una consulta de solo lectura no debe
  mutar el estado ni el historial.
- **Usar la hora local del proceso:** no es reproducible; se requiere el
  instante enviado por el consumidor.
- **Rellenar vencimientos históricos desde `creada_en`:** no existe una regla
  de duración aprobada y produciría fechas inventadas.
