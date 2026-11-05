using SistemaHotelaria.Repositories;
using SistemaHotelaria.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string? connectionString = Environment.GetEnvironmentVariable("HOTELARIA_DB");

if (connectionString == null)
{
    Console.WriteLine("Variável de ambiente HOTELARIA_DB não encontrada.");
    return;
}
builder.Services.AddScoped<ReservaRepository>(provider => new ReservaRepository(connectionString));
builder.Services.AddScoped<HospedeRepository>(provider => new HospedeRepository(connectionString));
builder.Services.AddScoped<HospedeService>();

builder.Services.AddScoped<QuartoRepository>(provider => new QuartoRepository(connectionString));
builder.Services.AddScoped<QuartoService>();

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

app.MapGet("/", () => Results.Redirect("/Hospedes"));
app.MapControllerRoute(name: "default", pattern: "{controller}/{action=Index}/{id?}");

app.Run();