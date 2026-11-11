namespace FirmaDocs.Tests.Circuito;

public class CircuitoEtapasTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");

    [Fact]
    public void Constructor_FirmanteRepetidoEntreEtapas_LanzaExcepcion_RF02()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Secuencial, [Ana]),
            new(ModoEtapaFirma.Paralela, [Luis, Ana])
        ];

        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new CircuitoFirma("DOC-001", "autor", etapas)));
    }

    [Fact]
    public void Constructor_FirmantesDistintosEntreEtapas_AceptaYConservaOrden_RF02()
    {
        EtapaFirma[] etapas =
        [
            new(ModoEtapaFirma.Secuencial, [Ana]),
            new(ModoEtapaFirma.Paralela, [Luis, Marta])
        ];

        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);

        Assert.Equal([Ana, Luis, Marta], circuito.Firmantes);
    }

    [Fact]
    public void Constructor_ListaPlana_CreaUnaEtapaSecuencialEnElMismoOrden_RF03()
    {
        var circuito = new CircuitoFirma("DOC-001", "autor", [Ana, Luis]);

        Assert.Equal([Ana, Luis], circuito.Firmantes);
        Assert.Single(circuito.Etapas);
        Assert.Equal(ModoEtapaFirma.Secuencial, circuito.Etapas[0].Modo);
        Assert.Equal([Ana, Luis], circuito.Etapas[0].Firmantes);
    }

    [Fact]
    public void Constructor_ListaPlanaConMasDeCincoFirmantes_LanzaExcepcion_RF03()
    {
        var firmantes = Enumerable.Range(1, 6)
            .Select(i => new Firmante($"firmante-{i}", $"Firmante {i}"))
            .ToArray();

        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new CircuitoFirma("DOC-001", "autor", firmantes)));
    }
}
