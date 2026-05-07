namespace ApiClinica.DTOs;

public class PacienteReadDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public DateOnly DataNasc { get; set; }
}