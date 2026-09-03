-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1:3306
-- Létrehozás ideje: 2026. Sze 03. 14:19
-- Kiszolgáló verziója: 8.4.7
-- PHP verzió: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `sneekerwebshop`
--
CREATE DATABASE IF NOT EXISTS `sneekerwebshop` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `sneekerwebshop`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `orderitems`
--

DROP TABLE IF EXISTS `orderitems`;
CREATE TABLE IF NOT EXISTS `orderitems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OrderId` int NOT NULL,
  `ProductId` int NOT NULL,
  `Quantity` int NOT NULL,
  `UnitPrice` decimal(10,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrderItems_OrderId` (`OrderId`),
  KEY `IX_OrderItems_ProductId` (`ProductId`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- A tábla adatainak kiíratása `orderitems`
--

INSERT INTO `orderitems` (`Id`, `OrderId`, `ProductId`, `Quantity`, `UnitPrice`) VALUES
(1, 1, 2, 1, 67990.00),
(2, 1, 4, 2, 39990.00),
(3, 2, 2, 1, 67990.00),
(4, 2, 4, 2, 39990.00),
(5, 3, 4, 1, 39990.00),
(6, 3, 6, 1, 49990.00);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `orders`
--

DROP TABLE IF EXISTS `orders`;
CREATE TABLE IF NOT EXISTS `orders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `OrderDate` datetime(6) NOT NULL,
  `TotalPrice` decimal(10,2) NOT NULL,
  `Status` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ShippingAddress` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Orders_UserId` (`UserId`)
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- A tábla adatainak kiíratása `orders`
--

INSERT INTO `orders` (`Id`, `UserId`, `OrderDate`, `TotalPrice`, `Status`, `ShippingAddress`) VALUES
(1, 2, '2026-09-03 15:15:47.011695', 147970.00, 'Feldolgozás alatt', '3525 Miskolc, Széchenyi utca 12.'),
(2, 2, '2026-09-03 15:19:50.689180', 147970.00, 'Új', '3525 Miskolc, Széchenyi utca 12.'),
(3, 4, '2026-09-03 16:09:27.424748', 89980.00, 'Feldolgozás alatt', 'Bp. Széchenyi u.33.');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `products`
--

DROP TABLE IF EXISTS `products`;
CREATE TABLE IF NOT EXISTS `products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Brand` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Price` decimal(10,2) NOT NULL,
  `Size` int NOT NULL,
  `Color` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Stock` int NOT NULL,
  `ImageUrl` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- A tábla adatainak kiíratása `products`
--

INSERT INTO `products` (`Id`, `Name`, `Brand`, `Description`, `Price`, `Size`, `Color`, `Stock`, `ImageUrl`, `CreatedAt`) VALUES
(1, 'Nike Air Max 90', 'Nike', 'Klasszikus utcai sportcipő látható légpárnás talppal, mindennapi viseletre.', 54990.00, 42, 'Fekete', 12, '/images/nike-air-max-90.jpg', '2026-09-03 15:09:07.202681'),
(2, 'Adidas Ultra Boost 4', 'Adidas', 'Futócipő rugalmas Boost középtalppal és kötött felsőrésszel.', 67990.00, 43, 'Fekete', 6, '/images/adidas-ultraboost.jpg', '2026-09-03 15:09:07.202961'),
(3, 'Puma Suede Classic', 'Puma', 'Időtlen velúrbőrből készült utcai sneaker, a Puma legendás modellje.', 44990.00, 41, 'Fekete/Fehér', 15, '/images/puma-suede.jpg', '2026-09-03 15:09:07.202963'),
(4, 'New Balance 574', 'New Balance', 'Kényelmes, időtlen szabadidőcipő bőr és textil kombinációval.', 39990.00, 44, 'Szürke', 5, '/images/new-balance-574.jpg', '2026-09-03 15:09:07.202963'),
(5, 'Converse Chuck Taylor All Star', 'Converse', 'Legendás vászon tornácipő magas szárú kivitelben.', 27990.00, 40, 'Fekete', 20, '/images/converse-chuck-taylor.jpg', '2026-09-03 15:09:07.202964'),
(6, 'Nike Air Force 1', 'Nike', 'Ikonikus fehér bőr sneaker, bármilyen öltözethez illik.', 49990.00, 43, 'Fehér', 5, '/images/d63fba5bdcc14b7982f39794e4dbd000.jpg', '2026-09-03 15:16:53.687347');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FullName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Address` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Phone` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Role` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Users_Email` (`Email`),
  UNIQUE KEY `IX_Users_UserName` (`UserName`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`Id`, `UserName`, `Email`, `PasswordHash`, `FullName`, `Address`, `Phone`, `Role`, `CreatedAt`) VALUES
(1, 'admin', 'admin@gmail.com', 'AQAAAAIAAYagAAAAEI5EAStfhoaN59l48q0iZDSiMbAj7mcJEVOl3p0QAST1ECxRhkpqFbYPR/ZIczVssw==', 'Rendszer Adminisztrátor', '3525 Miskolc, Fő utca 1.', '+36301234567', 'Admin', '2026-09-03 15:09:06.916416'),
(2, 'kovacsanna', 'anna@example.com', 'AQAAAAIAAYagAAAAELlfkNy9u+aOzqcgMOXPeQqBTkJyMX5Z1m1/n+MxZvTDUQJOnevdKEwsDrZQjx5hUQ==', 'Kovács Anna', '3525 Miskolc, Széchenyi utca 12.', '+36 30 555 1234', 'User', '2026-09-03 15:15:20.553934'),
(4, 'peti01', 'peti01@gmail.com', 'AQAAAAIAAYagAAAAEJeJ2fMbsy0N0QkcamrCi6BSmcoHn1eDEU8Wh/+uj3QsgtuZPjDAnM0DLSUbwo7caw==', 'Kiss Péter', 'Bp. Széchenyi u.33.', '+367012345678', 'User', '2026-09-03 16:08:30.574723');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- A tábla adatainak kiíratása `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260903130644_InitialCreate', '9.0.11');
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
