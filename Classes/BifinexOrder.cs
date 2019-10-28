using CriptoExchengLib.Interfaces;
using Newtonsoft.Json;

namespace CriptoExchengLib.Classes
{
    class BifinexOrder : IOrder
    {
        [JsonProperty("quantity")]
        public int Id { get; set; }
        [JsonProperty("quantity")]
        public ICurrencyPair Pair { get; set; }
        [JsonProperty("quantity")]
        public double Quantity { get; set; }
        [JsonProperty("quantity")]
        public double Price { get; set; }
        [JsonProperty("quantity")]
        public IOrderType Type { get; set; }
        [JsonProperty("quantity")]
        public double Amount { get; set; }
    }
}
