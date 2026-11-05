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

    [HttpGet]
    public IActionResult Editar(string? cpf)
    {
        try
        {
            Hospede hospede = _service.BuscarPorCpf(cpf ?? string.Empty);
            return View(hospede);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public IActionResult Editar(string? cpf, string? nome, string? telefone, string? email)
    {
        try
        {
            _service.Atualizar(cpf ?? string.Empty, nome ?? string.Empty, telefone ?? string.Empty, email ?? string.Empty);
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ViewBag.Erro = ex.Message;

            Hospede hospede = new Hospede();
            hospede.Cpf = cpf ?? string.Empty;
            hospede.Nome = nome ?? string.Empty;
            hospede.Telefone = telefone ?? string.Empty;
            hospede.Email = email ?? string.Empty;

            return View(hospede);
        }
    }
}