## 1. Migración de datos

- [x] 1.1 Añadir el script de migración que reconstruye la tabla, pasa las solicitudes afectadas a `PENDIENTE`, conserva sus datos y registra su origen en `migrado_desde` (RF-08).
- [x] 1.2 Añadir el script de rollback que restaura `ESPERANDO_CERTIFICADO` desde `migrado_desde`, preserva las demás columnas y recrea el índice (RF-08).
- [x] 1.3 Añadir una prueba que copie la base local y ejecute migración y rollback sobre esa copia; comprobar conservación de filas y datos, restricciones e índice, y que repetir el rollback no cambia el resultado (RF-08).

## 2. Estado y consumidores

- [x] 2.1 Retirar `EsperandoCertificado` de `EstadoSolicitud` y sus conversiones, sin cambiar firmas públicas ni afectar los demás estados (RF-02).
- [x] 2.2 Actualizar `BandejaFirmante` y su prueba de caracterización para clasificar solicitudes migradas en `PorFirmar`, no en una categoría de espera (RF-09).
- [x] 2.3 Actualizar `RecordatorioService` y su prueba de caracterización para no enviar el aviso de instalación del certificado y mantener el recordatorio normal de pendientes (RF-10).
- [x] 2.4 Actualizar `ReporteSla` y su prueba de caracterización para incluir solicitudes migradas desde su fecha original de creación (RF-11).

## 3. Regresión

- [ ] 3.1 Ejecutar `dotnet test` y corregir cualquier regresión relacionada con la retirada del estado y sus consumidores (RF-02, RF-08, RF-09, RF-10, RF-11).
