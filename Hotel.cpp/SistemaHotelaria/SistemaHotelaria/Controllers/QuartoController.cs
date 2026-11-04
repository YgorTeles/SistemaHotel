using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

public class QuartoController
{
    private QuartoService _service;

    public QuartoController(QuartoService service)
    {
        _service = service;
    }

    public void Cadastrar()
    {
        Console.Write("Número do quarto: ");
        string textoNumero = Console.ReadLine() ?? string.Empty;

        Console.Write("Tipo (Solteiro, Casal, Suíte...): ");
        string tipo = Console.ReadLine() ?? string.Empty;

        Console.Write("Capacidade: ");
        string textoCapacidade = Console.ReadLine() ?? string.Empty;

        Console.Write("Valor da diária: ");
        string textoValor = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(textoNumero, out int numero))
        {
            Console.WriteLine("Erro: o número do quarto deve ser um número inteiro.");
            return;
        }

        if (!int.TryParse(textoCapacidade, out int capacidade))
        {
            Console.WriteLine("Erro: a capacidade deve ser um número inteiro.");
            return;
        }

        if (!decimal.TryParse(textoValor, out decimal valorDiaria))
        {
            Console.WriteLine("Erro: o valor da diária deve ser um número.");
            return;
        }

        try
        {
            _service.Cadastrar(numero, tipo, capacidade, valorDiaria);
            Console.WriteLine("Quarto cadastrado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    public void Listar()
    {
        List<Quarto> quartos = _service.Listar();

        if (quartos.Count == 0)
        {
            Console.WriteLine("Nenhum quarto cadastrado.");
            return;
        }

        foreach (Quarto quarto in quartos)
        {
            Console.WriteLine($"Quarto {quarto.Numero} | Tipo: {quarto.Tipo} | Capacidade: {quarto.Capacidade} | Diária: {quarto.ValorDiaria:C}");
        }
    }

    public void BuscarPorNumero()
    {
        Console.Write("Número do quarto: ");
        string textoNumero = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(textoNumero, out int numero))
        {
            Console.WriteLine("Erro: o número do quarto deve ser um número inteiro.");
            return;
        }

        try
        {
            Quarto quarto = _service.BuscarPorNumero(numero);
            Console.WriteLine($"Quarto {quarto.Numero} | Tipo: {quarto.Tipo} | Capacidade: {quarto.Capacidade} | Diária: {quarto.ValorDiaria:C}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    public void Atualizar()
    {
        Console.Write("Número do quarto: ");
        string textoNumero = Console.ReadLine() ?? string.Empty;

        Console.Write("Novo tipo: ");
        string tipo = Console.ReadLine() ?? string.Empty;

        Console.Write("Nova capacidade: ");
        string textoCapacidade = Console.ReadLine() ?? string.Empty;

        Console.Write("Novo valor da diária: ");
        string textoValor = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(textoNumero, out int numero))
        {
            Console.WriteLine("Erro: o número do quarto deve ser um número inteiro.");
            return;
        }

        if (!int.TryParse(textoCapacidade, out int capacidade))
        {
            Console.WriteLine("Erro: a capacidade deve ser um número inteiro.");
            return;
        }

        if (!decimal.TryParse(textoValor, out decimal valorDiaria))
        {
            Console.WriteLine("Erro: o valor da diária deve ser um número.");
            return;
        }

        try
        {
            _service.Atualizar(numero, tipo, capacidade, valorDiaria);
            Console.WriteLine("Quarto atualizado com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    public void Remover()
    {
        Console.Write("Número do quarto a remover: ");
        string textoNumero = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(textoNumero, out int numero))
        {
            Console.WriteLine("Erro: o número do quarto deve ser um número inteiro.");
            return;
        }

        try
        {
            _service.Remover(numero);
            Console.WriteLine("Quarto removido com sucesso!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
    public void ExibirMenu()
    {
        bool noMenu = true;

        while (noMenu)
        {
            Console.WriteLine();
            Console.WriteLine("===== QUARTOS =====");
            Console.WriteLine("1 - Cadastrar");
            Console.WriteLine("2 - Listar");
            Console.WriteLine("3 - Buscar por número");
            Console.WriteLine("4 - Atualizar");
            Console.WriteLine("5 - Remover");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opção: ");

            string opcao = Console.ReadLine() ?? string.Empty;

            switch (opcao)
            {
                case "1":
                    Cadastrar();
                    break;
                case "2":
                    Listar();
                    break;
                case "3":
                    BuscarPorNumero();
                    break;
                case "4":
                    Atualizar();
                    break;
                case "5":
                    Remover();
                    break;
                case "0":
                    noMenu = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
}
