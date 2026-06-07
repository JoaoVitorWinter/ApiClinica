using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces
{
    public interface IMedicoMapper
    {
        Medico ToModel(MedicoCreateDTO dto);
        MedicoReadDTO ToDTO(Medico medico);
    }
}
