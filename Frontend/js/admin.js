/* ==========================================================================
   admin.js - az adminisztrációs felület működése
   Termékek, megrendelések és felhasználók listázása, felvitele,
   módosítása és törlése (CRUD) a védett végpontokon keresztül.
   ========================================================================== */

document.addEventListener("DOMContentLoaded", () => {
    // Az oldal csak admin szerepkörrel nyitható meg
    if (!adminJogSzukseges()) return;

    fulekBeallitasa();
    modalisokBeallitasa();

    termekekBetoltese();
    rendelesekBetoltese();
    felhasznalokBetoltese();

    document.getElementById("ujTermekGomb").onclick = () => termekUrlapNyitas(null);
    document.getElementById("ujFelhasznaloGomb").onclick = () => felhasznaloUrlapNyitas(null);

    document.getElementById("termekUrlap").onsubmit = termekMentes;
    document.getElementById("felhasznaloUrlap").onsubmit = felhasznaloMentes;
});

/* ---------------------------- Fülek ---------------------------- */

function fulekBeallitasa() {
    document.querySelectorAll(".ful").forEach(ful => {
        ful.onclick = () => {
            document.querySelectorAll(".ful").forEach(f => f.classList.remove("aktiv"));
            document.querySelectorAll(".lap").forEach(l => l.classList.remove("aktiv"));
            ful.classList.add("aktiv");
            document.getElementById("lap-" + ful.dataset.ful).classList.add("aktiv");
        };
    });
}

function modalisokBeallitasa() {
    document.querySelectorAll("[data-bezar]").forEach(gomb => {
        gomb.onclick = () => document.getElementById(gomb.dataset.bezar).classList.remove("nyitva");
    });
}

/* ---------------------------- Termékek ---------------------------- */

let termekek = [];

async function termekekBetoltese() {
    const tabla = document.getElementById("termekTabla");
    try {
        termekek = await apiKeres("/api/products");

        tabla.innerHTML = termekek.map(t => `
            <tr>
                <td>${t.id}</td>
                <td><img class="mini" src="${htmlVed(t.imageUrl)}" alt=""
                         onerror="this.src='images/placeholder.png'"></td>
                <td>${htmlVed(t.name)}</td>
                <td>${htmlVed(t.brand)}</td>
                <td>${arFormaz(t.price)}</td>
                <td>${t.size}</td>
                <td>${htmlVed(t.color)}</td>
                <td>${t.stock} db</td>
                <td>
                    <button class="gomb gomb-masodlagos gomb-kicsi" data-szerkeszt="${t.id}">Szerkeszt</button>
                    <button class="gomb gomb-veszely gomb-kicsi" data-torol="${t.id}">Töröl</button>
                </td>
            </tr>`).join("");

        tabla.querySelectorAll("[data-szerkeszt]").forEach(g => {
            g.onclick = () => termekUrlapNyitas(termekek.find(t => t.id === parseInt(g.dataset.szerkeszt, 10)));
        });

        tabla.querySelectorAll("[data-torol]").forEach(g => {
            g.onclick = () => termekTorles(parseInt(g.dataset.torol, 10));
        });
    } catch (hiba) {
        uzenetKiir("Nem sikerült betölteni a termékeket: " + hiba.message, "hiba");
    }
}

/** A termék űrlap megnyitása: új felvitelhez (null) vagy szerkesztéshez. */
function termekUrlapNyitas(termek) {
    document.getElementById("termekModalCim").textContent = termek ? "Cipő szerkesztése" : "Új cipő";
    document.getElementById("termekId").value = termek ? termek.id : "";
    document.getElementById("tNev").value = termek ? termek.name : "";
    document.getElementById("tMarka").value = termek ? termek.brand : "";
    document.getElementById("tSzin").value = termek ? termek.color : "";
    document.getElementById("tAr").value = termek ? termek.price : "";
    document.getElementById("tMeret").value = termek ? termek.size : 42;
    document.getElementById("tKeszlet").value = termek ? termek.stock : 1;
    document.getElementById("tLeiras").value = termek ? termek.description : "";
    document.getElementById("tKepUrl").value = termek ? termek.imageUrl : "";
    document.getElementById("tKepFajl").value = "";

    document.getElementById("termekModal").classList.add("nyitva");
}

async function termekMentes(e) {
    e.preventDefault();
    uzenetRejt();

    const id = document.getElementById("termekId").value;
    let kepUrl = document.getElementById("tKepUrl").value.trim();

    try {
        // Ha választott képfájlt, előbb feltöltjük a szerverre
        const fajlMezo = document.getElementById("tKepFajl");
        if (fajlMezo.files.length > 0) {
            const adat = new FormData();
            adat.append("file", fajlMezo.files[0]);
            const feltoltes = await apiKeres("/api/products/upload", { method: "POST", body: adat });
            kepUrl = feltoltes.imageUrl;
        }

        const termek = {
            name: document.getElementById("tNev").value.trim(),
            brand: document.getElementById("tMarka").value.trim(),
            description: document.getElementById("tLeiras").value.trim(),
            price: parseFloat(document.getElementById("tAr").value),
            size: parseInt(document.getElementById("tMeret").value, 10),
            color: document.getElementById("tSzin").value.trim(),
            stock: parseInt(document.getElementById("tKeszlet").value, 10),
            imageUrl: kepUrl
        };

        if (id) {
            await apiKeres("/api/products/" + id, { method: "PUT", body: termek });
            uzenetKiir("A termék módosítása sikeres.", "siker");
        } else {
            await apiKeres("/api/products", { method: "POST", body: termek });
            uzenetKiir("Az új termék felvitele sikeres.", "siker");
        }

        document.getElementById("termekModal").classList.remove("nyitva");
        termekekBetoltese();
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}

async function termekTorles(id) {
    if (!confirm("Biztosan törlöd ezt a terméket?")) return;

    try {
        await apiKeres("/api/products/" + id, { method: "DELETE" });
        uzenetKiir("A termék törölve.", "siker");
        termekekBetoltese();
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}

/* ---------------------------- Megrendelések ---------------------------- */

const ALLAPOTOK = ["Új", "Feldolgozás alatt", "Kiszállítva", "Teljesítve", "Törölve"];

async function rendelesekBetoltese() {
    const tabla = document.getElementById("rendelesTabla");
    try {
        const rendelesek = await apiKeres("/api/orders");

        if (rendelesek.length === 0) {
            tabla.innerHTML = "<tr><td colspan='7'>Még nincs megrendelés.</td></tr>";
            return;
        }

        tabla.innerHTML = rendelesek.map(r => `
            <tr>
                <td>#${r.id}</td>
                <td>${htmlVed(r.userName)}</td>
                <td>${datumFormaz(r.orderDate)}</td>
                <td>${r.items.map(t => htmlVed(t.productName) + " &times; " + t.quantity).join("<br>")}</td>
                <td>${arFormaz(r.totalPrice)}</td>
                <td>
                    <select data-allapot="${r.id}">
                        ${ALLAPOTOK.map(a =>
            `<option value="${a}" ${a === r.status ? "selected" : ""}>${a}</option>`).join("")}
                    </select>
                </td>
                <td><button class="gomb gomb-veszely gomb-kicsi" data-rendelestorol="${r.id}">Töröl</button></td>
            </tr>`).join("");

        tabla.querySelectorAll("[data-allapot]").forEach(lista => {
            lista.onchange = () => allapotModositas(parseInt(lista.dataset.allapot, 10), lista.value);
        });

        tabla.querySelectorAll("[data-rendelestorol]").forEach(g => {
            g.onclick = () => rendelesTorles(parseInt(g.dataset.rendelestorol, 10));
        });
    } catch (hiba) {
        uzenetKiir("Nem sikerült betölteni a rendeléseket: " + hiba.message, "hiba");
    }
}

async function allapotModositas(id, allapot) {
    try {
        await apiKeres("/api/orders/" + id + "/status", { method: "PUT", body: { status: allapot } });
        uzenetKiir("A(z) #" + id + " rendelés állapota módosítva: " + allapot, "siker");
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}

async function rendelesTorles(id) {
    if (!confirm("Biztosan törlöd a #" + id + " rendelést?")) return;

    try {
        await apiKeres("/api/orders/" + id, { method: "DELETE" });
        uzenetKiir("A rendelés törölve.", "siker");
        rendelesekBetoltese();
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}

/* ---------------------------- Felhasználók ---------------------------- */

let felhasznalok = [];

async function felhasznalokBetoltese() {
    const tabla = document.getElementById("felhasznaloTabla");
    try {
        felhasznalok = await apiKeres("/api/users");

        tabla.innerHTML = felhasznalok.map(f => `
            <tr>
                <td>${f.id}</td>
                <td>${htmlVed(f.userName)}</td>
                <td>${htmlVed(f.email)}</td>
                <td>${htmlVed(f.fullName)}</td>
                <td>${htmlVed(f.phone)}</td>
                <td><span class="cimke ${f.role === "Admin" ? "admin" : ""}">${htmlVed(f.role)}</span></td>
                <td>${datumFormaz(f.createdAt)}</td>
                <td>
                    <button class="gomb gomb-masodlagos gomb-kicsi" data-fszerkeszt="${f.id}">Szerkeszt</button>
                    <button class="gomb gomb-veszely gomb-kicsi" data-ftorol="${f.id}">Töröl</button>
                </td>
            </tr>`).join("");

        tabla.querySelectorAll("[data-fszerkeszt]").forEach(g => {
            g.onclick = () => felhasznaloUrlapNyitas(felhasznalok.find(f => f.id === parseInt(g.dataset.fszerkeszt, 10)));
        });

        tabla.querySelectorAll("[data-ftorol]").forEach(g => {
            g.onclick = () => felhasznaloTorles(parseInt(g.dataset.ftorol, 10));
        });
    } catch (hiba) {
        uzenetKiir("Nem sikerült betölteni a felhasználókat: " + hiba.message, "hiba");
    }
}

function felhasznaloUrlapNyitas(felhasznalo) {
    document.getElementById("felhasznaloModalCim").textContent =
        felhasznalo ? "Felhasználó szerkesztése" : "Új felhasználó";
    document.getElementById("felhasznaloId").value = felhasznalo ? felhasznalo.id : "";
    document.getElementById("fNev").value = felhasznalo ? felhasznalo.userName : "";
    document.getElementById("fEmail").value = felhasznalo ? felhasznalo.email : "";
    document.getElementById("fTeljesNev").value = felhasznalo ? felhasznalo.fullName : "";
    document.getElementById("fTelefon").value = felhasznalo ? felhasznalo.phone : "";
    document.getElementById("fCim").value = felhasznalo ? felhasznalo.address : "";
    document.getElementById("fSzerep").value = felhasznalo ? felhasznalo.role : "User";
    document.getElementById("fJelszo").value = "";
    document.getElementById("fJelszo").required = !felhasznalo;
    document.getElementById("jelszoMegj").style.display = felhasznalo ? "" : "none";

    document.getElementById("felhasznaloModal").classList.add("nyitva");
}

async function felhasznaloMentes(e) {
    e.preventDefault();
    uzenetRejt();

    const id = document.getElementById("felhasznaloId").value;
    const jelszo = document.getElementById("fJelszo").value;

    const adatok = {
        userName: document.getElementById("fNev").value.trim(),
        email: document.getElementById("fEmail").value.trim(),
        fullName: document.getElementById("fTeljesNev").value.trim(),
        address: document.getElementById("fCim").value.trim(),
        phone: document.getElementById("fTelefon").value.trim(),
        role: document.getElementById("fSzerep").value,
        password: jelszo
    };

    try {
        if (id) {
            await apiKeres("/api/users/" + id, { method: "PUT", body: adatok });
            uzenetKiir("A felhasználó módosítása sikeres.", "siker");
        } else {
            await apiKeres("/api/users?role=" + encodeURIComponent(adatok.role),
                { method: "POST", body: adatok });
            uzenetKiir("Az új felhasználó létrehozása sikeres.", "siker");
        }

        document.getElementById("felhasznaloModal").classList.remove("nyitva");
        felhasznalokBetoltese();
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}

async function felhasznaloTorles(id) {
    if (!confirm("Biztosan törlöd ezt a felhasználót? A rendelései is törlődnek.")) return;

    try {
        await apiKeres("/api/users/" + id, { method: "DELETE" });
        uzenetKiir("A felhasználó törölve.", "siker");
        felhasznalokBetoltese();
    } catch (hiba) {
        uzenetKiir(hiba.message, "hiba");
    }
}
