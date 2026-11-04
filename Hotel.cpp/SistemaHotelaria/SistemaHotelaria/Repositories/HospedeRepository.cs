using SistemaHotelaria.Entities;
using Npgsql;
namespace SistemaHotelaria.Repositories;

public class HospedeRepository
{

    private List<Hospede> _hospedes = new List<Hospede>();   // temporário
    private string _connectionString;

    public HospedeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Adicionar(Hospede hospede)
    {
        hospede.Id = Guid.NewGuid();

        NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "INSERT INTO hospedes (id, nome, cpf, telefone, email) " +
                     "VALUES (@id, @nome, @cpf, @telefone, @email)";

        NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", hospede.Id);
        comando.Parameters.AddWithValue("@nome", hospede.Nome);
        comando.Parameters.AddWithValue("@cpf", hospede.Cpf);
        comando.Parameters.AddWithValue("@telefone", hospede.Telefone);
        comando.Parameters.AddWithValue("@email", hospede.Email);

        comando.ExecuteNonQuery();

        conexao.Close();

        _hospedes.Add(hospede);   // temporário
    }
    public List<Hospede> ListarTodos()
    {
        return _hospedes;
    }

    public Hospede? BuscarPorCpf(string cpf)
    {
        foreach (Hospede hospede in _hospedes)
        {
            if (hospede.Cpf == cpf)
                return hospede;
        }

        return null;
    }

    public void Atualizar(Hospede hospedeAtualizado)
    {
        for (int i = 0; i < _hospedes.Count; i++)
        {
            if (_hospedes[i].Id == hospedeAtualizado.Id)
            {
                _hospedes[i] = hospedeAtualizado;
                return;
            }
        }
    }
    public void Remover(Guid id)
    {
        for (int i = 0; i < _hospedes.Count; i++)
        {
            if (_hospedes[i].Id == id)
            {
                _hospedes.RemoveAt(i);
                return;
            }
        }
    }
}