using System.ComponentModel.DataAnnotations;

namespace SneekerWebShop.Api.Models;

/// <summary>A kosarbol leadott megrendeles fejadatai.</summary>
public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public decimal TotalPrice { get; set; }

    /// <summary>Új / Feldolgozás alatt / Kiszállítva / Teljesítve</summary>
    [MaxLength(30)]
    public string Status { get; set; } = "Új";

    [MaxLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;

    public List<OrderItem> Items { get; set; } = new();
}
