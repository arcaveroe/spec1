## ADDED Requirements

### Requirement: RF-FSV01 Selección paginada de solicitudes vencidas

La API SHALL exponer `GET /api/solicitudes/vencidas`, recibir un instante
ISO-8601 con offset y devolver únicamente solicitudes con `vence_en` no nulo
anterior al instante de referencia. Los parámetros opcionales `page` y
`pageSize` SHALL usar los valores por defecto 1 y 50, respectivamente. La
respuesta SHALL contener `items`, `page`, `pageSize` y `totalCount`; los
elementos SHALL ordenarse por `vence_en` y luego por identificador ascendente.

#### Scenario: Solicitud vencida
- **GIVEN** una solicitud con vencimiento anterior al instante de referencia
- **WHEN** se consulta la página de vencidas
- **THEN** la respuesta incluye la solicitud

#### Scenario: Solicitud vigente o en el límite exacto
- **GIVEN** una solicitud cuyo vencimiento es igual o posterior al instante
- **WHEN** se consulta la página de vencidas
- **THEN** la respuesta no incluye la solicitud

#### Scenario: Solicitud sin vencimiento conocido
- **GIVEN** una solicitud cuyo vencimiento es `NULL`
- **WHEN** se consulta la página de vencidas
- **THEN** la respuesta no incluye la solicitud

#### Scenario: Página con orden estable
- **GIVEN** varias solicitudes vencidas, algunas con el mismo vencimiento
- **WHEN** se solicitan páginas consecutivas
- **THEN** los elementos se ordenan por vencimiento y luego por identificador, y `totalCount` es el mismo en cada página

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
- **GIVEN** dos consultas con el mismo instante de referencia y datos sin cambios
- **WHEN** se ejecutan ambas consultas
- **THEN** ambas producen el mismo conjunto de solicitudes

### Requirement: RF-FSV04 Vencimiento nullable y migración segura

El sistema SHALL conservar el vencimiento explícito de cada solicitud y SHALL
dejar en `NULL` las solicitudes existentes que no dispongan de ese dato al
aplicar la migración. Las solicitudes sin vencimiento SHALL permanecer
disponibles para las consultas generales y quedar excluidas del filtro de
vencidas.

#### Scenario: Migrar solicitudes existentes
- **GIVEN** una solicitud almacenada antes de agregar `vence_en`
- **WHEN** se ejecuta la migración
- **THEN** la solicitud conserva sus demás datos y su `vence_en` queda en `NULL`

#### Scenario: Revertir la migración
- **GIVEN** la columna `vence_en` agregada por la migración
- **WHEN** se ejecuta el rollback
- **THEN** se elimina la columna y se conservan las solicitudes
