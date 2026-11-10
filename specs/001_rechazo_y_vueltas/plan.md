# Plan · Rechazo y nueva vuelta

Este plan cubre RF-01 a RF-06 de [spec.md](./spec.md). Es una propuesta para aprobación antes de implementar. La spec deja varias decisiones abiertas; las señaladas como pendientes deberán acordarse antes de fijar sus pruebas.

## 1. Archivos a crear o modificar

- **Crear `FirmaDocs/Rechazo.cs`**: valor del historial de rechazos.
- **Modificar `FirmaDocs/EstadoDocumento.cs`**: añadir los estados `Devuelto` y `Rechazado`.
- **Modificar `FirmaDocs/CircuitoFirma.cs`**: registrar rechazos, reenviar por el autor, mantener el historial y calcular el turno solo con firmas de la versión vigente. Conservar intactos el constructor, `Enviar` y `Aprobar`.
- **Modificar `FirmaDocs.Tests/CircuitoFirmaTests.cs`**: añadir pruebas de los RF y regresiones, con el identificador RF al final del nombre de cada prueba.
- **Crear `specs/001_rechazo_y_vueltas/plan.md`**: este plan.

No se prevén cambios en `Program.cs`, proyectos ni dependencias.

## 2. Tipos y miembros públicos nuevos

- **`Rechazo`**, record público en `FirmaDocs/Rechazo.cs`, con `Firmante`, `Motivo`, `Version` e `Instante`. Documentar en español el tipo y sus parámetros.
- **`EstadoDocumento.Devuelto`** y **`EstadoDocumento.Rechazado`**.
- En `CircuitoFirma`:
  - `IReadOnlyList<Rechazo> Rechazos`, historial de rechazos en orden cronológico.
  - `void Rechazar(Firmante firmante, string motivo, DateTimeOffset instante)`.
  - `void Reenviar(string solicitanteId)`, para validar la identidad del autor.

Documentar en español todos los miembros públicos nuevos. El máximo de tres vueltas será una regla interna fija, no una opción pública. La propuesta trata `Version` como número de vuelta: comienza en 1 y cada reenvío la incrementa en 1; debe confirmarse esta equivalencia antes de implementar, pues figura entre las preguntas abiertas de la spec.

## 3. Cálculo del turno y firmas anteriores

El índice del firmante en turno será la cantidad de firmas cuyo `Version` coincide con la versión vigente:

`índice = Firmas.Count(firma => firma.Version == Version)`

Si el documento está `EnCircuito`, `FirmanteEnTurno` será el firmante en ese índice; en otros estados seguirá siendo `null`. Al aprobar, se determinará si termina la versión comparando esa misma cantidad —incluida la nueva firma— con el número de firmantes, no el total histórico de `_firmas`.

Así, las firmas de versiones anteriores permanecen intactas y consultables, pero no adelantan el turno ni completan las aprobaciones de la versión actual. Después de reenviar, la versión nueva no tiene firmas y el turno vuelve al primer firmante.

## 4. Alternativas descartadas

1. **Borrar las firmas al rechazar o reenviar.** Se descarta porque contradice RF-03 y el principio de evidencia inmutable de la constitución; también elimina la trazabilidad entre versiones.
2. **Llevar un contador de turno acumulado sobre todas las firmas históricas.** Se descarta porque las firmas anteriores no acreditan la versión vigente: produciría un turno incorrecto al reenviar y podría dejar el índice fuera de los firmantes disponibles. El filtrado por versión usa el historial existente como fuente de verdad sin duplicar estado.

## 5. Riesgos y mitigaciones

- **Confundir versión con vuelta:** podría cerrar el documento en un momento distinto al esperado. Confirmar antes de implementar que la versión inicial es la vuelta 1 y que cada reenvío consume una vuelta.
- **Regresiones en el orden o cierre de aprobaciones:** filtrar solo el turno, pero dejar el total histórico en la condición de aprobación, causaría un cierre prematuro. Usar el mismo conteo por versión para turno y finalización; añadir pruebas de reenvío con firmas previas.
- **Mutaciones ante errores:** un motivo inválido, un firmante fuera de turno o un solicitante que no sea el autor no debe alterar estado, versión ni historiales. Validar todas las precondiciones antes de agregar un rechazo o incrementar la versión, y comprobarlo en pruebas.
- **Estados inválidos y concurrencia:** rechazar fuera de `EnCircuito` lanza `CircuitoFirmaException` con el mensaje `Solo se puede rechazar un documento en circuito de firma.`. Las operaciones concurrentes quedan fuera del alcance; no se garantiza seguridad ni atomicidad entre hilos. El mensaje al reenviar fuera de `Devuelto` sigue pendiente.
- **Cambios de compatibilidad:** no modificar firmas públicas existentes ni añadir paquetes. Verificarlo con compilación y la suite existente sin alterar pruebas previas.

## 6. Orden de implementación

1. Aprobar este plan y resolver las preguntas abiertas restantes de la spec que afecten el contrato: versión/vuelta, conservación del circuito, comportamiento del reenvío en estados inválidos y contenido del documento.
2. Añadir primero pruebas fallidas para RF-01 a RF-06, incluidos los casos de error y las firmas previas; conservar intactas las pruebas existentes.
3. Añadir el record `Rechazo` y los estados `Devuelto` y `Rechazado`, con XML docs en español.
4. Implementar `Rechazar`: validar estado, turno y motivo; registrar firmante, motivo, versión e instante; devolver el documento, salvo al alcanzar la tercera vuelta, cuando quedará `Rechazado`.
5. Implementar `Reenviar`: validar estado e identidad del autor; incrementar la versión y devolver el documento a `EnCircuito`, empezando por el primer firmante.
6. Ajustar el cálculo del turno y la condición de aprobación para contar únicamente firmas de la versión vigente, preservando el historial completo.
7. Ejecutar `dotnet build` y `dotnet test`; corregir el código si falla alguna prueba, sin cambiar pruebas existentes para hacerlas pasar.
