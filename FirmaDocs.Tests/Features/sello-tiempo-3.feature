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

