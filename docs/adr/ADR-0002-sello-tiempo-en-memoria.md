# ADR-0002 · Sellado de tiempo y hash de contenido en memoria

- Estado: Proposed
- Fecha: 2026-11-13 · Decisores: no especificados
- Reemplaza a: —

## Contexto y problema

`CircuitoFirma` conserva su estado en memoria. El método público `Aprobar(Firmante, DateTimeOffset)` registra una Firma sin recibir el contenido de la versión ni comunicarse con una TSA. Se necesita asociar el contenido aprobado con un hash y asegurar que la TSA confirme ese hash antes de registrar la Firma, sin cambiar el método público existente.

ADR-0001 establece que `CircuitoFirma` permanece fuera de la persistencia SQLite. Por tanto, conservar el hash en la Firma no debe introducir una nueva persistencia ni alterar ese límite.

## Opciones consideradas

1. Integrar la llamada TSA directamente en `CircuitoFirma`.
2. Añadir una sobrecarga pública de `Aprobar` que reciba el contenido y gestione el sellado.
3. Coordinar el cálculo del hash, la llamada TSA y el registro mediante un servicio separado, manteniendo el hash en memoria.
4. Persistir el hash o la respuesta TSA en SQLite.

## Decisión

Elegimos la opción 3. Un nuevo `ServicioAprobacionSellada`, separado de `CircuitoFirma`, calcula SHA-256 de los bytes UTF-8 exactos del contenido y solicita a la TSA confirmar el hash. Solo después de una confirmación satisfactoria, el servicio registra internamente la Firma con el hash asociado. Si la TSA falla, el circuito no cambia.

El método público existente `Aprobar(Firmante, DateTimeOffset)` y los parámetros del constructor de `Firma` se mantienen sin cambios. Las Firmas creadas por el método existente no tendrán hash y no serán verificables por contenido. El hash asociado a una Firma sellada se conserva solo en memoria; no se persisten en SQLite ni el hash ni la respuesta TSA. La evidencia TSA no se conserva.

## Consecuencias

- Positivas: `CircuitoFirma` no depende de la TSA; ningún fallo TSA registra una Firma ni cambia el turno; se conserva la API existente; el contenido puede verificarse comparando su hash con el asociado a la Firma durante la ejecución del proceso.
- Negativas: el método existente aún permite registrar Firmas sin sellado; el hash no sobrevive al reinicio del proceso; la respuesta TSA no puede validarse posteriormente de forma independiente.
- Si se requiere verificación tras reiniciar el proceso o conservar la respuesta TSA, será necesario revisar el límite de persistencia establecido por ADR-0001 mediante una decisión posterior.

## Alternativas descartadas

- Integrar la llamada TSA en `CircuitoFirma`: acoplaría el dominio a una integración externa y realizaría una operación remota desde el dominio.
- Añadir una sobrecarga pública de `Aprobar`: ampliaría la API del dominio para coordinar una operación de aplicación externa, en vez de mantener esa coordinación en un servicio separado.
- Persistir el hash o la respuesta TSA en SQLite: ampliaría la persistencia del circuito más allá del límite establecido por ADR-0001.
