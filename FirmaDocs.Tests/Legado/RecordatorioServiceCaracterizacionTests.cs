using FirmaDocs.Legado;
using NSubstitute;

namespace FirmaDocs.Tests.Legado;

public class RecordatorioServiceCaracterizacionTests
{
    [Fact]
    public void Caracterizacion_RecordatorioService_AvisaPendientesAntiguasYSiempreAQuienesEsperanCertificado()
    {
        using var fixture = new SqliteCaracterizacionFixture();
        var canal = Substitute.For<ICanalRecordatorio>();
        var servicio = new RecordatorioService(new SolicitudFirmaRepository(fixture.ConnectionString), canal);
        var ahora = new DateTimeOffset(2026, 11, 11, 10, 0, 0, TimeSpan.FromHours(-5));

        var enviados = servicio.EnviarRecordatorios(ahora);

        Assert.Equal(4, enviados);
        canal.Received(1).Enviar("ana@example.com", "Tienes un documento por firmar: DOC-001");
        canal.Received(1).Enviar("carol@example.com", "Tienes un documento por firmar: DOC-008");
        canal.Received(1).Enviar("ana@example.com", "Instala tu certificado digital para firmar: DOC-003");
        canal.Received(1).Enviar("ana@example.com", "Instala tu certificado digital para firmar: DOC-004");
        canal.Received(0).Enviar("ana@example.com", "Tienes un documento por firmar: DOC-002");
    }
}
