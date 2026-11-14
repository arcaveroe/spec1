namespace FirmaDocs.Tests.Circuito;

public class DelegacionFirmaTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly Firmante Delegada = new("eva", "Eva");
    private static readonly DateTimeOffset Desde = new(2026, 11, 14, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Hasta = Desde.AddHours(2);

    [Fact]
    public void Constructor_DatosValidos_ConservaAutorizacionAcotada_RFDF01()
    {
        List<int> etapas = [1, 2];

        var delegacion = new DelegacionFirma(Ana, Delegada, etapas, Desde, Hasta);
        etapas.Clear();

        Assert.Equal(Ana, delegacion.FirmanteOriginal);
        Assert.Equal(Delegada, delegacion.Delegado);
        Assert.Equal([1, 2], delegacion.Etapas);
        Assert.Equal(Desde, delegacion.VigenteDesde);
        Assert.Equal(Hasta, delegacion.VigenteHasta);
    }

    [Fact]
    public void Constructor_DatosIncompletosOInvalidos_LanzaExcepcion_RFDF01()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DelegacionFirma(null!, Delegada, [1], Desde, Hasta));
        Assert.Throws<ArgumentNullException>(() =>
            new DelegacionFirma(Ana, null!, [1], Desde, Hasta));
        Assert.Throws<CircuitoFirmaException>(() =>
            new DelegacionFirma(Ana, Delegada, [], Desde, Hasta));
        Assert.Throws<CircuitoFirmaException>(() =>
            new DelegacionFirma(Ana, Delegada, [0], Desde, Hasta));
        Assert.Throws<CircuitoFirmaException>(() =>
            new DelegacionFirma(Ana, Delegada, [1, 1], Desde, Hasta));
        Assert.Throws<CircuitoFirmaException>(() =>
            new DelegacionFirma(Ana, Delegada, [1], Hasta, Desde));
        Assert.Throws<CircuitoFirmaException>(() =>
            new DelegacionFirma(Ana, Ana, [1], Desde, Hasta));
    }
}
