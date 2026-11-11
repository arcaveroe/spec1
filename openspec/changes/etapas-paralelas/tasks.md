## 1. Configuración de etapas

- [x] 1.1 Añadir pruebas de aceptación y límites para cantidad de etapas, firmantes y modos, incluyendo la inmutabilidad de la configuración (RF-01).
- [x] 1.2 Implementar `ModoEtapaFirma`, `EtapaFirma` y la validación de configuración según los límites especificados (RF-01).
- [x] 1.3 Añadir pruebas para firmantes duplicados entre etapas y para la compatibilidad del constructor de lista plana, incluido el límite máximo (RF-02, RF-03).
- [x] 1.4 Implementar la configuración por etapas, la validación de unicidad global y la adaptación de la lista plana como una etapa secuencial (RF-02, RF-03).

## 2. Aprobaciones y avance

- [x] 2.1 Añadir pruebas de orden secuencial y aprobación paralela en cualquier orden, verificando los firmantes habilitados (RF-04, RF-05).
- [ ] 2.2 Derivar los firmantes habilitados y el turno secuencial de las firmas de la versión vigente; validar `Aprobar` contra ese conjunto (RF-04, RF-05).
- [ ] 2.3 Añadir pruebas de avance solo al completar una etapa y de aprobación del circuito al completar la última (RF-06).
- [ ] 2.4 Implementar la transición derivada entre etapas y el estado aprobado al completar el circuito (RF-06).

## 3. Rechazo, reenvío e historial

- [ ] 3.1 Añadir pruebas de rechazo por firmante habilitado desde distintas etapas, reenvío desde la primera etapa y límite de tres vueltas (RF-07).
- [ ] 3.2 Adaptar rechazo y reenvío para usar los firmantes habilitados de la etapa actual, conservar la configuración y reiniciar el avance en la nueva versión (RF-07).
- [ ] 3.3 Añadir pruebas que comprueben que las firmas anteriores se conservan y no completan etapas de la versión vigente (RF-08).
- [ ] 3.4 Asegurar que el cálculo de avance solo cuenta firmas de la versión vigente sin modificar el historial registrado (RF-08).

## 4. Regresión

- [ ] 4.1 Ejecutar las pruebas de la solución y corregir regresiones relacionadas con las aprobaciones secuenciales existentes y las reglas de rechazo y vueltas (RF-03, RF-07, RF-08).
