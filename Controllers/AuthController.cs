using ApiClinica.Data;
using ApiClinica.DTOs;
using ApiClinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClinica.Interfaces;
using ApiClinica.Services.Exceptions;

namespace ApiClinica.Controllers;

[ApiController]

// Define a rota base da controller
// Exemplo: api/auth/login
[Route("api/auth")]
public class AuthController : ControllerBase
{
    // Contexto do banco de dados
    private readonly AppDbContext _context;

    private readonly IAuthService _service;

    public AuthController(
        AppDbContext context,
        IAuthService service)
    {
        _context = context;
        _service = service;
    }

    // Endpoint para cadastrar usuário
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO dto)
    {
        try
        {
            await _service.Register(dto, User);
            return Ok("Usuário criado com sucesso");
        }
        catch (ValidationErrorException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

    // Endpoint de login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO dto)
    {
        try
        {
            var token = await _service.Login(dto);
            return Ok(token);
        }
        catch (ValidationErrorException exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }
}