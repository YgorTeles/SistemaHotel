namespace SistemaHotelaria.Models;

public class ReservaDetalhe
{
    public Guid Id { get; set; }
    public string NomeHospede { get; set; } = string.Empty;
    public string CpfHospede { get; set; } = string.Empty;
    public int NumeroQuarto { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public decimal ValorTotal { get; set; }
}