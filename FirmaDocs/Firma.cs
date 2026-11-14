namespace FirmaDocs;

using FirmaDocs.SelloTiempo;

/// <summary>Aprobación registrada por un firmante sobre una versión del documento.</summary>
/// <param name="Firmante">Quién aprobó.</param>
/// <param name="Version">Versión del documento que aprobó.</param>
/// <param name="Instante">Momento de la aprobación (inyectado, nunca el reloj del sistema).</param>
public record Firma(Firmante Firmante, int Version, DateTimeOffset Instante)
{
    /// <summary>Hash del contenido aprobado, o <see langword="null"/> si no se registró.</summary>
    public HashContenido? HashContenido { get; init; }

    /// <summary>Delegado que ejecutó la aprobación, o <see langword="null"/> si firmó el firmante requerido.</summary>
    public Firmante? DelegadoEjecutor { get; init; }
}
