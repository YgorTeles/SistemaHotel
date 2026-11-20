namespace SistemaHotelaria.Dtos;

// Corpo (JSON) recebido pela API para criar uma reserva. As validações ficam no ReservaService.

public record CriarReservaDto(string? Cpf, int NumeroQuarto, DateTime DataEntrada, DateTime DataSaida);
