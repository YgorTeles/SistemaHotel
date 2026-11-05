using SistemaHotelaria.Entities;
using Npgsql;

namespace SistemaHotelaria.Repositories;

public class ReservaRepository
{
    private string _connectionString;

    public ReservaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Adicionar(Reserva reserva)
    {
        reserva.Id = Guid.NewGuid();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "INSERT INTO reservas (id, hospede_id, quarto_id, data_entrada, data_saida, valor_total) " +
                     "VALUES (@id, @hospede_id, @quarto_id, @data_entrada, @data_saida, @valor_total)";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", reserva.Id);
        comando.Parameters.AddWithValue("@hospede_id", reserva.HospedeId);
        comando.Parameters.AddWithValue("@quarto_id", reserva.QuartoId);
        comando.Parameters.AddWithValue("@data_entrada", reserva.DataEntrada);
        comando.Parameters.AddWithValue("@data_saida", reserva.DataSaida);
        comando.Parameters.AddWithValue("@valor_total", reserva.ValorTotal);

        comando.ExecuteNonQuery();
    }

    public List<Reserva> ListarTodos()
    {
        List<Reserva> reservas = new List<Reserva>();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, hospede_id, quarto_id, data_entrada, data_saida, valor_total " +
                     "FROM reservas ORDER BY data_entrada";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            Reserva reserva = new Reserva();
            reserva.Id = leitor.GetGuid(0);
            reserva.HospedeId = leitor.GetGuid(1);
            reserva.QuartoId = leitor.GetGuid(2);
            reserva.DataEntrada = leitor.GetDateTime(3);
            reserva.DataSaida = leitor.GetDateTime(4);
            reserva.ValorTotal = leitor.GetDecimal(5);

            reservas.Add(reserva);
        }

        return reservas;
    }

    public bool ExisteConflito(Guid quartoId, DateTime entrada, DateTime saida)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT COUNT(*) FROM reservas " +
                     "WHERE quarto_id = @quarto_id " +
                     "AND @entrada < data_saida " +
                     "AND @saida > data_entrada";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@quarto_id", quartoId);
        comando.Parameters.AddWithValue("@entrada", entrada);
        comando.Parameters.AddWithValue("@saida", saida);

        long quantidade = (long)comando.ExecuteScalar()!;

        return quantidade > 0;
    }

    public void Remover(Guid id)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "DELETE FROM reservas WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", id);

        comando.ExecuteNonQuery();
    }
}