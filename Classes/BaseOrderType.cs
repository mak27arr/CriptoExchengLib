using System;
using System.Collections.Generic;
using System.Text;
using CriptoExchengLib.Interfaces;

namespace CriptoExchengLib.Classes
{
    public class BaseOrderType : IOrderType
    {
        private BaseOrderType(string value) { Value = value; }
        public string Value { get; set; }

        public static BaseOrderType Buy { get { return new BaseOrderType("buy"); } }
        public static BaseOrderType Sell { get { return new BaseOrderType("sell"); } }
        public static BaseOrderType Market_buy { get { return new BaseOrderType("market_buy"); } }
        public static BaseOrderType Market_sell { get { return new BaseOrderType("market_sell"); } }
        public static BaseOrderType Market_buy_total { get { return new BaseOrderType("market_buy_total"); } }
        public static BaseOrderType Market_sell_total { get { return new BaseOrderType("market_sell_total"); } }
        public static BaseOrderType SetValue(string value) {  
            return new BaseOrderType(value);
        }
        public override string ToString() {
            return Value;
        }
    }
}
