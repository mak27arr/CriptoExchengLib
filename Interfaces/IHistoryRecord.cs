using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Interfaces
{
    public interface IHistoryRecord
    {
        int Id { get; set; }
        DateTime Time { get; set; }
        string Type { get; set; }
        string Currency { get; set; }
        string Status { get; set; }
        string Provaider { get; set; }
        string Amount { get; set; }
        string Account { get; set; }
        string Txit { get; set; }
    }
}
