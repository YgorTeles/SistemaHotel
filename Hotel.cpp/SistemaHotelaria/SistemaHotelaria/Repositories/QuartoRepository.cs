using SistemaHotelaria.Entities;
using Npgsql;

namespace SistemaHotelaria.Repositories;

public class QuartoRepository
{
    private string _connectionString;

    public QuartoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Adicionar(Quarto quarto)
    {
        quarto.Id = Guid.NewGuid();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "INSERT INTO quartos (id, numero, tipo, capacidade, valor_diaria) " +
                     "VALUES (@id, @numero, @tipo, @capacidade, @valor_diaria)";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", quarto.Id);
        comando.Parameters.AddWithValue("@numero", quarto.Numero);
        comando.Parameters.AddWithValue("@tipo", quarto.Tipo);
        comando.Parameters.AddWithValue("@capacidade", quarto.Capacidade);
        comando.Parameters.AddWithValue("@valor_diaria", quarto.ValorDiaria);

        comando.ExecuteNonQuery();
    }

    public List<Quarto> ListarTodos()
    {
        List<Quarto> quartos = new List<Quarto>();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, numero, tipo, capacidade, valor_diaria FROM quartos ORDER BY numero";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            Quarto quarto = new Quarto();
            quarto.Id = leitor.GetGuid(0);
            quarto.Numero = leitor.GetInt32(1);
            quarto.Tipo = leitor.GetString(2);
            quarto.Capacidade = leitor.GetInt32(3);
            quarto.ValorDiaria = leitor.GetDecimal(4);

            quartos.Add(quarto);
        }

        return quartos;
    }

    public Quarto? BuscarPorNumero(int numero)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, numero, tipo, capacidade, valor_diaria FROM quartos WHERE numero = @numero";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@numero", numero);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        Quarto? quarto = null;

        if (leitor.Read())
        {
            quarto = new Quarto();
            quarto.Id = leitor.GetGuid(0);
            quarto.Numero = leitor.GetInt32(1);
            quarto.Tipo = leitor.GetString(2);
            quarto.Capacidade = leitor.GetInt32(3);
            quarto.ValorDiaria = leitor.GetDecimal(4);
        }

        return quarto;
    }

    public void Atualizar(Quarto quartoAtualizado)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "UPDATE quartos SET tipo = @tipo, capacidade = @capacidade, valor_diaria = @valor_diaria " +
                     "WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@tipo", quartoAtualizado.Tipo);
        comando.Parameters.AddWithValue("@capacidade", quartoAtualizado.Capacidade);
        comando.Parameters.AddWithValue("@valor_diaria", quartoAtualizado.ValorDiaria);
        comando.Parameters.AddWithValue("@id", quartoAtualizado.Id);

        comando.ExecuteNonQuery();
    }

    public void Remover(Guid id)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "DELETE FROM quartos WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", id);

        comando.ExecuteNonQuery();
    }
}