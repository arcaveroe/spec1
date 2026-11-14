# Specification: etapas-paralelas

## ADDED Requirements

### Requirement: RF-09 Registrar la Firma solo tras confirmación de la TSA

El sistema MUST solicitar a la TSA la confirmación del hash del contenido antes de registrar una Firma en la etapa actual. Si la TSA no responde, el sistema MUST NOT registrar la Firma ni avanzar el turno.

#### Scenario: TSA no disponible durante una aprobación

- **GIVEN** el firmante `Ana` tiene el turno en una etapa secuencial y el contenido de la versión es `Contrato v2`
- **WHEN** la TSA no responde a la solicitud de sellado
- **THEN** el sistema no registra la Firma, mantiene el turno en `Ana` e informa el motivo `TSA_NO_DISPONIBLE`

```gherkin
# language: es
Característica: Exigir confirmación de la TSA antes de avanzar una etapa

  Escenario: La TSA no responde durante una aprobación
    Dado que Ana tiene el turno en una etapa secuencial y el contenido es "Contrato v2"
    Cuando la TSA no responde a la solicitud para sellar el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0"
    Entonces no se registra la Firma, el turno permanece en "Ana" y el motivo es "TSA_NO_DISPONIBLE"
```
