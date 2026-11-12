using FirmaDocs.Legado;

namespace FirmaDocs.Tests.Legado;

public class ReporteSlaCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_ReporteSla_ExcluyeEsperandoCertificadoYPromediaTambienPendientesSinResolver()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var reporte = new ReporteSla(new SolicitudFirmaRepository(fixture.ConnectionString));

        var resultado = reporte.HorasPromedioPorMes();

        Assert.Single(resultado);
        Assert.Equal(2.67, resultado["2026-11"]);
    }
}
