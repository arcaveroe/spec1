# Revisión adversaria: importar-firmas-externas

## Objeciones

1. **ALTA — No está decidido qué entidad modifica la importación.** La propuesta deja abierto si cada registro crea una Firma del circuito o resuelve una Solicitud de firma. El glosario establece que son conceptos distintos, con efectos diferentes sobre el historial y el estado. **Qué cambiaría:** decidir en la propuesta cuál de los dos casos cubre el cambio y cuál queda fuera de alcance; reflejar esa decisión en requisitos antes de implementar. ([proposal.md](./proposal.md), [glossary.md](../../glossary.md))
Decisión: ACEPTADA

2. **ALTA — El concepto central no tiene una definición acordada.** La expresión «firmas externas» no aparece en el glosario, y la propuesta descarta cambiar la definición de Firma sin explicar si los datos importados se aceptarán como Firmas del circuito ni cómo se distinguirán de las registradas por el sistema. **Qué cambiaría:** definir el concepto y sus límites en el glosario con una fuente citable; especificar qué representa cada registro importado y si se conservará su origen, sin asumir que el archivo demuestra por sí mismo la validez de una Firma. ([proposal.md](./proposal.md), [glossary.md](../../glossary.md))
Decisión: ACEPTADA

3. **MEDIA — La persistencia podría contradecir el ADR vigente.** El impacto menciona SQLite y el historial de firmas, pero el ADR indica que `CircuitoFirma` conserva su estado en memoria y no forma parte de esa persistencia. No queda claro si se almacenarán datos distintos del estado del circuito o si la importación pretende persistir ese estado. **Qué cambiaría:** precisar qué datos se escriben en `firmasimple.db` y cómo se relacionan con el historial; si se pretende persistir `CircuitoFirma`, revisar la decisión arquitectónica antes de implementar. ([proposal.md](./proposal.md), [ADR-0001](../../../docs/adr/ADR-0001-acceso-unico-desde-consola.md))

4. **MEDIA — Faltan reglas para entradas incompletas o repetidas.** La propuesta no decide qué hacer cuando no se encuentra el Documento, el Firmante o la Versión, cuando la Versión no es la vigente, ni cuando hay registros duplicados o errores parciales. Sin esas reglas no se puede determinar si el historial queda intacto ante una importación fallida. **Qué cambiaría:** definir la validación, la política de duplicados y si la operación es atómica; añadir escenarios para cada resultado. ([proposal.md](./proposal.md))

Decisión: RECHAZADA

5. **BAJA — Hay una premisa de persistencia sin alcance operativo.** Se afirma que la interacción será desde la consola, coherente con el ADR de acceso único, pero no se indica cómo se selecciona y valida el archivo ni cómo se informa el resultado de la importación. **Qué cambiaría:** delimitar en la propuesta el flujo de consola y la salida mínima ante éxito, rechazo o fallo, sin introducir una API externa. ([proposal.md](./proposal.md), [ADR-0001](../../../docs/adr/ADR-0001-acceso-unico-desde-consola.md))

Decisión: RECHAZADA
