using FirmaDocs.Legado;
using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.Legado;

public class ReporteSlaCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_ReporteSla_IncluyeMigradasDesdeLaCreacionOriginal()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        using (var conexion = new SqliteConnection(fixture.ConnectionString))
        {
            conexion.Open();
            using var comando = conexion.CreateCommand();
            comando.CommandText = """
                UPDATE solicitudes_firma
                SET estado = 'PENDIENTE',
                    migrado_desde = 'ESPERANDO_CERTIFICADO',
                    creada_en = CASE WHEN id = 3 THEN '2026-10-31T09:00:00-05:00' ELSE creada_en END
                WHERE id IN (3, 4);
                """;
            comando.ExecuteNonQuery();
        }

        var reporte = new ReporteSla(new SolicitudFirmaRepository(fixture.ConnectionString));

        var resultado = reporte.HorasPromedioPorMes();

        Assert.Equal(2, resultado.Count);
        Assert.Equal(0, resultado["2026-10"]);
        Assert.Equal(2.29, resultado["2026-11"]);
    }
}
