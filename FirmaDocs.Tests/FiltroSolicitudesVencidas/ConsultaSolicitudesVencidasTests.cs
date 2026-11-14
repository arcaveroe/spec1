using FirmaDocs.Legado;
using FirmaDocs.Tests.Legado;
using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.FiltroSolicitudesVencidas;

public class ConsultaSolicitudesVencidasTests
{
    private static readonly DateTimeOffset Instante =
        new(2026, 11, 13, 10, 0, 0, TimeSpan.FromHours(-5));

    [Fact]
    public void ListarVencidas_FiltraPorLimiteEstrictoOrdenaYPagina_RF01()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        AsignarVencimiento(fixture.ConnectionString, 1, "2026-11-13T09:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 2, "2026-11-13T08:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 3, "2026-11-13T10:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 4, "2026-11-13T09:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 5, "2026-11-13T11:00:00-05:00");

        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);

        var primeraPagina = repository.ListarVencidas(Instante, 1, 2, out var total);
        var segundaPagina = repository.ListarVencidas(Instante, 2, 2, out var totalSegundaPagina);

        Assert.Equal(3, total);
        Assert.Equal(total, totalSegundaPagina);
        Assert.Equal([2, 1], primeraPagina.Select(solicitud => solicitud.Id));
        Assert.Equal([4], segundaPagina.Select(solicitud => solicitud.Id));
    }

    [Fact]
    public void ListarVencidas_IgnoraFechasNulasYComparaInstantesConOffset_RF01()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        AsignarVencimiento(fixture.ConnectionString, 1, "2026-11-13T15:00:00+00:00");
        AsignarVencimiento(fixture.ConnectionString, 2, "2026-11-13T14:59:59+00:00");
        var instanteUtc = Instante.ToUniversalTime();
        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);

        var resultados = repository.ListarVencidas(instanteUtc, 1, 50, out var total);

        Assert.Equal(1, total);
        Assert.Equal([2], resultados.Select(solicitud => solicitud.Id));
        Assert.Equal(TimeSpan.Zero, resultados.Single().VenceEn!.Value.Offset);
    }

    [Fact]
    public void ListarVencidas_ConsultasRepetidasConservanDatosDeLaSolicitud_RF02()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        AsignarVencimiento(fixture.ConnectionString, 1, "2026-11-13T09:00:00-05:00");
        var repository = new SolicitudFirmaRepository(fixture.ConnectionString);
        var antes = Assert.Single(repository.Listar(), solicitud => solicitud.Id == 1);

        var primeraConsulta = repository.ListarVencidas(Instante, 1, 50, out _);
        var segundaConsulta = repository.ListarVencidas(Instante, 1, 50, out _);
        var despues = Assert.Single(repository.Listar(), solicitud => solicitud.Id == 1);

        Assert.Equal(primeraConsulta, segundaConsulta);
        Assert.Equal(antes with { VenceEn = DateTimeOffset.Parse("2026-11-13T09:00:00-05:00") }, despues);
        Assert.Equal(EstadoSolicitud.Pendiente, despues.Estado);
        Assert.Null(despues.ResueltaEn);
    }

    private static void AsignarVencimiento(string connectionString, int id, string? venceEn)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE solicitudes_firma SET vence_en = $venceEn WHERE id = $id";
        command.Parameters.AddWithValue("$venceEn", venceEn is null ? DBNull.Value : venceEn);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
