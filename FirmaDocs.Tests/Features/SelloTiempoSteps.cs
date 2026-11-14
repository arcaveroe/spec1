using FirmaDocs.SelloTiempo;
using NSubstitute;
using Reqnroll;

namespace FirmaDocs.Tests.Features;

[Binding]
public sealed class SelloTiempoSteps
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly DateTimeOffset Instante = new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));
    private const string HashContratoV2 = "74474ce77a56aa4b6c5bb611a6a4945a56380dd1a736fb47857df09bd9ddfde0";

    private CircuitoFirma _circuito = null!;
    private ITsa _tsa = null!;
    private string _contenido = null!;
    private ServicioAprobacionSellada _servicio = null!;
    private ResultadoAprobacionSellada? _resultadoAprobacion;
    private Firma _firma = null!;
    private VerificadorContenido _verificador = null!;
    private ResultadoVerificacionContenido? _resultadoVerificacion;
    private string _hashSolicitadoEsperado = HashContratoV2;

    [Given(@"que la versión 2 contiene ""(.*)""")]
    [Given(@"que Ana tiene el turno para la versión 2 y su contenido es ""(.*)""")]
    [Given(@"que Ana tiene el turno en una etapa secuencial y el contenido es ""(.*)""")]
    public void DadoQueLaVersionDosContiene(string contenido)
    {
        _circuito = CrearCircuitoVersionDos();
        _contenido = contenido;
        _tsa = Substitute.For<ITsa>();
        _servicio = new ServicioAprobacionSellada();
    }

    [When(@"la TSA confirma el hash y devuelve el sello ""(.*)""")]
    public void CuandoLaTsaConfirmaElHash(string sello)
    {
        _tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(true, sello, null));
        _resultadoAprobacion = _servicio.Aprobar(_circuito, Ana, _contenido, Instante, _tsa);
    }

    [When(@"la TSA no responde a la solicitud de sellado")]
    public void CuandoLaTsaNoRespondeSinHash()
    {
        CuandoLaTsaNoResponde();
    }

    [When(@"la TSA no responde a la solicitud para sellar el hash ""(.*)""")]
    public void CuandoLaTsaNoRespondeConHash(string hashEsperado)
    {
        _hashSolicitadoEsperado = hashEsperado;
        CuandoLaTsaNoResponde();
    }

    private void CuandoLaTsaNoResponde()
    {
        _tsa.Sellar(Arg.Any<HashContenido>())
            .Returns(new ResultadoTsa(false, null, "TSA_NO_DISPONIBLE"));
        _resultadoAprobacion = _servicio.Aprobar(_circuito, Ana, _contenido, Instante, _tsa);
    }

    [Then(@"se registra la Firma con el hash ""(.*)"" y el resultado incluye el sello ""(.*)""")]
    public void EntoncesSeRegistraLaFirmaSellada(string hashEsperado, string selloEsperado)
    {
        Assert.Equal(hashEsperado, _resultadoAprobacion?.Hash.Valor);
        Assert.Equal(selloEsperado, _resultadoAprobacion?.Sello);
        Assert.Equal(hashEsperado, _circuito.Firmas.SingleOrDefault()?.HashContenido?.Valor);
        _tsa.Received(1).Sellar(Arg.Is<HashContenido>(hash => hash.Valor == hashEsperado));
    }

    [Then(@"no se registra la Firma, el turno permanece en ""(.*)"" y el motivo es ""(.*)""")]
    public void EntoncesElCircuitoConservaElTurno(string firmanteEsperado, string motivoEsperado)
    {
        Assert.Empty(_circuito.Firmas);
        Assert.Equal(EstadoDocumento.EnCircuito, _circuito.Estado);
        Assert.Equal(ModoEtapaFirma.Secuencial, _circuito.Etapas[0].Modo);
        Assert.Equal(firmanteEsperado, _circuito.FirmanteEnTurno?.Nombre);
        Assert.Equal(motivoEsperado, _resultadoAprobacion?.Motivo);
        _tsa.Received(1).Sellar(Arg.Is<HashContenido>(hash =>
            hash.Valor == _hashSolicitadoEsperado));
    }

    [Given(@"que la Firma de la versión 2 tiene el hash ""(.*)""")]
    public void DadoQueLaFirmaTieneUnHash(string hashReferencia)
    {
        _circuito = CrearCircuitoVersionDos();
        _circuito.Aprobar(Ana, Instante);
        _firma = _circuito.Firmas.Single() with
        {
            HashContenido = new HashContenido(hashReferencia)
        };
        _tsa = Substitute.For<ITsa>();
        _verificador = new VerificadorContenido();
    }

    [When(@"se verifica el contenido ""(.*)""")]
    [When(@"se verifica el contenido cambiado ""(.*)""")]
    public void CuandoSeVerificaElContenido(string contenido)
    {
        _resultadoVerificacion = _verificador.Verificar(_firma, contenido);
    }

    [Then(@"el contenido coincide con el hash ""(.*)""")]
    public void EntoncesElContenidoCoincide(string hashEsperado)
    {
        Assert.Equal(EstadoVerificacionContenido.Coincide, _resultadoVerificacion?.Estado);
        Assert.Equal(hashEsperado, _resultadoVerificacion?.HashCalculado.Valor);
        Assert.Equal(hashEsperado, _resultadoVerificacion?.HashReferencia?.Valor);
        _tsa.DidNotReceive().Sellar(Arg.Any<HashContenido>());
    }

    [Then(@"el hash calculado es ""(.*)"" y el motivo es ""(.*)""")]
    public void EntoncesElHashCalculadoDifiere(string hashEsperado, string motivoEsperado)
    {
        Assert.Equal(EstadoVerificacionContenido.Diferente, _resultadoVerificacion?.Estado);
        Assert.Equal(hashEsperado, _resultadoVerificacion?.HashCalculado.Valor);
        Assert.Equal(motivoEsperado, _resultadoVerificacion?.Motivo);
        _tsa.DidNotReceive().Sellar(Arg.Any<HashContenido>());
    }

    private static CircuitoFirma CrearCircuitoVersionDos()
    {
        var circuito = new CircuitoFirma("DOC-001", "autor", new[] { Ana });
        circuito.Enviar();
        circuito.Rechazar(Ana, "Ajustar contenido.", Instante);
        circuito.Reenviar("autor");
        return circuito;
    }
}
