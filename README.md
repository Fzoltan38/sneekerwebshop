# SneekerWebShop

Sportcipő webáruház admin felülettel. Szoftverfejlesztő tanfolyam vizsgaremek projekt.

| Réteg | Technológia |
|---|---|
| Adatbázis | MySQL 8 + Entity Framework Core (Code First migráció) |
| Backend | ASP.NET Core Web API (net10.0), JWT Bearer authentikáció, LINQ |
| Frontend | HTML5, CSS3, vanilla JavaScript, `fetch()` – keretrendszer nélkül |

## Előfeltételek

- .NET SDK 10 vagy újabb
- Futó MySQL-kiszolgáló (`localhost:3306`, `root` felhasználó, üres jelszó – XAMPP/WAMP is jó)

A kapcsolati sztring a `Backend/SneekerWebShop.Api/appsettings.json` fájlban módosítható.

## Indítás

```bash
cd Backend/SneekerWebShop.Api && dotnet run
```

Az alkalmazás indításkor létrehozza és feltölti az adatbázist, majd egyetlen címen kiszolgálja
a backendet és a frontendet is:

- Webáruház: <http://localhost:5000>
- API dokumentáció (Swagger): <http://localhost:5000/swagger>

## Belépési adatok

| Szerepkör | E-mail | Jelszó |
|---|---|---|
| Admin | `admin@gmail.com` | `admin` |

Vásárlói fiókot a Regisztráció menüpontban lehet létrehozni; a nyilvános regisztrációval
létrejövő fiókok mindig `User` szerepkört kapnak.

## Jogosultsági körök

- **Nem regisztrált látogató** – a cipők böngészése, keresés, márkaszűrés
- **User** – bejelentkezés, kosár, megrendelés leadása, saját rendelések megtekintése
- **Admin** – teljes CRUD a termékeken, megrendeléseken és felhasználókon, képfeltöltés

## Mappaszerkezet

```
Backend/SneekerWebShop.Api/   ASP.NET Core Web API
├─ Controllers/               Auth, Products, Orders, Users
├─ Data/                      AppDbContext, DbInitializer (kezdő adatok)
├─ Dtos/                      adatátviteli objektumok
├─ Migrations/                EF Core migrációk
├─ Models/                    User, Product, Order, OrderItem
└─ Services/                  TokenService (JWT generálás)

Frontend/                     statikus weblapok (a backend szolgálja ki)
├─ css/style.css
├─ images/                    termékképek és a feltöltött képek
├─ js/                        api.js, auth.js, cart.js, products.js, admin.js
├─ index.html                 kezdőlap – böngészés bejelentkezés nélkül
├─ login.html / register.html belépés és regisztráció
├─ cart.html                  kosár és megrendelés
├─ orders.html                saját rendelések
└─ admin.html                 adminisztrációs felület

Documents/
├─ sneeker_dokumentacio.docx  a projekt dokumentációja (30 oldal)
├─ sneeker_test.docx          teszt dokumentáció (37 teszteset)
└─ screenshots/               a dokumentáció képei
```

## Hasznos parancsok

```bash
# Az adatbázis kézi létrehozása / frissítése
cd Backend/SneekerWebShop.Api && dotnet ef database update

# Alaphelyzetbe állítás: az adatbázis eldobása után az indítás újra létrehozza
mysql -u root -e "drop database sneekerwebshop;"
```

## Megjegyzések a megvalósításról

- Az Identity csomagot **kizárólag** a jelszóhasheléshez (`PasswordHasher`) használjuk;
  nincs `IdentityDbContext`, és az Identity saját táblái nincsenek migrálva az adatbázisba.
  A szerepkört a saját `Users` tábla `Role` mezője tárolja.
- A JWT token bejelentkezés után a `localStorage`-ba kerül, kilépéskor onnan törlődik.
- Online fizetés nincs: a kosár tartalmából objektum készül, amely az `Orders` és
  `OrderItems` táblákba mentődik. A végösszeget és a készletet mindig a szerver számolja,
  az árat az adatbázisból olvassa (nem a kliens által küldött értékből).
- A termékek nem törölhetők, ha már szerepelnek megrendelésben.
