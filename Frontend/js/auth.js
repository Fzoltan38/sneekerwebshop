/* ==========================================================================
   auth.js - bejelentkezés állapotának kezelése, menü és oldalvédelem
   A JWT token a localStorage-ban tárolódik, kilépéskor onnan törlődik.
   ========================================================================== */

/** A bejelentkezett felhasználó objektuma, vagy null. */
function aktualisFelhasznalo() {
    const mentett = localStorage.getItem("felhasznalo");
    return mentett ? JSON.parse(mentett) : null;
}

function beVanLepve() {
    return localStorage.getItem("token") !== null;
}

function adminE() {
    const f = aktualisFelhasznalo();
    return f !== null && f.role === "Admin";
}

/** Bejelentkezés után a token és a felhasználó mentése. */
function bejelentkezesMentes(eredmeny) {
    localStorage.setItem("token", eredmeny.token);
    localStorage.setItem("felhasznalo", JSON.stringify(eredmeny.user));
}

/** Kilépés: a token törlése a localStorage-ból. */
function kilepes() {
    localStorage.removeItem("token");
    localStorage.removeItem("felhasznalo");
    window.location.href = "index.html";
}

/**
 * A menü elemeinek megjelenítése a jogosultság szerint:
 *   data-lathato="vendeg" - csak kijelentkezett állapotban
 *   data-lathato="tag"    - csak bejelentkezve
 *   data-lathato="admin"  - csak admin szerepkörrel
 */
function menuFrissites() {
    const felhasznalo = aktualisFelhasznalo();

    document.querySelectorAll("[data-lathato]").forEach(elem => {
        const kell = elem.getAttribute("data-lathato");
        let latszik = false;

        if (kell === "vendeg") latszik = !beVanLepve();
        else if (kell === "tag") latszik = beVanLepve();
        else if (kell === "admin") latszik = adminE();

        elem.style.display = latszik ? "" : "none";
    });

    const cimke = document.getElementById("felhasznaloNev");
    if (cimke && felhasznalo) {
        cimke.textContent = felhasznalo.userName + (felhasznalo.role === "Admin" ? " (admin)" : "");
    }

    const kilepGomb = document.getElementById("kilepGomb");
    if (kilepGomb) {
        kilepGomb.onclick = function (e) {
            e.preventDefault();
            kilepes();
        };
    }

    // Mobil menü nyitása/zárása
    const menuGomb = document.getElementById("menuGomb");
    const menu = document.getElementById("fomenu");
    if (menuGomb && menu) {
        menuGomb.onclick = () => menu.classList.toggle("nyitva");
    }

    kosarSzamlaloFrissites();
}

/** Oldalvédelem: csak bejelentkezve érhető el. */
function belepesSzukseges() {
    if (!beVanLepve()) {
        window.location.href = "login.html";
        return false;
    }
    return true;
}

/** Oldalvédelem: csak admin szerepkörrel érhető el. */
function adminJogSzukseges() {
    if (!beVanLepve()) {
        window.location.href = "login.html";
        return false;
    }
    if (!adminE()) {
        alert("Ehhez az oldalhoz adminisztrátori jogosultság szükséges.");
        window.location.href = "index.html";
        return false;
    }
    return true;
}

document.addEventListener("DOMContentLoaded", menuFrissites);
