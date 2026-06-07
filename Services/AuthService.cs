using ApiClinica.Interfaces;
using ApiClinica.DTOs;
using ApiClinica.Models;
using ApiClinica.Data;
using ApiClinica.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiClinica.Services;

public class AuthService: IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task Register(RegisterDTO dto, ClaimsPrincipal currentUser)
    {
        var usuarioExistente = await _context.Usuarios
           .AnyAsync(u => u.Login == dto.Login);

        if (usuarioExistente)
        {
            throw new ValidationErrorException("Usuário já registrado com esse nome");
        }

        var perfilUsuario = !String.IsNullOrEmpty(dto.Perfil) ? dto.Perfil : "User";
        if (perfilUsuario != "Admin" && perfilUsuario != "User")
        {
            throw new ValidationErrorException("Role inválida para o usuário");
        }

        var existeAdmin = await _context.Usuarios.AnyAsync(u => u.Perfil == "Admin");

        if (perfilUsuario == "Admin")
        {
            var perfilLogado = currentUser.FindFirst(ClaimTypes.Role)?.Value;

            if (perfilLogado != "Admin" && existeAdmin)
            {
                throw new ValidationErrorException("Somente administradores podem criar usuários admin");
            }
        }

        var usuario = new Usuario
        {
            Login = dto.Login,
            Senha = dto.Senha,
            Perfil = perfilUsuario
        };

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();
    }

    public async Task<string> Login(LoginDTO dto)
    {
        // Busca usuário no banco
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Login == dto.Login &&
                u.Senha == dto.Senha);

        // Retorna erro se login inválido
        if (usuario == null)
        {
            throw new ValidationErrorException("Usuário ou senha inválidos");
        }

        // Informações que serão gravadas dentro do token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Login),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        // Chave usada para assinar o token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        // Define algoritmo de criptografia
        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Cria o token JWT
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,

            // Tempo de expiração do token
            expires: DateTime.Now.AddHours(2),

            signingCredentials: creds
        );

        // Converte token para string
        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // Retorna token para o usuário
        return tokenString;
    }
}