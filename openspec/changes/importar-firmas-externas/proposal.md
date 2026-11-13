## Why

Se necesita incorporar a FirmaDocs datos de firmas generados fuera del sistema y recibidos mediante un archivo. La propuesta busca hacerlo sin confundir una Firma con una Solicitud de firma ni alterar el historial de firmas ya registrado.

## What Changes

- Introducir una capacidad para importar desde un archivo datos relativos a firmas externas.
- Definir cómo se validan y relacionan esos datos con un Documento, un Firmante, una Versión y un instante.
- Mantener intacto el historial existente; las reglas de incorporación y de duplicados quedan pendientes de decisión.

## Capabilities

### New Capabilities

- `importar-firmas-externas`: incorporación desde archivo de datos relativos a firmas externas.

### Modified Capabilities

Ninguna identificada por ahora. Se revisará si corresponde modificar `solicitudes-firma` cuando se decida si la importación afecta el estado de una Solicitud de firma.

## Impact

- Interacción desde la consola, único proceso autorizado a leer y escribir la base de datos.
- Persistencia SQLite y el historial de firmas, sujeto a las reglas que se definan en la especificación.
- No se proponen dependencias nuevas ni una API externa en esta etapa.

## Fuera de alcance

- Conexión directa con proveedores externos; el origen considerado es un archivo.
- Cambiar la definición de Firma o de Solicitud de firma.
- Modificar o borrar firmas existentes.

## Preguntas abiertas

- ¿Cada registro importado debe crear una Firma del circuito o resolver una Solicitud de firma existente?
- ¿Qué formato, codificación, columnas e identificadores tendrá el archivo?
- ¿Qué debe ocurrir si un registro no puede relacionarse con un Documento, un Firmante o una Versión?
- ¿La importación debe ser atómica o permitir importar registros válidos aunque otros fallen?
- ¿Cómo se detectan registros duplicados y qué ocurre si la Versión indicada no es la vigente?
