using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospedesController : ControllerBase
    {
        private readonly HospedeService _hospedeService;

        public HospedesController(HospedeService hospedeService)
        {
            this._hospedeService = hospedeService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                List<Hospede> response = _hospedeService.Listar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{cpf}")]
        public IActionResult BuscarPorCpf(string cpf)
        {
            try
            {
                Hospede response = _hospedeService.BuscarPorCpf(cpf);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] CadastrarHospedeDto dto)
        {
            try
            {
                Hospede response = _hospedeService.Cadastrar(dto.Nome, dto.Cpf, dto.Telefone, dto.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{cpf}")]
        public IActionResult Atualizar(string cpf, [FromBody] AtualizarHospedeDto dto)
        {
            try
            {
                Hospede response = _hospedeService.Atualizar(cpf, dto.Nome, dto.Telefone, dto.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{cpf}")]
        public IActionResult Remover(string cpf)
        {
            try
            {
                _hospedeService.Remover(cpf);
                return Ok(new { mensagem = "Hóspede removido com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
