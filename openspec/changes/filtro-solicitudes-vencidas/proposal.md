## Why

La API debe permitir localizar solicitudes de firma vencidas sin obligar a los
consumidores a descargar y filtrar todos los registros. El filtro debe ser
explícito, reproducible y no alterar las solicitudes persistidas.

## What Changes

- Agregar `FirmaDocs.Api` con `GET /api/solicitudes/vencidas`.
- Recibir `instante` como ISO-8601 con offset; usar `page` y `pageSize` opcionales
  con valores por defecto 1 y 50.
- Devolver `{ items, page, pageSize, totalCount }`, ordenando por vencimiento e
  identificador ascendente.
- Seleccionar solo solicitudes con `vence_en` anterior al instante de referencia.
- Incorporar el campo nullable `vence_en` y una migración; las filas existentes
  sin fecha de vencimiento permanecen con `NULL` y no se incluyen en el filtro.
- Mantener la consulta en modo solo lectura y no cambiar estados ni historial.

## Capabilities

### New Capabilities

- `filtro-solicitudes-vencidas`: consulta paginada de solicitudes cuyo
  vencimiento almacenado ya pasó respecto de un instante de referencia.

### Modified Capabilities

- `solicitudes-firma`: agregar el vencimiento opcional a los datos persistidos
  de una solicitud.

## Impact

- Se agregará `FirmaDocs.Api` y se actualizarán el modelo/repositorio existentes.
- Se agregarán migraciones de avance y rollback para `vence_en`.
- La migración no inventa fechas: las solicitudes anteriores quedan sin
  vencimiento hasta que una fuente externa las complete.
- No se agregan paquetes NuGet.

## Fuera de alcance

- Renovar, cancelar o modificar solicitudes vencidas.
- Enviar notificaciones o recordatorios.
- Derivar automáticamente vencimientos o completar fechas de filas legadas.

## Preguntas abiertas

No quedan preguntas abiertas para este cambio.
