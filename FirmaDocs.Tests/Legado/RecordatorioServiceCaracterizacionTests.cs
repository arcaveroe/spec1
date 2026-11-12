using FirmaDocs.Legado;
using Microsoft.Data.Sqlite;
using NSubstitute;

namespace FirmaDocs.Tests.Legado;

public class RecordatorioServiceCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_RecordatorioService_AvisaPendientesMigradasSinAvisoDeCertificado()
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

        var canal = Substitute.For<ICanalRecordatorio>();
        var servicio = new RecordatorioService(new SolicitudFirmaRepository(fixture.ConnectionString), canal);
        var ahora = new DateTimeOffset(2026, 11, 11, 10, 0, 0, TimeSpan.FromHours(-5));

        var enviados = servicio.EnviarRecordatorios(ahora);

        Assert.Equal(2, enviados);
        canal.Received(1).Enviar("ana@example.com", "Tienes un documento por firmar: DOC-001");
        canal.Received(1).Enviar("carol@example.com", "Tienes un documento por firmar: DOC-008");
        canal.Received(0).Enviar("ana@example.com", "Tienes un documento por firmar: DOC-002");
        canal.DidNotReceive().Enviar("ana@example.com", "Instala tu certificado digital para firmar: DOC-003");
        canal.DidNotReceive().Enviar("ana@example.com", "Instala tu certificado digital para firmar: DOC-004");
    }
}
