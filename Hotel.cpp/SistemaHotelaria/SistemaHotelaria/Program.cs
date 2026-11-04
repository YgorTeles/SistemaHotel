using SistemaHotelaria.Controllers;
using SistemaHotelaria.Repositories;
using SistemaHotelaria.Services;


string? connectionString = Environment.GetEnvironmentVariable("HOTELARIA_DB");

if (connectionString == null)
{
    Console.WriteLine("Variável de ambiente HOTELARIA_DB não encontrada.");
    return;
}

HospedeRepository hospedeRepository = new HospedeRepository(connectionString);
HospedeService hospedeService = new HospedeService(hospedeRepository);
HospedeController hospedeController = new HospedeController(hospedeService);
QuartoRepository quartoRepository = new QuartoRepository(connectionString);
QuartoService quartoService = new QuartoService(quartoRepository);
QuartoController quartoController = new QuartoController(quartoService);

bool rodando = true;

while (rodando)
{
    Console.WriteLine();
    Console.WriteLine("===== HOTEL =====");
    Console.WriteLine("1 - Hóspedes");
    Console.WriteLine("2 - Quartos");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");

    string opcao = Console.ReadLine() ?? string.Empty;

    switch (opcao)
    {
        case "1":
            hospedeController.ExibirMenu();
            break;
        case "2":
            quartoController.ExibirMenu();
            break;
        case "0":
            rodando = false;
            break;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}