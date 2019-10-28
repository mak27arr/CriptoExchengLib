using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Interfaces
{
    public interface IOrder
    {
        int Id { get; set; }
        ICurrencyPair Pair { get; set; }
        double Quantity { get; set; }
        double Price { get; set; }
        IOrderType Type { get; set; }
        double Amount { get; set; }
    }
}
