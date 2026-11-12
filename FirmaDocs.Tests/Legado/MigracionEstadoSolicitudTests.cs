using Microsoft.Data.Sqlite;

namespace FirmaDocs.Tests.Legado;

public class MigracionEstadoSolicitudTests
{
    [Fact]
    public void MigracionYRollback_ConservanDatosYRestauranEstados_RF08()
    {
        var directorioProyecto = EncontrarDirectorioProyecto();
        var rutaBase = Path.Combine(Path.GetTempPath(), $"firmadocs-migracion-{Guid.NewGuid():N}.db");
        var cadenaConexion = new SqliteConnectionStringBuilder
        {
            DataSource = rutaBase,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false
        }.ToString();

        try
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();
            EjecutarArchivo(conexion, Path.Combine(directorioProyecto, "db", "schema.sql"));
            EjecutarArchivo(conexion, Path.Combine(directorioProyecto, "db", "seed.sql"));

            var originales = LeerSolicitudes(conexion);
            var conteoOriginal = ContarPorEstado(originales);

            EjecutarArchivo(conexion, Path.Combine(directorioProyecto, "db", "migrations", "003_eliminar_esperando_certificado.sql"));

            var migradas = LeerSolicitudes(conexion);
            var esperadasMigradas = originales
                .Select(solicitud => solicitud.Estado == "ESPERANDO_CERTIFICADO"
                    ? solicitud with { Estado = "PENDIENTE", MigradoDesde = "ESPERANDO_CERTIFICADO" }
                    : solicitud)
                .ToArray();

            Assert.Equal(esperadasMigradas, migradas);
            Assert.DoesNotContain(migradas, solicitud => solicitud.Estado == "ESPERANDO_CERTIFICADO");
            Assert.Equal(conteoOriginal.GetValueOrDefault("ESPERANDO_CERTIFICADO"), migradas.Count(solicitud => solicitud.MigradoDesde == "ESPERANDO_CERTIFICADO"));
            Assert.DoesNotContain("'ESPERANDO_CERTIFICADO'", LeerDefinicionTabla(conexion));
            AssertIndiceEstadoExiste(conexion);
            Assert.Throws<SqliteException>(() => Ejecutar(conexion, """
                INSERT INTO solicitudes_firma
                    (id, documento_id, firmante_email, vuelta, creada_en, estado, migrado_desde)
                VALUES
                    (1001, 'DOC-TEST', 'test@example.com', 1, '2026-11-11T00:00:00-05:00', 'ESPERANDO_CERTIFICADO', NULL);
                """));

            var rutaRollback = Path.Combine(directorioProyecto, "db", "migrations", "003_rollback.sql");
            EjecutarArchivo(conexion, rutaRollback);
            Assert.Equal(conteoOriginal.OrderBy(par => par.Key), ContarPorEstado(LeerSolicitudes(conexion)).OrderBy(par => par.Key));
            Assert.Equal(
                originales.Select(solicitud => solicitud.Estado == "ESPERANDO_CERTIFICADO"
                    ? solicitud with { MigradoDesde = "ESPERANDO_CERTIFICADO" }
                    : solicitud).ToArray(),
                LeerSolicitudes(conexion));
            Assert.Contains("'ESPERANDO_CERTIFICADO'", LeerDefinicionTabla(conexion));
            AssertIndiceEstadoExiste(conexion);

            EjecutarArchivo(conexion, rutaRollback);
            Assert.Equal(conteoOriginal.OrderBy(par => par.Key), ContarPorEstado(LeerSolicitudes(conexion)).OrderBy(par => par.Key));
            Assert.Equal(
                originales.Select(solicitud => solicitud.Estado == "ESPERANDO_CERTIFICADO"
                    ? solicitud with { MigradoDesde = "ESPERANDO_CERTIFICADO" }
                    : solicitud).ToArray(),
                LeerSolicitudes(conexion));
            AssertIndiceEstadoExiste(conexion);
        }
        finally
        {
            if (File.Exists(rutaBase))
                File.Delete(rutaBase);
        }
    }

    private static string EncontrarDirectorioProyecto()
    {
        for (var directorio = new DirectoryInfo(AppContext.BaseDirectory);
             directorio is not null;
             directorio = directorio.Parent)
        {
            if (File.Exists(Path.Combine(directorio.FullName, "db", "schema.sql"))
                && File.Exists(Path.Combine(directorio.FullName, "db", "seed.sql")))
                return directorio.FullName;
        }

        throw new DirectoryNotFoundException("No se encontró el directorio del proyecto con db/schema.sql y db/seed.sql.");
    }

    private static void EjecutarArchivo(SqliteConnection conexion, string ruta)
    {
        Ejecutar(conexion, File.ReadAllText(ruta));
    }

    private static void Ejecutar(SqliteConnection conexion, string sql)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        comando.ExecuteNonQuery();
    }

    private static List<SolicitudSnapshot> LeerSolicitudes(SqliteConnection conexion)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = """
            SELECT id, documento_id, firmante_email, vuelta, creada_en, resuelta_en, estado, migrado_desde
            FROM solicitudes_firma
            ORDER BY id;
            """;

        using var lector = comando.ExecuteReader();
        var solicitudes = new List<SolicitudSnapshot>();
        while (lector.Read())
        {
            solicitudes.Add(new SolicitudSnapshot(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                lector.GetInt32(3),
                lector.GetString(4),
                lector.IsDBNull(5) ? null : lector.GetString(5),
                lector.GetString(6),
                lector.IsDBNull(7) ? null : lector.GetString(7)));
        }

        return solicitudes;
    }

    private static Dictionary<string, int> ContarPorEstado(IEnumerable<SolicitudSnapshot> solicitudes)
    {
        return solicitudes
            .GroupBy(solicitud => solicitud.Estado)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Count());
    }

    private static string LeerDefinicionTabla(SqliteConnection conexion)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT sql FROM sqlite_master WHERE type = 'table' AND name = 'solicitudes_firma';";
        return (string)comando.ExecuteScalar()!;
    }

    private static void AssertIndiceEstadoExiste(SqliteConnection conexion)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = 'ix_solicitudes_estado';";
        Assert.Equal(1L, (long)comando.ExecuteScalar()!);
    }

    private sealed record SolicitudSnapshot(
        int Id,
        string DocumentoId,
        string FirmanteEmail,
        int Vuelta,
        string CreadaEn,
        string? ResueltaEn,
        string Estado,
        string? MigradoDesde);
}
