using FirmaDocs.Legado;

namespace FirmaDocs.Tests.Legado;

public class SolicitudFirmaRepositoryCaracterizacionTests
{
    private static readonly DateTimeOffset Instante =
        new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));

    [Theory]
    [InlineData(EstadoSolicitud.Firmada)]
    [InlineData(EstadoSolicitud.Rechazada)]
    [InlineData(EstadoSolicitud.Expirada)]
    public void Caracterizacion_SolicitudFirmaRepository_EstadoFinalGuardaFechaResolucion(
        EstadoSolicitud estado)
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);

        repository.CambiarEstado(1, estado, Instante);

        var solicitud = Assert.Single(repository.Listar(), item => item.Id == 1);
        Assert.Equal(estado, solicitud.Estado);
        Assert.Equal(Instante, solicitud.ResueltaEn);
    }

    [Fact]
    public void Caracterizacion_SolicitudFirmaRepository_EstadoPendienteConservaFechaResolucion()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);
        var fechaOriginal = new DateTimeOffset(2026, 11, 1, 12, 0, 0, TimeSpan.FromHours(-5));

        repository.CambiarEstado(5, EstadoSolicitud.Pendiente, Instante);

        var solicitud = Assert.Single(repository.Listar(), item => item.Id == 5);
        Assert.Equal(EstadoSolicitud.Pendiente, solicitud.Estado);
        Assert.Equal(fechaOriginal, solicitud.ResueltaEn);
    }

    [Fact]
    public void Caracterizacion_SolicitudFirmaRepository_CuentaSolicitudesPorEstado()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);

        var conteos = repository.ContarPorEstado();

        Assert.Equal(
            new Dictionary<string, int>
            {
                ["PENDIENTE"] = 5,
                ["FIRMADA"] = 1,
                ["RECHAZADA"] = 1,
                ["EXPIRADA"] = 1
            }.OrderBy(par => par.Key),
            conteos.OrderBy(par => par.Key));
    }
}
