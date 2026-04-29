using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.DTOs;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    [Phone(ErrorMessage = "Telefone inválido")]
    public string? Telefone { get; set; }
    public DateOnly? DataNasc { get; set; }
    public string? Cpf { get; set; }
}