using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    public class BaseAccount : IAccount
    {
        public BaseAccount(string name,double summ)
        {
            this.Name = name;
            this.Summ = summ;
        }
        public string Name { get; set; }
        public double Summ { get; set; }
    }
}
