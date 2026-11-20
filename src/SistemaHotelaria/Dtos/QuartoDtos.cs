namespace SistemaHotelaria.Dtos;

// Corpos (JSON) recebidos pela API de quartos. As validações ficam no QuartoService.

public record CadastrarQuartoDto(int Numero, string? Tipo, int Capacidade, decimal ValorDiaria);

public record AtualizarQuartoDto(string? Tipo, int Capacidade, decimal ValorDiaria);
