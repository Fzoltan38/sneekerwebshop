using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneekerWebShop.Api.Data;
using SneekerWebShop.Api.Dtos;
using SneekerWebShop.Api.Models;

namespace SneekerWebShop.Api.Controllers;

/// <summary>
/// Megrendelesek. A vasarlas csak bejelentkezve lehetseges, a sajat rendeleseit
/// mindenki lathatja, az osszes rendelest csak az admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>A kosar tartalmanak mentese megrendeleskent az adatbazisba.</summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(OrderCreateDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest(new { message = "A kosár üres." });

        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null) return Unauthorized();

        var order = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.Now,
            Status = "Új",
            ShippingAddress = string.IsNullOrWhiteSpace(dto.ShippingAddress) ? user.Address : dto.ShippingAddress
        };

        decimal total = 0m;

        foreach (var item in dto.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest(new { message = $"A(z) {item.ProductId} azonosítójú termék nem található." });

            if (item.Quantity < 1)
                return BadRequest(new { message = "A mennyiség legalább 1 legyen." });

            if (product.Stock < item.Quantity)
                return BadRequest(new { message = $"Nincs elég készlet: {product.Name} (raktáron: {product.Stock} db)." });

            // Az arat mindig az adatbazisbol vesszuk, nem a kliens altal kuldott ertekbol
            product.Stock -= item.Quantity;
            total += product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalPrice = total;

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(await GetOrderDto(order.Id));
    }

    /// <summary>A bejelentkezett felhasznalo sajat rendelesei.</summary>
    [HttpGet("my")]
    public async Task<ActionResult<List<OrderDto>>> MyOrders()
    {
        var id = CurrentUserId;
        var orders = await OrdersQuery()
            .Where(o => o.UserId == id)
            .ToListAsync();

        return Ok(orders);
    }

    /// <summary>Az osszes rendeles (csak admin).</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        return Ok(await OrdersQuery().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> Get(int id)
    {
        var order = await GetOrderDto(id);
        if (order == null) return NotFound(new { message = "A rendelés nem található." });

        // Sajat rendeleset barki, masokét csak az admin nezheti meg
        if (order.UserId != CurrentUserId && !User.IsInRole("Admin"))
            return Forbid();

        return Ok(order);
    }

    /// <summary>Rendeles statuszanak modositasa (csak admin).</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(int id, OrderStatusDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound(new { message = "A rendelés nem található." });

        order.Status = dto.Status;
        await _db.SaveChangesAsync();

        return Ok(await GetOrderDto(id));
    }

    /// <summary>Rendeles torlese (csak admin). A tetelek is torlodnek.</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound(new { message = "A rendelés nem található." });

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Kozos LINQ lekerdezes: rendelesek tetelekkel es felhasznalonevvel.</summary>
    private IQueryable<OrderDto> OrdersQuery() =>
        _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = o.User!.UserName,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });

    private async Task<OrderDto?> GetOrderDto(int id) =>
        await OrdersQuery().FirstOrDefaultAsync(o => o.Id == id);
}
