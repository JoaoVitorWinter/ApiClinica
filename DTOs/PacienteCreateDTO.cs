using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.DTOs;

public class PacienteCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    [Phone(ErrorMessage = "Telefone inválido")]
    [RegularExpression(@"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-\d{4}$",
        ErrorMessage = "Telefone deve estar no formato (XX) XXXX-XXXX ou (XX) 9XXXX-XXXX")]
    public required string Telefone { get; set; }
    public required DateOnly DataNasc { get; set; }
    [RegularExpression(@"^(?:\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$",
        ErrorMessage = "CPF deve estar no formato 11111111111 ou 111.111.111-11")]
    public required string Cpf { get; set; }
}