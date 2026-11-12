# Mapa del estado `ESPERANDO_CERTIFICADO`

Mapa de contratos, persistencia, filas afectadas y referencias de código, preparado para analizar la eliminación del estado. La base local se consultó en modo de solo lectura el 2026-11-11. Este documento describe el estado actual; no es una propuesta de implementación.

## 1. Contratos públicos y consumidores

- **Solicitud y estados:** `SolicitudFirma` representa una fila ([SolicitudFirma.cs:4](../FirmaDocs/Legado/SolicitudFirma.cs#L4)). `EstadoSolicitud` enumera estados ([EstadoSolicitud.cs:5](../FirmaDocs/Legado/EstadoSolicitud.cs#L5)) y `EstadoSolicitudTexto.ATexto` / `DesdeTexto` convierten entre enum y texto persistido ([EstadoSolicitud.cs:14](../FirmaDocs/Legado/EstadoSolicitud.cs#L14)).
  - El enum lo usan la bandeja, los recordatorios, el reporte y el repositorio.
  - El conversor lo usan el repositorio y el reporte.
- **Repositorio:** `SolicitudFirmaRepository` expone `Listar`, `ListarPorFirmante`, `CambiarEstado` y `ContarPorEstado` ([SolicitudFirmaRepository.cs:8](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L8), [SolicitudFirmaRepository.cs:20](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L20), [SolicitudFirmaRepository.cs:37](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L37), [SolicitudFirmaRepository.cs:47](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L47), [SolicitudFirmaRepository.cs:67](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L67)).
  - `BandejaFirmante` consume `ListarPorFirmante`.
  - `RecordatorioService` y `ReporteSla` consumen `Listar`.
  - No se encontraron consumidores internos de `CambiarEstado` ni `ContarPorEstado`.
- **Bandeja:** `BandejaFirmante.Armar(email)` devuelve `Bandeja`, cuyas listas son `PorFirmar`, `EsperandoCertificado` e `Historial` ([BandejaFirmante.cs:3](../FirmaDocs/Legado/BandejaFirmante.cs#L3), [BandejaFirmante.cs:18](../FirmaDocs/Legado/BandejaFirmante.cs#L18)). No se encontraron otros consumidores internos de `Bandeja` o `Armar`.
- **Recordatorios:** `ICanalRecordatorio.Enviar(email, mensaje)` es consumido por `RecordatorioService`; este expone `EnviarRecordatorios(ahora)` ([ICanalRecordatorio.cs:4](../FirmaDocs/Legado/ICanalRecordatorio.cs#L4), [RecordatorioService.cs:9](../FirmaDocs/Legado/RecordatorioService.cs#L9), [RecordatorioService.cs:16](../FirmaDocs/Legado/RecordatorioService.cs#L16)). No se encontró una implementación concreta del canal ni un consumidor interno del servicio.
- **SLA:** `ReporteSla.HorasPromedioPorMes()` consume solicitudes del repositorio ([ReporteSla.cs:5](../FirmaDocs/Legado/ReporteSla.cs#L5), [ReporteSla.cs:17](../FirmaDocs/Legado/ReporteSla.cs#L17)); no se encontró un consumidor interno del reporte.
- No se encontró composición de estos servicios en la consola; [Program.cs:2](../FirmaDocs/Program.cs#L2) solo escribe `Hello, World!`.

## 2. Restricciones de base de datos

- `estado` es `TEXT NOT NULL` con un `CHECK` que admite `PENDIENTE`, `ESPERANDO_CERTIFICADO`, `FIRMADA`, `RECHAZADA` y `EXPIRADA`: [schema.sql:9](../db/schema.sql#L9) y [001_inicial.sql:9](../db/migrations/001_inicial.sql#L9).
- Existe el índice **no único** `ix_solicitudes_estado` sobre `estado`: [schema.sql:12](../db/schema.sql#L12) y [002_indice_estado.sql:2](../db/migrations/002_indice_estado.sql#L2).
- No se encontraron declaraciones de triggers en el esquema ni las migraciones. La consulta de solo lectura al catálogo SQLite tampoco encontró triggers asociados a `solicitudes_firma`.

## 3. Filas afectadas y comportamiento actual

La base local [firmasimple.db](../firmasimple.db) contiene **250** filas `ESPERANDO_CERTIFICADO`, de **1.000** solicitudes en total. Las 250 tienen `resuelta_en = NULL`; su fecha de creación va de `2025-05-06` a `2026-11-01`. El comentario del seed también declara 250 en [seed.sql:2](../db/seed.sql#L2).

Actualmente estas solicitudes:

- Aparecen en la sección separada `Bandeja.EsperandoCertificado` ([BandejaFirmante.cs:28](../FirmaDocs/Legado/BandejaFirmante.cs#L28)).
- Reciben siempre un recordatorio, sin umbral de antigüedad ([RecordatorioService.cs:30](../FirmaDocs/Legado/RecordatorioService.cs#L30)).
- Se excluyen del promedio SLA mediante la lista explícita de estados que cuentan ([ReporteSla.cs:8](../FirmaDocs/Legado/ReporteSla.cs#L8), aplicada en [ReporteSla.cs:25](../FirmaDocs/Legado/ReporteSla.cs#L25)).

## 4. Referencias de código que leen o escriben el estado

- Enum: [EstadoSolicitud.cs:8](../FirmaDocs/Legado/EstadoSolicitud.cs#L8).
- Conversión enum a texto y texto a enum: [EstadoSolicitud.cs:21](../FirmaDocs/Legado/EstadoSolicitud.cs#L21) y [EstadoSolicitud.cs:34](../FirmaDocs/Legado/EstadoSolicitud.cs#L34).
- Campo público de la bandeja y clasificación del registro leído: [BandejaFirmante.cs:5](../FirmaDocs/Legado/BandejaFirmante.cs#L5) y [BandejaFirmante.cs:28](../FirmaDocs/Legado/BandejaFirmante.cs#L28).
- Consulta de solicitudes de ese estado y envío del aviso: [RecordatorioService.cs:30](../FirmaDocs/Legado/RecordatorioService.cs#L30).
- Exclusión del SLA: comentario explicativo en [ReporteSla.cs:7](../FirmaDocs/Legado/ReporteSla.cs#L7), lista de inclusión —que omite este estado— en [ReporteSla.cs:8](../FirmaDocs/Legado/ReporteSla.cs#L8), y filtro aplicado en [ReporteSla.cs:25](../FirmaDocs/Legado/ReporteSla.cs#L25).
- Filtro por estado, escritura al cambiar estado y lectura al materializar filas: [SolicitudFirmaRepository.cs:32](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L32), [SolicitudFirmaRepository.cs:62](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L62) y [SolicitudFirmaRepository.cs:95](../FirmaDocs/Legado/SolicitudFirmaRepository.cs#L95).
- El estado también aparece en el `CHECK` del esquema y en las filas de [seed.sql](../db/seed.sql).

No se encontraron referencias en las pruebas del proyecto.

## 5. Riesgos de migración

- Si se quita el estado del conversor y se dejan sus filas, `DesdeTexto` lanzará `ArgumentException` al materializarlas. Esto puede hacer fallar consultas sin filtro y consultas por firmante que incluyan alguna de las 250 filas.
- Si se quita el valor del enum, también hay referencias de código que deben desaparecer o adaptarse: la rama de la bandeja y la consulta de recordatorios.
- El `CHECK` vigente seguirá rechazando los valores fuera de su conjunto y todavía admite el estado retirado. No hay una migración posterior que cambie ese contrato.
- Las 250 filas están sin resolver: su destino afecta la sección de bandeja, los recordatorios y el SLA. Dejarlas con el texto antiguo, convertirlas o eliminarlas no produce el mismo comportamiento.
- En una reconstrucción de tabla para cambiar el `CHECK`, hay que considerar la preservación de datos y la recreación del índice. No se encontró un ejecutor de migraciones en la aplicación.
