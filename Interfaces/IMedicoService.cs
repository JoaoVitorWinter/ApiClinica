using ApiClinica.DTOs;

namespace ApiClinica.Interfaces
{
    public interface IMedicoService
    {
        Task<List<MedicoReadDTO>> GetMedicos();
        Task<MedicoReadDTO> GetMedicoById(int id);
        Task<MedicoReadDTO> CreateMedico(MedicoCreateDTO dto);
        Task<MedicoReadDTO> UpdateMedico(int id, MedicoUpdateDTO dto);
        Task<MedicoReadDTO> DeleteMedico(int id);
    }
}
