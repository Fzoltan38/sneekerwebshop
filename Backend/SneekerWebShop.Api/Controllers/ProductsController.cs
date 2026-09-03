using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneekerWebShop.Api.Data;
using SneekerWebShop.Api.Dtos;
using SneekerWebShop.Api.Models;

namespace SneekerWebShop.Api.Controllers;

/// <summary>
/// Termekek (sportcipok) kezelese. A listazas es a reszletek nyilvanosak,
/// a felvitel, modositas es torles csak admin joggal erheto el.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ProductsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    /// <summary>Osszes termek, opcionalis kereso- es markaszurovel (bejelentkezes nelkul is).</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(string? search, string? brand)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p => p.Brand == brand);

        var list = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Description = p.Description,
                Price = p.Price,
                Size = p.Size,
                Color = p.Color,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        return Ok(list);
    }

    /// <summary>A letezo markak listaja a szuro legordulohoz.</summary>
    [HttpGet("brands")]
    public async Task<ActionResult<List<string>>> GetBrands()
    {
        var brands = await _db.Products
            .Select(p => p.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();

        return Ok(brands);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound(new { message = "A termék nem található." });
        return Ok(ToDto(product));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Brand = dto.Brand,
            Description = dto.Description,
            Price = dto.Price,
            Size = dto.Size,
            Color = dto.Color,
            Stock = dto.Stock,
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "/images/placeholder.png" : dto.ImageUrl,
            CreatedAt = DateTime.Now
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = product.Id }, ToDto(product));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, ProductCreateDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound(new { message = "A termék nem található." });

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Size = dto.Size;
        product.Color = dto.Color;
        product.Stock = dto.Stock;
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            product.ImageUrl = dto.ImageUrl;

        await _db.SaveChangesAsync();
        return Ok(ToDto(product));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound(new { message = "A termék nem található." });

        if (await _db.OrderItems.AnyAsync(i => i.ProductId == id))
            return BadRequest(new { message = "A termék nem törölhető, mert már szerepel megrendelésben." });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Kepfeltoltes az admin feluletrol a Frontend/images mappaba.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("upload")]
    public async Task<ActionResult<object>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "Nem érkezett fájl." });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string[] allowed = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowed.Contains(extension))
            return BadRequest(new { message = "Csak képfájl tölthető fel (jpg, png, webp, gif)." });

        var folder = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        await using (var stream = System.IO.File.Create(Path.Combine(folder, fileName)))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { imageUrl = $"/images/{fileName}" });
    }

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Brand = p.Brand,
        Description = p.Description,
        Price = p.Price,
        Size = p.Size,
        Color = p.Color,
        Stock = p.Stock,
        ImageUrl = p.ImageUrl
    };
}
