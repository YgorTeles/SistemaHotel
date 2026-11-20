using SistemaHotelaria.Entities;
using Npgsql;
using SistemaHotelaria.Dtos;
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
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "INSERT INTO reservas (id, hospede_id, quarto_id, data_entrada, data_saida, valor_total, status) " +
                     "VALUES (@id, @hospede_id, @quarto_id, @data_entrada, @data_saida, @valor_total, @status)";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", reserva.Id);
        comando.Parameters.AddWithValue("@hospede_id", reserva.HospedeId);
        comando.Parameters.AddWithValue("@quarto_id", reserva.QuartoId);
        comando.Parameters.AddWithValue("@data_entrada", reserva.DataEntrada);
        comando.Parameters.AddWithValue("@data_saida", reserva.DataSaida);
        comando.Parameters.AddWithValue("@valor_total", reserva.ValorTotal);
        comando.Parameters.AddWithValue("@status", reserva.Status.ToString());

        comando.ExecuteNonQuery();
    }

    public List<ReservaDetalheDto> ListarComDetalhes()
    {
        List<ReservaDetalheDto> reservas = new List<ReservaDetalheDto>();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT r.id, h.nome, h.cpf, q.numero, r.data_entrada, r.data_saida, r.valor_total, r.status " +
                     "FROM reservas r " +
                     "JOIN hospedes h ON h.id = r.hospede_id " +
                     "JOIN quartos q ON q.id = r.quarto_id " +
                     "ORDER BY r.data_entrada";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            ReservaDetalheDto reserva = new ReservaDetalheDto();
            reserva.Id = leitor.GetGuid(0);
            reserva.NomeHospede = leitor.GetString(1);
            reserva.CpfHospede = leitor.GetString(2);
            reserva.NumeroQuarto = leitor.GetInt32(3);
            reserva.DataEntrada = leitor.GetDateTime(4);
            reserva.DataSaida = leitor.GetDateTime(5);
            reserva.ValorTotal = leitor.GetDecimal(6);
            reserva.Status = leitor.GetString(7);

            reservas.Add(reserva);
        }

        return reservas;
    }

    public Reserva? BuscarPorId(Guid id)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, hospede_id, quarto_id, data_entrada, data_saida, valor_total, status " +
                     "FROM reservas WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", id);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        if (!leitor.Read())
            return null;

        return new Reserva(
            leitor.GetGuid(0),
            leitor.GetGuid(1),
            leitor.GetGuid(2),
            leitor.GetDateTime(3),
            leitor.GetDateTime(4),
            leitor.GetDecimal(5),
            Enum.Parse<StatusReserva>(leitor.GetString(6)));
    }

    public void AtualizarStatus(Reserva reserva)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "UPDATE reservas SET status = @status WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@status", reserva.Status.ToString());
        comando.Parameters.AddWithValue("@id", reserva.Id);

        comando.ExecuteNonQuery();
    }

    public bool ExisteConflito(Guid quartoId, DateTime entrada, DateTime saida)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        // Reservas canceladas não bloqueiam o quarto
        string sql = "SELECT COUNT(*) FROM reservas " +
                     "WHERE quarto_id = @quarto_id " +
                     "AND status <> 'Cancelada' " +
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
