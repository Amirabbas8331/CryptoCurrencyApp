let cryptoData = [];
let dollarRate = 60000;

async function fetchDollarRate() {
    try {
        const res = await fetch('/api/exchange/usd-rate');
        const data = await res.json();
        dollarRate = data.usdRate / 10 || 60000;
        document.getElementById('dollar-rate').innerText = `نرخ دلار: ${dollarRate.toLocaleString('fa-IR')} تومان`;
    } catch {
        document.getElementById('dollar-rate').innerText = 'خطا در دریافت نرخ دلار';
    }
}


async function fetchCryptos() {
    const container = document.getElementById('crypto-prices');
    container.textContent = 'در حال دریافت اطلاعات...';

    try {
        const res = await fetch('/api/exchange/top-cryptos');
        cryptoData = await res.json();
        displayCryptos(cryptoData);
    } catch (err) {
        container.innerHTML = `<p style="color:red;">خطا در دریافت اطلاعات</p>`;
    }
}

function displayCryptos(data) {
    const container = document.getElementById('crypto-prices');
    container.innerHTML = '';

    data.forEach((crypto, index) => {
        const priceInToman = (crypto.currentPrice * dollarRate).toLocaleString('fa-IR');
        const el = document.createElement('div');
        el.className = 'crypto-card p-3';
        el.innerHTML = `
                        <div class="crypto-name">${index + 1}. ${crypto.name}</div>
                        <div class="crypto-symbol">(${crypto.symbol.toUpperCase()})</div>
                        <div class="crypto-price">
                            $${crypto.currentPrice.toLocaleString('en-US')}<br>
                            ${priceInToman} تومان
                        </div>
                    `;
        container.appendChild(el);
    });
}

document.getElementById('search').addEventListener('input', function (e) {
    const term = e.target.value.toLowerCase();
    const filtered = cryptoData.filter(c =>
        c.name.toLowerCase().includes(term) || c.symbol.toLowerCase().includes(term)
    );
    displayCryptos(filtered);
});
document.getElementById('refresh-btn').addEventListener('click', async () => {
    await fetchDollarRate();
    await fetchCryptos();
});
(async () => {
    await fetchDollarRate();
    await fetchCryptos();
})();