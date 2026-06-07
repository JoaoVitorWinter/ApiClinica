using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
    public interface IConsultaService
    {
        Task<List<ConsultaReadDTO>> GetConsultas();
        Task<ConsultaReadDTO> GetConsultaById(int id);
        Task<ConsultaReadDTO> CreateConsulta(ConsultaCreateDTO dto);
        Task<ConsultaReadDTO> UpdateConsulta(int id, ConsultaUpdateDTO dto);
        Task<ConsultaReadDTO> DeleteConsulta(int id);
    }
}
