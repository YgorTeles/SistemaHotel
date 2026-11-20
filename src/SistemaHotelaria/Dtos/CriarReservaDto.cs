namespace SistemaHotelaria.Dtos;

public class CriarReservaDto
{
    public string Cpf { get; set; } = string.Empty;
    public int NumeroQuarto { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
}
