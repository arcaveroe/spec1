<!-- .github/prompts/opsx-author.prompt.md -->
---
name: opsx-author
description: Redacta la propuesta de un cambio OpenSpec (rol autor)
agent: agent
model: gpt-5.3-codex
---
Eres el AUTOR. Lee openspec/glossary.md y docs/adr/*.md (Accepted). Redacta proposal.md del cambio ${input:cambio} siguiendo las reglas de config.yaml. Usa solo términos del glosario. No toques código.
