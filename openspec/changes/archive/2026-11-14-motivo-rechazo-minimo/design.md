## Context

`CircuitoFirma.Rechazar` ya rechaza motivos vacíos o compuestos solo por espacios, pero acepta textos demasiado breves. El motivo forma parte del historial inmutable de rechazos y la firma pública de `Rechazar` debe conservarse.

## Goals / Non-Goals

**Goals:**

- Validar que el motivo tenga al menos 10 caracteres después de quitar los espacios iniciales y finales.
- Rechazar entradas inválidas antes de modificar el estado o el historial.
- Conservar el texto original cuando el motivo sea válido.
- Mantener la firma pública y el resto de las reglas de rechazo sin cambios.

**Non-Goals:**

- Cambiar la firma pública de `Rechazar`.
- Normalizar, recortar o reescribir el motivo almacenado.
- Añadir moderación semántica o validación de idioma.
- Cambiar el número máximo de vueltas o las reglas de autorización del rechazo.

## Decisions

1. **Validar con el texto recortado y conservar el texto original.** Se calculará `motivo.Trim().Length` para aplicar el mínimo de 10 caracteres, pero se pasará `motivo` sin modificar al registro `Rechazo`. Así los espacios que el firmante proporcionó no se pierden del historial.
   - **Alternativa descartada:** validar la longitud del texto original. Se descarta porque permitiría cumplir el mínimo agregando espacios exteriores.
   - **Alternativa descartada:** almacenar siempre el texto recortado. Se descarta porque cambia la evidencia textual registrada y no es necesario para la regla.

2. **Usar una excepción de dominio con mensaje específico.** Una entrada con menos de 10 caracteres significativos lanzará `CircuitoFirmaException` con el mensaje `El motivo de rechazo debe tener al menos 10 caracteres sin contar espacios iniciales ni finales.`.
   - **Alternativa descartada:** reutilizar `El motivo de rechazo es obligatorio.`. Se descarta porque no describe el incumplimiento de longitud.
   - **Alternativa descartada:** devolver un valor booleano. Se descarta porque las reglas de dominio existentes informan errores mediante `CircuitoFirmaException`.

3. **Mantener la validación después de comprobar circuito y turno.** Se conserva el orden actual de validaciones para no cambiar los mensajes observables de un documento fuera de circuito o de un firmante no habilitado.
   - **Alternativa descartada:** validar el motivo antes de comprobar autorización. Se descarta porque podría revelar o priorizar información de una operación no permitida.

## Risks / Trade-offs

- Motivos válidos existentes con menos de 10 caracteres dejarán de aceptarse; es el cambio de comportamiento requerido.
- La comparación por `Length` cuenta unidades UTF-16 de .NET, igual que otras validaciones de texto del dominio; no se introduce una política distinta de conteo.

## Affected Types

- `FirmaDocs.CircuitoFirma`: validación del motivo en `Rechazar`.
- `FirmaDocs.Rechazo`: sin cambios de contrato.
- `FirmaDocs.Tests.CircuitoFirmaTests` y `FirmaDocs.Tests.Circuito.RechazoEtapasTests`: cobertura de frontera, espacios e invariantes.
