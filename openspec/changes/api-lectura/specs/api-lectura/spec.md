## ADDED Requirements

### Requirement: RF-01 Consultar datos mediante la API de lectura

Cuando un consumidor invoque una consulta de lectura disponible, la API MUST devolver los datos correspondientes de `firmasimple.db`.

#### Scenario: Consultar datos existentes

- **GIVEN** que `firmasimple.db` contiene datos disponibles para una consulta
- **WHEN** un consumidor invoca esa consulta mediante la API
- **THEN** la respuesta contiene los datos correspondientes

### Requirement: RF-02 Mantener la API sin capacidad de escritura

La API MUST NOT insertar, actualizar ni eliminar datos en `firmasimple.db`.

#### Scenario: Procesar una consulta sin alterar la base de datos

- **GIVEN** una base de datos con datos persistidos
- **WHEN** la API procesa una solicitud de lectura
- **THEN** los datos persistidos permanecen sin cambios
