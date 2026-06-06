namespace ApiClinica.DTOs;

public class PacienteReadDTO
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Telefone { get; set; }
    public DateOnly DataNasc { get; set; }
}