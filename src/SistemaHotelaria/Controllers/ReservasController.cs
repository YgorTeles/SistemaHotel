using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly ReservaService _reservaService;

        public ReservasController(ReservaService reservaService)
        {
            this._reservaService = reservaService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                List<ReservaDetalheDto> response = _reservaService.Listar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CriarReservaDto dto)
        {
            try
            {
                Reserva reserva = _reservaService.Criar(dto.Cpf, dto.NumeroQuarto, dto.DataEntrada, dto.DataSaida);
                return Ok(new { id = reserva.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id:guid}/checkin")]
        public IActionResult FazerCheckIn(Guid id)
        {
            try
            {
                _reservaService.FazerCheckIn(id);
                return Ok(new { mensagem = "Check-in realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id:guid}/checkout")]
        public IActionResult FazerCheckOut(Guid id)
        {
            try
            {
                _reservaService.FazerCheckOut(id);
                return Ok(new { mensagem = "Check-out realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("{id:guid}/cancelar")]
        public IActionResult Cancelar(Guid id)
        {
            try
            {
                _reservaService.Cancelar(id);
                return Ok(new { mensagem = "Reserva cancelada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
