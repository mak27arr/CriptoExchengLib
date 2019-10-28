using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    public class BaseHistoryRecord : IHistoryRecord
    {
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Provaider { get; set; }
        public string Amount { get; set; }
        public string Account { get; set; }
        public string Txit { get; set; }
    }
}
