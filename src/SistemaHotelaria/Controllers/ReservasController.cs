using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

// Erros vindos do service:
//   KeyNotFoundException -> 404 (reserva, hóspede ou quarto não encontrado)
//   ArgumentException    -> 400 (dado inválido / regra de negócio)
[ApiController]
[Route("api/reservas")]
[Produces("application/json")]
public class ReservasController : ControllerBase
{
    private ReservaService _service;

    public ReservasController(ReservaService service)
    {
        _service = service;
    }

    /// <summary>Lista todas as reservas (todos os status), ordenadas pela data de entrada.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReservaDetalheDto>), StatusCodes.Status200OK)]
    public ActionResult<List<ReservaDetalheDto>> Listar()
    {
        return Ok(_service.Listar());
    }

    /// <summary>Cria uma reserva com status Confirmada. Retorna o id da reserva criada.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Criar(CriarReservaDto request)
    {
        try
        {
            Reserva reserva = _service.Criar(
                request.Cpf ?? string.Empty,
                request.NumeroQuarto,
                request.DataEntrada,
                request.DataSaida);

            return Created($"/api/reservas/{reserva.Id}", new { id = reserva.Id });
        }
        catch (KeyNotFoundException erro)
        {
            return NotFound(new { mensagem = erro.Message });
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Faz o check-in: reserva vira EmAndamento e o quarto fica ocupado.</summary>
    [HttpPost("{id:guid}/checkin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FazerCheckIn(Guid id)
    {
        try
        {
            _service.FazerCheckIn(id);
            return NoContent();
        }
        catch (KeyNotFoundException erro)
        {
            return NotFound(new { mensagem = erro.Message });
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Faz o check-out: reserva vira Finalizada e o quarto é liberado.</summary>
    [HttpPost("{id:guid}/checkout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FazerCheckOut(Guid id)
    {
        try
        {
            _service.FazerCheckOut(id);
            return NoContent();
        }
        catch (KeyNotFoundException erro)
        {
            return NotFound(new { mensagem = erro.Message });
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Cancela uma reserva Confirmada (antes do check-in). A reserva não é apagada.</summary>
    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Cancelar(Guid id)
    {
        try
        {
            _service.Cancelar(id);
            return NoContent();
        }
        catch (KeyNotFoundException erro)
        {
            return NotFound(new { mensagem = erro.Message });
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }
}
