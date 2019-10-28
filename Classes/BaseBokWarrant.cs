using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    public class BaseBookWarrant : IBookWarrant
    {
        public string Name { get; set; }
        public double Ask_quantity { get; set; }
        public double Ask_amount { get; set; }
        public double Ask_top { get; set; }
        public double Bid_quantity { get; set; }
        public double Bid_amount { get; set; }
        public double Bid_top { get; set; }
        public double[,] Ask { get; set; }
        public double[,] Bid { get; set; }
    }
}
