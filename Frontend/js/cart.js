/* ==========================================================================
   cart.js - a kosár kezelése a böngésző localStorage-ában
   Megrendeléskor a kosárból egy objektum készül, amit a szerver adatbázisba ment.
   ========================================================================== */

/** A kosár tartalma tömbként: [{ id, name, price, imageUrl, quantity }] */
function kosarBetolt() {
    const mentett = localStorage.getItem("kosar");
    return mentett ? JSON.parse(mentett) : [];
}

function kosarMentes(kosar) {
    localStorage.setItem("kosar", JSON.stringify(kosar));
    kosarSzamlaloFrissites();
}

function kosarUrites() {
    localStorage.removeItem("kosar");
    kosarSzamlaloFrissites();
}

/** Termék kosárba tétele (ha már benne van, növeli a darabszámot). */
function kosarbaTesz(termek, darab = 1) {
    const kosar = kosarBetolt();
    const meglevo = kosar.find(t => t.id === termek.id);

    if (meglevo) {
        meglevo.quantity += darab;
    } else {
        kosar.push({
            id: termek.id,
            name: termek.name,
            price: termek.price,
            imageUrl: termek.imageUrl,
            quantity: darab
        });
    }

    kosarMentes(kosar);
}

function kosarbolTorol(termekId) {
    kosarMentes(kosarBetolt().filter(t => t.id !== termekId));
}

function kosarDarabModositas(termekId, darab) {
    const kosar = kosarBetolt();
    const tetel = kosar.find(t => t.id === termekId);
    if (tetel) {
        tetel.quantity = Math.max(1, parseInt(darab, 10) || 1);
        kosarMentes(kosar);
    }
}

/** A kosárban lévő darabszámok összege. */
function kosarDarabszam() {
    return kosarBetolt().reduce((osszeg, t) => osszeg + t.quantity, 0);
}

function kosarVegosszeg() {
    return kosarBetolt().reduce((osszeg, t) => osszeg + t.price * t.quantity, 0);
}

/** A menüben látható kosár-számláló frissítése. */
function kosarSzamlaloFrissites() {
    const elem = document.getElementById("kosarSzam");
    if (elem) {
        elem.textContent = kosarDarabszam();
    }
}
