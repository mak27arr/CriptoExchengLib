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


            BitfinexCryptoExchenge bf = new BitfinexCryptoExchenge();
            bf.SetAutentification("oxeBAjTBesbrcrhTI8sK5kBqfo09g5P3BT9fBJrtrAp", "pXLRVAgJvPdp2Za9uAQKl3kKN6s5tylrpz8WB0464fU");
            List<BaseCurrencyPair> cps = new List<BaseCurrencyPair>();
            //BaseCurrencyPair bc = new BaseCurrencyPair("tBTCUSD");
            //cps.Add(bc);
            //bf.GetBookWarrants(cps,100);
            //bf.GetCurrencyPair();
            BaseOrder bo = new BaseOrder();
            bo.Pair = new BaseCurrencyPair("tBTCUSD");
            bo.Price = 0;
            bo.Quantity = 0;
            bo.Type = BifinexOrderType.Exchange_fok;
            bf.PostOrder(bo);
            Console.ReadKey();
        }
    }
}
