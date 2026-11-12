using FirmaDocs.Legado;
using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.Legado;

public class BandejaFirmanteCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_BandejaFirmante_IncluyeSolicitudesMigradasEnPorFirmar()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        using (var conexion = new SqliteConnection(fixture.ConnectionString))
        {
            conexion.Open();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                UPDATE solicitudes_firma
                SET estado = 'PENDIENTE', migrado_desde = 'ESPERANDO_CERTIFICADO'
                WHERE id IN (3, 4);
                """;
            comando.ExecuteNonQuery();
        }

        var bandeja = new BandejaFirmante(new SolicitudFirmaRepository(fixture.ConnectionString))
            .Armar("ana@example.com");

        Assert.Equal(["DOC-001", "DOC-004", "DOC-002", "DOC-003"], bandeja.PorFirmar.Select(solicitud => solicitud.DocumentoId));
        Assert.Empty(bandeja.EsperandoCertificado);
        Assert.Equal(["DOC-005"], bandeja.Historial.Select(solicitud => solicitud.DocumentoId));
    }
}
