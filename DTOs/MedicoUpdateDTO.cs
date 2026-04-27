using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiClinica.DTOs;

public class MedicoUpdateDTO
{
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    [Phone(ErrorMessage = "Telefone inválido")]
    public string? Telefone { get; set; }
    public string? CRM { get; set; } 
}