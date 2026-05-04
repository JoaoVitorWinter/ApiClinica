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

    #region Verificar CPF

    private static bool ValidarCPF(string cpf)
    {
        // Formatando CPF
        cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "").Trim();
        // Separando numeros do CPF
        char[] cpfParts = cpf.ToCharArray();
        if (cpfParts.Length != 11) return false;

        // (Primeiro número * 10) + (segundo número * 9) + (terceiro número * 8)...
        int charSoma = 0;
        for (int charIndex = 0; charIndex <= 8; charIndex++)
        {
            int intValue = 0;
            if (!int.TryParse(cpfParts[charIndex].ToString(), out intValue)) {
                return false;
            }

            charSoma += (intValue * (10 - charIndex));
        }

        // Calculando primeiro dígito verificador necessário
        int restoDivisao = charSoma % 11;
        int digitoVerificador1Necessario = 0;
        if (restoDivisao > 1) digitoVerificador1Necessario = 11 - restoDivisao;

        // (Segundo número * 10) + (terceiro número * 9) + (quarto número * 8)...
        charSoma = 0;
        for (int charIndex = 1; charIndex <= 9; charIndex++)
        {
            // Para o último número na conta (que seria o primeiro dígito verificador), usa o necessário ao invés do recebido
            if (charIndex == 9)
            {
                charSoma += (digitoVerificador1Necessario * (10 - (charIndex - 1)));
                continue;
            }

            int intValue = 0;
            if (!int.TryParse(cpfParts[charIndex].ToString(), out intValue))
            {
                return false;
            }

            charSoma += (intValue * (10 - (charIndex - 1)));
        }

        // Calculando segundo dígito verificador necessário
        restoDivisao = charSoma % 11;
        int digitoVerificador2Necessario = 0;
        if (restoDivisao > 1) digitoVerificador2Necessario = 11 - restoDivisao;

        // Buscando dígitos verificadores recebidos
        int digitoVerificador1 = 0;
        int digitoVerificador2 = 0;
        if (!int.TryParse(cpfParts[9].ToString(), out digitoVerificador1)) return false;
        if (!int.TryParse(cpfParts[10].ToString(), out digitoVerificador2)) return false;

        // Verificando dígitos verificadores
        if (digitoVerificador1 != digitoVerificador1Necessario) return false;
        if (digitoVerificador2 != digitoVerificador2Necessario) return false;

        return true;
    }

    #endregion

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

        if (!ValidarCPF(dto.Cpf))
        {
            return BadRequest(new { mensagem = "O CPF é inválido" });
        }

        var paciente = PacienteMapper.ToModel(dto);
        
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // PATCH: api/pacientes
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        if (dto.DataNasc.HasValue)
        {
            if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });
            }
            else
            {
                paciente.DataNasc = dto.DataNasc;
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.Nome))
        {
            paciente.Nome = dto.Nome;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            paciente.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Telefone))
        {
            paciente.Telefone = dto.Telefone;
        }

        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);
        return Ok(pacienteDTO);
    }

    // DELETE: api/pacientes
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            return NotFound();

        var dataAtual = DateTime.Now;

        var possuiConsultas = await _context.Consultas.AnyAsync(consulta => consulta.PacienteId == id && consulta.DataHora > dataAtual);
        if (possuiConsultas)
        {
            return BadRequest(new { mensagem = "O paciente possui consultas marcadas, e não pode ser removido." });
        }
        
        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = PacienteMapper.ToDTO(paciente);
        return Ok(pacienteDTO);
    }
}