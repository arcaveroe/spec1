using System.Text.Json;
using FirmaDocs.Api;
using FirmaDocs.Legado;
using FirmaDocs.Tests.Legado;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace FirmaDocs.Tests.FiltroSolicitudesVencidas;

public class ApiSolicitudesVencidasTests
{
    [Fact]
    public async Task GetSolicitudesVencidas_AplicaLimiteYPaginacion_RF_FSV01()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        AsignarVencimiento(fixture.ConnectionString, 1, "2026-11-13T09:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 2, "2026-11-13T10:00:00-05:00");
        AsignarVencimiento(fixture.ConnectionString, 3, "2026-11-13T11:00:00-05:00");
        var api = await IniciarApi(fixture.ConnectionString);
        await using var app = api.App;
        using var client = api.Client;

        using var response = await client.GetAsync(
            "/api/solicitudes/vencidas?instante=2026-11-13T10%3A00%3A00-05%3A00&page=1&pageSize=1");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = body.RootElement;

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(1, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(1, root.GetProperty("totalCount").GetInt64());
        Assert.Equal(1, root.GetProperty("items")[0].GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetSolicitudesVencidas_RechazaInstanteAusente_RF_FSV03()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var api = await IniciarApi(fixture.ConnectionString);
        await using var app = api.App;
        using var client = api.Client;

        using var response = await client.GetAsync("/api/solicitudes/vencidas");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetSolicitudesVencidas_UsaPaginacionPorDefecto_RF_FSV01()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        AsignarVencimiento(fixture.ConnectionString, 1, "2026-11-13T09:00:00-05:00");
        var api = await IniciarApi(fixture.ConnectionString);
        await using var app = api.App;
        using var client = api.Client;

        using var response = await client.GetAsync(
            "/api/solicitudes/vencidas?instante=2026-11-13T10%3A00%3A00-05%3A00");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = body.RootElement;

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(50, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(1, root.GetProperty("totalCount").GetInt64());
    }

    [Theory]
    [InlineData("page=0")]
    [InlineData("pageSize=0")]
    public async Task GetSolicitudesVencidas_RechazaPaginacionNoPositiva_RF_FSV01(string paginacion)
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var api = await IniciarApi(fixture.ConnectionString);
        await using var app = api.App;
        using var client = api.Client;

        using var response = await client.GetAsync(
            $"/api/solicitudes/vencidas?instante=2026-11-13T10%3A00%3A00-05%3A00&{paginacion}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<(WebApplication App, HttpClient Client)> IniciarApi(string connectionString)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSingleton(new SolicitudFirmaRepository(connectionString));
        var app = builder.Build();
        app.MapSolicitudesVencidas();
        await app.StartAsync();
        var addresses = app.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!;
        return (app, new HttpClient { BaseAddress = new Uri(addresses.Addresses.Single()) });
    }

    private static void AsignarVencimiento(string connectionString, int id, string venceEn)
    {
        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE solicitudes_firma SET vence_en = $venceEn WHERE id = $id";
        command.Parameters.AddWithValue("$venceEn", venceEn);
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
