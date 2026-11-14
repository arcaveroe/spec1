## ADDED Requirements

### Requirement: RF-FSV01 Selección de solicitudes vencidas

La API SHALL devolver únicamente solicitudes cuyo vencimiento sea anterior al
instante de referencia de la consulta.

#### Scenario: Solicitud vencida
- **GIVEN** una solicitud cuyo vencimiento es anterior al instante de referencia
- **WHEN** se consulta el filtro de vencidas
- **THEN** la respuesta incluye la solicitud

#### Scenario: Solicitud vigente
- **GIVEN** una solicitud cuyo vencimiento es posterior al instante de referencia
- **WHEN** se consulta el filtro de vencidas
- **THEN** la respuesta no incluye la solicitud

### Requirement: RF-FSV02 Consulta sin efectos secundarios

La API SHALL ejecutar el filtro en modo solo lectura y no cambiar el estado,
vencimiento ni historial de las solicitudes devueltas.

#### Scenario: Consulta repetida
- **GIVEN** una solicitud vencida
- **WHEN** se ejecuta dos veces la consulta
- **THEN** ambas respuestas son equivalentes y la solicitud conserva sus datos

### Requirement: RF-FSV03 Instante determinista

La API SHALL evaluar el vencimiento usando el instante de referencia recibido
por la capa de aplicación, sin leer directamente el reloj del sistema en la
consulta.

#### Scenario: Mismo instante
- **GIVEN** dos consultas con el mismo instante de referencia
- **WHEN** la fuente de datos no cambia
- **THEN** ambas consultas producen el mismo conjunto de solicitudes
