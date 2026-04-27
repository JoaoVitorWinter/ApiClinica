using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.DTOs;

public class MedicoCreateDTO
{
    public required string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    [Phone(ErrorMessage = "Telefone inválido")]
    public required string Telefone { get; set; }
    public required string CRM { get; set; }
}