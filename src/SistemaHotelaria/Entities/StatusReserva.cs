namespace SistemaHotelaria.Entities;

public enum StatusReserva
{
    Confirmada,   // reserva criada, hóspede ainda não chegou
    EmAndamento,  // check-in feito, hóspede no quarto
    Finalizada,   // check-out feito
    Cancelada     // cancelada antes do check-in
}
