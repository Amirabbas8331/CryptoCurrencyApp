using CryptoCurrency.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace CryptoCurrency.Controllers
{
    [Route("api/[controller]")]

    public class ExchangeController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoContext _context;

        public ExchangeController(HttpClient httpClient, CryptoContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        // دریافت نرخ دلار و ذخیره آن در پایگاه داده
        [HttpGet("usd-rate")]
        public async Task<IActionResult> GetUsdRate()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://api.nobitex.ir/market/stats");
                var json = JObject.Parse(response);
                var rate = json["stats"]?["usdt-rls"]?["latest"]?.Value<decimal>() ?? 60000;

                // ذخیره نرخ دلار در پایگاه داده
                var dollarRate = new DollarRate
                {
                    Rate = rate,
                    DateTime = DateTime.Now
                };

                _context.DollarRates.Add(dollarRate);
                await _context.SaveChangesAsync();

                return Ok(new { usdRate = rate });
            }
            catch
            {
                return Ok(new { usdRate = 60000 });
            }
        }

        // دریافت داده‌های رمز ارزها و ذخیره آن‌ها در پایگاه داده
        [HttpGet("top-cryptos")]
        public async Task<IActionResult> GetTopCryptos()
        {
            const string cacheKey = "topCryptos";

            // چک کردن پایگاه داده برای داده‌های رمز ارز
            var cryptos = await _context.Cryptos.ToListAsync();
            if (cryptos.Count > 0)
            {
                return Ok(cryptos);
            }

            // درخواست از API برای رمز ارزها

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");


            var url = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=100&page=1&sparkline=false";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { error = "Error fetching crypto prices", message = response.ReasonPhrase });

                var json = await response.Content.ReadAsStringAsync();
                var cryptosData = JsonConvert.DeserializeObject<List<Crypto>>(json);

                // ذخیره داده‌های رمز ارزها در پایگاه داده
                _context.Cryptos.AddRange(cryptosData);
                await _context.SaveChangesAsync();

                return Ok(cryptosData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "JSON parsing error", message = ex.Message });
            }
        }

    }
}