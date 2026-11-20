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
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "INSERT INTO quartos (id, numero, tipo, capacidade, valor_diaria, ocupado) " +
                     "VALUES (@id, @numero, @tipo, @capacidade, @valor_diaria, @ocupado)";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", quarto.Id);
        comando.Parameters.AddWithValue("@numero", quarto.Numero);
        comando.Parameters.AddWithValue("@tipo", quarto.Tipo);
        comando.Parameters.AddWithValue("@capacidade", quarto.Capacidade);
        comando.Parameters.AddWithValue("@valor_diaria", quarto.ValorDiaria);
        comando.Parameters.AddWithValue("@ocupado", quarto.Ocupado);

        comando.ExecuteNonQuery();
    }

    public List<Quarto> ListarTodos()
    {
        List<Quarto> quartos = new List<Quarto>();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, numero, tipo, capacidade, valor_diaria, ocupado FROM quartos ORDER BY numero";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            quartos.Add(Mapear(leitor));
        }

        return quartos;
    }

    // Quartos sem reserva (não cancelada) que se sobreponha ao período informado
    public List<Quarto> ListarDisponiveis(DateTime entrada, DateTime saida)
    {
        List<Quarto> quartos = new List<Quarto>();

        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT q.id, q.numero, q.tipo, q.capacidade, q.valor_diaria, q.ocupado " +
                     "FROM quartos q " +
                     "WHERE NOT EXISTS (" +
                     "    SELECT 1 FROM reservas r " +
                     "    WHERE r.quarto_id = q.id " +
                     "    AND r.status <> 'Cancelada' " +
                     "    AND @entrada < r.data_saida " +
                     "    AND @saida > r.data_entrada" +
                     ") " +
                     "ORDER BY q.numero";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@entrada", entrada);
        comando.Parameters.AddWithValue("@saida", saida);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        while (leitor.Read())
        {
            quartos.Add(Mapear(leitor));
        }

        return quartos;
    }

    public Quarto? BuscarPorNumero(int numero)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, numero, tipo, capacidade, valor_diaria, ocupado FROM quartos WHERE numero = @numero";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@numero", numero);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        if (leitor.Read())
            return Mapear(leitor);

        return null;
    }

    public Quarto? BuscarPorId(Guid id)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "SELECT id, numero, tipo, capacidade, valor_diaria, ocupado FROM quartos WHERE id = @id";
        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@id", id);
        using NpgsqlDataReader leitor = comando.ExecuteReader();

        if (leitor.Read())
            return Mapear(leitor);

        return null;
    }

    public void Atualizar(Quarto quartoAtualizado)
    {
        using NpgsqlConnection conexao = new NpgsqlConnection(_connectionString);
        conexao.Open();

        string sql = "UPDATE quartos SET tipo = @tipo, capacidade = @capacidade, valor_diaria = @valor_diaria, ocupado = @ocupado " +
                     "WHERE id = @id";

        using NpgsqlCommand comando = new NpgsqlCommand(sql, conexao);
        comando.Parameters.AddWithValue("@tipo", quartoAtualizado.Tipo);
        comando.Parameters.AddWithValue("@capacidade", quartoAtualizado.Capacidade);
        comando.Parameters.AddWithValue("@valor_diaria", quartoAtualizado.ValorDiaria);
        comando.Parameters.AddWithValue("@ocupado", quartoAtualizado.Ocupado);
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

    // Monta o Quarto a partir da linha lida (colunas: id, numero, tipo, capacidade, valor_diaria, ocupado)
    private Quarto Mapear(NpgsqlDataReader leitor)
    {
        return new Quarto(
            leitor.GetGuid(0),
            leitor.GetInt32(1),
            leitor.GetString(2),
            leitor.GetInt32(3),
            leitor.GetDecimal(4),
            leitor.GetBoolean(5));
    }
}
