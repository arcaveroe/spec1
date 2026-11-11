## Why

El circuito actual organiza la aprobación como un único orden de firmantes, lo que no permite representar equipos que deben aprobar en paralelo dentro de una etapa. Incorporar etapas configurables permitirá combinar aprobaciones secuenciales y paralelas sin perder el comportamiento vigente de rechazo y reenvío.

## What Changes

- Permitir configurar cada circuito con entre 1 y 5 etapas, y cada etapa con entre 1 y 5 firmantes y un modo `SECUENCIAL` o `PARALELA`.
- En una etapa `SECUENCIAL`, requerir aprobaciones en el orden configurado. En una etapa `PARALELA`, requerir la aprobación de todos sus firmantes, en cualquier orden.
- Iniciar la etapa siguiente solo cuando todos los firmantes de la etapa actual hayan aprobado.
- Rechazar desde cualquier etapa con el comportamiento vigente: devolver el documento y, al reenviarlo, iniciar una nueva vuelta desde la primera etapa, conforme a `specs/001_rechazo_y_vueltas/spec.md`.
- Impedir que un mismo firmante aparezca en más de una etapa del circuito.
- Mantener compatible el circuito actual como una sola etapa `SECUENCIAL`, conservando el comportamiento de su lista plana de firmantes.
- Mantener las reglas vigentes de versiones, historial y máximo de tres vueltas.

## Fuera de alcance

- Quórum parcial de aprobación (por ejemplo, 2 de 3 firmantes).
- Notificaciones.
- Persistencia.
- Plazos.

## Capabilities

### New Capabilities

- `etapas-paralelas`: configuración y aprobación de circuitos organizados en etapas secuenciales o paralelas.

### Modified Capabilities

- Ninguna especificación de capacidad existente en `openspec/specs/` se modifica.

## Impact

- Dominio del circuito de firma y sus pruebas, para representar etapas, modos, validaciones de firmantes y transiciones entre etapas.
- Especificación vigente de rechazo y vueltas en [specs/001_rechazo_y_vueltas/spec.md](../../../specs/001_rechazo_y_vueltas/spec.md), como referencia de compatibilidad para el rechazo y el reinicio desde la primera etapa.
- El circuito actual de lista plana deberá seguir funcionando como una etapa secuencial. No se modificarán firmas públicas existentes; podrán añadirse tipos o miembros necesarios para modelar las etapas.
- No se anticipan dependencias nuevas.

## Preguntas abiertas

- Ninguna pendiente para el alcance acordado.
