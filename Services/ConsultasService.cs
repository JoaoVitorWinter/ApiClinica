using ApiClinica.Interfaces;
using ApiClinica.Models;
using ApiClinica.Data;
using ApiClinica.DTOs;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Services.Exceptions;

namespace ApiClinica.Services
{
    public class ConsultasService : IConsultaService
    {
        private readonly AppDbContext _context;
        private readonly IConsultaMapper _mapper;

        public ConsultasService(AppDbContext context, IConsultaMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ConsultaReadDTO>> GetConsultas()
        {
            var consultas = await _context.Consultas.ToListAsync();

            var consultasDTO = consultas
                .Select(c => _mapper.ToDTO(c))
                .ToList();

            return consultasDTO;
        }

        public async Task<ConsultaReadDTO> GetConsultaById(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);

            if (consulta == null)
                throw new NotFoundException("Consulta não encontrada");

            return _mapper.ToDTO(consulta);
        }

        public async Task<ConsultaReadDTO> CreateConsulta(ConsultaCreateDTO dto)
        {
            var paciente = await _context.Pacientes.FindAsync(dto.PacienteId);

            if (paciente == null)
            {
                throw new ValidationErrorException("Paciente com o ID informado não existe");
            }

            var medico = await _context.Medicos.FindAsync(dto.MedicoId);

            if (medico == null)
            {
                throw new ValidationErrorException("Médico com o ID informado não existe");
            }

            if (dto.DataHora < DateTime.Now)
            {
                throw new ValidationErrorException("Data e hora da consulta não podem ser no passado");
            }

            var DataHoraInicio = dto.DataHora;
            var DataHoraFim = dto.DataHora.AddMinutes(30);

            var consultasNoPeriodo = await _context.Consultas
                .Include(c => c.paciente)
                .Include(c => c.medico)
                .Where(c => (c.MedicoId == dto.MedicoId || c.PacienteId == dto.PacienteId) && c.DataHora >= DataHoraInicio && c.DataHora <= DataHoraFim)
                .ToArrayAsync();

            if (consultasNoPeriodo.Length > 0)
            {
                throw new ValidationErrorException("O paciente ou médico já tem uma consulta agendada para esse horário");
            }

            var consulta = _mapper.ToModel(dto);

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            var consultaDTO = _mapper.ToDTO(consulta);

            return consultaDTO;
        }

        public async Task<ConsultaReadDTO> UpdateConsulta(int id, ConsultaUpdateDTO dto)
        {
            var consulta = await _context.Consultas.FindAsync(id);

            if (consulta == null)
            {
                throw new NotFoundException("Consulta não encontrada");
            }

            if (dto.DataHora.HasValue)
            {
                if (dto.DataHora < DateTime.Now)
                {
                    throw new ValidationErrorException("Data e hora da consulta não podem ser no passado");
                }

                var DataHoraInicio = dto.DataHora.Value;
                var DataHoraFim = dto.DataHora.Value.AddMinutes(30);

                var consultasNoPeriodo = await _context.Consultas
                    .Include(c => c.paciente)
                    .Include(c => c.medico)
                    .Where(c => (c.MedicoId == dto.MedicoId || c.PacienteId == dto.PacienteId) && c.DataHora >= DataHoraInicio && c.DataHora <= DataHoraFim && c.Id != id)
                    .ToArrayAsync();

                if (consultasNoPeriodo.Length > 0)
                {
                    throw new ValidationErrorException("O paciente ou médico já tem uma consulta agendada para esse horário");
                }

                consulta.DataHora = dto.DataHora.Value;
            }

            if (dto.PacienteId.HasValue)
            {
                var paciente = await _context.Pacientes.FindAsync(dto.PacienteId);

                if (paciente == null)
                {
                    throw new ValidationErrorException("Paciente com o ID informado não existe");
                }

                consulta.paciente = paciente;
            }

            if (dto.MedicoId.HasValue)
            {
                var medico = await _context.Medicos.FindAsync(dto.MedicoId);

                if (medico == null)
                {
                    throw new ValidationErrorException("Médico com o ID informado não existe");
                }

                consulta.medico = medico;
            }

            await _context.SaveChangesAsync();

            var consultaDTO = _mapper.ToDTO(consulta);

            return consultaDTO;
        }

        public async Task<ConsultaReadDTO> DeleteConsulta(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);

            if (consulta == null)
                throw new NotFoundException("Consulta não encontrada");

            _context.Consultas.Remove(consulta);

            await _context.SaveChangesAsync();

            var consultaDTO = _mapper.ToDTO(consulta);
            return consultaDTO;
        }
    }
}
