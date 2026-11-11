namespace FirmaDocs.Tests.Circuito;

public class HistorialFirmasTests
{
    [Fact]
    public void Firmas_AlConsultarHistorial_NoPermiteModificarRegistros_RF08()
    {
        var ana = new Firmante("ana", "Ana");
        var circuito = new CircuitoFirma("DOC-001", "autor", [ana]);
        circuito.Enviar();
        circuito.Aprobar(ana, new DateTimeOffset(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5)));
        var firma = circuito.Firmas[0];

        Assert.Throws<NotSupportedException>((Action)(() => ((IList<Firma>)circuito.Firmas).Clear()));
        Assert.Equal([firma], circuito.Firmas);
    }
}
