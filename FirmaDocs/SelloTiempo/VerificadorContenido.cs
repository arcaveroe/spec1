namespace FirmaDocs.SelloTiempo;

/// <summary>Verifica el contenido actual frente al hash asociado a una Firma.</summary>
public sealed class VerificadorContenido
{
    /// <summary>Calcula y compara el hash del contenido con el de la Firma indicada.</summary>
    /// <param name="firma">Firma que contiene el hash de referencia.</param>
    /// <param name="contenido">Contenido actual de la versión.</param>
    /// <returns>Resultado tipado de la comparación.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="firma"/> o <paramref name="contenido"/> es nulo.</exception>
    public ResultadoVerificacionContenido Verificar(Firma firma, string contenido)
    {
        ArgumentNullException.ThrowIfNull(firma);
        ArgumentNullException.ThrowIfNull(contenido);

        var hashCalculado = HashContenido.Calcular(contenido);
        var hashReferencia = firma.HashContenido;

        if (hashReferencia is null)
            return new ResultadoVerificacionContenido(
                EstadoVerificacionContenido.SinHashReferencia,
                hashCalculado,
                null,
                null);

        var coincide = string.Equals(hashCalculado.Valor, hashReferencia.Valor, StringComparison.Ordinal);
        return new ResultadoVerificacionContenido(
            coincide ? EstadoVerificacionContenido.Coincide : EstadoVerificacionContenido.Diferente,
            hashCalculado,
            hashReferencia,
            coincide ? null : "DOCUMENTO_ALTERADO");
    }
}
