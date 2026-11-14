namespace FirmaDocs.SelloTiempo;

/// <summary>Coordina la aprobación del circuito con la confirmación previa de una TSA.</summary>
public sealed class ServicioAprobacionSellada
{
    /// <summary>Solicita el sellado del contenido y registra la aprobación en el circuito.</summary>
    /// <param name="circuito">Circuito real que recibirá la aprobación.</param>
    /// <param name="firmante">Firmante que aprueba.</param>
    /// <param name="contenido">Contenido exacto de la versión.</param>
    /// <param name="instante">Instante de la aprobación.</param>
    /// <param name="tsa">Autoridad a la que se solicita el sellado.</param>
    /// <returns>Resultado de la aprobación sellada.</returns>
    /// <exception cref="NotImplementedException">La coordinación está pendiente de implementación.</exception>
    public ResultadoAprobacionSellada Aprobar(
        CircuitoFirma circuito,
        Firmante firmante,
        string contenido,
        DateTimeOffset instante,
        ITsa tsa)
    {
        throw new NotImplementedException();
    }
}
