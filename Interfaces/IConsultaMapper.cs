using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
    public interface IConsultaMapper
    {
        Consulta ToModel(ConsultaCreateDTO dto);
        ConsultaReadDTO ToDTO(Consulta consulta);
    }
}
