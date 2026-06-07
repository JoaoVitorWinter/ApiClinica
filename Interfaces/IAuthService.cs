using ApiClinica.DTOs;

namespace ApiClinica.Interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterDTO dto);
        Task<string> Login(LoginDTO dto);
    }
}