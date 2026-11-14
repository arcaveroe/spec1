## MODIFIED Requirements

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
