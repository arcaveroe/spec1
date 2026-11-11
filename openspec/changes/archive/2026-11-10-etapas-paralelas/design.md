# Design: Etapas secuenciales y paralelas

## Contexto

`CircuitoFirma` concentra hoy la configuración plana de firmantes, el turno secuencial, el estado del documento y los historiales de firmas y rechazos. La nueva configuración por etapas debe conservar esas responsabilidades y las firmas públicas existentes, a la vez que permite aprobar en paralelo.

El progreso puede derivarse del historial de firmas de la versión vigente: una aprobación anterior se conserva, pero no completa etapas de una versión nueva. La unicidad global de los firmantes permite determinar a qué etapa corresponde cada firma sin cambiar el registro `Firma`.

## Decisiones

### Representar la configuración como etapas inmutables

- Añadir `ModoEtapaFirma`, con los modos `Secuencial` y `Paralela`.
- Añadir `EtapaFirma`, que expone su modo y una copia de solo lectura de sus firmantes.
- Añadir un constructor de `CircuitoFirma` que reciba las etapas, conservando sin cambios la firma del constructor de lista plana.
- Validar entre 1 y 5 etapas, entre 1 y 5 firmantes por etapa, modos admitidos y ausencia de firmantes repetidos en todo el circuito.
- El constructor heredado transforma su lista plana en una única etapa `Secuencial` y aplica los mismos límites, incluido el máximo de cinco firmantes. Esto mantiene el orden y el comportamiento secuencial de configuraciones dentro de los límites, pero rechaza listas heredadas más largas.
- Mantener `Firmantes` como vista de solo lectura de los firmantes aplanados en orden de etapa y de configuración, para preservar la consulta existente.

### Derivar el avance de las firmas de la versión actual

- La etapa actual es la primera cuyas aprobaciones no están completas en `Version`.
- Una firma cuenta para completar una etapa solo si pertenece a esa versión y a uno de sus firmantes.
- Una etapa secuencial habilita únicamente al primer firmante aún pendiente; una etapa paralela habilita a todos sus firmantes pendientes.
- Exponer `FirmantesHabilitados` como la lista de firmantes que pueden actuar en la etapa actual. Mantener `FirmanteEnTurno` para circuitos secuenciales y devolver `null` cuando no exista un turno único, incluido durante una etapa paralela.
- `Aprobar` valida contra los firmantes habilitados y registra una `Firma` existente. La etapa siguiente se vuelve activa al completarse la actual; al completar la última, el circuito pasa a `Aprobado`.
- `Rechazar` permite actuar solo a un firmante habilitado de la etapa actual y conserva la validación del motivo y el límite de vueltas vigente.
- `Reenviar` conserva la configuración y aumenta `Version`. Como el avance se deriva de las firmas de la versión vigente, la nueva vuelta comienza en la primera etapa sin borrar ni modificar el historial.

No se añade estado mutable independiente para la etapa o el turno: el historial de firmas por versión sigue siendo la fuente de verdad.

## Módulos afectados

- `FirmaDocs/CircuitoFirma.cs`: integrar la configuración por etapas y derivar turnos, aprobaciones y transiciones.
- `FirmaDocs/Circuito/ModoEtapaFirma.cs` y `FirmaDocs/Circuito/EtapaFirma.cs`: nuevos tipos del dominio.
- `FirmaDocs.Tests/Circuito/`: pruebas de límites, validación, modos, transición de etapas, rechazo y compatibilidad.
- `Program.cs`, persistencia y dependencias externas no se modifican.

## Alternativas descartadas

- **Mantener un contador mutable de aprobaciones por etapa:** duplicaría la información ya registrada en `Firmas` y exigiría reiniciarlo correctamente al reenviar.
- **Reemplazar el constructor plano por uno de etapas:** rompería la firma pública existente; se añade una sobrecarga y se adapta internamente la lista plana.
- **Representar la etapa paralela con un único turno:** impondría un orden que la especificación no exige. Se conserva `FirmanteEnTurno` para casos secuenciales y se incorpora una consulta plural para quienes están habilitados.
- **Permitir más de cinco firmantes solo en la lista heredada:** produciría una excepción al límite de RF-01. Se decidió aplicar el mismo límite a ambos constructores.

## Verificación

Las pruebas cubrirán los RF-01 a RF-08: configuraciones válidas e inválidas, firmantes repetidos, compatibilidad secuencial, aprobación secuencial y paralela, avance solo al completar una etapa, rechazo y reenvío desde cualquier etapa, y conservación del historial entre versiones. Se mantendrán las pruebas existentes como regresión y los nombres nuevos citarán el RF correspondiente.
