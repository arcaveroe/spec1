# Propuesta: motivo-rechazo-minimo

## Why

El circuito actualmente solo exige que el motivo de rechazo no esté vacío o compuesto únicamente por espacios. Se necesita exigir una longitud mínima que asegure que el motivo aporte información útil para la revisión del documento.

## What Changes

- Exigir que el motivo de rechazo tenga al menos 10 caracteres después de quitar los espacios iniciales y finales.
- Rechazar los motivos que no alcancen esa longitud con un error explícito, sin registrar el rechazo ni cambiar el estado del documento.
- Mantener el motivo original, incluidos sus espacios exteriores, asociado al firmante, la versión y el instante del rechazo cuando sea válido.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `rechazo-y-vueltas`: endurecer la validación del motivo exigido al rechazar un documento.

## Impact

- Validación de `Rechazar` y mensaje de `CircuitoFirmaException` (`El motivo de rechazo debe tener al menos 10 caracteres sin contar espacios iniciales ni finales.`).
- Pruebas del circuito de firma y de los rechazos en etapas.
- No se proponen dependencias nuevas ni cambios en la firma pública existente.

## Fuera de alcance

- Cambiar quién puede rechazar o cuándo puede rechazarse un documento.
- Cambiar el número máximo de vueltas o el comportamiento del reenvío.
- Modificar el historial de rechazos ya registrados.
- Añadir moderación semántica, detección de lenguaje ofensivo o formato estructurado para el motivo.

## Preguntas abiertas

- Ninguna. La longitud, el tratamiento de espacios y el mensaje de error quedan definidos por esta propuesta.
