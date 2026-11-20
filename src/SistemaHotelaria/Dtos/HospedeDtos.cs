namespace SistemaHotelaria.Dtos;

// Corpos (JSON) recebidos pela API de hóspedes. As validações ficam no HospedeService.

public record CadastrarHospedeDto(string? Nome, string? Cpf, string? Telefone, string? Email);

public record AtualizarHospedeDto(string? Nome, string? Telefone, string? Email);
