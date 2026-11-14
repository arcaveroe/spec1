using FirmaDocs.Api;
using FirmaDocs.Legado;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Solicitudes")
    ?? throw new InvalidOperationException(
        "Configure ConnectionStrings:Solicitudes con la conexión a la base de datos.");

builder.Services.AddSingleton(new SolicitudFirmaRepository(connectionString));

var app = builder.Build();
app.MapSolicitudesVencidas();
app.Run();
