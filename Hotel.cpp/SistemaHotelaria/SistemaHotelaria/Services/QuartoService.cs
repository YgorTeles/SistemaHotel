using SistemaHotelaria.Entities;
using SistemaHotelaria.Repositories;

namespace SistemaHotelaria.Services;

public class QuartoService
{
    private QuartoRepository _repository;

    public QuartoService(QuartoRepository repository)
    {
        _repository = repository;
    }

    public void Cadastrar(int numero, string tipo, int capacidade, decimal valorDiaria)
    {
        if (numero <= 0)
            throw new ArgumentException("Número do quarto deve ser maior que zero.");

        ValidarDados(tipo, capacidade, valorDiaria);

        if (_repository.BuscarPorNumero(numero) != null)
            throw new ArgumentException("Já existe um quarto com esse número.");

        Quarto quarto = new Quarto();
        quarto.Numero = numero;
        quarto.Tipo = tipo;
        quarto.Capacidade = capacidade;
        quarto.ValorDiaria = valorDiaria;

        _repository.Adicionar(quarto);
    }

    public List<Quarto> Listar()
    {
        return _repository.ListarTodos();
    }

    public Quarto BuscarPorNumero(int numero)
    {
        Quarto? quarto = _repository.BuscarPorNumero(numero);

        if (quarto == null)
            throw new ArgumentException("Nenhum quarto encontrado com esse número.");

        return quarto;
    }

    public void Atualizar(int numero, string tipo, int capacidade, decimal valorDiaria)
    {
        ValidarDados(tipo, capacidade, valorDiaria);

        Quarto quarto = BuscarPorNumero(numero);

        quarto.Tipo = tipo;
        quarto.Capacidade = capacidade;
        quarto.ValorDiaria = valorDiaria;

        _repository.Atualizar(quarto);
    }

    public void Remover(int numero)
    {
        Quarto quarto = BuscarPorNumero(numero);
        _repository.Remover(quarto.Id);
    }

    private void ValidarDados(string tipo, int capacidade, decimal valorDiaria)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo é obrigatório.");

        if (capacidade <= 0)
            throw new ArgumentException("Capacidade deve ser maior que zero.");

        if (valorDiaria <= 0)
            throw new ArgumentException("Valor da diária deve ser maior que zero.");
    }
}