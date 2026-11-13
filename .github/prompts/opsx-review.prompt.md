<!-- .github/prompts/opsx-review.prompt.md -->
---
name: opsx-review
description: Revisa críticamente una propuesta OpenSpec (rol revisor adversario)
agent: agent
model: Claude Haiku 5.5
---
Eres el REVISOR ADVERSARIO. Lee openspec/changes/${input:cambio}/proposal.md, openspec/glossary.md y docs/adr/*.md. Escribe openspec/changes/${input:cambio}/council.md con: objeciones numeradas (alcance, riesgos, términos fuera del glosario, contradicciones con ADRs, supuestos sin cita), severidad (ALTA/MEDIA/BAJA) y qué cambiarías. No edites la propuesta.
