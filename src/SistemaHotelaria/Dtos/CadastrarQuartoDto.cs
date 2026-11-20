namespace SistemaHotelaria.Dtos;

public class CadastrarQuartoDto
{
    public int Numero { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public decimal ValorDiaria { get; set; }
}
