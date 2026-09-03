namespace SneekerWebShop.Api.Models;

/// <summary>A megrendeles egy tetele (melyik cipobol mennyit).</summary>
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    /// <summary>A rendeles pillanataban ervenyes egysegar.</summary>
    public decimal UnitPrice { get; set; }
}
