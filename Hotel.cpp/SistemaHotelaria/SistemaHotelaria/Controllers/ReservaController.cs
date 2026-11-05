using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Models;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

public class ReservasController : Controller
{
    private ReservaService _service;

    public ReservasController(ReservaService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        List<ReservaDetalhe> reservas = _service.Listar();
        return View(reservas);
    }

    [HttpGet]
    public IActionResult Criar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Criar(string? cpf, int numeroQuarto, DateTime dataEntrada, DateTime dataSaida)
    {
        try
        {
            _service.Criar(cpf ?? string.Empty, numeroQuarto, dataEntrada, dataSaida);
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ViewBag.Erro = ex.Message;
            return View();
        }
    }

    [HttpPost]
    public IActionResult Cancelar(Guid id)
    {
        _service.Cancelar(id);
        return RedirectToAction("Index");
    }
}