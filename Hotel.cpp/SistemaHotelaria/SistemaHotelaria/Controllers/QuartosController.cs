using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

public class QuartosController : Controller
{
    private QuartoService _service;

    public QuartosController(QuartoService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        List<Quarto> quartos = _service.Listar();
        return View(quartos);
    }

    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Cadastrar(int numero, string? tipo, int capacidade, decimal valorDiaria)
    {
        try
        {
            _service.Cadastrar(numero, tipo ?? string.Empty, capacidade, valorDiaria);
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ViewBag.Erro = ex.Message;
            return View();
        }
    }

    [HttpGet]
    public IActionResult Editar(int numero)
    {
        try
        {
            Quarto quarto = _service.BuscarPorNumero(numero);
            return View(quarto);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public IActionResult Editar(int numero, string? tipo, int capacidade, decimal valorDiaria)
    {
        try
        {
            _service.Atualizar(numero, tipo ?? string.Empty, capacidade, valorDiaria);
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ViewBag.Erro = ex.Message;

            Quarto quarto = new Quarto();
            quarto.Numero = numero;
            quarto.Tipo = tipo ?? string.Empty;
            quarto.Capacidade = capacidade;
            quarto.ValorDiaria = valorDiaria;

            return View(quarto);
        }
    }

    [HttpPost]
    public IActionResult Remover(int numero)
    {
        try
        {
            _service.Remover(numero);
        }
        catch (ArgumentException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}