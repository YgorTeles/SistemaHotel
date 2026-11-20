using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuartosController : ControllerBase
    {
        private readonly QuartoService _quartoService;

        public QuartosController(QuartoService quartoService)
        {
            this._quartoService = quartoService;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                List<Quarto> response = _quartoService.Listar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("disponiveis")]
        public IActionResult ListarDisponiveis([FromQuery] DateTime entrada, [FromQuery] DateTime saida)
        {
            try
            {
                List<Quarto> response = _quartoService.ListarDisponiveis(entrada, saida);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{numero:int}")]
        public IActionResult BuscarPorNumero(int numero)
        {
            try
            {
                Quarto response = _quartoService.BuscarPorNumero(numero);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] CadastrarQuartoDto dto)
        {
            try
            {
                Quarto response = _quartoService.Cadastrar(dto.Numero, dto.Tipo, dto.Capacidade, dto.ValorDiaria);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{numero:int}")]
        public IActionResult Atualizar(int numero, [FromBody] AtualizarQuartoDto dto)
        {
            try
            {
                Quarto response = _quartoService.Atualizar(numero, dto.Tipo, dto.Capacidade, dto.ValorDiaria);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{numero:int}")]
        public IActionResult Remover(int numero)
        {
            try
            {
                _quartoService.Remover(numero);
                return Ok(new { mensagem = "Quarto removido com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
