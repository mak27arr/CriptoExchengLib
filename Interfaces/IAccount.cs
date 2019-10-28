using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Interfaces
{
    public interface IAccount
    {
        string Name { get; set; }
        double Summ { get; set; }
    }
}
