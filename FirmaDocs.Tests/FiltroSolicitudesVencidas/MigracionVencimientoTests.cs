using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.FiltroSolicitudesVencidas;

public class MigracionVencimientoTests
{
    [Fact]
    public void MigrarVencimiento_ConservaFilasSinInventarFechas_RF_FSV04()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE solicitudes_firma (
                id INTEGER PRIMARY KEY,
                documento_id TEXT NOT NULL,
                firmante_email TEXT NOT NULL,
                vuelta INTEGER NOT NULL DEFAULT 1,
                creada_en TEXT NOT NULL,
                resuelta_en TEXT NULL,
                estado TEXT NOT NULL,
                migrado_desde TEXT NULL
            );
            INSERT INTO solicitudes_firma
                (id, documento_id, firmante_email, creada_en, estado)
            VALUES
                (1, 'DOC-001', 'ana@example.com', '2026-11-07T10:00:00-05:00', 'PENDIENTE');
            """;
        command.ExecuteNonQuery();

        command.CommandText = File.ReadAllText(EncontrarMigracion("004_agregar_vence_en.sql"));
        command.ExecuteNonQuery();
        command.CommandText = "SELECT documento_id, vence_en FROM solicitudes_firma WHERE id = 1";
        using (var reader = command.ExecuteReader())
        {
            Assert.True(reader.Read());
            Assert.Equal("DOC-001", reader.GetString(0));
            Assert.True(reader.IsDBNull(1));
        }

        command.CommandText = File.ReadAllText(EncontrarMigracion("004_rollback.sql"));
        command.ExecuteNonQuery();
        command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('solicitudes_firma') WHERE name = 'vence_en'";
        Assert.Equal(0L, (long)(command.ExecuteScalar() ?? -1L));
        command.CommandText = "SELECT documento_id FROM solicitudes_firma WHERE id = 1";
        Assert.Equal("DOC-001", command.ExecuteScalar());
    }

    private static string EncontrarMigracion(string nombre)
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            var path = Path.Combine(directory.FullName, "db", "migrations", nombre);
            if (File.Exists(path))
                return path;
        }

        throw new FileNotFoundException($"No se encontró la migración {nombre}.");
    }
}
