using FirmaDocs.SelloTiempo;

namespace FirmaDocs.Tests.SelloTiempo;

public class HashContenidoTests
{
    [Fact]
    public void Calcular_ContenidoNulo_LanzaArgumentNullException_RF03()
    {
        var excepcion = Assert.Throws<ArgumentNullException>(() =>
            HashContenido.Calcular(null!));

        Assert.Equal("contenido", excepcion.ParamName);
    }
}
