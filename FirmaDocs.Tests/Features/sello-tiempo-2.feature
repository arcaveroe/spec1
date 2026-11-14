# language: es
Característica: Conservar el circuito si la TSA no responde

  Escenario: La TSA no responde durante el registro
    Dado que Ana tiene el turno para la versión 2 y su contenido es "Contrato v2"
    Cuando la TSA no responde a la solicitud de sellado
    Entonces no se registra la Firma, el turno permanece en "Ana" y el motivo es "TSA_NO_DISPONIBLE"

