using System.ComponentModel.DataAnnotations;

namespace SneekerWebShop.Api.Models;

/// <summary>Egy elado sportcipo (termek).</summary>
public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    /// <summary>Cipomeret (EU), pl. 42.</summary>
    public int Size { get; set; }

    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    public int Stock { get; set; }

    /// <summary>Relativ kep utvonal, pl. /images/nike-air-max-90.jpg</summary>
    [MaxLength(300)]
    public string ImageUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<OrderItem> OrderItems { get; set; } = new();
}
