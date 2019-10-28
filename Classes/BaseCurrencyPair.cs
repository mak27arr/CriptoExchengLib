using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    public class BaseCurrencyPair : ICurrencyPair
    {
        public BaseCurrencyPair(string pair_name)
        {
            this.PairName = pair_name;
        }
        public string PairName { get; set; }

        public override string ToString()
        {
            return PairName;
        }
    }
}
