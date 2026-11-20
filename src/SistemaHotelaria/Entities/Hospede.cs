namespace SistemaHotelaria.Entities;

public class Hospede
{
    public Guid Id { get; private set; }
    public string Cpf { get; private set; }
    public string Nome { get; private set; }
    public string Telefone { get; private set; }
    public string Email { get; private set; }

    // Novo hóspede: o Id é gerado aqui, na criação do objeto
    public Hospede(string nome, string cpf, string telefone, string email)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Cpf = cpf;
        Telefone = telefone;
        Email = email;
    }

    // Hóspede vindo do banco: recebe o Id que já existe
    public Hospede(Guid id, string nome, string cpf, string telefone, string email)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Telefone = telefone;
        Email = email;
    }

    public void Atualizar(string nome, string telefone, string email)
    {
        Nome = nome;
        Telefone = telefone;
        Email = email;
    }
}
