using ApiClinica.Data;
using ApiClinica.Interfaces;
using ApiClinica.Services.Exceptions;
using ApiClinica.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ApiClinica.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly AppDbContext _context;
        private readonly IMedicoMapper _mapper;

        public MedicoService(AppDbContext context, IMedicoMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<MedicoReadDTO>> GetMedicos()
        {
            var medicos = await _context.Medicos.ToListAsync();
            var medicosDTO = medicos
                .Select(p => _mapper.ToDTO(p))
                .ToList();
            return medicosDTO;
        }

        public async Task<MedicoReadDTO> GetMedicoById(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
                throw new NotFoundException("Médico não encontrado");

            return _mapper.ToDTO(medico);
        }

        public async Task<MedicoReadDTO> CreateMedico(MedicoCreateDTO dto)
        {
            var medico = _mapper.ToModel(dto);

            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();

            var medicoDTO = _mapper.ToDTO(medico);

            return medicoDTO;
        }

        public async Task<MedicoReadDTO> UpdateMedico(int id, MedicoUpdateDTO dto)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
                throw new NotFoundException("Médico não encontrado");

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

            var medicoDTO = _mapper.ToDTO(medico);
            return medicoDTO;
        }

        public async Task<MedicoReadDTO> DeleteMedico(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);

            if (medico == null)
                throw new NotFoundException("Médico não encontrado");

            var dataAtual = DateTime.Now;

            var possuiConsultas = await _context.Consultas.AnyAsync(consulta => consulta.MedicoId == id && consulta.DataHora > dataAtual);
            if (possuiConsultas)
            {
                throw new ValidationErrorException("O médico possui consultas marcadas, e não pode ser removido");
            }

            _context.Medicos.Remove(medico);

            await _context.SaveChangesAsync();

            var medicoDTO = _mapper.ToDTO(medico);
            return medicoDTO;
        }
    }
}
