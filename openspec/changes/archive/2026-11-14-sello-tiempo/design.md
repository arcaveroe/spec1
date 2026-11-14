## Context

`CircuitoFirma` conserva en memoria el estado del documento. Su método público `Aprobar(Firmante, DateTimeOffset)` registra una `Firma` con firmante, versión e instante, pero no recibe el contenido ni una TSA. La propuesta requiere calcular el hash de contenido de la versión y obtener un resultado satisfactorio de la TSA antes de registrar la Firma, sin cambiar esa firma pública.

### Decisiones previas

- [ADR-0001](../../../docs/adr/ADR-0001-acceso-unico-desde-consola.md) está Accepted: la consola es el único proceso autorizado a leer y escribir `firmasimple.db`, y `CircuitoFirma` conserva su estado en memoria, fuera de esa persistencia. Este diseño mantiene ese límite: el hash se conserva en la `Firma` en memoria y no se añade persistencia SQLite ni una migración.
- La respuesta o evidencia devuelta por la TSA no se conserva. El hash de contenido no es la evidencia TSA: permite comparar el contenido actual de la versión con el contenido asociado a la Firma durante la ejecución del proceso.

## Goals / Non-Goals

**Goals:**

- Calcular un hash determinista sobre los bytes UTF-8 exactos del contenido de una versión.
- Registrar la Firma sellada solo después de que la TSA confirme el hash solicitado.
- Conservar el hash de contenido junto a la Firma en memoria y poder comparar ese valor con el contenido actual de la misma versión.
- Mantener sin cambios la firma pública `Aprobar(Firmante, DateTimeOffset)`.

**Non-Goals:**

- Persistir en SQLite el estado del circuito, el hash de contenido o la respuesta TSA.
- Elegir un proveedor TSA o incorporar una dependencia externa.
- Añadir firma criptográfica con certificado.
- Cambiar las reglas del circuito de firma o el significado del instante de la Firma.

## Decisions

1. **Coordinar el registro mediante un servicio nuevo.** Añadir `ServicioAprobacionSellada` en `FirmaDocs/SelloTiempo/`. Recibe el circuito, firmante, contenido e instante; calcula el hash; solicita el sellado a una interfaz `ITsa`; y solo ante un resultado satisfactorio invoca la operación interna de `CircuitoFirma` que registra una Firma con el hash. La petición TSA ocurre antes de cualquier mutación del circuito. La firma pública existente se conserva y queda fuera de este flujo de aplicación; las llamadas que requieran sellado deben usar el servicio nuevo.

2. **Mantener el dominio independiente de la TSA.** `CircuitoFirma` conserva sus validaciones de estado y turno. Se añade una operación interna para registrar la Firma con el hash después de validar esas mismas reglas, compartiendo la lógica de registro con `Aprobar`. `Firma` conserva sus parámetros de constructor actuales y añade una propiedad de hash opcional, de modo que no cambie la firma pública existente del constructor. Las Firmas creadas por el método existente no tienen hash y no se consideran verificables por contenido.

3. **Usar SHA-256 sobre UTF-8 sin normalización.** `HashContenido` representa el digest SHA-256 codificado en hexadecimal minúsculo. Se codifica el texto tal como se recibe, sin normalización Unicode ni BOM; dos textos cuyos bytes UTF-8 difieran producen hashes distintos. La implementación usa `System.Security.Cryptography`, incluido en la plataforma, sin paquetes nuevos.

4. **Representar explícitamente el resultado TSA.** `ITsa` recibe el hash y devuelve un resultado tipado de éxito o fallo. Un resultado satisfactorio significa que el adaptador TSA confirmó una respuesta válida asociada al hash solicitado; una respuesta inválida, un rechazo del servicio o un fallo de transporte no permite registrar la Firma. Los fallos esperados se informan explícitamente; las excepciones no previstas se propagan. En cualquiera de esos casos, el circuito conserva el estado, el turno y el historial previos.

5. **Verificar comparando hashes.** Un verificador calcula SHA-256 del contenido actual de la misma versión y lo compara con el hash asociado a la Firma. El resultado distingue coincidencia, diferencia y ausencia de hash de referencia; la ausencia no se presenta como coincidencia ni como verificación satisfactoria.

6. **Tipos y módulos afectados.**
   - Nuevos: `HashContenido`, `ITsa`, el resultado tipado de la TSA, `ServicioAprobacionSellada` y el verificador, en `FirmaDocs/SelloTiempo/`.
   - Modificados: `FirmaDocs/Firma.cs` para conservar el hash opcional y `FirmaDocs/CircuitoFirma.cs` para registrar internamente la Firma sellada sin alterar el método público existente.
   - Pruebas nuevas: `FirmaDocs.Tests/SelloTiempo/`, cubriendo hash UTF-8, éxito TSA, rechazo/fallo TSA sin mutación, verificación coincidente/no coincidente y Firma sin hash.

7. **Alternativas descartadas.**
   - Añadir una sobrecarga pública de `Aprobar` que reciba contenido o TSA: se elige el servicio coordinador para no acoplar la API del dominio al proveedor ni crear otra entrada pública de aprobación.
   - Ejecutar la llamada TSA dentro de `CircuitoFirma`: el dominio no debe depender de una integración externa; además, el resultado debe obtenerse antes de modificar el circuito.
   - Guardar el hash o la respuesta TSA en SQLite: el estado de `CircuitoFirma` no forma parte de esa persistencia según ADR-0001. Hacerlo requeriría una decisión arquitectónica y un ADR que amplíe o sustituya esa decisión.

## Risks / Trade-offs

- El método público existente `Aprobar(Firmante, DateTimeOffset)` seguirá permitiendo registrar una Firma sin TSA. Esto preserva la API y su comportamiento; el límite del cambio es que solo el servicio nuevo garantiza el sellado. Los llamadores que necesiten la garantía deberán usar ese servicio.
- El hash solo vive mientras exista en memoria la Firma correspondiente. No se podrá verificar después de reiniciar el proceso o reconstruir el circuito; la verificación durable queda fuera del alcance y exigiría revisar ADR-0001.
- No guardar la respuesta TSA impide validar posteriormente esa respuesta de forma independiente. La garantía de éxito depende del contrato del adaptador TSA durante el registro.
- Un fallo remoto puede retrasar el registro de la Firma. No se registra una Firma como alternativa ni se avanza el turno cuando la TSA no confirma el hash.

## Migration Plan

- No hay migración de datos ni de esquema SQLite.
- Añadir el servicio y el adaptador TSA sin paquetes externos, y dirigir por el servicio las operaciones que requieran sellado.
- Los circuitos y Firmas existentes permanecen válidos en memoria; las Firmas sin hash se informan como no verificables por contenido.
- Para revertir, dejar de usar el servicio nuevo; no hay datos persistidos que restaurar.

## Open Questions

- ¿Qué proveedor TSA se implementará y cómo se configurará su adaptador? La selección queda para una decisión posterior y no añade dependencias en este cambio.
- Si se requiere verificación después de reiniciar el proceso, ¿qué decisión sustituirá o ampliará ADR-0001 para conservar el hash de referencia?
