namespace FirmaDocs.Tests;

public class CircuitoFirmaTests
{
    private static readonly Firmante Ana = new("ana", "Ana Legal");
    private static readonly Firmante Luis = new("luis", "Luis Finanzas");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    private static CircuitoFirma CircuitoEnviado()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);
        circuito.Enviar();
        return circuito;
    }

    [Fact]
    public void Aprobar_TodosEnOrden_DocumentoAprobado()
    {
        var circuito = CircuitoEnviado();

        circuito.Aprobar(Ana, Ahora);
        circuito.Aprobar(Luis, Ahora.AddHours(1));

        Assert.Equal(EstadoDocumento.Aprobado, circuito.Estado);
        Assert.Equal(2, circuito.Firmas.Count);
        Assert.Null(circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_PrimerFirmante_PasaElTurnoAlSiguiente()
    {
        var circuito = CircuitoEnviado();

        circuito.Aprobar(Ana, Ahora);

        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
        Assert.Equal(new Firma(Ana, 1, Ahora), circuito.Firmas[0]);
    }

    [Fact]
    public void Aprobar_FueraDeTurno_LanzaExcepcion()
    {
        var circuito = CircuitoEnviado();

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Luis, Ahora));

        Assert.Equal("No es el turno de Luis Finanzas.", ex.Message);
    }

    [Fact]
    public void Aprobar_SinEnviar_LanzaExcepcion()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Ana, Ahora));

        Assert.Equal("El documento no está en circuito de firma.", ex.Message);
    }

    [Fact]
    public void Rechazar_SinEnviar_LanzaExcepcionConMensaje_RF01()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);

        var ex = Assert.Throws<CircuitoFirmaException>(() =>
            circuito.Rechazar(Ana, "Corregir el documento.", Ahora));

        Assert.Equal("Solo se puede rechazar un documento en circuito de firma.", ex.Message);
        Assert.Equal(EstadoDocumento.Borrador, circuito.Estado);
        Assert.Empty(circuito.Rechazos);
    }

    [Fact]
    public void Rechazar_TerceraVuelta_CierraDocumentoYNoAdmiteReenvio_RF04()
    {
        var circuito = CircuitoEnviado();

        Assert.Equal(1, circuito.Version);

        circuito.Rechazar(Ana, "Corregir el documento.", Ahora);
        circuito.Reenviar("marta");
        circuito.Rechazar(Ana, "Aún requiere cambios.", Ahora.AddDays(1));
        circuito.Reenviar("marta");
        circuito.Rechazar(Ana, "No cumple los requisitos.", Ahora.AddDays(2));

        Assert.Equal(3, circuito.Version);
        Assert.Equal(EstadoDocumento.Rechazado, circuito.Estado);
        Assert.Throws<CircuitoFirmaException>(() => circuito.Reenviar("marta"));
    }

    [Fact]
    public void Reenviar_FueraDeDevuelto_LanzaExcepcionConMensaje_RF02()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Reenviar("marta"));

        Assert.Equal("Solo se puede reenviar un documento devuelto.", ex.Message);
        Assert.Equal(EstadoDocumento.Borrador, circuito.Estado);
        Assert.Equal(1, circuito.Version);
    }

    [Fact]
    public void Reenviar_Devuelto_ConservaFirmantesYOrden_RF02()
    {
        var circuito = CircuitoEnviado();
        circuito.Rechazar(Ana, "Corregir el documento.", Ahora);

        circuito.Reenviar("marta");

        Assert.Equal([Ana, Luis], circuito.Firmantes);
    }

    [Fact]
    public void Constructor_FirmanteRepetido_LanzaExcepcion()
    {
        var ex = Assert.Throws<CircuitoFirmaException>(() => new CircuitoFirma("DOC-001", "marta", [Ana, Ana]));

        Assert.Equal("Un firmante no puede repetirse en el circuito.", ex.Message);
    }
}
