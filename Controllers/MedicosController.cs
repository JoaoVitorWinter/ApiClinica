using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        var medicos = await _context.Medicos.ToListAsync();
        var medicosDTO = medicos
            .Select(p => MedicoMapper.ToDTO(p))
            .ToList();

        return Ok(medicosDTO);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico == null)
            return NotFound();

        return Ok(MedicoMapper.ToDTO(medico));
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        var medico = MedicoMapper.ToModel(dto);

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        var medicoDTO = MedicoMapper.ToDTO(medico);

        return CreatedAtAction(nameof(GetMedicoById), new { id = medico.Id }, medicoDTO);
    }

    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Nome))
        {
            medico.Nome = dto.Nome;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            medico.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Telefone))
        {
            medico.Telefone = dto.Telefone;
        }

        if (!string.IsNullOrWhiteSpace(dto.CRM))
        {
            medico.CRM = dto.CRM;
        }

        await _context.SaveChangesAsync();
        
        var medicoDTO = MedicoMapper.ToDTO(medico);
        return Ok(medicoDTO);
    }

    // DELETE: api/medicos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        var medico = await _context.Medicos.FindAsync(id);

        if (medico == null)
            return NotFound();

        var dataAtual = DateTime.Now;

        var possuiConsultas = await _context.Consultas.AnyAsync(consulta => consulta.MedicoId == id && consulta.DataHora > dataAtual);
        if (possuiConsultas)
        {
            return BadRequest(new { mensagem = "O médico possui consultas marcadas, e não pode ser removido." });
        }

        _context.Medicos.Remove(medico);

        await _context.SaveChangesAsync();

        var medicoDTO = MedicoMapper.ToDTO(medico);
        return Ok(medicoDTO);
    }
}