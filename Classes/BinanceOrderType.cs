using System;
using System.Collections.Generic;
using System.Text;
using CriptoExchengLib.Interfaces;

namespace CriptoExchengLib.Classes
{

    class BinanceOrderType : IOrderType
    {
        public string Value { get; set; }

        private BinanceOrderType(string value) { Value = value; }
        public static BinanceOrderType Buy { get { return new BinanceOrderType("BUY"); } }
        public static BinanceOrderType Sell { get { return new BinanceOrderType("SELL"); } }
        public static BinanceOrderType Market { get { return new BinanceOrderType("MARKET"); } }
        public static BinanceOrderType Limit { get { return new BinanceOrderType("LIMIT"); } }
        public static BinanceOrderType Stop_loss { get { return new BinanceOrderType("STOP_LOSS"); } }
        public static BinanceOrderType Stop_loss_limit { get { return new BinanceOrderType("STOP_LOSS_LIMIT"); } }
        public static BinanceOrderType Take_profit { get { return new BinanceOrderType("TAKE_PROFIT"); } }
        public static BinanceOrderType Take_profit_limit { get { return new BinanceOrderType("TAKE_PROFIT_LIMIT"); } }
        public static BinanceOrderType Limit_market { get { return new BinanceOrderType("LIMIT_MAKER"); } }
        public static BinanceOrderType Empty { get { return new BinanceOrderType(""); } }
        public static BinanceOrderType SetValue(string value)
        {
            return new BinanceOrderType(value);
        }
        public override string ToString()
        {
            return Value;
        }
    }
}
