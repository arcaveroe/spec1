## Context

La aplicación de consola es el único proceso autorizado a leer y escribir `firmasimple.db`. La base usa SQLite y migraciones versionadas. `CircuitoFirma` conserva su estado en memoria; no forma parte de esa persistencia. El esquema actual contiene solicitudes de firma, por lo que guardar allí declaraciones externas las confundiría con una entidad distinta.

La propuesta limita la importación a registros que declaran una firma ocurrida fuera del sistema. No son Firmas del circuito ni resuelven Solicitudes de firma, y el archivo de origen no demuestra por sí mismo la validez de una Firma.

### Decisiones previas

- **ADR-0001:** la consola es el único proceso con acceso a la base de datos; `CircuitoFirma` permanece en memoria. Este diseño mantiene ese límite y no persiste el estado del circuito.

## Goals / Non-Goals

**Goals:**

- Incorporar las declaraciones importadas como datos persistidos, separados de las Firmas del circuito y de las Solicitudes de firma.
- Conservar el origen de cada declaración.
- Mantener intactos el historial existente y el estado en memoria de `CircuitoFirma`.
- Usar la consola y las migraciones SQLite existentes, sin añadir dependencias ni una API externa.

**Non-Goals:**

- Definir todavía el formato del archivo, sus columnas o identificadores.
- Determinar cómo relacionar cada registro con un Documento, Firmante o Versión.
- Resolver la política de duplicados, errores parciales o atomicidad; esas reglas se fijarán en la especificación.
- Crear Firmas del circuito o resolver Solicitudes de firma a partir de los datos importados.

## Decisions

1. **Persistir las declaraciones en una tabla propia de SQLite.** La tabla se añadirá mediante una migración versionada y no reutilizará las tablas de Firmas del circuito ni de Solicitudes de firma. Así se conservan los datos importados sin cambiar el significado ni el historial de esas entidades.
   - **Alternativa descartada:** insertarlos en el historial de Firmas. Se descarta porque una declaración externa no es una Firma del circuito y no debe alterar ese historial.
   - **Alternativa descartada:** guardarlos como Solicitudes de firma. Se descarta porque no representan peticiones pendientes ni resuelven su estado.

2. **Mantener el acceso a través de la consola y las capas existentes.** La consola delegará la importación en una capacidad propia, que traducirá los datos de entrada mediante una capa anticorrupción y utilizará un repositorio para la persistencia. El módulo nuevo estará en `FirmaDocs/ImportacionFirmasExternas/`, con sus pruebas en `FirmaDocs.Tests/ImportacionFirmasExternas/`. La lógica de negocio no se ubicará en `Program.cs`.
   - **Alternativa descartada:** acceso directo a SQLite desde `Program.cs`. Se descarta para mantener el límite de capas y evitar lógica de negocio en el punto de composición.
   - **Alternativa descartada:** un servicio REST o acceso independiente a la base. Se descarta porque contradice ADR-0001 y queda fuera del alcance.

3. **Representar explícitamente la declaración externa.** La capacidad tendrá un tipo de dominio `DeclaracionFirmaExterna` que no se modelará como `Firma` ni como `SolicitudFirma`. Su forma exacta y los datos de origen que conserva se concretarán al fijar el formato de entrada y los requisitos de validación.
   - **Alternativa descartada:** reutilizar el tipo `Firma`. Se descarta porque afirmaría una aprobación del circuito que el registro externo no acredita.

## Risks / Trade-offs

- **Esquema de entrada aún sin decidir** → la implementación del lector no debe comenzar hasta definir formato, codificación, columnas e identificadores en la especificación.
- **Relaciones o datos de origen incompletos** → definir en la especificación qué validar y cómo informar cada resultado antes de implementar la persistencia.
- **Errores o duplicados durante una importación** → mantener atomicidad y política de duplicados como decisiones pendientes de requisitos verificables, sin asumir una estrategia en este diseño.
- **Persistencia en una tabla nueva** → incluir una migración versionada y comprobar que la importación no modifica las tablas ni el historial existentes.

## Migration Plan

Añadir una migración SQLite aditiva para la tabla propia de declaraciones y desplegarla junto con la capacidad de importación. La migración no modifica las tablas existentes. La política de reversión de datos importados debe definirse antes de implementar la migración, de forma que revertir el esquema no implique perder declaraciones ya incorporadas.

## Open Questions

- ¿Cuál será el formato, la codificación, las columnas y los identificadores del archivo?
- ¿Qué resultado se informa si falta un Documento, Firmante o Versión relacionado?
- ¿La importación será atómica o permitirá incorporar registros válidos cuando otros fallen?
- ¿Cómo se detectan los duplicados y qué ocurre si la Versión indicada no es la vigente?
- ¿Qué datos concretos identifican y conservan el origen de cada declaración?
