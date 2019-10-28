using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    class BaseOrderStatus : IOrderStatus
    {
        private BaseOrderStatus(string value) { Value = value; }
        public string Value { get; set; }

        public static BaseOrderStatus Exsist { get { return new BaseOrderStatus("Exsist"); } }
        public static BaseOrderStatus NoExsist { get { return new BaseOrderStatus("NoExsist"); } }
        public static BaseOrderStatus Error { get { return new BaseOrderStatus("Error"); } }
        public override string ToString()
        {
            return Value;
        }
    }
}
