# Glosario de FirmaDocs

## Términos del dominio

### Documento
- **Definición operativa:** contenido sujeto a un circuito de aprobación.
- **Sinónimos prohibidos:** no usar «solicitud de firma» para nombrar el documento.
- **Ejemplo:** el documento se reenvía con una nueva versión.
- **Cita:** `docs/negocio.md`, definición «Documento»; `specs/001_rechazo_y_vueltas/spec.md`, RF-02.

### Versión
- **Definición operativa:** edición vigente del documento, cuyo número inicial es 1.
- **Sinónimos prohibidos:** no usar «vuelta» como sinónimo de versión.
- **Ejemplo:** al reenviar un documento devuelto, su versión aumenta en 1.
- **Cita:** `docs/negocio.md`, definición «Versión»; `specs/001_rechazo_y_vueltas/spec.md`, RF-02.

### Autor
- **Definición operativa:** responsable del documento y única persona autorizada a reenviarlo.
- **Sinónimos prohibidos:** no usar «firmante» como sinónimo de autor.
- **Ejemplo:** solo el autor reenvía el documento devuelto.
- **Cita:** `docs/negocio.md`, definición «Autor»; `specs/001_rechazo_y_vueltas/spec.md`, RF-02.

### Firmante
- **Definición operativa:** persona habilitada que aprueba o rechaza cuando le corresponde actuar en el circuito.
- **Sinónimos prohibidos:** «usuario» y «aprobador».
- **Ejemplo:** el firmante en turno registra su firma en la versión vigente.
- **Cita:** `docs/negocio.md`, definición «Firmante»; `openspec/specs/etapas-paralelas/spec.md`, RF-04 y RF-05.

### Firma
- **Definición operativa:** registro de aprobación de un documento, asociado a un firmante, una versión y un instante. Es distinta de una solicitud de firma.
- **Sinónimos prohibidos:** «visto bueno» y «aprobación» cuando se refieren al registro.
- **Ejemplo:** queda registrada la firma de Ana en la versión 2 del documento.
- **Cita:** `docs/negocio.md`, definición «Firma»; `openspec/specs/etapas-paralelas/spec.md`, RF-08.

### Rechazo
- **Definición operativa:** devolución del documento registrada con firmante, motivo, versión e instante.
- **Sinónimos prohibidos:** no usar «firma» para referirse al registro de rechazo.
- **Ejemplo:** el firmante en turno registra un rechazo con un motivo no vacío.
- **Cita:** `docs/negocio.md`, definición «Rechazo» y «Reglas vigentes»; `specs/001_rechazo_y_vueltas/spec.md`, RF-01 y RF-05.

### Circuito de firma
- **Definición operativa:** estructura que determina las etapas, sus firmantes y el orden o modo en que se aprueba un documento.
- **Sinónimos prohibidos:** «flujo» y «workflow».
- **Ejemplo:** un circuito puede tener una etapa secuencial seguida de una etapa paralela.
- **Cita:** `docs/negocio.md`, definición «Circuito de firma»; `openspec/specs/etapas-paralelas/spec.md`, RF-01, RF-03 y RF-06.

### Etapa
- **Definición operativa:** unidad de un circuito con entre 1 y 5 firmantes y un modo secuencial o paralelo; la etapa siguiente comienza al completarse la actual.
- **Sinónimos prohibidos:** no usar «vuelta» para nombrar una etapa.
- **Ejemplo:** una etapa paralela con dos firmantes se completa cuando ambos aprueban.
- **Cita:** `openspec/specs/etapas-paralelas/spec.md`, RF-01, RF-05 y RF-06.

### Vuelta
- **Definición operativa:** ciclo de revisión asociado a una versión; se permiten como máximo tres.
- **Sinónimos prohibidos:** «ronda» e «iteración».
- **Ejemplo:** al reenviar el documento tras un rechazo, comienza una nueva vuelta.
- **Cita:** `docs/negocio.md`, definición «Vuelta»; `specs/001_rechazo_y_vueltas/spec.md`, contexto y RF-04.

### Solicitud de firma
- **Definición operativa:** registro de una petición de firma que tiene un estado propio; no es la firma registrada en un circuito.
- **Sinónimos prohibidos:** no usar «firma» para nombrar la solicitud.
- **Ejemplo:** una solicitud puede estar pendiente sin que exista todavía una firma registrada.
- **Cita:** `openspec/specs/solicitudes-firma/spec.md`, Purpose y RF-01, RF-03 a RF-08.

### Estado de solicitud
- **Definición operativa:** condición textual de una solicitud de firma. Los estados admitidos son `PENDIENTE`, `FIRMADA`, `RECHAZADA` y `EXPIRADA`.
- **Sinónimos prohibidos:** no usar los estados del documento como si fueran estados de la solicitud.
- **Ejemplo:** al cambiar una solicitud a `FIRMADA`, se registra el instante de resolución recibido.
- **Cita:** `openspec/specs/solicitudes-firma/spec.md`, RF-01 a RF-07.

### Declaración de firma externa
- **Definición operativa:** registro que declara que una firma ocurrió fuera del sistema, conserva su origen y no demuestra por sí mismo la validez de una Firma ni crea una Firma o una Solicitud de firma.
- **Sinónimos prohibidos:** no usar «firma» ni «solicitud de firma» para nombrar este registro.
- **Ejemplo:** el registro importado conserva el origen de la declaración de firma externa.
- **Cita:** `openspec/changes/importar-firmas-externas/proposal.md`, Why y What Changes.

### Anti-Corruption Layer (ACL; capa anticorrupción)
- **Definición operativa:** capa que traduce datos externos al modelo del dominio sin contaminarlo.
- **Sinónimos prohibidos:** no sustituir el término por «integración» cuando se alude específicamente a esta capa de traducción.
- **Ejemplo:** una ACL convierte el estado recibido de un proveedor externo a un estado del dominio.
- **Cita:** Evans, *Domain-Driven Design*, capítulo 14.

## Términos pendientes de fuente

No se localizó una definición operativa ni un requisito citable para los siguientes términos. Se dejan sin definición para no introducir reglas no respaldadas:

- **Plazo de firma**
- **Delegación**
- **Evidencia de firma externa**
