using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        var consultas = await _context.Consultas.ToListAsync();

        var consultasDTO = consultas
            .Select(c => ConsultaMapper.ToDTO(c))
            .ToList();

        return Ok(consultasDTO);
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);

        if (consulta == null)
            return NotFound();

        return Ok(ConsultaMapper.ToDTO(consulta));
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {

        if (dto.DataHora < DateTime.Now)
        {
            return BadRequest(new { mensagem = "Data e hora da consulta não podem ser no passado" });
        }

        var paciente = await _context.Pacientes.FindAsync(dto.PacienteId);

        if (paciente == null)
        {
            return BadRequest(new { mensagem = "Paciente com o ID informado não existe" });
        }

        var medico = await _context.Medicos.FindAsync(dto.MedicoId);

        if (medico == null)
        {
            return BadRequest(new { mensagem = "Médico com o ID informado não existe" });
        }

        var DataHoraInicio = dto.DataHora;
        var DataHoraFim = dto.DataHora.AddMinutes(30);

        var consultasNoPeriodo = await _context.Consultas
            .Include(c => c.paciente)
            .Include(c => c.medico)
            .Where(c => (c.MedicoId == dto.MedicoId || c.PacienteId == dto.PacienteId) && c.DataHora >= DataHoraInicio && c.DataHora <= DataHoraFim)
            .ToArrayAsync();

        if(consultasNoPeriodo.Length > 0)
        {
            return BadRequest(new { mensagem = "O paciente ou médico já tem uma consulta agendada para esse horário" });
        }

        var consulta = ConsultaMapper.ToModel(dto);

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        var consultaDTO = ConsultaMapper.ToDTO(consulta);

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consultaDTO);
    }

    // PATCH: api/consultas/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        //var paciente = await _context.Pacientes.FindAsync(id);

        //if (paciente == null)
        //    return NotFound();

        //if (dto.Nome != null) paciente.Nome = dto.Nome;
        //if (dto.Email != null) paciente.Email = dto.Email;
        //if (dto.Telefone != null) paciente.Telefone = dto.Telefone;

        //if (dto.Cpf != null)
        //{
        //    if (await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf && p.Id != id))
        //    {
        //        return BadRequest(new { mensagem = "Outro usuário com o CPF informado já existe" });
        //    }
        //    else
        //    {
        //        paciente.Cpf = dto.Cpf;
        //    }
        //}

        //if (dto.DataNasc != null)
        //{
        //    if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        //    {
        //        return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
        //    }
        //    else
        //    {
        //        paciente.DataNasc = dto.DataNasc.Value;
        //    }
        //}

        //await _context.SaveChangesAsync();

        //var pacienteDTO = PacienteMapper.ToDTO(paciente);
        //return Ok(pacienteDTO);

        return NoContent();
    }

    // DELETE: api/consultas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        //var paciente = await _context.Pacientes.FindAsync(id);

        //if (paciente == null)
        //    return NotFound();

        //_context.Pacientes.Remove(paciente);

        //await _context.SaveChangesAsync();

        ////ADICIONAR VALIDAÇÃO PARA VER SE O PACIENTE TEM CONSULTAS FUTURAS AGENDADAS, CASO TENHA, NÃO PERMITIR A EXCLUSÃO

        //var pacienteDTO = PacienteMapper.ToDTO(paciente);
        //return Ok(pacienteDTO);
        return NoContent();
    }

}