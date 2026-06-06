using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ApiClinica.Services.Exceptions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _service.GetPacientes();
        return Ok(pacientes);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        try
        {
            var paciente = await _service.GetPacienteById(id);
            return Ok(paciente);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePaciente([FromBody] PacienteCreateDTO dto)
    {
        try
        {
            var paciente = await _service.CreatePaciente(dto);
            return Created(nameof(GetPacienteById), paciente);
        }
        catch (ValidationErrorException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PATCH: api/pacientes
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        try
        {
            var paciente = await _service.UpdatePaciente(id, dto);
            return Ok(paciente);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationErrorException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE: api/pacientes
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePacienteById(int id)
    {
        try
        {
            var paciente = await _service.DeletePacienteById(id);
            return Ok(paciente);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationErrorException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}