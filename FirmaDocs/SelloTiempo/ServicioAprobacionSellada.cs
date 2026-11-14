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
    public ResultadoAprobacionSellada Aprobar(
        CircuitoFirma circuito,
        Firmante firmante,
        string contenido,
        DateTimeOffset instante,
        ITsa tsa)
    {
        ArgumentNullException.ThrowIfNull(circuito);
        ArgumentNullException.ThrowIfNull(firmante);
        ArgumentNullException.ThrowIfNull(contenido);
        ArgumentNullException.ThrowIfNull(tsa);

        var hash = HashContenido.Calcular(contenido);
        var resultadoTsa = tsa.Sellar(hash);

        if (!resultadoTsa.Exito || string.IsNullOrWhiteSpace(resultadoTsa.Sello))
        {
            var motivo = string.IsNullOrWhiteSpace(resultadoTsa.Motivo)
                ? "TSA_NO_DISPONIBLE"
                : resultadoTsa.Motivo;
            return new ResultadoAprobacionSellada(false, hash, null, motivo);
        }

        circuito.AprobarSellada(firmante, instante, hash);
        return new ResultadoAprobacionSellada(true, hash, resultadoTsa.Sello, null);
    }
}
