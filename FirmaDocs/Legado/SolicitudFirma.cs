namespace FirmaDocs.Legado;

// Una fila de la tabla solicitudes_firma.
public record SolicitudFirma(
    int Id,
    string DocumentoId,
    string FirmanteEmail,
    int Vuelta,
    DateTimeOffset CreadaEn,
    DateTimeOffset? ResueltaEn,
    EstadoSolicitud Estado,
    string? MigradoDesde)
{
    /// <summary>
    /// Obtiene el instante en que vence la solicitud, si fue informado.
    /// </summary>
    public DateTimeOffset? VenceEn { get; init; }
}
