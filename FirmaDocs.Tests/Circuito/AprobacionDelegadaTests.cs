namespace FirmaDocs.Tests.Circuito;

public class AprobacionDelegadaTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Delegada = new("eva", "Eva");
    private static readonly DateTimeOffset Desde = new(2026, 11, 14, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Hasta = Desde.AddHours(2);

    [Fact]
    public void AprobarPorDelegacion_Vigente_RegistraFirmanteOriginalYEjecutor_RFDF03()
    {
        var circuito = CrearCircuito();
        circuito.RegistrarDelegacion(new DelegacionFirma(Ana, Delegada, [1], Desde, Hasta));
        circuito.Enviar();

        circuito.AprobarPorDelegacion(Ana, Delegada, Desde);

        Assert.Equal(Ana, circuito.Firmas[0].Firmante);
        Assert.Equal(Delegada, circuito.Firmas[0].DelegadoEjecutor);
    }

    [Fact]
    public void AprobarPorDelegacion_CaducadaOFueraDeTurno_RechazaLaOperacion_RFDF02()
    {
        var circuito = CrearCircuito();
        circuito.RegistrarDelegacion(new DelegacionFirma(Ana, Delegada, [1], Desde, Hasta));
        circuito.Enviar();

        Assert.Throws<CircuitoFirmaException>(() => circuito.AprobarPorDelegacion(Ana, Delegada, Hasta));
        Assert.Throws<CircuitoFirmaException>(() => circuito.AprobarPorDelegacion(new Firmante("luis", "Luis"), Delegada, Desde));
        Assert.Empty(circuito.Firmas);
    }

    [Fact]
    public void Aprobar_FirmanteOriginal_NoRegistraDelegado_RFDF03()
    {
        var circuito = CrearCircuito();
        circuito.RegistrarDelegacion(new DelegacionFirma(Ana, Delegada, [1], Desde, Hasta));
        circuito.Enviar();

        circuito.Aprobar(Ana, Desde);

        Assert.Null(circuito.Firmas[0].DelegadoEjecutor);
    }

    private static CircuitoFirma CrearCircuito() =>
        new("DOC-001", "autor", [new EtapaFirma(ModoEtapaFirma.Secuencial, [Ana])]);
}
