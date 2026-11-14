using FirmaDocs.Legado;

namespace FirmaDocs.Api;

/// <summary>
/// Registra el endpoint de consulta paginada de solicitudes vencidas.
/// </summary>
public static class SolicitudesVencidasEndpoints
{
    /// <summary>
    /// Añade GET /api/solicitudes/vencidas al enrutador.
    /// </summary>
    /// <param name="endpoints">Enrutador donde se registra el endpoint.</param>
    /// <returns>El enrutador recibido.</returns>
    public static IEndpointRouteBuilder MapSolicitudesVencidas(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/solicitudes/vencidas", BuscarVencidas);
        return endpoints;
    }

    private static IResult BuscarVencidas(
        DateTimeOffset? instante,
        int? page,
        int? pageSize,
        SolicitudFirmaRepository repository)
    {
        if (instante is null)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["instante"] = ["El instante de referencia es obligatorio."]
            });

        var pagina = page ?? 1;
        var tamanoPagina = pageSize ?? 50;
        var errores = new Dictionary<string, string[]>();
        if (pagina < 1)
            errores["page"] = ["La página debe ser mayor que cero."];
        if (tamanoPagina < 1)
            errores["pageSize"] = ["El tamaño de página debe ser mayor que cero."];
        if (errores.Count > 0)
            return Results.ValidationProblem(errores);

        var solicitudes = repository.ListarVencidas(instante.Value, pagina, tamanoPagina, out var total);
        var items = solicitudes.Select(solicitud => new
        {
            solicitud.Id,
            solicitud.DocumentoId,
            solicitud.FirmanteEmail,
            solicitud.Vuelta,
            solicitud.CreadaEn,
            solicitud.ResueltaEn,
            Estado = EstadoSolicitudTexto.ATexto(solicitud.Estado),
            solicitud.MigradoDesde,
            solicitud.VenceEn
        });

        return Results.Ok(new
        {
            Items = items,
            Page = pagina,
            PageSize = tamanoPagina,
            TotalCount = total
        });
    }
}
