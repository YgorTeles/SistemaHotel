namespace SistemaHotelaria.Dtos;

public class AtualizarQuartoDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Capacidade { get; set; }
    public decimal ValorDiaria { get; set; }
}
