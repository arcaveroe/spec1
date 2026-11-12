# Specification: solicitudes-firma

## Purpose

Definir la retirada del estado `ESPERANDO_CERTIFICADO`, la migración de las solicitudes existentes y su nuevo tratamiento en la bandeja, los recordatorios y el cálculo del SLA.

## MODIFIED Requirements

### Requirement: RF-02 Estado ESPERANDO_CERTIFICADO

El sistema MUST NOT admitir `ESPERANDO_CERTIFICADO` como estado de una solicitud.

#### Scenario: Intentar asignar el estado retirado

- **GIVEN** una solicitud en cualquiera de los estados admitidos
- **WHEN** se intenta cambiar su estado a `ESPERANDO_CERTIFICADO`
- **THEN** el sistema rechaza el cambio

## ADDED Requirements

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
