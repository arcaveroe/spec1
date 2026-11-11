# Negocio
- **Documento:** contenido sujeto a un circuito de aprobación.
- **Versión:** edición vigente del documento; empieza en 1.
- **Autor:** responsable del documento y único autorizado a reenviarlo.
- **Firmante:** persona que aprueba o rechaza cuando le corresponde el turno.
- **Firma:** aprobación registrada con firmante, versión e instante.
- **Rechazo:** devolución registrada con firmante, motivo, versión e instante.
- **Circuito de firma:** secuencia ordenada de firmantes para aprobar el documento.
- **Vuelta:** ciclo de revisión asociado a una versión; máximo de 3.
## Reglas vigentes
- Todo rechazo requiere un motivo no vacío ni compuesto solo por espacios.
- Un firmante no puede aparecer más de una vez en el circuito.
- Las firmas de versiones anteriores se conservan, pero no aprueban la versión vigente.
- El historial de firmas y rechazos nunca se borra ni se modifica.
- Referencia: [spec 001, rechazo y vueltas](../specs/001_rechazo_y_vueltas/spec.md).
