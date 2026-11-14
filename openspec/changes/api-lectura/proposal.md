## Why

Los consumidores no pueden consultar los datos de `firmasimple.db` sin abrir la consola. Se necesita una API de solo lectura para consultar esos datos, manteniendo la escritura centralizada en la consola FirmaDocs.

## What Changes

- Incorporar una API de lectura implementada con Minimal API, separada de la consola y sin capacidad de escritura en la base de datos.
- Mantener la consola FirmaDocs como único proceso autorizado a escribir `firmasimple.db`.
- El diseño documentará la relación con ADR-0001, identificando qué decisiones se mantienen y cuál cambia. La ampliación del acceso de lectura se formalizará mediante un ADR que superseda la decisión vigente.
- El diseño incluirá un diagrama C4 de contenedores en un bloque `mermaid` con las personas `Autor` y `Firmante`, y los contenedores `Consola FirmaDocs` (escritura), `API de lectura` (Minimal API) y la base SQLite `firmasimple.db`.
- El diseño evaluará y descartará gRPC, GraphQL y la exportación periódica a JSON, indicando el motivo de cada descarte.

## Capabilities

### New Capabilities

- `api-lectura`: consulta de datos de FirmaDocs mediante una API que no escribe en la base de datos.

### Modified Capabilities

Ninguna identificada por ahora.

## Impact

- Nuevo contenedor de lectura y su integración con SQLite; la consola conserva la escritura.
- Cambio de la decisión de acceso de ADR-0001, que deberá quedar registrado mediante un ADR nuevo que la superseda.
- No se proponen dependencias nuevas en esta etapa.

## Fuera de alcance

- Escritura en `firmasimple.db` desde la API.
- Cambios en las reglas del circuito de firma o en el estado de `CircuitoFirma`.
- Definir contratos, rutas y filtros concretos de consulta antes de acordarlos en la especificación.

## Preguntas abiertas

- ¿Qué consultas y datos estarán disponibles para `Autor` y `Firmante`?
- ¿Cómo se autenticará y autorizará el acceso a la API?
- ¿La API se alojará como proceso independiente y cómo se configurará su acceso de solo lectura a SQLite?
