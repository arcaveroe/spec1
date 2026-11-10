# Rechazo y nueva vuelta · spec

## 1. Contexto
Hoy CircuitoFirma permite aprobar en orden y conserva las firmas por versión. El dominio todavía no modela rechazo ni reenvío; sus estados actuales son `Borrador`, `EnCircuito` y `Aprobado`.

La funcionalidad permitirá que el firmante en turno devuelva el documento con un motivo y que su autor lo reenvíe, preservando el historial. Habrá un máximo fijo de tres vueltas; el rechazo en la tercera cierra el documento.

## 2. Historias de usuario
- Como firmante en turno quiero rechazar un documento indicando el motivo para que el autor pueda corregirlo.
- Como autor quiero reenviar un documento devuelto para continuar el circuito de aprobación.
- Como parte interesada quiero conservar las aprobaciones anteriores con su versión para mantener la trazabilidad.

## 3. Requisitos funcionales (EARS)
- RF-01 · CUANDO el firmante en turno rechace indicando un motivo no vacío, el sistema DEBERÁ registrar el rechazo con firmante, motivo, versión e instante. El documento pasará a `Devuelto`, salvo en la tercera vuelta, según RF-04.
- RF-02 · CUANDO el autor reenvíe un documento `Devuelto`, el sistema DEBERÁ conservar el circuito y el orden de firmantes, incrementar la versión en 1, pasar el documento a `EnCircuito` y asignar el turno al primer firmante. SI quien intenta reenviar no coincide con `AutorId`, ENTONCES el sistema DEBERÁ lanzar `CircuitoFirmaException` con el mensaje `Solo el autor puede reenviar el documento.`.
- RF-03 · El sistema DEBERÁ conservar las firmas de versiones anteriores sin modificarlas y NO DEBERÁ contarlas para aprobar la versión vigente.
- RF-04 · SI se rechaza el documento en la tercera vuelta, ENTONCES el sistema DEBERÁ pasar el documento a `Rechazado` y no admitir más reenvíos. El máximo de vueltas será 3.
- RF-05 · SI el motivo de rechazo está vacío o solo contiene espacios, ENTONCES el sistema DEBERÁ lanzar `CircuitoFirmaException` con el mensaje `El motivo de rechazo es obligatorio.`.
- RF-06 · CUANDO rechace alguien que no sea el firmante en turno, el sistema DEBERÁ lanzar `CircuitoFirmaException` con el mismo mensaje que usa `Aprobar` para un firmante fuera de turno.

## 4. Casos límite y errores
- Motivo vacío o compuesto solo por espacios → lanzar la excepción de RF-05; no registrar el rechazo ni cambiar el estado.
- Rechazo por un firmante fuera de turno → lanzar la excepción de RF-06; no registrar el rechazo ni cambiar el estado.
- Reenvío por alguien distinto de `AutorId` → lanzar la excepción de RF-02; no incrementar la versión ni cambiar el estado.
- Reenvío cuando el documento no está `Devuelto` → lanzar `CircuitoFirmaException` con el mensaje `Solo se puede reenviar un documento devuelto.`; no incrementar la versión ni cambiar el estado.
- Rechazo en la tercera vuelta → cambiar a `Rechazado`; no admitir reenvíos posteriores.
- Firmas de versiones anteriores → mantenerlas en el historial sin contarlas para aprobar la versión vigente.
- Rechazo cuando el documento no está `EnCircuito` → lanzar `CircuitoFirmaException` con el mensaje `Solo se puede rechazar un documento en circuito de firma.`; no registrar el rechazo ni cambiar el estado.
- Rechazo y aprobación concurrentes, o reenvío concurrente con otra operación → quedan fuera del alcance; no se garantiza seguridad ni atomicidad entre hilos.

## 5. Criterios de finalización
- CF-01 · `dotnet test` termina en verde.
- CF-02 · Cada RF tiene al menos un test que cita su identificador en el nombre, por ejemplo, con el sufijo `_RF01`.
- CF-03 · Se conservan sin cambios las firmas públicas existentes, incluido el constructor, `Enviar` y `Aprobar`.
- CF-04 · Los miembros públicos nuevos tienen XML docs en español.

## Preguntas abiertas
- ¿La vuelta inicial corresponde a la versión 1 y cada reenvío incrementa tanto la versión como el número de vuelta?
- ¿Cómo se consulta y ordena el historial de rechazos? RF-01 define los datos que se registran, pero no su representación pública.
- El contenido del documento no está modelado actualmente; queda por definir cómo comprobar que cambió antes del reenvío.
