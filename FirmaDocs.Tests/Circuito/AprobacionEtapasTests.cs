namespace FirmaDocs.Tests.Circuito;

public class AprobacionEtapasTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Aprobar_Secuencial_SoloSiguienteFirmanteHabilitado_RF04()
    {
        EtapaFirma[] etapas = [new(ModoEtapaFirma.Secuencial, [Ana, Luis])];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        Assert.Equal([Ana], circuito.FirmantesHabilitados);
        Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Luis, Ahora));

        circuito.Aprobar(Ana, Ahora);

        Assert.Equal([Luis], circuito.FirmantesHabilitados);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_ParalelaEnCualquierOrden_HabilitaPendientesHastaCompletarEtapa_RF05()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Paralela, [Ana, Luis]),
            new(ModoEtapaFirma.Secuencial, [Marta])
        ];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        Assert.Equal([Ana, Luis], circuito.FirmantesHabilitados);
        Assert.Null(circuito.FirmanteEnTurno);

        circuito.Aprobar(Luis, Ahora);

        Assert.Equal([Ana], circuito.FirmantesHabilitados);
        circuito.Aprobar(Ana, Ahora.AddMinutes(1));

        Assert.Equal([Marta], circuito.FirmantesHabilitados);
        Assert.Equal(Marta, circuito.FirmanteEnTurno);
    }
}
