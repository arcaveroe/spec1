namespace FirmaDocs.Tests.Circuito;

public class TransicionEtapasTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Aprobar_EtapaIncompleta_NoAvanzaNiAprueba_RF06()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Paralela, [Ana, Luis]),
            new(ModoEtapaFirma.Secuencial, [Marta])
        ];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        circuito.Aprobar(Ana, Ahora);

        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal([Luis], circuito.FirmantesHabilitados);
        Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Marta, Ahora));
        Assert.Single(circuito.Firmas);
    }

    [Fact]
    public void Aprobar_UltimoFirmanteUltimaEtapa_CircuitoAprobado_RF06()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Secuencial, [Ana]),
            new(ModoEtapaFirma.Paralela, [Luis, Marta])
        ];
        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);
        circuito.Enviar();

        circuito.Aprobar(Ana, Ahora);
        Assert.Equal([Luis, Marta], circuito.FirmantesHabilitados);

        circuito.Aprobar(Luis, Ahora.AddMinutes(1));
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal([Marta], circuito.FirmantesHabilitados);

        circuito.Aprobar(Marta, Ahora.AddMinutes(2));

        Assert.Equal(EstadoDocumento.Aprobado, circuito.Estado);
        Assert.Empty(circuito.FirmantesHabilitados);
        Assert.Null(circuito.FirmanteEnTurno);
        Assert.Equal(3, circuito.Firmas.Count);
    }
}
