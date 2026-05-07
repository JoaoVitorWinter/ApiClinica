using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiClinica.Models;

public class Consulta
{
	public int Id { get; set; }
	[ForeignKey("paciente")]
	public required int PacienteId { get; set; }
	public virtual Paciente paciente { get; set; } = null!;
	[ForeignKey("medico")]
	public required int MedicoId { get; set; }
	public virtual Medico medico { get; set; } = null!;
	public required DateTime DataHora { get; set; }
}