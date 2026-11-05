using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

public class HospedesController : Controller
{
    private HospedeService _service;

    public HospedesController(HospedeService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        List<Hospede> hospedes = _service.Listar();
        return View(hospedes);
    }
    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(string? nome, string? cpf, string? telefone, string? email)
    {
        try
        {
            _service.Cadastrar(nome ?? string.Empty, cpf ?? string.Empty, telefone ?? string.Empty, email ?? string.Empty);
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ViewBag.Erro = ex.Message;
            return View();
        }
    }
}