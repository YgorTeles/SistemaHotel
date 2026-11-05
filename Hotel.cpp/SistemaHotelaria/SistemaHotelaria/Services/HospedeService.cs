using SistemaHotelaria.Entities;
using SistemaHotelaria.Repositories;

namespace SistemaHotelaria.Services;

public class HospedeService
{
    private HospedeRepository _repository;

    public HospedeService(HospedeRepository repository)
    {
        _repository = repository;
    }

    public void Cadastrar(string nome, string cpf, string telefone, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF é obrigatório.");

        cpf = LimparCpf(cpf);

        if (cpf.Length != 11)
            throw new ArgumentException("CPF deve ter 11 dígitos.");

        if (_repository.BuscarPorCpf(cpf) != null)
            throw new ArgumentException("Já existe um hóspede com esse CPF.");

        Hospede hospede = new Hospede();
        hospede.Nome = nome;
        hospede.Cpf = cpf;
        hospede.Telefone = telefone;
        hospede.Email = email;

        _repository.Adicionar(hospede);
    }
    public List<Hospede> Listar()
    {
        return _repository.ListarTodos();
    }

    public Hospede BuscarPorCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF é obrigatório.");

        cpf = LimparCpf(cpf);

        if (cpf.Length != 11)
            throw new ArgumentException("CPF deve ter 11 dígitos.");

        Hospede? hospede = _repository.BuscarPorCpf(cpf);

        if (hospede == null)
            throw new ArgumentException("Nenhum hóspede encontrado com esse CPF.");

        return hospede;
    }

    public void Atualizar(string cpf, string nome, string telefone, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        Hospede hospede = BuscarPorCpf(cpf);

        hospede.Nome = nome;
        hospede.Telefone = telefone;
        hospede.Email = email;

        _repository.Atualizar(hospede);
    }
    public void Remover(string cpf)
    {
        Hospede hospede = BuscarPorCpf(cpf);
        _repository.Remover(hospede.Id);
    }
    private string LimparCpf(string cpf)
    {
        string apenasNumeros = string.Empty;

        foreach (char caractere in cpf)
        {
            if (char.IsDigit(caractere))
                apenasNumeros += caractere;
        }

        return apenasNumeros;
    }
}