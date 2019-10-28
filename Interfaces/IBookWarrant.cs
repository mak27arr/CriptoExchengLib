using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Interfaces
{
    public interface IBookWarrant
    {
        string Name { get; set; }
        double Ask_quantity { get; set; }
        double Ask_amount { get; set; }
        double Ask_top { get; set; }
        double Bid_quantity { get; set; }
        double Bid_amount { get; set; }
        double Bid_top { get; set; }
        double [,] Ask { get; set; }
        double [,] Bid { get; set; }
    }
}
