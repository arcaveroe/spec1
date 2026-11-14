namespace FirmaDocs.SelloTiempo;

/// <summary>Resultado de una solicitud de sellado a una TSA.</summary>
/// <param name="Exito">Indica si la TSA confirmó el hash solicitado.</param>
/// <param name="Sello">Identificador o evidencia devuelta por la TSA cuando tiene éxito.</param>
/// <param name="Motivo">Motivo del fallo esperado, si la solicitud no tuvo éxito.</param>
public sealed record ResultadoTsa(bool Exito, string? Sello, string? Motivo);
