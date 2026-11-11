# Specification: etapas-paralelas

## Purpose

Definir la configuración y aprobación de circuitos organizados en etapas secuenciales o paralelas, manteniendo la compatibilidad con los circuitos actuales y las reglas vigentes de rechazo, versiones e historial.

## Requirements

### Requirement: RF-01 Configurar etapas y firmantes

El sistema DEBERÁ permitir configurar un circuito con entre 1 y 5 etapas. Cada etapa DEBERÁ contener entre 1 y 5 firmantes y tener un modo `SECUENCIAL` o `PARALELA`.

#### Scenario: Configuración dentro de los límites

- **GIVEN** un circuito con entre 1 y 5 etapas y entre 1 y 5 firmantes por etapa
- **WHEN** cada etapa tiene un modo `SECUENCIAL` o `PARALELA`
- **THEN** el sistema acepta la configuración del circuito

#### Scenario: Configuración fuera de los límites

- **GIVEN** una configuración con cero o más de cinco etapas, o con una etapa sin firmantes o con más de cinco
- **WHEN** se intenta configurar el circuito
- **THEN** el sistema rechaza la configuración

#### Scenario: Modo no admitido

- **GIVEN** una etapa cuyo modo no es `SECUENCIAL` ni `PARALELA`
- **WHEN** se intenta configurar el circuito
- **THEN** el sistema rechaza la configuración

### Requirement: RF-02 Evitar firmantes repetidos entre etapas

El sistema NO DEBERÁ aceptar un circuito en el que el mismo firmante aparezca en más de una etapa.

#### Scenario: Firmante repetido en otra etapa

- **GIVEN** un firmante incluido en una etapa del circuito
- **WHEN** se configura otra etapa con ese mismo firmante
- **THEN** el sistema rechaza la configuración

#### Scenario: Firmantes distintos en todas las etapas

- **GIVEN** etapas cuyos firmantes no se repiten entre sí
- **WHEN** se configura el circuito
- **THEN** el sistema acepta la configuración si cumple los demás requisitos

### Requirement: RF-03 Mantener compatibilidad con la lista plana

El sistema DEBERÁ mantener el comportamiento de los circuitos creados con la lista plana vigente de firmantes, tratándolos como una sola etapa `SECUENCIAL` en el mismo orden configurado.

#### Scenario: Circuito existente con lista plana

- **GIVEN** un circuito creado con la lista plana de firmantes
- **WHEN** se inicia su aprobación
- **THEN** el sistema solicita las aprobaciones en el orden de esa lista como una única etapa secuencial

### Requirement: RF-04 Aprobar una etapa secuencial en orden

Mientras la etapa actual sea `SECUENCIAL`, el sistema DEBERÁ permitir aprobar únicamente al siguiente firmante pendiente en el orden configurado.

#### Scenario: Aprueba el firmante siguiente

- **GIVEN** una etapa secuencial con un firmante como siguiente en el orden
- **WHEN** ese firmante aprueba
- **THEN** el sistema registra su aprobación y avanza el turno al siguiente firmante pendiente de la etapa

#### Scenario: Intenta aprobar fuera de orden

- **GIVEN** una etapa secuencial con un firmante anterior todavía pendiente
- **WHEN** un firmante posterior intenta aprobar
- **THEN** el sistema rechaza la aprobación y no la registra

### Requirement: RF-05 Aprobar una etapa paralela en cualquier orden

Mientras la etapa actual sea `PARALELA`, el sistema DEBERÁ permitir que sus firmantes aprueben en cualquier orden y DEBERÁ requerir la aprobación de todos ellos para completar la etapa.

#### Scenario: Firmante de la etapa paralela aprueba

- **GIVEN** una etapa paralela con aprobaciones pendientes
- **WHEN** uno de sus firmantes aprueba
- **THEN** el sistema registra esa aprobación independientemente del orden de los demás firmantes

#### Scenario: Quedan aprobaciones pendientes

- **GIVEN** una etapa paralela en la que al menos un firmante aún no ha aprobado
- **WHEN** se consulta el avance del circuito
- **THEN** la etapa permanece incompleta

### Requirement: RF-06 Avanzar solo al completar la etapa actual

El sistema DEBERÁ iniciar la etapa siguiente únicamente cuando todos los firmantes de la etapa actual hayan aprobado. Cuando todos los firmantes de la última etapa hayan aprobado, el sistema DEBERÁ completar el circuito como aprobado.

#### Scenario: Todas las aprobaciones de la etapa actual están registradas

- **GIVEN** una etapa actual cuyos firmantes ya aprobaron
- **WHEN** se registra la última aprobación pendiente
- **THEN** el sistema inicia la etapa siguiente si existe

#### Scenario: Queda una aprobación pendiente en la etapa actual

- **GIVEN** una etapa actual con al menos un firmante pendiente
- **WHEN** otro firmante de esa etapa aprueba
- **THEN** el sistema no inicia la etapa siguiente

#### Scenario: Se completa la última etapa

- **GIVEN** que todos los firmantes de la última etapa aprobaron
- **WHEN** se registra la última aprobación pendiente
- **THEN** el sistema marca el circuito como aprobado

### Requirement: RF-07 Rechazar y reenviar desde cualquier etapa

El sistema DEBERÁ permitir el rechazo por un firmante habilitado de la etapa actual, aplicando las reglas vigentes de rechazo. Al reenviar un documento devuelto, DEBERÁ iniciar la nueva vuelta desde la primera etapa y conservar el circuito configurado. Al alcanzar el máximo vigente de tres vueltas, el documento DEBERÁ quedar rechazado y no admitir más reenvíos.

#### Scenario: Rechazo durante cualquier etapa

- **GIVEN** un documento en circuito y un firmante habilitado de la etapa actual
- **WHEN** ese firmante rechaza el documento con un motivo no vacío
- **THEN** el sistema devuelve el documento conforme a las reglas vigentes de rechazo

#### Scenario: Reenvío tras un rechazo

- **GIVEN** un documento devuelto con una nueva versión
- **WHEN** su autor lo reenvía
- **THEN** el sistema conserva las etapas y modos configurados e inicia la aprobación en la primera etapa

#### Scenario: Rechazo en la tercera vuelta

- **GIVEN** un documento que alcanza la tercera vuelta
- **WHEN** se rechaza conforme a las reglas vigentes
- **THEN** el documento queda rechazado y el sistema no admite un nuevo reenvío

### Requirement: RF-08 Conservar versiones e historial de aprobaciones

El sistema DEBERÁ conservar las firmas registradas en versiones anteriores sin modificarlas y NO DEBERÁ contarlas para completar las etapas de la versión vigente.

#### Scenario: Aprobaciones anteriores a un reenvío

- **GIVEN** un documento reenviado con firmas registradas en una versión anterior
- **WHEN** se evalúan las aprobaciones de sus etapas vigentes
- **THEN** solo cuentan las aprobaciones de la versión vigente y las firmas anteriores permanecen en el historial
