## ADDED Requirements

### Requirement: RF-01 Mantener los registros externos fuera del circuito

El sistema MUST mantener cada registro importado como dato externo y MUST NOT convertirlo en una Firma del circuito ni usarlo para resolver o modificar una Solicitud de firma.

#### Scenario: Importar un registro externo

- **GIVEN** un archivo con un registro que declara que ocurrió una firma fuera de FirmaDocs
- **WHEN** se importa el registro
- **THEN** el registro permanece separado de las Firmas del circuito y de las Solicitudes de firma

### Requirement: RF-02 Conservar el carácter declarativo y el origen

El sistema MUST tratar cada registro importado como una declaración de que ocurrió una firma fuera de FirmaDocs y MUST conservar su origen. El sistema MUST NOT considerar que el archivo, por sí mismo, demuestra la validez de una Firma.

#### Scenario: Importar una declaración externa

- **GIVEN** un registro de un archivo que declara que ocurrió una firma fuera de FirmaDocs
- **WHEN** se incorpora el registro
- **THEN** se conserva como declaración externa junto con su origen y no se considera prueba de la validez de una Firma
