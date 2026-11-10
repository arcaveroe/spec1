namespace FirmaDocs;

/// <summary>Rechazo registrado en una vuelta del circuito de firma.</summary>
/// <param name="Firmante">Firmante que rechazó el documento.</param>
/// <param name="Motivo">Motivo indicado para el rechazo.</param>
/// <param name="Version">Versión del documento al momento del rechazo.</param>
/// <param name="Instante">Momento en que se registró el rechazo.</param>
public record Rechazo(Firmante Firmante, string Motivo, int Version, DateTimeOffset Instante);
