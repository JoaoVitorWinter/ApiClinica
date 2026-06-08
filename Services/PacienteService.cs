using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ApiClinica.Services;
public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;
    private readonly IPacienteMapper _mapper;

    public PacienteService(AppDbContext context, IPacienteMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region Verificar CPF
    private static bool ValidarCPF(string cpf)
    {
        // Separando numeros do CPF
        char[] cpfParts = cpf.ToCharArray();
        if (cpfParts.Length != 11) return false;

        // Se números forem todos iguais, CPF também é inválido (ex: 111.111.111-11)
        bool numerosTodosIguais = true;
        char ultimoCaractere = cpfParts[0];
        foreach (char caractereAtual in cpfParts)
        {
            if (caractereAtual != ultimoCaractere) numerosTodosIguais = false;
        }
        if (numerosTodosIguais == true) return false;

        // (Primeiro número * 10) + (segundo número * 9) + (terceiro número * 8) ... (nono número * 2)
        int charSoma = 0;
        for (int charIndex = 0; charIndex <= 8; charIndex++)
        {
            int intValue = 0;
            if (!int.TryParse(cpfParts[charIndex].ToString(), out intValue))
            {
                return false;
            }

            charSoma += (intValue * (10 - charIndex));
        }

        // Calculando primeiro dígito verificador necessário
        int restoDivisao = charSoma % 11;
        int digitoVerificador1Necessario = 0;
        if (restoDivisao > 1) digitoVerificador1Necessario = 11 - restoDivisao;

        // (Segundo número * 10) + (terceiro número * 9) + (quarto número * 8) ... (décimo número * 2)
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

    public static bool ValidarTelefoneBR(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone)) return false;

        // Expressão Regular para validar números com ou sem máscara (formato 8 ou 9 dígitos)
        // Aceita formatos como: (XX) XXXX-XXXX ou (XX) 9XXXX-XXXX
        string padrao = @"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-\d{4}$";

        return Regex.IsMatch(telefone, padrao);
    }

    public async Task<List<PacienteReadDTO>> GetPacientes()
    {
        var pacientes = await _context.Pacientes.ToListAsync();

        var pacientesDTO = pacientes
            .Select(p => _mapper.ToDTO(p))
            .ToList();

        return pacientesDTO;
    }

    public async Task<PacienteReadDTO> GetPacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            throw new NotFoundException("Paciente não encontrado");

        return _mapper.ToDTO(paciente);
    }

    public async Task<PacienteReadDTO> CreatePaciente(PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ValidationErrorException("Data de nascimento não pode ser futura");
        }

        dto.Cpf = dto.Cpf.Replace(".", "").Replace("-", "").Trim();
        if (await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf))
        {
            throw new ValidationErrorException("Usuário com o CPF informado já existe");
        }

        if (!ValidarCPF(dto.Cpf))
        {
            throw new ValidationErrorException("O CPF é inválido");
        }

        if (!ValidarTelefoneBR(dto.Telefone))
        {
            throw new ValidationErrorException("Telefone inválido");
        }

        var paciente = _mapper.ToModel(dto);

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = _mapper.ToDTO(paciente);

        return await GetPacienteById(paciente.Id);
    }

    public async Task<PacienteReadDTO> UpdatePaciente(int id, PacienteUpdateDTO dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            throw new NotFoundException("Paciente não encontrado");

        if (dto.DataNasc.HasValue)
        {
            if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ValidationErrorException("Data de nascimento não pode ser futura");
            }
            else
            {
                paciente.DataNasc = dto.DataNasc.Value;
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
            if (!ValidarTelefoneBR(dto.Telefone))
            {
                throw new ValidationErrorException("Telefone inválido");
            }

            paciente.Telefone = dto.Telefone;
        }

        await _context.SaveChangesAsync();

        var pacienteDTO = _mapper.ToDTO(paciente);
        return pacienteDTO;
    }

    public async Task<PacienteReadDTO> DeletePacienteById(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);

        if (paciente == null)
            throw new NotFoundException("Paciente não encontrado");

        var dataAtual = DateTime.Now;

        var possuiConsultas = await _context.Consultas.AnyAsync(consulta => consulta.PacienteId == id && consulta.DataHora > dataAtual);
        if (possuiConsultas)
        {
            throw new ValidationErrorException("O paciente possui consultas marcadas, e não pode ser removido");
        }

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        var pacienteDTO = _mapper.ToDTO(paciente);
        return pacienteDTO;
    }

}
