using SistemaHotelaria.Entities;
using SistemaHotelaria.Models;
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

    public void Criar(string cpf, int numeroQuarto, DateTime dataEntrada, DateTime dataSaida)
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

        Reserva reserva = new Reserva();
        reserva.HospedeId = hospede.Id;
        reserva.QuartoId = quarto.Id;
        reserva.DataEntrada = dataEntrada;
        reserva.DataSaida = dataSaida;
        reserva.ValorTotal = quarto.ValorDiaria * noites;

        _repository.Adicionar(reserva);
    }

    public List<ReservaDetalhe> Listar()
    {
        return _repository.ListarComDetalhes();
    }

    public void Cancelar(Guid id)
    {
        _repository.Remover(id);
    }
}