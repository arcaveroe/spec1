## 1. Tests de validación del motivo (RF-05)

- [x] 1.1 Añadir un test de frontera para aceptar exactamente 10 caracteres significativos y conservar los espacios exteriores (RF-05).
- [x] 1.2 Añadir un test para rechazar un motivo con 9 caracteres significativos, verificando excepción, mensaje, estado e historial (RF-05).
- [x] 1.3 Añadir un test para rechazar un motivo compuesto solo por espacios o vacío con el nuevo mensaje y las invariantes existentes (RF-05).

## 2. Implementación de dominio (RF-05)

- [x] 2.1 Actualizar la validación de `CircuitoFirma.Rechazar` para medir `Trim().Length` con mínimo 10, conservando el texto original y el orden de validaciones (RF-05).
- [x] 2.2 Ejecutar `dotnet test` y corregir cualquier regresión directamente relacionada con RF-05.

## 3. Verificación de mutaciones (RF-05)

- [x] 3.1 Ejecutar Stryker sobre la validación del motivo y confirmar que las mutaciones relevantes quedan cubiertas (RF-05).
- [x] 3.2 Revisar el log de ejecución y documentar que no queda el patrón de validación anterior sin detectar (RF-05).

> Verificación Stryker: la ejecución enfocada en `CircuitoFirma.cs` mató las 7 mutaciones de la condición RF-05, incluida la mutación lógica que sustituye `||` por `&&`; no queda viva la validación anterior. La configuración existente (`**/Circuito/**/*.cs`) no incluye `FirmaDocs/CircuitoFirma.cs`, por lo que la ejecución configurada la marcó como `Ignored`; se usó un filtro explícito para superar esa limitación. El análisis enfocado obtuvo 72,53 % global por mutantes no relacionados y sin cobertura, aunque RF-05 quedó cubierto.
