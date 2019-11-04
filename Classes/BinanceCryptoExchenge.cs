using CriptoExchengLib.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CriptoExchengLib.Classes
{
    class BinanceCryptoExchenge : ICryptoExchenge
    {
        private string base_url;
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastErrorInfo { get; set; }

        public BinanceCryptoExchenge()
        {
            base_url = "https://api.binance.com";
        }

        bool ICryptoExchenge.CanselOrder(int order_id)
        {
            LastErrorInfo = "symbol required";
            throw new NotImplementedException();
        }

        public bool CanselOrder(BaseCurrencyPair cp, int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return false;
            }
            WebConector wc = new WebConector();
            string api_name = "/api/v3/order";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            var jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            string timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            string data_for_encript = "symbol=" + cp.PairName + "&orderId=" + order_id + "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript, 256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            string jsonRezalt = wc.ReqwestDeleteAsync(string.Format("{0}{1}", base_url, api_name), heder, data_for_encript).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["msg"] == null)
            {
                LastErrorInfo = "";
                return true;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["msg"].ToString();
                return false;
            }
        }

        public List<BaseAccount> GetAccountsList()
        {
            throw new NotImplementedException();
        }

        public List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pairs, int limit=100)
        {
            List<BaseBookWarrant> rezalt = new List<BaseBookWarrant>();
            if (limit > 5000)
                limit = 5000;
            WebConector wc = new WebConector();
            string api_name = "/api/v3/depth";
            foreach (var pair in pairs)
            {
                string pair_name = pair.PairName;
                string paramtr = "?&symbol=" + pair_name + "&limit=" + limit;
                string jsonRezalt = wc.ReqwestGetAsync(string.Format("{0}{1}{2}", base_url, api_name, paramtr), new List<Tuple<string, string>>(), "").Result;
                var jsonRezaltArray = JObject.Parse(jsonRezalt);
                if (jsonRezaltArray.Count > 0)
                {
                        BaseBookWarrant bbw = new BaseBookWarrant();
                        bbw.Name = pair_name;
                        bbw.Ask = JArray.Parse(jsonRezaltArray["asks"].ToString()).ToObject<double[,]>();
                        bbw.Bid = JArray.Parse(jsonRezaltArray["bids"].ToString()).ToObject<double[,]>();
                    double ask_amount = 0.0;
                    for(int i = 0; i < bbw.Ask.GetLength(0); i++)
                    {
                        ask_amount += bbw.Ask[i,1];
                    }
                    bbw.Ask_amount = ask_amount;
                    bbw.Ask_quantity = ask_amount / bbw.Ask.GetLength(0);
                    double bid_amount = 0.0;
                    for (int i = 0; i < bbw.Bid.GetLength(0); i++)
                    {
                        bid_amount += bbw.Bid[i, 1];
                    }
                    bbw.Bid_amount = bid_amount;
                    bbw.Bid_quantity = bid_amount / bbw.Bid.GetLength(0);
                    rezalt.Add(bbw);
                }
            }
            return rezalt;
        }

        public List<BaseCurrencyPair> GetCurrencyPair()
        {
            WebConector wc = new WebConector();
            string api_name = "/api/v3/ticker/price";   
            string jsonRezalt = wc.ReqwestGetAsync(string.Format("{0}{1}", base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            List<BaseCurrencyPair> rezalt = new List<BaseCurrencyPair>();

            if (jsonRezaltArray.Count > 0)
            {
                foreach (var cp_json in jsonRezaltArray)
                {
                        BaseCurrencyPair cp = new BaseCurrencyPair(cp_json["symbol"].ToString());
                        rezalt.Add(cp);
                }
            }

            return rezalt;
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
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return -1;
            }
            if (!(order is BinanceOrder))
            {
                LastErrorInfo = "Use BinanceOrder type";
                return -1;
            }
            WebConector wc = new WebConector();
            string api_name = "/api/v3/order";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            var jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            string timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            string data_for_encript = "symbol=" + order.Pair.PairName + "&side=" + ((BinanceOrder)order).Side.Value + "&type=" + order.Type.Value + "&timeInForce=" + ((BinanceOrder)order).TimeInForce.ToString("G") + "&quantity=" + order.Quantity + "&price=" + order.Price + "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript,256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format("{0}{1}",base_url, api_name), heder, data_for_encript).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["orderId"] != null)
            {
                LastErrorInfo = "";
                return Int32.Parse(jsonRezaltArray["orderId"].ToString());
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["msg"].ToString();
                return -1;
            }
        }

        public bool PostOrders(List<IOrder> orders)
        {
            foreach (var order in orders)
                if (PostOrder(order) == -1)
                    return false;
            return true;
                
        }

        public bool SetAutentification(string user, string password)
        {
            this.Username = user;
            this.Password = password;
            return true;
        }

    }
}
