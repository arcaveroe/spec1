namespace FirmaDocs.SelloTiempo;

/// <summary>Verifica el contenido actual frente al hash asociado a una Firma.</summary>
public sealed class VerificadorContenido
{
    /// <summary>Calcula y compara el hash del contenido con el de la Firma indicada.</summary>
    /// <param name="firma">Firma que contiene el hash de referencia.</param>
    /// <param name="contenido">Contenido actual de la versión.</param>
    /// <returns>Resultado tipado de la comparación.</returns>
    /// <exception cref="NotImplementedException">La verificación está pendiente de implementación.</exception>
    public ResultadoVerificacionContenido Verificar(Firma firma, string contenido)
    {
        throw new NotImplementedException();
    }
}
