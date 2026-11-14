using FirmaDocs.SelloTiempo;
using NSubstitute;

namespace FirmaDocs.Tests.SelloTiempo;

public sealed class AprobacionSelladaTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly DateTimeOffset Instante = new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Aprobar_TsaConfirmaHash_RegistrarFirmaConSello_RF01()
    {
        var circuito = CircuitoEnviado();
        var tsa = TsaExito("TSA-000123");
        var servicio = new ServicioAprobacionSellada();

        var resultado = servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa);

        Assert.True(resultado.Exito);
        Assert.Equal("TSA-000123", resultado.Sello);
        Assert.Equal("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0", resultado.Hash.Valor);
        Assert.Equal(resultado.Hash, circuito.Firmas.Single().HashContenido);
        Assert.Equal(Instante, circuito.Firmas.Single().Instante);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
        tsa.Received(1).Sellar(resultado.Hash);
    }

    [Fact]
    public void Aprobar_ContenidoUnicode_NoNormalizaBytesUtf8_RF03()
    {
        var tsaCompuesto = TsaExito("TSA-UNICODE-1");
        var tsaDescompuesto = TsaExito("TSA-UNICODE-2");
        var servicio = new ServicioAprobacionSellada();

        var resultadoCompuesto = servicio.Aprobar(CircuitoEnviado(), Ana, "á", Instante, tsaCompuesto);
        var resultadoDescompuesto = servicio.Aprobar(CircuitoEnviado(), Ana, "a\u0301", Instante, tsaDescompuesto);

        Assert.NotEqual(resultadoCompuesto.Hash, resultadoDescompuesto.Hash);
    }

    [Fact]
    public void Aprobar_TsaNoResponde_NoRegistraFirmaNiAvanzaTurno_RF02()
    {
        var circuito = CircuitoEnviado();
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(false, null, "TSA_NO_DISPONIBLE"));
        var servicio = new ServicioAprobacionSellada();

        var resultado = servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa);

        Assert.False(resultado.Exito);
        Assert.Equal("TSA_NO_DISPONIBLE", resultado.Motivo);
        Assert.Empty(circuito.Firmas);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_TsaExcepcion_PropagaSinMutarCircuito_RF02()
    {
        var circuito = CircuitoEnviado();
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(_ => throw new InvalidOperationException("Fallo TSA."));
        var servicio = new ServicioAprobacionSellada();

        var excepcion = Assert.Throws<InvalidOperationException>(() =>
            servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa));

        Assert.Equal("Fallo TSA.", excepcion.Message);
        Assert.Empty(circuito.Firmas);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_TsaNoDevuelveSello_NoRegistraFirma_RF02()
    {
        var circuito = CircuitoEnviado();
        var tsa = TsaExito(null);
        var servicio = new ServicioAprobacionSellada();

        var resultado = servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa);

        Assert.False(resultado.Exito);
        Assert.Equal("TSA_NO_DISPONIBLE", resultado.Motivo);
        Assert.Empty(circuito.Firmas);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_FirmanteFueraDeTurno_LanzaSinRegistrarFirma_RF09()
    {
        var circuito = CircuitoEnviado();
        var tsa = TsaExito("TSA-000123");
        var servicio = new ServicioAprobacionSellada();

        Assert.Throws<CircuitoFirmaException>(() =>
            servicio.Aprobar(circuito, Luis, "Contrato v2", Instante, tsa));

        Assert.Empty(circuito.Firmas);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    private static CircuitoFirma CircuitoEnviado()
    {
        var circuito = new CircuitoFirma("DOC-001", "autor", [Ana, Luis]);
        circuito.Enviar();
        return circuito;
    }

    private static ITsa TsaExito(string? sello)
    {
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(true, sello, null));
        return tsa;
    }
}
