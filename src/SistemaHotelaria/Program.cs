using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Repositories;
using SistemaHotelaria.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string? connectionString = Environment.GetEnvironmentVariable("HOTELARIA_DB");

if (connectionString == null)
{
    Console.WriteLine("Variável de ambiente HOTELARIA_DB não encontrada.");
    return;
}

builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<ReservaRepository>(provider => new ReservaRepository(connectionString));
builder.Services.AddScoped<HospedeRepository>(provider => new HospedeRepository(connectionString));
builder.Services.AddScoped<HospedeService>();

builder.Services.AddScoped<QuartoRepository>(provider => new QuartoRepository(connectionString));
builder.Services.AddScoped<QuartoService>();

builder.Services.AddControllers(options =>
    {
        // Deixa as validações de campo obrigatório para os services (com as mensagens deles)
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // JSON mal formado ou com tipo errado (ex.: texto no campo numero) também responde { "mensagem": "..." }
        options.InvalidModelStateResponseFactory = context =>
            new BadRequestObjectResult(new { mensagem = "Dados inválidos. Verifique os campos enviados." });
    });

// Libera o frontend React (Vite) para chamar a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string arquivoXml = Path.Combine(AppContext.BaseDirectory, "SistemaHotelaria.xml");
    if (File.Exists(arquivoXml))
        options.IncludeXmlComments(arquivoXml);
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
