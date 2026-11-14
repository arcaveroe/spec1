namespace FirmaDocs.SelloTiempo;

/// <summary>Contrato para solicitar a una autoridad de sellado de tiempo que selle un hash.</summary>
public interface ITsa
{
    /// <summary>Solicita el sellado del hash indicado.</summary>
    /// <param name="hash">Hash de contenido que se solicita sellar.</param>
    /// <returns>Resultado tipado de la solicitud a la TSA.</returns>
    ResultadoTsa Sellar(HashContenido hash);
}
