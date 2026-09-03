using System.ComponentModel.DataAnnotations;

namespace SneekerWebShop.Api.Models;

/// <summary>
/// Az alkalmazas felhasznaloja. Ket szerepkor letezik: "Admin" es "User".
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>A jelszo sosem nyersen, hanem hash-elve tarolodik (Identity PasswordHasher).</summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Order> Orders { get; set; } = new();
}
