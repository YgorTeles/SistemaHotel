using SistemaHotelaria.Controllers;
using SistemaHotelaria.Repositories;
using SistemaHotelaria.Services;

HospedeRepository hospedeRepository = new HospedeRepository();
HospedeService hospedeService = new HospedeService(hospedeRepository);
HospedeController hospedeController = new HospedeController(hospedeService);



bool rodando = true;

while (rodando)
{
    Console.WriteLine();
    Console.WriteLine("===== HÓSPEDES =====");
    Console.WriteLine("1 - Cadastrar");
    Console.WriteLine("2 - Listar");
    Console.WriteLine("3 - Buscar por CPF");
    Console.WriteLine("4 - Atualizar");
    Console.WriteLine("5 - Remover");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");

    string opcao = Console.ReadLine() ?? string.Empty;

    switch (opcao)
    {
        case "1":
            hospedeController.Cadastrar();
            break;
        case "2":
            hospedeController.Listar();
            break;
        case "3":
            hospedeController.BuscarPorCpf();
            break;
        case "4":
            hospedeController.Atualizar();
            break;
        case "5":
            hospedeController.Remover();
            break;
        case "0":
            rodando = false;
            break;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}