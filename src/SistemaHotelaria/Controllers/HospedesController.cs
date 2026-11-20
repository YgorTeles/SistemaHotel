using Microsoft.AspNetCore.Mvc;
using SistemaHotelaria.Entities;
using SistemaHotelaria.Dtos;
using SistemaHotelaria.Services;

namespace SistemaHotelaria.Controllers;

// Erros vindos do service:
//   KeyNotFoundException -> 404 (hóspede não encontrado)
//   ArgumentException    -> 400 (dado inválido / regra de negócio)
[ApiController]
[Route("api/hospedes")]
[Produces("application/json")]
public class HospedesController : ControllerBase
{
    private HospedeService _service;

    public HospedesController(HospedeService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os hóspedes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<Hospede>), StatusCodes.Status200OK)]
    public ActionResult<List<Hospede>> Listar()
    {
        return Ok(_service.Listar());
    }

    /// <summary>Busca um hóspede pelo CPF (aceita com ou sem máscara).</summary>
    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(Hospede), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Hospede> BuscarPorCpf(string cpf)
    {
        try
        {
            return Ok(_service.BuscarPorCpf(cpf));
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

    /// <summary>Cadastra um novo hóspede.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Hospede), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Hospede> Cadastrar(CadastrarHospedeDto request)
    {
        try
        {
            Hospede hospede = _service.Cadastrar(
                request.Nome ?? string.Empty,
                request.Cpf ?? string.Empty,
                request.Telefone ?? string.Empty,
                request.Email ?? string.Empty);

            return Created($"/api/hospedes/{hospede.Cpf}", hospede);
        }
        catch (ArgumentException erro)
        {
            return BadRequest(new { mensagem = erro.Message });
        }
    }

    /// <summary>Atualiza nome, telefone e e-mail do hóspede. O CPF não pode ser alterado.</summary>
    [HttpPut("{cpf}")]
    [ProducesResponseType(typeof(Hospede), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Hospede> Atualizar(string cpf, AtualizarHospedeDto request)
    {
        try
        {
            Hospede hospede = _service.Atualizar(
                cpf,
                request.Nome ?? string.Empty,
                request.Telefone ?? string.Empty,
                request.Email ?? string.Empty);

            return Ok(hospede);
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

    /// <summary>Remove um hóspede.</summary>
    [HttpDelete("{cpf}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Remover(string cpf)
    {
        try
        {
            _service.Remover(cpf);
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
