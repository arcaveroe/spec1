namespace FirmaDocs.SelloTiempo;

/// <summary>Resultado del intento de registrar una aprobación sellada.</summary>
/// <param name="Exito">Indica si se registró la aprobación sellada.</param>
/// <param name="Hash">Hash de contenido solicitado a la TSA.</param>
/// <param name="Sello">Sello devuelto por la TSA cuando tiene éxito.</param>
/// <param name="Motivo">Motivo del fallo esperado, si no se registró la aprobación.</param>
public sealed record ResultadoAprobacionSellada(
    bool Exito,
    HashContenido Hash,
    string? Sello,
    string? Motivo);
