namespace FirmaDocs.Tests.Circuito;

public class EtapaFirmaTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Luis = new("luis", "Luis");
    private static readonly Firmante Marta = new("marta", "Marta");

    [Fact]
    public void Constructor_EntreUnoYCincoFirmantes_EtapaInmutable_RF01()
    {
        var firmantes = new List<Firmante> { Ana, Luis };
        var etapa = new EtapaFirma(ModoEtapaFirma.Secuencial, firmantes);

        firmantes.Add(Marta);

        Assert.Equal(ModoEtapaFirma.Secuencial, etapa.Modo);
        Assert.Equal([Ana, Luis], etapa.Firmantes);
        Assert.Throws<NotSupportedException>((Action)(() => ((IList<Firmante>)etapa.Firmantes).Add(Marta)));
    }

    [Fact]
    public void Constructor_SinFirmantes_LanzaExcepcion_RF01()
    {
        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new EtapaFirma(ModoEtapaFirma.Secuencial, Array.Empty<Firmante>())));
    }

    [Fact]
    public void Constructor_MasDeCincoFirmantes_LanzaExcepcion_RF01()
    {
        var firmantes = Enumerable.Range(1, 6)
            .Select(i => new Firmante($"firmante-{i}", $"Firmante {i}"))
            .ToArray();

        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new EtapaFirma(ModoEtapaFirma.Secuencial, firmantes)));
    }

    [Fact]
    public void Constructor_ModoNoAdmitido_LanzaExcepcion_RF01()
    {
        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new EtapaFirma((ModoEtapaFirma)99, [Ana])));
    }

    [Fact]
    public void Constructor_CeroOMasDeCincoEtapas_LanzaExcepcion_RF01()
    {
        var sinEtapas = Array.Empty<EtapaFirma>();
        var seisEtapas = Enumerable.Range(0, 6)
            .Select(_ => new EtapaFirma(ModoEtapaFirma.Secuencial, [Ana]))
            .ToArray();

        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new CircuitoFirma("DOC-001", "autor", sinEtapas)));
        Assert.Throws<CircuitoFirmaException>((Action)(() =>
            _ = new CircuitoFirma("DOC-001", "autor", seisEtapas)));
    }

    [Fact]
    public void Constructor_EntreUnaYCincoEtapas_AceptaConfiguracion_RF01()
    {
        var etapas = new[]
        {
            new EtapaFirma(ModoEtapaFirma.Secuencial, [Ana]),
            new EtapaFirma(ModoEtapaFirma.Paralela, [Luis, Marta])
        };

        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);

        Assert.Equal(etapas, circuito.Etapas);
    }

    [Fact]
    public void Constructor_CincoEtapasYFirmantes_AceptaLimite_RF01()
    {
        var cincoFirmantes = Enumerable.Range(1, 5)
            .Select(i => new Firmante($"firmante-{i}", $"Firmante {i}"))
            .ToArray();
        var etapas = Enumerable.Range(0, 5)
            .Select(i => new EtapaFirma(ModoEtapaFirma.Secuencial, [cincoFirmantes[i]]))
            .ToArray();

        var circuito = new CircuitoFirma("DOC-001", "autor", etapas);

        Assert.Equal(5, circuito.Etapas.Count);
        Assert.Equal(5, new EtapaFirma(ModoEtapaFirma.Paralela, cincoFirmantes).Firmantes.Count);
    }
}
