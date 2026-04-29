using ApiClinica.Models;
using ApiClinica.DTOs;

namespace ApiClinica.Mappers;

public static class ConsultaMapper
{
    public static Consulta ToModel(ConsultaCreateDTO dto)
    {
        return new Consulta
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = dto.DataHora
        };
    }

    public static ConsultaReadDTO ToDTO(Consulta consulta)
    {
        return new ConsultaReadDTO
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            paciente = consulta.paciente,
            MedicoId = consulta.MedicoId,
            medico = consulta.medico,
            DataHora = consulta.DataHora
        };
    }
}