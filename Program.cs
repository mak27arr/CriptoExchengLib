using System;
using System.Collections.Generic;
using CriptoExchengLib.Classes;
using Newtonsoft.Json;

namespace CriptoExchengLib
{
    class Program
    {
        static void Main(string[] args)
        {
            //K-d1b145144afec7e52afd98ab18fcdd3b5d9ce6c8
            //S-b457848406b1cebaa503782234048dade4e92e42
            //ExmoCryptoExchenge ex = new ExmoCryptoExchenge();
            //ex.SetAutentification("K-d1b145144afec7e52afd98ab18fcdd3b5d9ce6c8", "S-b457848406b1cebaa503782234048dade4e92e42");
            //List<CriptoExchengLib.Interfaces.ICurrencyPair> cps = new List<CriptoExchengLib.Interfaces.ICurrencyPair>();
            //BaseCurrencyPair cp = new BaseCurrencyPair("BTC_USD");
            //cps.Add(cp);
            //ex.GetCurrencyPair();
            //BaseOrder bo = new BaseOrder();
            //bo.Pair = new BaseCurrencyPair("BTC_USD");
            //bo.Price = 0;
            //bo.Quantity = 0;
            //bo.Type = BaseOrderType.Buy;
            //ex.PostOrder(bo);
            //ex.GetOrderStatus(12345);
            //ex.GetAccountsList();
            //ex.GetHistoryRecords(DateTime.Now);
            //ex.GetOrdersHistory(cp,100);
            //string json = JsonConvert.SerializeObject(ex.GetBookWarrants(cps, 10), Formatting.Indented);
            //Console.WriteLine(json);

            //nead fix error
            //BitfinexCryptoExchenge bf = new BitfinexCryptoExchenge();
            //bf.SetAutentification("oxeBAjTBesbrcrhTI8sK5kBqfo09g5P3BT9fBJrtrAp", "pXLRVAgJvPdp2Za9uAQKl3kKN6s5tylrpz8WB0464fU");
            //List<BaseCurrencyPair> cps = new List<BaseCurrencyPair>();
            ////BaseCurrencyPair bc = new BaseCurrencyPair("tBTCUSD");
            ////cps.Add(bc);
            ////bf.GetBookWarrants(cps,100);
            ////bf.GetCurrencyPair();
            //BaseOrder bo = new BaseOrder();
            //bo.Pair = new BaseCurrencyPair("tBTCUSD");
            //bo.Price = 0;
            //bo.Quantity = 0;
            //bo.Type = BifinexOrderType.Exchange_fok;
            //bf.PostOrder(bo);
            //bf.CanselOrder(1);
            //Console.ReadKey();
            List<BaseCurrencyPair> cps = new List<BaseCurrencyPair>();
            BaseCurrencyPair cp = new BaseCurrencyPair("ADACAD");
            cps.Add(cp);
            KrakenCryptoExchenge kc = new KrakenCryptoExchenge();
            kc.SetAutentification("OIav92RTeccxQp4zrM6SH3RN07jEyk3POiPByg/54w1wToBRTtVz3120", "eDxzDp0LL1JQhK+pJF2MYbNz+B/WA203vg76PNtAqnT+zgpURWGO/t/S0aqhO1plyIs3OgNjRaHbk0cwkk6prw==");
            //kc.GetCurrencyPair();
            //kc.GetBookWarrants(cps, 100);
            AddOrder("ADACAD","buy","limit",1,1,1);
            KrakenOrder ko = new KrakenOrder();
            ko.Pair = new BaseCurrencyPair("ADACAD");
            ko.Type = KrakenOrderType.Buy;
            ko.Ordertype = KrakenOrderType.Limit;
            ko.Price = 1;
            ko.Quantity = 1;
            kc.PostOrder(ko);
            //kc.CanselOrder(10);
        }

        public static void AddOrder(string pair,
           string type,
           string ordertype,
           decimal volume,
           decimal? price,
           decimal? price2,
           string leverage = "none",
           string position = "",
           string oflags = "",
           string starttm = "",
           string expiretm = "",
           string userref = "",
           bool validate = false,
           Dictionary<string, string> close = null)
        {
            string reqs = string.Format("&pair={0}&type={1}&ordertype={2}&volume={3}&leverage={4}", pair, type, ordertype, volume, leverage);
            if (price.HasValue)
                reqs += string.Format("&price={0}", price.Value);
            if (price2.HasValue)
                reqs += string.Format("&price2={0}", price2.Value);
            if (!string.IsNullOrEmpty(position))
                reqs += string.Format("&position={0}", position);
            if (!string.IsNullOrEmpty(starttm))
                reqs += string.Format("&starttm={0}", starttm);
            if (!string.IsNullOrEmpty(expiretm))
                reqs += string.Format("&expiretm={0}", expiretm);
            if (!string.IsNullOrEmpty(oflags))
                reqs += string.Format("&oflags={0}", oflags);
            if (!string.IsNullOrEmpty(userref))
                reqs += string.Format("&userref={0}", userref);
            if (validate)
                reqs += "&validate=true";
            if (close != null)
            {
                string closeString = string.Format("&close[ordertype]={0}&close[price]={1}&close[price2]={2}", close["ordertype"], close["price"], close["price2"]);
                reqs += closeString;
            }
        }
    }
}
