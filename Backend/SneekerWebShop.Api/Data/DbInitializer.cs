using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SneekerWebShop.Api.Models;

namespace SneekerWebShop.Api.Data;

/// <summary>
/// Inditaskor letrehozza az adatbazist (migracio) es feltolti a kezdo adatokkal:
/// egy admin felhasznaloval es ot sportcipovel.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.Migrate();

        var hasher = new PasswordHasher<User>();

        if (!db.Users.Any())
        {
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FullName = "Rendszer Adminisztrátor",
                Address = "3525 Miskolc, Fő utca 1.",
                Phone = "+36301234567",
                Role = "Admin",
                CreatedAt = DateTime.Now
            };
            admin.PasswordHash = hasher.HashPassword(admin, "admin");
            db.Users.Add(admin);
            db.SaveChanges();
        }

        if (!db.Products.Any())
        {
            db.Products.AddRange(
                new Product
                {
                    Name = "Nike Air Max 90",
                    Brand = "Nike",
                    Description = "Klasszikus utcai sportcipő látható légpárnás talppal, mindennapi viseletre.",
                    Price = 54990m,
                    Size = 42,
                    Color = "Fekete",
                    Stock = 12,
                    ImageUrl = "/images/nike-air-max-90.jpg"
                },
                new Product
                {
                    Name = "Adidas Ultra Boost 4",
                    Brand = "Adidas",
                    Description = "Futócipő rugalmas Boost középtalppal és kötött felsőrésszel.",
                    Price = 67990m,
                    Size = 43,
                    Color = "Fekete",
                    Stock = 8,
                    ImageUrl = "/images/adidas-ultraboost.jpg"
                },
                new Product
                {
                    Name = "Puma Suede Classic",
                    Brand = "Puma",
                    Description = "Időtlen velúrbőrből készült utcai sneaker, a Puma legendás modellje.",
                    Price = 44990m,
                    Size = 41,
                    Color = "Fekete/Fehér",
                    Stock = 15,
                    ImageUrl = "/images/puma-suede.jpg"
                },
                new Product
                {
                    Name = "New Balance 574",
                    Brand = "New Balance",
                    Description = "Kényelmes, időtlen szabadidőcipő bőr és textil kombinációval.",
                    Price = 39990m,
                    Size = 44,
                    Color = "Szürke",
                    Stock = 10,
                    ImageUrl = "/images/new-balance-574.jpg"
                },
                new Product
                {
                    Name = "Converse Chuck Taylor All Star",
                    Brand = "Converse",
                    Description = "Legendás vászon tornácipő magas szárú kivitelben.",
                    Price = 27990m,
                    Size = 40,
                    Color = "Fekete",
                    Stock = 20,
                    ImageUrl = "/images/converse-chuck-taylor.jpg"
                });
            db.SaveChanges();
        }
    }
}
