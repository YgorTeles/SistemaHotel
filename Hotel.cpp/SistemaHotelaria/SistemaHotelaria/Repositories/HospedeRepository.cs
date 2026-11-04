using SistemaHotelaria.Entities;
using Npgsql;
namespace SistemaHotelaria.Repositories;

public class HospedeRepository
{

    private List<Hospede> _hospedes = new List<Hospede>();  
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
        List<Hospede> hospedes = new List<Hospede>();

        NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, nome, cpf, telefone, email FROM hospedes";
        NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);

        NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            Hospede hospede = new Hospede();
            hospede.Id = leitor.GetGuid(0);
            hospede.Nome = leitor.GetString(1);
            hospede.Cpf = leitor.GetString(2);
            hospede.Telefone = leitor.IsDBNull(3) ? string.Empty : leitor.GetString(3);
            hospede.Email = leitor.IsDBNull(4) ? string.Empty : leitor.GetString(4);

            hospedes.Add(hospede);
        }

        leitor.Close();
        conexao.Close();

        return hospedes;
    }
    
    public Hospede? BuscarPorCpf(string cpf)
    {
        NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, nome, cpf, telefone, email FROM hospedes WHERE cpf = @cpf";
        NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@cpf", cpf);

        NpgsqlDataReader leitor = comando.ExecuteReader();

        Hospede? hospede = null;

        if (leitor.Read())
        {
            hospede = new Hospede();
            hospede.Id = leitor.GetGuid(0);
            hospede.Nome = leitor.GetString(1);
            hospede.Cpf = leitor.GetString(2);
            hospede.Telefone = leitor.IsDBNull(3) ? string.Empty : leitor.GetString(3);
            hospede.Email = leitor.IsDBNull(4) ? string.Empty : leitor.GetString(4);
        }

        leitor.Close();
        conexao.Close();

        return hospede;
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