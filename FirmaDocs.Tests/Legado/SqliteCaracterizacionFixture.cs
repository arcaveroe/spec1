using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.Legado;

internal sealed class SqliteCaracterizacionFixture : IDisposable
{
    private readonly string _databasePath;

    public SqliteCaracterizacionFixture()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"firmadocs-caracterizacion-{Guid.NewGuid():N}.db");
        ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false
        }.ToString();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = File.ReadAllText(EncontrarSchema());
        command.ExecuteNonQuery();

        command.CommandText = """
            INSERT INTO solicitudes_firma
                (id, documento_id, firmante_email, vuelta, creada_en, resuelta_en, estado, migrado_desde)
            VALUES
                (1, 'DOC-001', 'ana@example.com', 1, '2026-11-07T10:00:00-05:00', NULL, 'PENDIENTE', NULL),
                (2, 'DOC-002', 'ana@example.com', 1, '2026-11-10T10:00:00-05:00', NULL, 'PENDIENTE', NULL),
                (3, 'DOC-003', 'ana@example.com', 1, '2026-11-11T09:00:00-05:00', NULL, 'PENDIENTE', 'ESPERANDO_CERTIFICADO'),
                (4, 'DOC-004', 'ana@example.com', 1, '2026-11-09T10:00:00-05:00', NULL, 'PENDIENTE', 'ESPERANDO_CERTIFICADO'),
                (5, 'DOC-005', 'ana@example.com', 1, '2026-11-01T08:00:00-05:00', '2026-11-01T12:00:00-05:00', 'FIRMADA', NULL),
                (6, 'DOC-006', 'bob@example.com', 1, '2026-11-02T08:00:00-05:00', '2026-11-02T20:00:00-05:00', 'RECHAZADA', NULL),
                (7, 'DOC-007', 'bob@example.com', 1, '2026-11-03T08:00:00-05:00', NULL, 'EXPIRADA', NULL),
                (8, 'DOC-008', 'carol@example.com', 1, '2026-11-04T08:00:00-05:00', NULL, 'PENDIENTE', NULL);
            """;
        command.ExecuteNonQuery();
    }

    public string ConnectionString { get; }

    public void Dispose()
    {
        File.Delete(_databasePath);
    }

    private static string EncontrarSchema()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            var schemaPath = Path.Combine(directory.FullName, "db", "schema.sql");
            if (File.Exists(schemaPath))
                return schemaPath;
        }

        throw new FileNotFoundException("No se encontró db/schema.sql desde el directorio de pruebas.");
    }
}
