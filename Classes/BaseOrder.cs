using CriptoExchengLib.Classes.JsonDecorator;
using CriptoExchengLib.Interfaces;
using Newtonsoft.Json;

namespace CriptoExchengLib.Classes
{
    public class BaseOrder : IOrder
    {
        [JsonConverter(typeof(CurrencyPairConverter))]
        [JsonProperty("pair")]
        public ICurrencyPair Pair { get; set; }
        [JsonProperty("quantity")]
        public double Quantity { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        [JsonConverter(typeof(OrderTypeConverter))]
        [JsonProperty("type")]
        public IOrderType Type { get; set; }
        [JsonProperty("order_id")]
        public int Id { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
