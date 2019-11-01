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
        public static BaseOrderStatus Opened { get { return new BaseOrderStatus("Opened"); } }
        public static BaseOrderStatus Closed { get { return new BaseOrderStatus("Closed"); } }
        public static BaseOrderStatus Canceled { get { return new BaseOrderStatus("Canceled"); } }
        public static BaseOrderStatus Expired { get { return new BaseOrderStatus("Expired"); } }
        public static BaseOrderStatus Pending { get { return new BaseOrderStatus("Pending"); } }
        public override string ToString()
        {
            return Value;
        }
    }
}
