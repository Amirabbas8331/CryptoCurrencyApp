using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CryptoCurrency.Models;

public class Crypto
{
    [Key]
    public int InternalId { get; set; } // برای EF

    [JsonProperty("id")]
    public string Id { get; set; } // رشته شد

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("current_price")]
    public decimal CurrentPrice { get; set; }
}


