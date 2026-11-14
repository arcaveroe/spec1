# language: es
Característica: Exigir confirmación de la TSA antes de avanzar una etapa

  Escenario: La TSA no responde durante una aprobación
    Dado que Ana tiene el turno en una etapa secuencial y el contenido es "Contrato v2"
    Cuando la TSA no responde a la solicitud para sellar el hash "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0"
    Entonces no se registra la Firma, el turno permanece en "Ana" y el motivo es "TSA_NO_DISPONIBLE"

