using ApiClinica.DTOs;

namespace ApiClinica.Interfaces
{
    public interface IPacienteService
    {
        Task<List<PacienteReadDTO>> GetPacientes();
        Task<PacienteReadDTO> GetPacienteById(int id);
        Task<PacienteReadDTO> CreatePaciente(PacienteCreateDTO pacienteDTO);
        Task<PacienteReadDTO> UpdatePaciente(int id, PacienteUpdateDTO pacienteDTO);
        Task<PacienteReadDTO> DeletePacienteById(int id);
    }
}
