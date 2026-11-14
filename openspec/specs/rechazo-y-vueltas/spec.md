# Specification: rechazo-y-vueltas

## Purpose

Definir el rechazo y reenvío de documentos en un circuito de firma, conservando el historial de aprobaciones por versión.
## Requirements
### Requirement: RF-01 Registrar un rechazo

Cuando el firmante en turno rechace un documento indicando un motivo no vacío, el sistema MUST registrar el rechazo con el firmante, motivo, versión e instante, y MUST pasar el documento a `Devuelto`, salvo en la tercera vuelta.

#### Scenario: Rechazo válido

- **GIVEN** un documento en circuito y el firmante habilitado
- **WHEN** el firmante rechaza con un motivo no vacío
- **THEN** el sistema registra el rechazo con sus datos y pasa el documento a `Devuelto`

### Requirement: RF-02 Reenviar un documento devuelto

Cuando el autor reenvíe un documento `Devuelto`, el sistema MUST conservar el circuito y el orden de firmantes, incrementar la versión en 1, pasar el documento a `EnCircuito` y asignar el turno al primer firmante. Si quien intenta reenviar no coincide con `AutorId`, el sistema MUST lanzar `CircuitoFirmaException` con el mensaje `Solo el autor puede reenviar el documento.`.

#### Scenario: Reenvío por el autor

- **GIVEN** un documento devuelto por un rechazo
- **WHEN** el autor lo reenvía
- **THEN** el sistema incrementa la versión, conserva el circuito y habilita al primer firmante

#### Scenario: Reenvío por otra persona

- **GIVEN** un documento devuelto
- **WHEN** una persona distinta del autor intenta reenviarlo
- **THEN** el sistema lanza `CircuitoFirmaException` con el mensaje `Solo el autor puede reenviar el documento.`

### Requirement: RF-03 Conservar firmas de versiones anteriores

El sistema MUST conservar las firmas de versiones anteriores sin modificarlas y MUST NOT contarlas para aprobar la versión vigente.

#### Scenario: Aprobaciones anteriores a un reenvío

- **GIVEN** un documento reenviado con firmas registradas en una versión anterior
- **WHEN** se evalúan las aprobaciones de la versión vigente
- **THEN** solo cuentan las firmas de la versión vigente y las anteriores permanecen en el historial

### Requirement: RF-04 Limitar las vueltas

Si se rechaza el documento en la tercera vuelta, el sistema MUST pasar el documento a `Rechazado` y MUST NOT admitir más reenvíos. El máximo de vueltas MUST ser 3.

#### Scenario: Rechazo en la tercera vuelta

- **GIVEN** un documento en su tercera vuelta
- **WHEN** un firmante habilitado lo rechaza
- **THEN** el documento queda `Rechazado` y no admite un nuevo reenvío

### Requirement: RF-05 Motivo de rechazo mínimo

Cuando un firmante rechace un documento, el sistema MUST exigir que el motivo, una vez eliminados sus espacios iniciales y finales, tenga al menos 10 caracteres. Si no alcanza esa longitud, el sistema MUST lanzar `CircuitoFirmaException` con el mensaje `El motivo de rechazo debe tener al menos 10 caracteres sin contar espacios iniciales ni finales.` y MUST no registrar el rechazo ni cambiar el estado del documento.

#### Scenario: Motivo con diez caracteres significativos

- **GIVEN** un documento en circuito y un firmante habilitado
- **WHEN** el firmante rechaza con un motivo cuyo texto recortado tiene exactamente 10 caracteres
- **THEN** el sistema registra el rechazo y conserva el texto original del motivo

#### Scenario: Motivo con menos de diez caracteres significativos

- **GIVEN** un documento en circuito y un firmante habilitado
- **WHEN** el firmante rechaza con un motivo cuyo texto recortado tiene menos de 10 caracteres
- **THEN** el sistema lanza `CircuitoFirmaException` con el mensaje de longitud mínima, no registra el rechazo y mantiene el estado del documento

#### Scenario: Espacios exteriores no cuentan para el mínimo

- **GIVEN** un documento en circuito y un firmante habilitado
- **WHEN** el firmante rechaza con un motivo que contiene espacios iniciales o finales y 10 caracteres entre ellos
- **THEN** el sistema acepta el rechazo y conserva también esos espacios exteriores en el motivo registrado

### Requirement: RF-06 Rechazar solo desde el turno habilitado

Cuando rechace alguien que no sea un firmante habilitado de la etapa actual, el sistema MUST lanzar `CircuitoFirmaException` con el mismo mensaje que usa `Aprobar` para un firmante fuera de turno y MUST no registrar el rechazo ni cambiar el estado.

#### Scenario: Rechazo fuera de turno

- **GIVEN** un documento en circuito con un firmante habilitado
- **WHEN** otro firmante intenta rechazarlo
- **THEN** el sistema lanza `CircuitoFirmaException` indicando que no es su turno y no modifica el documento

