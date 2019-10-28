using CriptoExchengLib.Classes;
using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Interfaces
{
    public interface ICryptoExchenge
    {
        string Username { get; set; }
        string Password { get; set; }
        string LastErrorInfo { get; set; }
        List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pair,int limit);
        List<BaseCurrencyPair> GetCurrencyPair();
        int PostOrder(IOrder order);
        bool PostOrders(List<IOrder> orders);
        bool CanselOrder(int order_id);
        IOrderStatus GetOrderStatus(int order_id);
        List<BaseAccount> GetAccountsList();
        List<BaseHistoryRecord> GetHistoryRecords(DateTime dateTime);
        List<BaseOrder> GetOrdersHistory(BaseCurrencyPair currencyPair,int top_count=-1);
        bool SetAutentification(string user,string password);
    }
}
