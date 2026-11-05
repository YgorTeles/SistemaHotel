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
}