using System.ComponentModel.DataAnnotations;

namespace SneekerWebShop.Api.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Size { get; set; }
    public string Color { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

/// <summary>Termek letrehozasa / modositasa (admin).</summary>
public class ProductCreateDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 10000000)]
    public decimal Price { get; set; }

    [Range(20, 60)]
    public int Size { get; set; }

    [MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    [Range(0, 100000)]
    public int Stock { get; set; }

    [MaxLength(300)]
    public string ImageUrl { get; set; } = string.Empty;
}
