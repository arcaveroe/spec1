## ADDED Requirements

### Requirement: RF-DF01 Delegación acotada

El sistema SHALL aceptar una delegación únicamente cuando identifique un
firmante original, un delegado, un alcance y una vigencia válidos.

#### Scenario: Delegación válida
- **GIVEN** una delegación con firmante original, delegado, alcance y fechas válidos
- **WHEN** se registra en el circuito
- **THEN** el circuito la conserva como autorización diferenciada del firmante original

#### Scenario: Datos incompletos
- **GIVEN** una delegación sin firmante original, delegado, alcance o vigencia
- **WHEN** se intenta registrar
- **THEN** el sistema rechaza la operación sin modificar el circuito

### Requirement: RF-DF02 Habilitación temporal

El sistema SHALL habilitar al delegado solo durante la vigencia de la delegación
y cuando el firmante original esté habilitado en el alcance indicado.

#### Scenario: Delegado dentro de vigencia
- **GIVEN** una delegación vigente cuyo alcance contiene la etapa actual
- **WHEN** se consultan los firmantes habilitados en el instante actual
- **THEN** el delegado aparece como ejecutor autorizado del firmante original

#### Scenario: Delegación caducada
- **GIVEN** una delegación fuera de su vigencia
- **WHEN** el delegado intenta aprobar
- **THEN** el circuito rechaza la aprobación

### Requirement: RF-DF03 Auditoría de la firma delegada

Cuando una aprobación se ejecute mediante delegación, el sistema SHALL conservar
el firmante original y el delegado que ejecutó la operación.

#### Scenario: Aprobación delegada
- **GIVEN** un delegado habilitado para actuar por un firmante original
- **WHEN** registra una aprobación
- **THEN** el historial identifica al firmante original y al delegado ejecutor

#### Scenario: Aprobación no delegada
- **GIVEN** el firmante original actúa por sí mismo
- **WHEN** registra una aprobación
- **THEN** el historial no atribuye la operación a un delegado distinto
