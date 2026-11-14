namespace FirmaDocs.SelloTiempo;

/// <summary>Resultado de verificar el contenido actual frente a una Firma.</summary>
/// <param name="Estado">Estado de la comparación.</param>
/// <param name="HashCalculado">Hash del contenido verificado.</param>
/// <param name="HashReferencia">Hash asociado a la Firma, si existe.</param>
/// <param name="Motivo">Motivo del resultado no coincidente, si corresponde.</param>
public sealed record ResultadoVerificacionContenido(
    EstadoVerificacionContenido Estado,
    HashContenido HashCalculado,
    HashContenido? HashReferencia,
    string? Motivo);
