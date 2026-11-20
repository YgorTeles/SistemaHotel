using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

// Erros vindos do service:
//   KeyNotFoundException -> 404 (quarto não encontrado)
//   ArgumentException    -> 400 (dado inválido / regra de negócio)
[ApiController]
[Route("api/quartos")]
[Produces("application/json")]
public class QuartosController : ControllerBase
{
    private QuartoService _service;

    public QuartosController(QuartoService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os quartos, ordenados por número.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Quarto>), StatusCodes.Status200OK)]
    public ActionResult<List<Quarto>> Listar()
    {
        return Ok(_service.Listar());
    }

    /// <summary>Lista os quartos sem reserva (não cancelada) no período informado.</summary>
    /// <param name="entrada">Data de entrada (YYYY-MM-DD).</param>
    /// <param name="saida">Data de saída (YYYY-MM-DD).</param>
    [HttpGet("disponiveis")]
    [ProducesResponseType(typeof(List<Quarto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<List<Quarto>> ListarDisponiveis([FromQuery] DateTime entrada, [FromQuery] DateTime saida)
    {
        try
        {
            return Ok(_service.ListarDisponiveis(entrada, saida));
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Busca um quarto pelo número.</summary>
    [HttpGet("{numero:int}")]
    [ProducesResponseType(typeof(Quarto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Quarto> BuscarPorNumero(int numero)
    {
        try
        {
            return Ok(_service.BuscarPorNumero(numero));
        }
        catch (KeyNotFoundException erro)
        {
            return NotFound(new { mensagem = erro.Message });
        }
    }

    /// <summary>Cadastra um novo quarto (sempre criado como não ocupado).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Quarto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Quarto> Cadastrar(CadastrarQuartoDto request)
    {
        try
        {
            Quarto quarto = _service.Cadastrar(
                request.Numero,
                request.Tipo ?? string.Empty,
                request.Capacidade,
                request.ValorDiaria);

            return Created($"/api/quartos/{quarto.Numero}", quarto);
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Atualiza tipo, capacidade e diária. O número não pode ser alterado.</summary>
    [HttpPut("{numero:int}")]
    [ProducesResponseType(typeof(Quarto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Quarto> Atualizar(int numero, AtualizarQuartoDto request)
    {
        try
        {
            Quarto quarto = _service.Atualizar(
                numero,
                request.Tipo ?? string.Empty,
                request.Capacidade,
                request.ValorDiaria);

            return Ok(quarto);
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

    /// <summary>Remove um quarto (não permitido se estiver ocupado).</summary>
    [HttpDelete("{numero:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Remover(int numero)
    {
        try
        {
            _service.Remover(numero);
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
