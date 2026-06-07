using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Authorization;
using ApiClinica.Interfaces;
using ApiClinica.Services.Exceptions;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _service.GetConsultas();
        return Ok(consultas);
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        try
        {
            var consulta = await _service.GetConsultaById(id);
            return Ok(consulta);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        try
        {
            var consulta = await _service.CreateConsulta(dto);
            return Created(nameof(GetConsultaById), consulta);
        }
        catch (ValidationErrorException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    // PATCH: api/consultas/{id}
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        try
        {
            var consulta = await _service.UpdateConsulta(id, dto);
            return Ok(consulta);
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

    // DELETE: api/consultas/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        try
        {
            var consulta = await _service.DeleteConsulta(id);
            return Ok(consulta);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

}