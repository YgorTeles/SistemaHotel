namespace SistemaHotelaria.Entities;

public class Reserva
{
    public Guid Id { get; set; }
    public Guid HospedeId { get; set; }
    public Guid QuartoId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public decimal ValorTotal { get; set; }
}
