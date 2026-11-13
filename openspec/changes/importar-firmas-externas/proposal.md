## Why

Se necesita incorporar a FirmaDocs registros recibidos mediante un archivo que declaran que se produjo una firma fuera del sistema. Cada registro conserva su origen y es solo una declaración externa: el archivo no demuestra por sí mismo la validez de una Firma.

## What Changes

- Introducir una capacidad para importar esos registros como datos externos, sin convertirlos en Firmas del circuito ni en Solicitudes de firma.
- Mantener separado el registro externo de la Firma del circuito y del estado de cualquier Solicitud de firma.
- Mantener intacto el historial existente; las reglas de incorporación y de duplicados quedan pendientes de decisión.

## Capabilities

### New Capabilities

- `importar-firmas-externas`: incorporación desde archivo de datos relativos a firmas externas.

### Modified Capabilities

Ninguna identificada por ahora. La importación no resuelve ni modifica Solicitudes de firma.

## Impact

- Interacción desde la consola, único proceso autorizado a leer y escribir la base de datos.
- Persistencia SQLite y el historial de firmas, sujeto a las reglas que se definan en la especificación.
- No se proponen dependencias nuevas ni una API externa en esta etapa.

## Fuera de alcance

- Conexión directa con proveedores externos; el origen considerado es un archivo.
- Crear Firmas del circuito o resolver Solicitudes de firma a partir de los registros importados.
- Modificar o borrar firmas existentes.

## Preguntas abiertas

- ¿Qué formato, codificación, columnas e identificadores tendrá el archivo?
- ¿Qué debe ocurrir si un registro no puede relacionarse con un Documento, un Firmante o una Versión?
- ¿La importación debe ser atómica o permitir importar registros válidos aunque otros fallen?
- ¿Cómo se detectan registros duplicados y qué ocurre si la Versión indicada no es la vigente?

## Cambios incorporados

- **Objeción 1 (ACEPTADA):** se limita la importación a registros externos separados; no crean Firmas del circuito ni resuelven Solicitudes de firma.
- **Objeción 2 (ACEPTADA):** cada registro se define como una declaración de que se produjo una firma fuera del sistema, conserva su origen y no se considera prueba de validez por el mero hecho de estar en un archivo.
