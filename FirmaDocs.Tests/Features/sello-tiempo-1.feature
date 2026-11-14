# language: es
Característica: Registrar una Firma con hash de contenido confirmado por TSA

  Escenario: La TSA confirma el hash del contenido
    Dado que la versión 2 contiene "Contrato v2"
    Cuando la TSA confirma el hash y devuelve el sello "TSA-000123"
    Entonces se registra la Firma con el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0" y el resultado incluye el sello "TSA-000123"

