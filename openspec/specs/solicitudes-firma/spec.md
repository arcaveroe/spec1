# solicitudes-firma

## Purpose

Describir el comportamiento actual de los estados de una solicitud de firma y sus transiciones, de acuerdo con FirmaDocs/Legado y los estados permitidos por el esquema de base de datos.

## Requirements

### Requirement: RF-01 Estado PENDIENTE

Cuando una solicitud tenga el estado `PENDIENTE`, el sistema DEBERÁ reconocerlo y conservar su representación textual como `PENDIENTE`.

#### Scenario: Leer una solicitud pendiente

- **GIVEN** una solicitud almacenada con el estado `PENDIENTE`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Pendiente`

### Requirement: RF-02 Estado ESPERANDO_CERTIFICADO

Cuando una solicitud tenga el estado `ESPERANDO_CERTIFICADO`, el sistema DEBERÁ reconocerlo y conservar su representación textual como `ESPERANDO_CERTIFICADO`.

#### Scenario: Leer una solicitud esperando certificado

- **GIVEN** una solicitud almacenada con el estado `ESPERANDO_CERTIFICADO`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `EsperandoCertificado`

### Requirement: RF-03 Estado FIRMADA

Cuando una solicitud tenga el estado `FIRMADA`, el sistema DEBERÁ reconocerlo y conservar su representación textual como `FIRMADA`.

#### Scenario: Leer una solicitud firmada

- **GIVEN** una solicitud almacenada con el estado `FIRMADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Firmada`

### Requirement: RF-04 Estado RECHAZADA

Cuando una solicitud tenga el estado `RECHAZADA`, el sistema DEBERÁ reconocerlo y conservar su representación textual como `RECHAZADA`.

#### Scenario: Leer una solicitud rechazada

- **GIVEN** una solicitud almacenada con el estado `RECHAZADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Rechazada`

### Requirement: RF-05 Estado EXPIRADA

Cuando una solicitud tenga el estado `EXPIRADA`, el sistema DEBERÁ reconocerlo y conservar su representación textual como `EXPIRADA`.

#### Scenario: Leer una solicitud expirada

- **GIVEN** una solicitud almacenada con el estado `EXPIRADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Expirada`

### Requirement: RF-06 Permitir transiciones entre estados admitidos

Cuando se cambie el estado de una solicitud, el sistema DEBERÁ permitir cualquiera de los cinco estados admitidos como nuevo estado, sin restringirlo según el estado anterior.

#### Scenario: Cambiar de cualquier estado admitido a otro

- **GIVEN** una solicitud en cualquiera de los estados admitidos
- **WHEN** se cambia al mismo estado o a cualquiera de los otros estados admitidos
- **THEN** el sistema registra el nuevo estado

### Requirement: RF-07 Registrar la resolución según el nuevo estado

Cuando el nuevo estado sea `FIRMADA`, `RECHAZADA` o `EXPIRADA`, el sistema DEBERÁ asignar a `resuelta_en` el instante recibido para el cambio. Cuando el nuevo estado sea `PENDIENTE` o `ESPERANDO_CERTIFICADO`, el sistema NO DEBERÁ modificar `resuelta_en`.

#### Scenario: Cambiar a un estado final

- **GIVEN** una solicitud cuyo nuevo estado es `FIRMADA`, `RECHAZADA` o `EXPIRADA`
- **WHEN** se cambia su estado con un instante recibido
- **THEN** `resuelta_en` queda asignado a ese instante

#### Scenario: Cambiar a PENDIENTE o ESPERANDO_CERTIFICADO

- **GIVEN** una solicitud cuyo nuevo estado es `PENDIENTE` o `ESPERANDO_CERTIFICADO`
- **WHEN** se cambia su estado
- **THEN** el valor existente de `resuelta_en` no cambia
