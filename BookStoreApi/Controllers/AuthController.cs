using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Requests;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BookStoreApi.Interfaces;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsersService _usersService;

    public AuthController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _usersService.GetByUsernameAsync(request.Username);
        if (existingUser is not null)
        {
            return BadRequest("Username already exists.");
        }

        var newUser = await _usersService.Register(request);

        return Ok(newUser);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _usersService.Login(request);

        if (token is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(new { Token = token });
    }
}