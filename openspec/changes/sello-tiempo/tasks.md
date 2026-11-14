## 1. Hash del contenido

- [x] 1.1 Añadir pruebas para el hash SHA-256 de los bytes UTF-8 exactos, incluido el valor conocido de `Contrato v2` y contenido con caracteres no ASCII (RF-01, RF-03).
- [x] 1.2 Implementar `HashContenido` para calcular y representar el digest hexadecimal minúsculo sin normalizar el texto; pasar las pruebas del hash (RF-01, RF-03).

## 2. Registro de la Firma sellada

- [x] 2.1 Añadir pruebas para registrar una Firma con hash mediante la operación interna del circuito, validar estado y turno como en `Aprobar` y conservar intacta la API pública existente (RF-01, RF-09).
- [x] 2.2 Implementar el registro interno con hash compartiendo las validaciones y la lógica de transición de `Aprobar`, y asociar el hash opcional a `Firma` sin cambiar firmas públicas existentes (RF-01, RF-09).
- [x] 2.3 Completar y pasar las pruebas de aprobación sellada: solicitar a la TSA el hash calculado y devolver el sello junto con el hash al confirmar la TSA (RF-01).
- [x] 2.4 Completar y pasar las pruebas de fallo de TSA: informar `TSA_NO_DISPONIBLE`, no registrar una Firma ni avanzar el turno, y propagar excepciones no previstas sin mutar el circuito (RF-02, RF-09).
- [x] 2.5 Implementar `ServicioAprobacionSellada` para solicitar el sellado antes de modificar el circuito y registrar la Firma solo tras una confirmación válida (RF-01, RF-02, RF-09).

## 3. Verificación del contenido

- [x] 3.1 Añadir pruebas para contenido coincidente, alterado y Firma sin hash de referencia, comprobando los hashes calculados y el resultado informado (RF-03).
- [x] 3.2 Implementar `VerificadorContenido` para comparar el hash UTF-8 actual con el asociado a la Firma, distinguir la ausencia de referencia y devolver `DOCUMENTO_ALTERADO` cuando difieran (RF-03).

## 4. Regresión

- [x] 4.1 Ejecutar los escenarios Gherkin del cambio y `dotnet test`; corregir regresiones relacionadas con la aprobación sellada, el turno y la verificación de contenido (RF-01, RF-02, RF-03, RF-09).
