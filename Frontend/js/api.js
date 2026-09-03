/* ==========================================================================
   api.js - a szerverrel való kommunikáció (fetch alapú AJAX kérések)
   ========================================================================== */

// A backend címe. Ha a frontendet a backend szolgálja ki, elég az üres előtag.
const API_BASE = "";

/**
 * Egységes fetch hívás: JSON küldés/fogadás és a JWT token automatikus csatolása.
 * @param {string} utvonal pl. "/api/products"
 * @param {object} beallitasok { method, body, ... }
 */
async function apiKeres(utvonal, beallitasok = {}) {
    const fejlecek = { ...(beallitasok.headers || {}) };

    // A bejelentkezéskor kapott tokent minden kéréshez hozzátesszük
    const token = localStorage.getItem("token");
    if (token) {
        fejlecek["Authorization"] = "Bearer " + token;
    }

    // Ha van törzs és nem fájlfeltöltés, JSON-ná alakítjuk
    let torzs = beallitasok.body;
    if (torzs && !(torzs instanceof FormData)) {
        fejlecek["Content-Type"] = "application/json";
        torzs = JSON.stringify(torzs);
    }

    const valasz = await fetch(API_BASE + utvonal, {
        method: beallitasok.method || "GET",
        headers: fejlecek,
        body: torzs
    });

    // Lejárt vagy hibás token: kiléptetjük a felhasználót
    if (valasz.status === 401 && token) {
        localStorage.removeItem("token");
        localStorage.removeItem("felhasznalo");
    }

    if (valasz.status === 204) {
        return null;
    }

    const szoveg = await valasz.text();
    const adat = szoveg ? JSON.parse(szoveg) : null;

    if (!valasz.ok) {
        const uzenet = (adat && (adat.message || adat.title)) || "Hiba történt (" + valasz.status + ").";
        throw new Error(uzenet);
    }

    return adat;
}

/** Forint formátumú árkiírás, pl. 54 990 Ft */
function arFormaz(ertek) {
    return new Intl.NumberFormat("hu-HU").format(ertek) + " Ft";
}

/** Dátum formázása, pl. 2026. 09. 03. 14:25 */
function datumFormaz(iso) {
    const d = new Date(iso);
    return d.toLocaleString("hu-HU", {
        year: "numeric", month: "2-digit", day: "2-digit",
        hour: "2-digit", minute: "2-digit"
    });
}

/** Üzenet megjelenítése az oldal tetején lévő #uzenet elemben. */
function uzenetKiir(szoveg, tipus = "siker") {
    const doboz = document.getElementById("uzenet");
    if (!doboz) {
        alert(szoveg);
        return;
    }
    doboz.textContent = szoveg;
    doboz.className = "uzenet " + tipus;
    window.scrollTo({ top: 0, behavior: "smooth" });
}

/** Üzenetdoboz elrejtése. */
function uzenetRejt() {
    const doboz = document.getElementById("uzenet");
    if (doboz) {
        doboz.className = "uzenet rejtett";
    }
}

/** HTML-be illesztés előtti egyszerű escape-elés. */
function htmlVed(szoveg) {
    const div = document.createElement("div");
    div.textContent = szoveg === null || szoveg === undefined ? "" : szoveg;
    return div.innerHTML;
}
