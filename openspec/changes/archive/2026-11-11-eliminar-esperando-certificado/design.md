# Design: Eliminar ESPERANDO_CERTIFICADO

## Contexto

La tabla `solicitudes_firma` admite `ESPERANDO_CERTIFICADO` en su `CHECK`, y ese estado todavía forma parte del enum y de tres consumidores del legado. La base local contiene 250 solicitudes en ese estado. La retirada debe conservarlas, tratarlas como `PENDIENTE` y registrar el estado anterior en `migrado_desde`, manteniendo el índice por estado y las firmas públicas existentes.

## Decisiones

### Migración de datos

La migración se implementará como una reconstrucción transaccional de `solicitudes_firma`, porque SQLite no permite modificar directamente un `CHECK` existente. El script de avance y el de rollback serán repetibles: cada ejecución construye una tabla de reemplazo desde la tabla vigente, copia todas las columnas y filas, sustituye la tabla y recrea el índice. `BEGIN IMMEDIATE` mantiene el reemplazo atómico; un fallo revierte la transacción y conserva la tabla original.

#### Avance

1. Crear `solicitudes_firma_nueva` con las mismas columnas, tipos, nulabilidad y clave primaria, pero con un `CHECK` que solo admita `PENDIENTE`, `FIRMADA`, `RECHAZADA` y `EXPIRADA`.
2. Copiar todas las filas y columnas. Para cada fila cuyo estado sea `ESPERANDO_CERTIFICADO`, escribir `PENDIENTE` en `estado` y `ESPERANDO_CERTIFICADO` en `migrado_desde`; para las demás filas, conservar ambos valores. No se eliminan solicitudes ni se alteran sus fechas.
3. Eliminar `solicitudes_firma`, renombrar `solicitudes_firma_nueva` a `solicitudes_firma` y recrear `ix_solicitudes_estado` sobre `estado`.

El script elimina primero cualquier tabla de reemplazo residual dentro de la transacción (`DROP TABLE IF EXISTS solicitudes_firma_nueva`). Al volver a ejecutarlo, copia el esquema ya migrado y los marcadores existentes sin volver a transformar filas; por tanto, el resultado de datos y esquema permanece igual.

#### Rollback

El rollback reconstruirá la tabla con el `CHECK` original, que incluye `ESPERANDO_CERTIFICADO`. Al copiar filas, restaurará `estado` a `ESPERANDO_CERTIFICADO` cuando `migrado_desde` tenga ese valor y conservará los demás estados y columnas. Luego eliminará la tabla actual, renombrará la tabla reconstruida y recreará `ix_solicitudes_estado`. Mantener `migrado_desde` permite auditar el origen; volver a ejecutar el rollback produce el mismo estado.

#### Prueba local del rollback

La prueba se hará exclusivamente sobre una copia de la base local:

1. Copiar `firmasimple.db` a `firmasimple.rollback-test.db`.
2. Aplicar el script de avance y comprobar que el total de filas no cambia, no queda ningún estado `ESPERANDO_CERTIFICADO`, las 250 filas afectadas pasan a `PENDIENTE` con `migrado_desde = 'ESPERANDO_CERTIFICADO'`, el `CHECK` ya no admite ese estado y existe `ix_solicitudes_estado`.
3. Aplicar el rollback a esa misma copia. Comprobar que el total y los demás datos se conservan, las 250 filas vuelven a `ESPERANDO_CERTIFICADO`, el `CHECK` original vuelve a admitirlo y el índice existe.
4. Ejecutar de nuevo el rollback y comprobar que no cambia datos ni esquema. Comparar con la base original los identificadores, datos de negocio y fechas; para las 250 filas afectadas, comprobar además el estado restaurado y que `migrado_desde` conserva la marca de origen. La base `firmasimple.db` original no se modifica.

### Consumidores

Cada consumidor se actualizará y probará de forma independiente, con una tarea por consumidor:

- **Bandeja (`BandejaFirmante`, RF-09):** clasificar las solicitudes migradas a `PENDIENTE` en `PorFirmar` y verificar que no aparezcan en `EsperandoCertificado`. Conservar la firma pública de `Bandeja`.
- **Recordatorios (`RecordatorioService`, RF-10):** retirar la consulta y el envío del mensaje «Instala tu certificado digital»; verificar que no se envíe ese aviso y que se mantenga el recordatorio normal de pendientes.
- **SLA (`ReporteSla`, RF-11):** incluir las solicitudes migradas en el cálculo desde `CreadaEn`; verificar la agrupación por mes y la duración respecto de la fecha original.

### Estado y persistencia

- Retirar `EsperandoCertificado` del enum y de las conversiones de `EstadoSolicitudTexto`; ninguna lectura o escritura debe requerir el estado retirado.
- Actualizar el esquema vigente y los datos de seed al nuevo conjunto de estados. Mantener `001_inicial.sql` y `002_indice_estado.sql` como historial y añadir la migración de avance y su rollback.
- Mantener las firmas públicas existentes. El repositorio seguirá leyendo y escribiendo `migrado_desde` y el comportamiento de los otros estados no cambiará.

## Módulos afectados

- `FirmaDocs/Legado/EstadoSolicitud.cs` y `SolicitudFirmaRepository.cs`: representación y persistencia de estados.
- `FirmaDocs/Legado/BandejaFirmante.cs`, `RecordatorioService.cs` y `ReporteSla.cs`: tareas de consumidores descritas arriba.
- `db/schema.sql`, `db/seed.sql` y nuevas migraciones en `db/migrations/`: esquema, datos de ejemplo, avance y rollback.
- `FirmaDocs.Tests/Legado/`: pruebas focalizadas para cada consumidor y para la migración/rollback sobre una base temporal.

## Alternativas descartadas

- **Actualizar filas sin reconstruir la tabla:** el `CHECK` existente seguiría admitiendo el estado retirado.
- **Eliminar las solicitudes afectadas:** perdería datos e historial; la migración debe preservar las 250 filas.
- **Dejar el estado antiguo en el enum y convertirlo al leerlo:** mantendría el estado retirado como parte del contrato de aplicación y permitiría que los consumidores siguieran dependiendo de él.
- **Modificar o reemplazar migraciones históricas:** haría que la secuencia de migración existente no fuese reproducible; se conserva el historial y se añade una migración nueva.
