
namespace SistemaHotelaria.Controllers;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

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
    public void Listar()
    {
        List<Hospede> hospedes = _service.Listar();

        if (hospedes.Count == 0)
        {
            Console.WriteLine("Nenhum hóspede cadastrado.");
            return;
        }

        foreach (Hospede hospede in hospedes)
        {
            Console.WriteLine($"Nome: {hospede.Nome} | CPF: {hospede.Cpf} | Telefone: {hospede.Telefone} | Email: {hospede.Email}");
        }
    }
    public void BuscarPorCpf()
    {
        Console.Write("CPF: ");
        string cpf = Console.ReadLine() ?? string.Empty;

        try
        {
            Hospede hospede = _service.BuscarPorCpf(cpf);
            Console.WriteLine($"Nome: {hospede.Nome} | CPF: {hospede.Cpf} | Telefone: {hospede.Telefone} | Email: {hospede.Email}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
    public void Atualizar()
    {
        Console.Write("CPF do hóspede: ");
        string cpf = Console.ReadLine() ?? string.Empty;

        Console.Write("Novo nome: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Novo telefone: ");
        string telefone = Console.ReadLine() ?? string.Empty;

        Console.Write("Novo email: ");
        string email = Console.ReadLine() ?? string.Empty;

        try
        {
            _service.Atualizar(cpf, nome, telefone, email);
            Console.WriteLine("Hóspede atualizado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
    public void Remover()
    {
        Console.Write("CPF do hóspede a remover: ");
        string cpf = Console.ReadLine() ?? string.Empty;

        try
        {
            _service.Remover(cpf);
            Console.WriteLine("Hóspede removido com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}