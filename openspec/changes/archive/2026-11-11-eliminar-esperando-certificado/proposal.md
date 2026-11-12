## Why

`ESPERANDO_CERTIFICADO` es un estado especial de las solicitudes de firma que sigue admitido por el esquema y tiene tratamiento propio en tres consumidores. Retirarlo evita que nuevas solicitudes dependan de ese estado. Las 250 filas existentes se conservarán y pasarán a `PENDIENTE`, dejando una marca de auditoría de su estado anterior.

## What Changes

- Retirar `ESPERANDO_CERTIFICADO` del conjunto de estados admitidos para una solicitud y de la restricción de persistencia.
- Migrar las filas existentes a `PENDIENTE`, asignando `migrado_desde = 'ESPERANDO_CERTIFICADO'` y sin eliminar registros.
- En `BandejaFirmante`, dejar de separar estas solicitudes en la categoría de espera; una vez migradas a `PENDIENTE`, se clasifican en `PorFirmar`.
- Hacer que `RecordatorioService` deje de enviar el aviso «Instala tu certificado digital».
- Hacer que `ReporteSla` incluya estas solicitudes en el SLA desde su `creada_en` original.

## Capabilities

### New Capabilities

- Ninguna.

### Modified Capabilities

- `solicitudes-firma`: dejar de admitir el estado `ESPERANDO_CERTIFICADO`.

## Contexto encontrado

- La columna `estado` tiene una restricción `CHECK` que admite `PENDIENTE`, `ESPERANDO_CERTIFICADO`, `FIRMADA`, `RECHAZADA` y `EXPIRADA` (`db/schema.sql:9`).
- Hay tres consumidores actuales del estado:
  - `BandejaFirmante` clasifica estas solicitudes en una sección separada (`FirmaDocs/Legado/BandejaFirmante.cs:28`).
  - `RecordatorioService` busca solicitudes con ese estado y les envía recordatorios (`FirmaDocs/Legado/RecordatorioService.cs:30`).
  - `ReporteSla` las excluye del cálculo del promedio (`FirmaDocs/Legado/ReporteSla.cs:8`, `:25`).
- La base local contiene **250** filas `ESPERANDO_CERTIFICADO`, de **1.000** solicitudes; las 250 tienen `resuelta_en = NULL` (`docs/explore-legado.md`, sección 3).

## Fuera de alcance

- Cambios en `CircuitoFirma` o en sus reglas.
- Cambios de UI.
- Modificar o recalcular reportes históricos ya emitidos.
- Cualquier comportamiento que no afecte directamente al estado `ESPERANDO_CERTIFICADO`.

## Impact

- Capacidad existente `solicitudes-firma` en `openspec/specs/solicitudes-firma/spec.md`.
- Estado y conversión entre el enum y su representación textual en `FirmaDocs/Legado/EstadoSolicitud.cs`.
- Persistencia y transiciones en `FirmaDocs/Legado/SolicitudFirmaRepository.cs`, además del `CHECK` de `db/schema.sql`.
- Los tres consumidores descritos en “Contexto encontrado”: clasificación en `BandejaFirmante`, envío de recordatorios en `RecordatorioService` y cálculo de `ReporteSla`.
- Datos existentes: migrar las 250 filas a `PENDIENTE`, conservarlas y registrar el estado anterior en `migrado_desde`.

## Preguntas abiertas

- Al incluir estas solicitudes en el SLA desde su `creada_en` original, ¿debe considerarse que eso «empeora» el SLA histórico de los períodos afectados?
