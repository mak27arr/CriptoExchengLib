using System;
using System.Collections.Generic;
using System.Text;
using CriptoExchengLib.Interfaces;

namespace CriptoExchengLib.Classes
{
    class KrakenOrderType : IOrderType
    {
        public string Value { get; set; }

        private KrakenOrderType(string value) { Value = value; }
        public static KrakenOrderType Buy { get { return new KrakenOrderType("buy"); } }
        public static KrakenOrderType Sell { get { return new KrakenOrderType("sell"); } }
        public static KrakenOrderType Market { get { return new KrakenOrderType("market"); } }
        public static KrakenOrderType Limit { get { return new KrakenOrderType("limit"); } }
        public static KrakenOrderType Stop_loss { get { return new KrakenOrderType("stop-loss"); } }
        public static KrakenOrderType Take_profit { get { return new KrakenOrderType("take-profit"); } }
        public static KrakenOrderType Stop_loss_profit { get { return new KrakenOrderType("stop-loss-profit"); } }
        public static KrakenOrderType Stop_loss_profit_limit { get { return new KrakenOrderType("stop-loss-profit-limit"); } }
        public static KrakenOrderType Stop_loss_limit { get { return new KrakenOrderType("stop-loss-limit"); } }
        public static KrakenOrderType Take_profit_limit { get { return new KrakenOrderType("take-profit-limit"); } }
        public static KrakenOrderType Trailing_stop { get { return new KrakenOrderType("trailing-stop"); } }
        public static KrakenOrderType Trailing_stop_limit { get { return new KrakenOrderType("trailing-stop-limit"); } }
        public static KrakenOrderType Stop_loss_and_limit { get { return new KrakenOrderType("stop-loss-and-limit"); } }
        public static KrakenOrderType Settle_position { get { return new KrakenOrderType("settle-position"); } }
        public static KrakenOrderType Empty { get { return new KrakenOrderType(""); } }
        public static KrakenOrderType SetValue(string value)
        {
            return new KrakenOrderType(value);
        }
        public override string ToString()
        {
            return Value;
        }
    }
}
