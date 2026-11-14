## 1. Contrato de consulta

- [x] 1.1 Acordar ruta, parámetros, respuesta y límite temporal, citando RF-FSV01 y RF-FSV03.
- [x] 1.2 Añadir pruebas de solicitudes vencidas, vigentes, límite exacto y vencimientos nulos, citando RF-FSV01.

## 2. Persistencia y consulta de solo lectura

- [x] 2.1 Agregar `vence_en` nullable y migración segura para registros legados, citando RF-FSV04.
- [x] 2.2 Implementar la consulta paginada de solo lectura con instante explícito, citando RF-FSV01 y RF-FSV03.
- [x] 2.3 Exponer el endpoint HTTP y verificar que no modifica solicitudes ni historial, citando RF-FSV01 y RF-FSV02.
- [ ] 2.4 Ejecutar `dotnet test` y `dotnet build` de la solución.
