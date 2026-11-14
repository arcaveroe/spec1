## Contexto

La capacidad debe vivir en el dominio del circuito y mantener separadas la
identidad del firmante requerido y la identidad de quien ejecuta la firma.

## Diseño propuesto

- `FirmaDocs/Circuito/`: tipos de valor y reglas de delegación.
- `CircuitoFirma`: consulta de habilitación y registro de la delegación
  aplicada a la firma.
- `Firma`: metadatos de auditoría del ejecutor, manteniendo el firmante
  requerido.
- `FirmaDocs.Tests/Circuito/`: pruebas de vigencia, alcance, revocación y
  auditoría.

La decisión final sobre la forma exacta de los tipos queda condicionada a las
preguntas abiertas de la propuesta. Las fechas deberán representarse con
`DateTimeOffset` inyectado en las operaciones que evalúen vigencia.

## Alternativas descartadas

- **Sustituir el firmante original por el delegado:** se descarta porque
  destruye la trazabilidad y altera quién debía aprobar.
- **Resolver la delegación fuera del dominio:** se descarta porque permitiría
  registrar firmas que el circuito no considera habilitadas.
- **Permitir delegaciones sin fecha o alcance:** se descarta porque no ofrece
  un límite verificable de autorización.
