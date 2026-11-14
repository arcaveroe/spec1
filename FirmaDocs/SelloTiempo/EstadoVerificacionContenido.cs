namespace FirmaDocs.SelloTiempo;

/// <summary>Resultado categórico de comparar el contenido con el hash asociado a una Firma.</summary>
public enum EstadoVerificacionContenido
{
    /// <summary>El hash calculado coincide con el asociado a la Firma.</summary>
    Coincide,

    /// <summary>El hash calculado difiere del asociado a la Firma.</summary>
    Diferente,

    /// <summary>La Firma no tiene un hash de referencia.</summary>
    SinHashReferencia
}
