using FirmaDocs.Legado;

namespace FirmaDocs.Tests.Legado;

public class EstadoSolicitudTextoCaracterizacionTests
{
    [Theory]
    [InlineData(EstadoSolicitud.Pendiente, "PENDIENTE")]
    [InlineData(EstadoSolicitud.Firmada, "FIRMADA")]
    [InlineData(EstadoSolicitud.Rechazada, "RECHAZADA")]
    [InlineData(EstadoSolicitud.Expirada, "EXPIRADA")]
    public void Caracterizacion_EstadoSolicitudTexto_ConvierteEstadoATexto(
        EstadoSolicitud estado,
        string texto)
    {
        Assert.Equal(texto, EstadoSolicitudTexto.ATexto(estado));
    }

    [Theory]
    [InlineData("PENDIENTE", EstadoSolicitud.Pendiente)]
    [InlineData("FIRMADA", EstadoSolicitud.Firmada)]
    [InlineData("RECHAZADA", EstadoSolicitud.Rechazada)]
    [InlineData("EXPIRADA", EstadoSolicitud.Expirada)]
    public void Caracterizacion_EstadoSolicitudTexto_ConvierteTextoAEstado(
        string texto,
        EstadoSolicitud estado)
    {
        Assert.Equal(estado, EstadoSolicitudTexto.DesdeTexto(texto));
    }

    [Fact]
    public void Caracterizacion_EstadoSolicitudTexto_RechazaTextoDesconocido()
    {
        var excepcion = Assert.Throws<ArgumentException>(() =>
            EstadoSolicitudTexto.DesdeTexto("INVALIDO"));

        Assert.Equal("texto", excepcion.ParamName);
        Assert.Contains("Estado desconocido: INVALIDO", excepcion.Message);
    }
}
