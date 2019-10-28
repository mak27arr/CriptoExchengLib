using System;
using System.Collections.Generic;
using System.Text;
using CriptoExchengLib.Interfaces;

namespace CriptoExchengLib.Classes
{
    class BifinexOrderType : IOrderType
    {
        public string Value { get; set; }

        private BifinexOrderType(string value) { Value = value; }

        public static BifinexOrderType Exchange_limit { get { return new BifinexOrderType("EXCHANGE LIMIT"); } }
        public static BifinexOrderType Limit { get { return new BifinexOrderType("LIMIT"); } }
        public static BifinexOrderType Market { get { return new BifinexOrderType("MARKET"); } }
        public static BifinexOrderType Stop { get { return new BifinexOrderType("STOP"); } }
        public static BifinexOrderType Trailing_stop { get { return new BifinexOrderType("TRAILING STOP"); } }
        public static BifinexOrderType Excheng_market { get { return new BifinexOrderType("EXCHANGE MARKET"); } }
        public static BifinexOrderType Excheng_limit { get { return new BifinexOrderType("EXCHANGE LIMIT"); } }
        public static BifinexOrderType Excheng_stop { get { return new BifinexOrderType("EXCHANGE STOP"); } }
        public static BifinexOrderType Excheng_trailing_stop { get { return new BifinexOrderType("EXCHANGE TRAILING STOP"); } }
        public static BifinexOrderType Fok { get { return new BifinexOrderType("FOK"); } }
        public static BifinexOrderType Exchange_fok { get { return new BifinexOrderType("EXCHANGE FOK"); } }
        public static BifinexOrderType Ioc { get { return new BifinexOrderType("IOC"); } }
        public static BifinexOrderType Exchange_ioc { get { return new BifinexOrderType("EXCHANGE IOC"); } }
        public static BifinexOrderType SetValue(string value)
        {
            return new BifinexOrderType(value);
        }
        public override string ToString()
        {
            return Value;
        }
    }
}
