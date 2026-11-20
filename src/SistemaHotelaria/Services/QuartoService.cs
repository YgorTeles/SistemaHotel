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

    public Quarto Cadastrar(int numero, string tipo, int capacidade, decimal valorDiaria)
    {
        if (numero <= 0)
            throw new ArgumentException("Número do quarto deve ser maior que zero.");

        ValidarDados(tipo, capacidade, valorDiaria);

        if (_repository.BuscarPorNumero(numero) != null)
            throw new ArgumentException("Já existe um quarto com esse número.");

        Quarto quarto = new Quarto(numero, tipo, capacidade, valorDiaria);

        _repository.Adicionar(quarto);

        return quarto;
    }

    public List<Quarto> Listar()
    {
        return _repository.ListarTodos();
    }

    public List<Quarto> ListarDisponiveis(DateTime entrada, DateTime saida)
    {
        entrada = entrada.Date;
        saida = saida.Date;

        if (entrada < DateTime.Today)
            throw new ArgumentException("A data de entrada não pode ser no passado.");

        if (saida <= entrada)
            throw new ArgumentException("A data de saída deve ser depois da data de entrada.");

        return _repository.ListarDisponiveis(entrada, saida);
    }

    public Quarto BuscarPorNumero(int numero)
    {
        Quarto? quarto = _repository.BuscarPorNumero(numero);

        if (quarto == null)
            throw new KeyNotFoundException("Nenhum quarto encontrado com esse número.");

        return quarto;
    }

    public Quarto BuscarPorId(Guid id)
    {
        Quarto? quarto = _repository.BuscarPorId(id);

        if (quarto == null)
            throw new KeyNotFoundException("Quarto não encontrado.");

        return quarto;
    }

    public void Ocupar(Quarto quarto)
    {
        if (quarto.Ocupado)
            throw new ArgumentException("O quarto já está ocupado.");

        quarto.Ocupar();
        _repository.Atualizar(quarto);
    }

    public void Liberar(Quarto quarto)
    {
        quarto.Liberar();
        _repository.Atualizar(quarto);
    }

    public Quarto Atualizar(int numero, string tipo, int capacidade, decimal valorDiaria)
    {
        ValidarDados(tipo, capacidade, valorDiaria);

        Quarto quarto = BuscarPorNumero(numero);

        quarto.Atualizar(tipo, capacidade, valorDiaria);

        _repository.Atualizar(quarto);

        return quarto;
    }

    public void Remover(int numero)
    {
        Quarto quarto = BuscarPorNumero(numero);

        if (quarto.Ocupado)
            throw new ArgumentException("Não é possível remover um quarto ocupado.");

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
