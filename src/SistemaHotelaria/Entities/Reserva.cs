namespace SistemaHotelaria.Entities;

public class Reserva
{
    public Guid Id { get; private set; }
    public Guid HospedeId { get; private set; }
    public Guid QuartoId { get; private set; }
    public DateTime DataEntrada { get; private set; }
    public DateTime DataSaida { get; private set; }
    public decimal ValorTotal { get; private set; }
    public StatusReserva Status { get; private set; }

    // Nova reserva: o Id é gerado aqui, na criação do objeto
    public Reserva(Guid hospedeId, Guid quartoId, DateTime dataEntrada, DateTime dataSaida, decimal valorTotal)
    {
        Id = Guid.NewGuid();
        HospedeId = hospedeId;
        QuartoId = quartoId;
        DataEntrada = dataEntrada;
        DataSaida = dataSaida;
        ValorTotal = valorTotal;
        Status = StatusReserva.Confirmada;
    }

    // Reserva vinda do banco: recebe o Id que já existe
    public Reserva(Guid id, Guid hospedeId, Guid quartoId, DateTime dataEntrada, DateTime dataSaida, decimal valorTotal, StatusReserva status)
    {
        Id = id;
        HospedeId = hospedeId;
        QuartoId = quartoId;
        DataEntrada = dataEntrada;
        DataSaida = dataSaida;
        ValorTotal = valorTotal;
        Status = status;
    }

    public void FazerCheckIn()
    {
        Status = StatusReserva.EmAndamento;
    }

    public void FazerCheckOut()
    {
        Status = StatusReserva.Finalizada;
    }

    public void Cancelar()
    {
        Status = StatusReserva.Cancelada;
    }
}
