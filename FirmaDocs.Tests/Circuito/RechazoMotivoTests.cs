namespace FirmaDocs.Tests.Circuito;

public class RechazoMotivoTests
{
    private static readonly Firmante Ana = new("ana", "Ana");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void Rechazar_MotivoConDiezCaracteresSignificativos_ConservaEspaciosYRegistraRechazo_RF05()
    {
        var circuito = CircuitoEnviado();
        const string motivo = "  1234567890  ";

        circuito.Rechazar(Ana, motivo, Ahora);

        Assert.Equal(EstadoDocumento.Devuelto, circuito.Estado);
        Assert.Equal(new Rechazo(Ana, motivo, 1, Ahora), circuito.Rechazos.Single());
    }

    [Fact]
    public void Rechazar_MotivoConNueveCaracteresSignificativos_LanzaExcepcionSinMutarEstadoNiHistorial_RF05()
    {
        var circuito = CircuitoEnviado();
        const string motivo = "  123456789  ";

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Rechazar(Ana, motivo, Ahora));

        Assert.Equal("El motivo de rechazo debe tener al menos 10 caracteres sin contar espacios iniciales ni finales.", ex.Message);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(1, circuito.Version);
        Assert.Empty(circuito.Rechazos);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rechazar_MotivoVacioOSoloEspacios_LanzaExcepcionSinMutarEstadoNiHistorial_RF05(string motivo)
    {
        var circuito = CircuitoEnviado();

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Rechazar(Ana, motivo, Ahora));

        Assert.Equal("El motivo de rechazo debe tener al menos 10 caracteres sin contar espacios iniciales ni finales.", ex.Message);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Empty(circuito.Rechazos);
    }

    private static CircuitoFirma CircuitoEnviado()
    {
        var circuito = new CircuitoFirma("DOC-001", "autor", [Ana]);
        circuito.Enviar();
        return circuito;
    }
}
