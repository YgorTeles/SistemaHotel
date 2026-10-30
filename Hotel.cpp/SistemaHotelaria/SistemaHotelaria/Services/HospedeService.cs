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
}