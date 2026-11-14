## 1. Modelo y reglas de delegación

- [ ] 1.1 Definir el contrato de delegación, su alcance y vigencia, citando RF-DF01 y RF-DF02; añadir pruebas de validación en `FirmaDocs.Tests/Circuito/`.
- [ ] 1.2 Implementar el tipo de dominio de delegación en `FirmaDocs/Circuito/` sin cambiar firmas públicas existentes, citando RF-DF01.

## 2. Integración con el circuito

- [ ] 2.1 Añadir pruebas de habilitación en vigor, caducidad y alcance, citando RF-DF02.
- [ ] 2.2 Integrar la delegación en la aprobación y conservar la auditoría del ejecutor, citando RF-DF03.
- [ ] 2.3 Ejecutar `dotnet test` y verificar que las pruebas existentes del circuito permanecen verdes.
