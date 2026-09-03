using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneekerWebShop.Api.Data;
using SneekerWebShop.Api.Dtos;
using SneekerWebShop.Api.Models;

namespace SneekerWebShop.Api.Controllers;

/// <summary>Felhasznalok teljes CRUD kezelese - kizarolag admin szerepkorrel.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public UsersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _db.Users
            .OrderBy(u => u.Id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = u.FullName,
                Address = u.Address,
                Phone = u.Phone,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "A felhasználó nem található." });
        return Ok(AuthController.ToDto(user));
    }

    /// <summary>Uj felhasznalo felvitele adminkent (szerepkorrel egyutt).</summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(RegisterDto dto, [FromQuery] string role = "User")
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
            Role = role == "Admin" ? "Admin" : "User",
            CreatedAt = DateTime.Now
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = user.Id }, AuthController.ToDto(user));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, UserUpdateDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "A felhasználó nem található." });

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            return BadRequest(new { message = "Ez az e-mail cím már foglalt." });

        if (await _db.Users.AnyAsync(u => u.UserName == dto.UserName && u.Id != id))
            return BadRequest(new { message = "Ez a felhasználónév már foglalt." });

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.Address = dto.Address;
        user.Phone = dto.Phone;
        user.Role = dto.Role == "Admin" ? "Admin" : "User";

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        await _db.SaveChangesAsync();
        return Ok(AuthController.ToDto(user));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "A felhasználó nem található." });

        var currentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (user.Id == currentId)
            return BadRequest(new { message = "Saját magadat nem törölheted." });

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
