using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneekerWebShop.Api.Data;
using SneekerWebShop.Api.Dtos;
using SneekerWebShop.Api.Models;
using SneekerWebShop.Api.Services;

namespace SneekerWebShop.Api.Controllers;

/// <summary>Regisztracio, bejelentkezes (JWT generalas) es a sajat profil lekerdezese.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    /// <summary>Nyilvanos regisztracio. Az igy letrejovo felhasznalo mindig "User" szerepkort kap.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<LoginResultDto>> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "Ez az e-mail cím már foglalt." });

        if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName))
            return BadRequest(new { message = "Ez a felhasználónév már foglalt." });

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            Address = dto.Address,
            Phone = dto.Phone,
            Role = "User",
            CreatedAt = DateTime.Now
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Regisztracio utan rogton be is leptetjuk a felhasznalot
        return Ok(new LoginResultDto { Token = _tokenService.CreateToken(user), User = ToDto(user) });
    }

    /// <summary>Bejelentkezes email + jelszo parossal. Siker eseten JWT tokent ad vissza.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Hibás e-mail cím vagy jelszó." });

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Hibás e-mail cím vagy jelszó." });

        return Ok(new LoginResultDto { Token = _tokenService.CreateToken(user), User = ToDto(user) });
    }

    /// <summary>A bejelentkezett felhasznalo sajat adatai (a tokenben levo azonosito alapjan).</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(ToDto(user));
    }

    public static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        FullName = u.FullName,
        Address = u.Address,
        Phone = u.Phone,
        Role = u.Role,
        CreatedAt = u.CreatedAt
    };
}
