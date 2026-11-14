# solicitudes-firma

## Purpose

Describir los estados admitidos de una solicitud de firma, sus transiciones y el tratamiento de las solicitudes migradas tras retirar `ESPERANDO_CERTIFICADO`.

## Requirements

### Requirement: RF-01 Estado PENDIENTE

Cuando una solicitud tenga el estado `PENDIENTE`, el sistema MUST reconocerlo y conservar su representación textual como `PENDIENTE`.

#### Scenario: Leer una solicitud pendiente

- **GIVEN** una solicitud almacenada con el estado `PENDIENTE`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Pendiente`

### Requirement: RF-02 Retirar el estado ESPERANDO_CERTIFICADO

El sistema MUST NOT admitir `ESPERANDO_CERTIFICADO` como estado de una solicitud.

#### Scenario: Intentar asignar el estado retirado

- **GIVEN** una solicitud en cualquiera de los estados admitidos
- **WHEN** se intenta cambiar su estado a `ESPERANDO_CERTIFICADO`
- **THEN** el sistema rechaza el cambio

### Requirement: RF-03 Estado FIRMADA

Cuando una solicitud tenga el estado `FIRMADA`, el sistema MUST reconocerlo y conservar su representación textual como `FIRMADA`.

#### Scenario: Leer una solicitud firmada

- **GIVEN** una solicitud almacenada con el estado `FIRMADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Firmada`

### Requirement: RF-04 Estado RECHAZADA

Cuando una solicitud tenga el estado `RECHAZADA`, el sistema MUST reconocerlo y conservar su representación textual como `RECHAZADA`.

#### Scenario: Leer una solicitud rechazada

- **GIVEN** una solicitud almacenada con el estado `RECHAZADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Rechazada`

### Requirement: RF-05 Estado EXPIRADA

Cuando una solicitud tenga el estado `EXPIRADA`, el sistema MUST reconocerlo y conservar su representación textual como `EXPIRADA`.

#### Scenario: Leer una solicitud expirada

- **GIVEN** una solicitud almacenada con el estado `EXPIRADA`
- **WHEN** el sistema lee la solicitud
- **THEN** la expone con el estado `Expirada`

### Requirement: RF-06 Permitir transiciones entre estados admitidos

Cuando se cambie el estado de una solicitud, el sistema MUST permitir cualquiera de los cuatro estados admitidos (`PENDIENTE`, `FIRMADA`, `RECHAZADA` o `EXPIRADA`) como nuevo estado, sin restringirlo según el estado anterior.

#### Scenario: Cambiar entre estados admitidos

- **GIVEN** una solicitud en cualquiera de los estados admitidos
- **WHEN** se cambia al mismo estado o a cualquiera de los otros estados admitidos
- **THEN** el sistema registra el nuevo estado

### Requirement: RF-07 Registrar la resolución según el nuevo estado

Cuando el nuevo estado sea `FIRMADA`, `RECHAZADA` o `EXPIRADA`, el sistema MUST asignar a `resuelta_en` el instante recibido para el cambio. Cuando el nuevo estado sea `PENDIENTE`, el sistema MUST NOT modificar `resuelta_en`.

#### Scenario: Cambiar a un estado final

- **GIVEN** una solicitud cuyo nuevo estado es `FIRMADA`, `RECHAZADA` o `EXPIRADA`
- **WHEN** se cambia su estado con un instante recibido
- **THEN** `resuelta_en` queda asignado a ese instante

#### Scenario: Cambiar a PENDIENTE

- **GIVEN** una solicitud cuyo nuevo estado es `PENDIENTE`
- **WHEN** se cambia su estado
- **THEN** el valor existente de `resuelta_en` no cambia

### Requirement: RF-08 Migrar solicitudes existentes

Al migrar solicitudes con estado `ESPERANDO_CERTIFICADO`, el sistema MUST conservar cada solicitud, cambiar su estado a `PENDIENTE` y registrar `ESPERANDO_CERTIFICADO` como su estado de origen en `migrado_desde`.

#### Scenario: Migrar solicitudes que esperan certificado

- **GIVEN** una solicitud con estado `ESPERANDO_CERTIFICADO`
- **WHEN** se ejecuta la migración
- **THEN** la solicitud permanece registrada con estado `PENDIENTE` y `migrado_desde` igual a `ESPERANDO_CERTIFICADO`

### Requirement: RF-09 Incluir solicitudes pendientes en la bandeja

La bandeja del firmante MUST clasificar las solicitudes migradas a `PENDIENTE` en `PorFirmar` y MUST NOT mostrarlas en una categoría separada de espera por certificado.

#### Scenario: Consultar la bandeja tras la migración

- **GIVEN** una solicitud previamente `ESPERANDO_CERTIFICADO` que fue migrada a `PENDIENTE`
- **WHEN** el firmante consulta su bandeja
- **THEN** la solicitud aparece en `PorFirmar` y no aparece en una categoría de espera por certificado

### Requirement: RF-10 No enviar recordatorios de certificado

El sistema MUST NOT enviar el aviso «Instala tu certificado digital» asociado al estado `ESPERANDO_CERTIFICADO`.

#### Scenario: Procesar recordatorios tras retirar el estado

- **GIVEN** que no se admiten solicitudes en estado `ESPERANDO_CERTIFICADO`
- **WHEN** se procesan los recordatorios
- **THEN** el sistema no envía el aviso «Instala tu certificado digital»

### Requirement: RF-11 Incluir solicitudes migradas en el SLA

El reporte de SLA MUST incluir las solicitudes migradas a `PENDIENTE` y calcular su duración desde su instante original de creación (`creada_en`).

#### Scenario: Calcular el SLA de una solicitud migrada

- **GIVEN** una solicitud migrada de `ESPERANDO_CERTIFICADO` a `PENDIENTE`
- **WHEN** se calcula el reporte de SLA
- **THEN** la solicitud se incluye en el cálculo desde su `creada_en` original
