using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

public class HospedeController
{
    private HospedeService _service;

    public HospedeController(HospedeService service)
    {
        _service = service;
    }

    public void Cadastrar()
    {
        Console.Write("Nome: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("CPF: ");
        string cpf = Console.ReadLine() ?? string.Empty;

        Console.Write("Telefone: ");
        string telefone = Console.ReadLine() ?? string.Empty;

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? string.Empty;

        try
        {
            _service.Cadastrar(nome, cpf, telefone, email);
            Console.WriteLine("Hóspede cadastrado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}