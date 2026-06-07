using ApiClinica.DTOs;
using System.Security.Claims;

namespace ApiClinica.Interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterDTO dto, ClaimsPrincipal currentUser);
        Task<string> Login(LoginDTO dto);
    }
}