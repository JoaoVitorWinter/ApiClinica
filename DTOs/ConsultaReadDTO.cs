using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiClinica.Models;

public class ConsultaReadDTO
{
    public int Id { get; set; }
    public int PacienteId { get; set; }
    public virtual Paciente paciente { get; set; } = null!;
    public int MedicoId { get; set; }
    public virtual Medico medico { get; set; } = null!;
    public DateTime DataHora { get; set; }
}