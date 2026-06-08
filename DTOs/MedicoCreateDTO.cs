using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.DTOs;

public class MedicoCreateDTO
{
    public required string Nome { get; set; }
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    [Phone(ErrorMessage = "Telefone inválido")]
    [RegularExpression(@"^\(?\d{2}\)?\s?(9\d{4}|\d{4})-\d{4}$",
        ErrorMessage = "Telefone deve estar no formato (XX) XXXX-XXXX ou (XX) 9XXXX-XXXX")]
    public required string Telefone { get; set; }
    public required string CRM { get; set; }
}