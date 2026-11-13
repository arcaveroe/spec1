# ADR-0001 · La consola es el único proceso con acceso a la base de datos

- Estado: Accepted
- Fecha: 2026-11-12 · Decisores: no especificados
- Reemplaza a: —

## Contexto y problema
FirmaDocs necesita definir qué procesos pueden leer y escribir `firmasimple.db`. La base es SQLite y se accede mediante `Microsoft.Data.Sqlite`; los cambios de esquema se gestionan con scripts versionados en `db/migrations`. `CircuitoFirma` conserva su estado en memoria y no forma parte de esta persistencia.

Se requiere un límite único y claro para el acceso a los datos, evitando que varios consumidores gestionen directamente la base o dependan de copias exportadas.

## Opciones consideradas
1. La aplicación de consola FirmaDocs es el único proceso que lee y escribe `firmasimple.db`.
2. Un servicio REST único centraliza la lectura y escritura; la consola y otros consumidores acceden a través de su API.
3. Compartir archivos JSON exportados para distribuir los datos entre consumidores.

## Decisión
Elegimos la opción 1: la aplicación de consola FirmaDocs es el único proceso autorizado a leer y escribir `firmasimple.db`, usando SQLite con `Microsoft.Data.Sqlite` y scripts de esquema versionados en `db/migrations`. `CircuitoFirma` continúa en memoria.

Esta opción mantiene el alcance y la operación de la aplicación simples, y evita introducir un servicio o un mecanismo de sincronización de archivos. El acceso a los datos queda centralizado en la consola.

## Consecuencias
- Positivas: un solo proceso es responsable de las lecturas y escrituras; se evita mantener una API REST o sincronizar exportaciones JSON; el circuito de firma no queda acoplado a la persistencia.
- Negativas: nadie puede consultar la bandeja ni el estado de una solicitud sin abrir la consola; SQLite no admite escritura concurrente; modificar el `CHECK` requiere reconstruir la tabla mediante una migración, no puede hacerse en sitio (véase la sesión 3).
