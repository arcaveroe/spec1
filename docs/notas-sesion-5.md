# Notas de sesión 5

Score inicial CircuitoFirma: 62,86 % (22 killed, 5 survived, 8 no coverage, 27 compile error)

## Clasificación de sobrevivientes

- **Borde** (`>=` ↔ `>`, `+1`): faltan casos con el valor límite exacto, mediante `[InlineData]`: vuelta 3, cinco firmantes por etapa y exactamente dos días hábiles.
- **Mensaje** (string vacío o cambiado): las aserciones no comprueban el texto exacto de la excepción ni el motivo esperado.
- **Interacción** (llamada eliminada): falta verificar con `Received()` que el sustituto recibió la llamada esperada.
- **Lógica** (`&&` ↔ `||`, condición negada): las pruebas deben distinguir una etapa paralela con firmantes pendientes de una etapa sin pendientes.
