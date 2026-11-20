using SistemaHotelaria.Entities;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Repositories;

namespace SistemaHotelaria.Services;

public class ReservaService
{
    private ReservaRepository _repository;
    private HospedeService _hospedeService;
    private QuartoService _quartoService;

    public ReservaService(ReservaRepository repository, HospedeService hospedeService, QuartoService quartoService)
    {
        _repository = repository;
        _hospedeService = hospedeService;
        _quartoService = quartoService;
    }

    public Reserva Criar(string cpf, int numeroQuarto, DateTime dataEntrada, DateTime dataSaida)
    {
        dataEntrada = dataEntrada.Date;
        dataSaida = dataSaida.Date;

        if (dataEntrada < DateTime.Today)
            throw new ArgumentException("A data de entrada não pode ser no passado.");

        if (dataSaida <= dataEntrada)
            throw new ArgumentException("A data de saída deve ser depois da data de entrada.");

        Hospede hospede = _hospedeService.BuscarPorCpf(cpf);
        Quarto quarto = _quartoService.BuscarPorNumero(numeroQuarto);

        if (_repository.ExisteConflito(quarto.Id, dataEntrada, dataSaida))
            throw new ArgumentException("O quarto já está reservado nesse período.");

        TimeSpan periodo = dataSaida - dataEntrada;
        int noites = periodo.Days;

        Reserva reserva = new Reserva(hospede.Id, quarto.Id, dataEntrada, dataSaida, quarto.ValorDiaria * noites);

        _repository.Adicionar(reserva);

        return reserva;
    }

    public List<ReservaDetalheDto> Listar()
    {
        return _repository.ListarComDetalhes();
    }

    public Reserva BuscarPorId(Guid id)
    {
        Reserva? reserva = _repository.BuscarPorId(id);

        if (reserva == null)
            throw new KeyNotFoundException("Reserva não encontrada.");

        return reserva;
    }

    public void FazerCheckIn(Guid id)
    {
        Reserva reserva = BuscarPorId(id);

        if (reserva.Status != StatusReserva.Confirmada)
            throw new ArgumentException("Só é possível fazer check-in de uma reserva confirmada.");

        if (DateTime.Today < reserva.DataEntrada)
            throw new ArgumentException("O check-in só pode ser feito a partir da data de entrada.");

        if (DateTime.Today >= reserva.DataSaida)
            throw new ArgumentException("O período desta reserva já terminou.");

        Quarto quarto = _quartoService.BuscarPorId(reserva.QuartoId);

        // Valida e marca o quarto como ocupado (erro se ainda houver alguém nele)
        _quartoService.Ocupar(quarto);

        reserva.FazerCheckIn();
        _repository.AtualizarStatus(reserva);
    }

    public void FazerCheckOut(Guid id)
    {
        Reserva reserva = BuscarPorId(id);

        if (reserva.Status != StatusReserva.EmAndamento)
            throw new ArgumentException("Só é possível fazer check-out de uma reserva em andamento.");

        Quarto quarto = _quartoService.BuscarPorId(reserva.QuartoId);
        _quartoService.Liberar(quarto);

        reserva.FazerCheckOut();
        _repository.AtualizarStatus(reserva);
    }

    public void Cancelar(Guid id)
    {
        Reserva reserva = BuscarPorId(id);

        if (reserva.Status != StatusReserva.Confirmada)
            throw new ArgumentException("Só é possível cancelar uma reserva antes do check-in.");

        reserva.Cancelar();
        _repository.AtualizarStatus(reserva);
    }
}