/* ==========================================================================
   products.js - a kezdőlap: cipők listázása, keresés, szűrés, kosárba tétel
   ========================================================================== */

document.addEventListener("DOMContentLoaded", () => {
    markakBetoltese();
    termekekBetoltese();

    document.getElementById("szuresGomb").onclick = termekekBetoltese;
    document.getElementById("torlesGomb").onclick = () => {
        document.getElementById("kereso").value = "";
        document.getElementById("markaSzuro").value = "";
        termekekBetoltese();
    };

    // Enter billentyűre is induljon a keresés
    document.getElementById("kereso").addEventListener("keyup", e => {
        if (e.key === "Enter") termekekBetoltese();
    });
    document.getElementById("markaSzuro").onchange = termekekBetoltese;
});

/** A márkaszűrő legördülő feltöltése az adatbázisban lévő márkákkal. */
async function markakBetoltese() {
    try {
        const markak = await apiKeres("/api/products/brands");
        const lista = document.getElementById("markaSzuro");
        markak.forEach(m => {
            const opcio = document.createElement("option");
            opcio.value = m;
            opcio.textContent = m;
            lista.appendChild(opcio);
        });
    } catch (hiba) {
        console.error(hiba);
    }
}

/** Termékek lekérése a szerverről és kártyákban megjelenítése. */
async function termekekBetoltese() {
    const lista = document.getElementById("termekLista");
    lista.innerHTML = "<p>Betöltés...</p>";

    const kereses = document.getElementById("kereso").value.trim();
    const marka = document.getElementById("markaSzuro").value;

    const parameterek = new URLSearchParams();
    if (kereses) parameterek.append("search", kereses);
    if (marka) parameterek.append("brand", marka);

    try {
        const termekek = await apiKeres("/api/products?" + parameterek.toString());

        if (termekek.length === 0) {
            lista.innerHTML = "<div class='ures'>Nincs a keresésnek megfelelő termék.</div>";
            return;
        }

        lista.innerHTML = termekek.map(termekKartya).join("");

        // Kosárba gombok eseménykezelője
        lista.querySelectorAll("[data-kosarba]").forEach(gomb => {
            gomb.onclick = () => {
                const termek = termekek.find(t => t.id === parseInt(gomb.dataset.kosarba, 10));
                kosarbaTesz(termek, 1);
                uzenetKiir(termek.name + " a kosárba került.", "siker");
            };
        });
    } catch (hiba) {
        lista.innerHTML = "<div class='ures'>Nem sikerült betölteni a termékeket: " + htmlVed(hiba.message) + "</div>";
    }
}

/** Egy termék HTML kártyája. */
function termekKartya(t) {
    const elfogyott = t.stock <= 0;

    return `
        <div class="kartya">
            <img src="${htmlVed(t.imageUrl)}" alt="${htmlVed(t.name)}"
                 onerror="this.src='images/placeholder.png'">
            <div class="kartya-torzs">
                <div class="kartya-marka">${htmlVed(t.brand)}</div>
                <h3>${htmlVed(t.name)}</h3>
                <p class="kartya-leiras">${htmlVed(t.description)}</p>
                <div class="kartya-adatok">
                    Méret: <strong>${t.size}</strong> &nbsp;|&nbsp;
                    Szín: <strong>${htmlVed(t.color)}</strong> &nbsp;|&nbsp;
                    Készlet: <strong>${t.stock} db</strong>
                </div>
                <div class="ar">${arFormaz(t.price)}</div>
                ${elfogyott
            ? '<div class="elfogyott">Jelenleg nincs készleten</div>'
            : `<button class="gomb" data-kosarba="${t.id}">Kosárba</button>`}
            </div>
        </div>`;
}
