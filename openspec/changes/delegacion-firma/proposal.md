## Why

Un firmante puede necesitar que otra persona actúe en su nombre durante un
circuito de firma. El dominio debe expresar esa delegación sin perder la
identidad del firmante original ni permitir que una delegación cambie el
historial ya registrado.

## What Changes

- Incorporar la delegación temporal de un firmante en un circuito.
- Permitir que el delegado actúe únicamente durante la vigencia y el alcance
  autorizados.
- Conservar en cada firma el firmante original y la identidad del delegado que
  ejecutó la acción.
- Rechazar delegaciones ambiguas, caducadas o incompatibles con el circuito.

## Capabilities

### New Capabilities

- `delegacion-firma`: autorización temporal para que un delegado firme por un
  firmante dentro de un circuito.

### Modified Capabilities

- `circuito-firma`: resolución del firmante habilitado y registro de firmas.

## Impact

- Se modifica `FirmaDocs/Circuito/` y los tipos de dominio relacionados con
  firmas.
- Será necesario ampliar las pruebas del circuito sin cambiar firmas públicas
  existentes hasta acordar el contrato definitivo.
- No se proponen dependencias nuevas.

## Fuera de alcance

- Delegación global para todos los documentos de un usuario.
- Delegación de autoría, rechazo o reenvío del documento.
- Integración con un proveedor externo de identidad.

## Preguntas abiertas

- ¿La delegación se limita a una etapa, a un circuito completo o a un
  intervalo de fechas?
- ¿Puede un delegado acumular varias delegaciones?
- ¿Qué autoridad crea, revoca y audita la delegación?
