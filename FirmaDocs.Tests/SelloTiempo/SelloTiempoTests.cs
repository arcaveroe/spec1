using FirmaDocs.SelloTiempo;
using NSubstitute;

namespace FirmaDocs.Tests.SelloTiempo;

public sealed class SelloTiempoTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly DateTimeOffset Instante = new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Calcular_ContratoV2_DevuelveHashSha256Utf8_RF01()
    {
        var hash = HashContenido.Calcular("Contrato v2");

        Assert.Equal("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0", hash.Valor);
    }

    [Fact]
    public void Calcular_TextoUnicodeSinNormalizar_DistingueSecuenciasUtf8_RF03()
    {
        var textoCompuesto = HashContenido.Calcular("á");
        var textoDescompuesto = HashContenido.Calcular("a\u0301");

        Assert.NotEqual(textoCompuesto, textoDescompuesto);
    }

    [Fact]
    public void Aprobar_TsaConfirmaHash_RegistrarFirmaSellada_RF01()
    {
        var circuito = CircuitoEnviado();
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(true, "TSA-000123", null));
        var servicio = new ServicioAprobacionSellada();

        var resultado = servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa);

        Assert.True(resultado.Exito);
        Assert.Equal("TSA-000123", resultado.Sello);
        Assert.Equal("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0", resultado.Hash.Valor);
        Assert.Equal(resultado.Hash, circuito.Firmas.Single().HashContenido);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
        tsa.Received(1).Sellar(resultado.Hash);
    }

    [Fact]
    public void Aprobar_TsaNoDisponible_NoRegistraFirmaNiAvanzaTurno_RF02()
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
    public void Aprobar_TsaLanzaExcepcion_PropagaSinMutarCircuito_RF02()
    {
        var circuito = CircuitoEnviado();
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(_ => throw new InvalidOperationException("Fallo inesperado de TSA."));
        var servicio = new ServicioAprobacionSellada();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            servicio.Aprobar(circuito, Ana, "Contrato v2", Instante, tsa));

        Assert.Equal("Fallo inesperado de TSA.", ex.Message);
        Assert.Empty(circuito.Firmas);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_FueraDeTurnoTsaConfirma_LanzaSinRegistrarFirma_RF09()
    {
        var circuito = CircuitoEnviado();
        var tsa = Substitute.For<ITsa>();
        tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(true, "TSA-000123", null));
        var servicio = new ServicioAprobacionSellada();

        Assert.Throws<CircuitoFirmaException>(() =>
            servicio.Aprobar(circuito, Luis, "Contrato v2", Instante, tsa));

        Assert.Empty(circuito.Firmas);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Verificar_ContenidoCoincide_DevuelveHashesYEstadoCoincidente_RF03()
    {
        var hash = HashContenido.Calcular("Contrato v2");
        var firma = new Firma(Ana, 2, Instante) { HashContenido = hash };
        var verificador = new VerificadorContenido();

        var resultado = verificador.Verificar(firma, "Contrato v2");

        Assert.Equal(EstadoVerificacionContenido.Coincide, resultado.Estado);
        Assert.Equal(hash, resultado.HashCalculado);
        Assert.Equal(hash, resultado.HashReferencia);
        Assert.Null(resultado.Motivo);
    }

    [Fact]
    public void Verificar_ContenidoAlterado_DevuelveDocumentoAlterado_RF03()
    {
        var hashReferencia = new HashContenido("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0");
        var firma = new Firma(Ana, 2, Instante) { HashContenido = hashReferencia };
        var verificador = new VerificadorContenido();

        var resultado = verificador.Verificar(firma, "Contrato v2 bis");

        Assert.Equal(EstadoVerificacionContenido.Diferente, resultado.Estado);
        Assert.Equal("a6577c3f7f8e59f087527d5fc31e76788300e87a9ecdd863593ad99f90967803", resultado.HashCalculado.Valor);
        Assert.Equal(hashReferencia, resultado.HashReferencia);
        Assert.Equal("DOCUMENTO_ALTERADO", resultado.Motivo);
    }

    [Fact]
    public void Verificar_FirmaSinHash_NoLaDeclaraCoincidente_RF03()
    {
        var firma = new Firma(Ana, 2, Instante);
        var verificador = new VerificadorContenido();

        var resultado = verificador.Verificar(firma, "Contrato v2");

        Assert.Equal(EstadoVerificacionContenido.SinHashReferencia, resultado.Estado);
        Assert.Equal("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0", resultado.HashCalculado.Valor);
        Assert.Null(resultado.HashReferencia);
        Assert.Null(resultado.Motivo);
    }

    private static CircuitoFirma CircuitoEnviado()
    {
        var circuito = new CircuitoFirma("DOC-001", "autor", [Ana, Luis]);
        circuito.Enviar();
        return circuito;
    }
}
