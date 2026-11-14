# Specification: sello-tiempo

## Purpose

Definir cómo se asocia el hash de contenido de una versión con una Firma tras la confirmación de una TSA y cómo se verifica ese contenido posteriormente.

## ADDED Requirements

### Requirement: RF-01 Registrar el hash de contenido confirmado por la TSA

El sistema DEBERÁ calcular el hash SHA-256 de los bytes UTF-8 del contenido de la versión y solicitar su sellado a una TSA antes de registrar la Firma. Solo DEBERÁ registrar la Firma si la TSA confirma el hash solicitado, y DEBERÁ asociar ese hash con la Firma.

#### Scenario: La TSA confirma el hash del contenido

- **GIVEN** la versión 2 contiene `Contrato v2`
- **WHEN** la TSA confirma el hash solicitado y devuelve el sello `TSA-000123`
- **THEN** el sistema registra la Firma con el hash `74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0` y el resultado incluye el sello `TSA-000123`

```gherkin
# language: es
Característica: Registrar una Firma con hash de contenido confirmado por TSA

  Escenario: La TSA confirma el hash del contenido
    Dado que la versión 2 contiene "Contrato v2"
    Cuando la TSA confirma el hash y devuelve el sello "TSA-000123"
    Entonces se registra la Firma con el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0" y el resultado incluye el sello "TSA-000123"
```

### Requirement: RF-02 No registrar la Firma si la TSA no responde

Si la TSA no responde, el sistema NO DEBERÁ registrar la Firma ni avanzar el turno, y DEBERÁ informar el motivo `TSA_NO_DISPONIBLE`.

#### Scenario: La TSA no responde durante el registro

- **GIVEN** el firmante `Ana` tiene el turno para la versión 2, cuyo contenido es `Contrato v2`
- **WHEN** la TSA no responde al solicitar el sellado
- **THEN** el sistema no registra la Firma, mantiene el turno en `Ana` e informa el motivo `TSA_NO_DISPONIBLE`

```gherkin
# language: es
Característica: Conservar el circuito si la TSA no responde

  Escenario: La TSA no responde durante el registro
    Dado que Ana tiene el turno para la versión 2 y su contenido es "Contrato v2"
    Cuando la TSA no responde a la solicitud de sellado
    Entonces no se registra la Firma, el turno permanece en "Ana" y el motivo es "TSA_NO_DISPONIBLE"
```

### Requirement: RF-03 Verificar el contenido de la versión asociada a una Firma

El sistema DEBERÁ verificar el contenido calculando su hash SHA-256 sobre los bytes UTF-8 y comparándolo con el hash asociado a la Firma de esa misma versión. Si los hashes difieren, DEBERÁ informar el motivo `DOCUMENTO_ALTERADO`.

#### Scenario: El contenido de la versión coincide con el hash de la Firma

- **GIVEN** la Firma de la versión 2 está asociada al contenido `Contrato v2` y al hash `74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0`
- **WHEN** se verifica el contenido `Contrato v2`
- **THEN** el sistema informa que el contenido coincide con el hash `74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0`

#### Scenario: El contenido de la versión fue alterado

- **GIVEN** la Firma de la versión 2 está asociada al hash `74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0`
- **WHEN** se verifica el contenido cambiado `Contrato v2 bis`
- **THEN** el sistema informa que el hash calculado es `a6577c3f7f8e59f087527d5fc31e76788300e87a9ecdd863593ad99f90967803`, conserva como hash asociado `74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0` y devuelve el motivo `DOCUMENTO_ALTERADO`

```gherkin
# language: es
Característica: Verificar el contenido asociado a una Firma

  Escenario: El contenido original coincide con el hash de la Firma
    Dado que la Firma de la versión 2 tiene el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0"
    Cuando se verifica el contenido "Contrato v2"
    Entonces el contenido coincide con el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0"

  Escenario: El contenido cambiado no coincide con el hash de la Firma
    Dado que la Firma de la versión 2 tiene el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0"
    Cuando se verifica el contenido cambiado "Contrato v2 bis"
    Entonces el hash calculado es "a6577c3f7f8e59f087527d5fc31e76788300e87a9ecdd863593ad99f90967803" y el motivo es "DOCUMENTO_ALTERADO"
```
