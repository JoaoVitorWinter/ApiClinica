using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(p => PacienteMapper.ToDTO(p))
            .ToList();

        return Ok(pacientesDTO);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePacient([FromBody] PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        }

        if (await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf))
        {
            return BadRequest(new { mensagem = "Usuário com o CPF informado já existe" });
        }

        var paciente = PacienteMapper.ToModel(dto);
        
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        if (dto.Nome != null) paciente.Nome = dto.Nome;
        if (dto.Email != null) paciente.Email = dto.Email;
        if (dto.Telefone != null) paciente.Telefone = dto.Telefone;

        if (dto.Cpf != null)
        {
            if (await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf && p.Id != id))
            {
                return BadRequest(new { mensagem = "Outro usuário com o CPF informado já existe" });
            }
            else
            {
                paciente.Cpf = dto.Cpf;
            }
        }
        
        if (dto.DataNasc != null)
        {
            if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
            } 
            else
            {
                paciente.DataNasc = dto.DataNasc.Value;
            }
        }

        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);
        return Ok(pacienteDTO);
    }

}