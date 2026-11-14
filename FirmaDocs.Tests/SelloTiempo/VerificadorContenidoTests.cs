using FirmaDocs.SelloTiempo;

namespace FirmaDocs.Tests.SelloTiempo;

public sealed class VerificadorContenidoTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly DateTimeOffset Instante = new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Verificar_ContenidoCoincide_DevuelveHashYEstadoCoincidente_RF03()
    {
        var hash = new HashContenido("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0");
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
    public void Verificar_FirmaSinHash_IndicaReferenciaAusente_RF03()
    {
        var firma = new Firma(Ana, 2, Instante);
        var verificador = new VerificadorContenido();

        var resultado = verificador.Verificar(firma, "Contrato v2");

        Assert.Equal(EstadoVerificacionContenido.SinHashReferencia, resultado.Estado);
        Assert.Equal("74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0", resultado.HashCalculado.Valor);
        Assert.Null(resultado.HashReferencia);
        Assert.Null(resultado.Motivo);
    }

    [Fact]
    public void Verificar_FirmaNula_LanzaArgumentNullException_RF03()
    {
        var verificador = new VerificadorContenido();

        var excepcion = Assert.Throws<ArgumentNullException>(() =>
            verificador.Verificar(null!, "Contrato v2"));

        Assert.Equal("firma", excepcion.ParamName);
    }
}
