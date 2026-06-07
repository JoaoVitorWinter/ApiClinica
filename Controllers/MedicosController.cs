using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using ApiClinica.Interfaces;
using ApiClinica.Services.Exceptions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMedicoService _service;

    public MedicosController(AppDbContext context, IMedicoService service)
    {
        _context = context;
        _service = service;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _service.GetMedicos();
        return Ok(medicos);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        try
        {
            var medico = await _service.GetMedicoById(id);
            return Ok(medico);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        try
        {
            var medico = await _service.CreateMedico(dto);
            return Created(nameof(GetMedicoById), medico);
        }
        catch (ValidationErrorException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        try
        {
            var medico = await _service.UpdateMedico(id, dto);
            return Ok(medico);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationErrorException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    // DELETE: api/medicos/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        try
        {
            var medico = await _service.DeleteMedico(id);
            return Ok(medico);
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