## Why

La API debe permitir localizar solicitudes de firma vencidas sin obligar a
los consumidores a descargar y filtrar todos los registros. El filtro debe
ser explícito, reproducible y no alterar las solicitudes persistidas.

## What Changes

- Incorporar una consulta de solicitudes vencidas en `FirmaDocs.Api`.
- Definir el instante de referencia de la consulta y el criterio de
  vencimiento.
- Mantener la consulta en modo solo lectura y conservar la paginación y el
  orden acordados para la API.

## Capabilities

### New Capabilities

- `filtro-solicitudes-vencidas`: consulta de solicitudes cuyo vencimiento ya
  pasó respecto de un instante de referencia.

### Modified Capabilities

Ninguna identificada por ahora.

## Impact

- Se modificará `FirmaDocs.Api` y sus pruebas de consulta.
- La API no podrá escribir ni cambiar el estado de una solicitud al aplicar el
  filtro.
- No se proponen dependencias nuevas.

## Fuera de alcance

- Renovar, cancelar o modificar solicitudes vencidas.
- Enviar notificaciones o recordatorios.
- Cambiar la definición de vencimiento almacenada en la base de datos.

## Preguntas abiertas

- ¿El filtro será un parámetro de una ruta existente o un endpoint dedicado?
- ¿El límite se interpreta estrictamente (`fecha < instante`) o incluyendo el
  instante exacto?
- ¿Qué zona horaria y reloj serán la referencia operativa?
