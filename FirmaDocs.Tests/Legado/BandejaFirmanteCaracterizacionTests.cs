using FirmaDocs.Legado;

namespace FirmaDocs.Tests.Legado;

public class BandejaFirmanteCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_BandejaFirmante_SeparaSolicitudesPorEstadoYOrdenaPorCreacion()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var bandeja = new BandejaFirmante(new SolicitudFirmaRepository(fixture.ConnectionString))
            .Armar("ana@example.com");

        Assert.Equal(["DOC-001", "DOC-002"], bandeja.PorFirmar.Select(solicitud => solicitud.DocumentoId));
        Assert.Equal(["DOC-004", "DOC-003"], bandeja.EsperandoCertificado.Select(solicitud => solicitud.DocumentoId));
        Assert.Equal(["DOC-005"], bandeja.Historial.Select(solicitud => solicitud.DocumentoId));
    }
}
