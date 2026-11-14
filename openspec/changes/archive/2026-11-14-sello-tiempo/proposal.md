# Propuesta: sello-tiempo

## Why

Se necesita detectar si cambió el contenido de una versión después de registrar su Firma. Para ello, el cambio asociará el contenido aprobado con un hash calculado sobre sus bytes UTF-8 y permitirá compararlo con el contenido actual de esa versión.

## What Changes

- El sistema MUST calcular el hash del contenido de la versión aprobada a partir de sus bytes UTF-8 y solicitar su sellado a una TSA antes de registrar la Firma.
- Si la TSA falla, el sistema MUST no registrar la Firma y MUST mantener el turno en el mismo firmante.
- El sistema MUST permitir verificar una Firma comparando el hash guardado con el hash calculado a partir del contenido actual de esa versión.
- El sistema MUST mantener sin cambios la firma pública `Aprobar(Firmante, DateTimeOffset)`. El diseño decidirá si la extensión requiere una sobrecarga o un servicio nuevo.

## Capabilities

### New Capabilities

- `sello-tiempo`: asociar el hash del contenido aprobado con una operación TSA y verificarlo contra el contenido actual de la versión.

### Modified Capabilities

- `etapas-paralelas`: condicionar el registro de la Firma al éxito de la TSA y conservar el turno en el mismo firmante si la TSA falla.

## Impact

- Afecta al registro de Firmas durante la aprobación y añade la verificación del contenido de una versión.
- La firma pública existente `Aprobar(Firmante, DateTimeOffset)` se conserva; el diseño decidirá entre una sobrecarga o un servicio nuevo.
- No se elige proveedor TSA ni se incorpora una dependencia de proveedor en esta propuesta.

## Fuera de alcance

- Firma criptográfica con certificado.
- Elección del proveedor TSA.
- Persistencia de la evidencia.

## Preguntas abiertas

- ¿Qué abarca «persistencia de la evidencia» fuera de alcance, dado que la verificación requiere comparar el hash guardado? ¿Se conserva el hash de referencia, mientras que no se persiste la respuesta TSA?
- ¿Qué condiciones determinan que la operación TSA haya tenido éxito?
- ¿Qué mecanismo de API se elige en el diseño: sobrecarga de `Aprobar` o servicio nuevo?
