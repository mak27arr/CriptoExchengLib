using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    class BinanceOrderStatus : IOrderStatus
    {
        private BinanceOrderStatus(string value) { this.Value = value; }
        public string Value { get; set; }

        public static BinanceOrderStatus Exsist { get { return new BinanceOrderStatus("Exsist"); } }
        public static BinanceOrderStatus Error { get { return new BinanceOrderStatus("Error"); } }
        public static BinanceOrderStatus NoExsist { get { return new BinanceOrderStatus("NoExsist"); } }
        public static BinanceOrderStatus New { get { return new BinanceOrderStatus("NEW"); } }
        public static BinanceOrderStatus Partially_Filled { get { return new BinanceOrderStatus("PARTIALLY_FILLED"); } }
        public static BinanceOrderStatus Filled { get { return new BinanceOrderStatus("FILLED"); } }
        public static BinanceOrderStatus Canceled { get { return new BinanceOrderStatus("CANCELED"); } }
        public static BinanceOrderStatus Pending_Cancel { get { return new BinanceOrderStatus("PENDING_CANCEL"); } }
        public static BinanceOrderStatus Rejected { get { return new BinanceOrderStatus("REJECTED"); } }
        public static BinanceOrderStatus Expired { get { return new BinanceOrderStatus("EXPIRED"); } }
        public static BinanceOrderStatus SetValue(string value) { return new BinanceOrderStatus(value); }
        public override string ToString()
        {
            return Value;
        }
    }
}
