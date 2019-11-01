using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CriptoExchengLib.Classes
{
    class BinanceCryptoExchenge : ICryptoExchenge
    {
        private string base_url;
        public string Username { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string LastErrorInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BinanceCryptoExchenge()
        {
            base_url = "https://api.binance.com";
        }

        public bool CanselOrder(int order_id)
        {
            throw new NotImplementedException();
        }

        public List<BaseAccount> GetAccountsList()
        {
            throw new NotImplementedException();
        }

        public List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pair, int limit)
        {
            throw new NotImplementedException();
        }

        public List<BaseCurrencyPair> GetCurrencyPair()
        {
            throw new NotImplementedException();
        }

        public List<BaseHistoryRecord> GetHistoryRecords(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public List<BaseOrder> GetOrdersHistory(BaseCurrencyPair currencyPair, int top_count = -1)
        {
            throw new NotImplementedException();
        }

        public IOrderStatus GetOrderStatus(int order_id)
        {
            throw new NotImplementedException();
        }

        public int PostOrder(IOrder order)
        {
            throw new NotImplementedException();
        }

        public bool PostOrders(List<IOrder> orders)
        {
            throw new NotImplementedException();
        }

        public bool SetAutentification(string user, string password)
        {
            throw new NotImplementedException();
        }
    }
}
