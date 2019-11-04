using CriptoExchengLib.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using CriptoExchengLib.Classes.Helper;

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
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return new List<BaseAccount>();
            }
            WebConector wc = new WebConector();
            string api_name = "/api/v3/account";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            var jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            string timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            string data_for_encript = "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript, 256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            var url_me = string.Format("{0}{1}?{2}", base_url, api_name, data_for_encript);
            string jsonRezalt = wc.ReqwestGetAsync(string.Format("{0}{1}?{2}", base_url, api_name,data_for_encript), heder, data_for_encript).Result;
            try
            {
                var jsonRezaltArray = JObject.Parse(jsonRezalt);
                if (jsonRezaltArray["msg"] == null)
                {
                    var jsonAccounts = JArray.Parse(jsonRezaltArray["balances"].ToString());
                    var rezalt = new List<BaseAccount>();
                    foreach (var json_acc in jsonAccounts)
                    {
                        var ba = new BaseAccount(json_acc["asset"].ToString(), json_acc["free"].ToObject<double>());
                        rezalt.Add(ba);
                    }
                    return rezalt;
                }
                else
                {
                    LastErrorInfo = jsonRezaltArray["msg"].ToString();
                    return new List<BaseAccount>();
                }
            }catch(Exception ex)
            {
                LastErrorInfo = jsonRezalt;
                return new List<BaseAccount>();
            }
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
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return new List<BaseHistoryRecord>();
            }
            WebConector wc = new WebConector();
            string api_name = "wapi/v3/depositHistory.html";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            var jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            string timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            string data_for_encript = "&startTime=" + dateTime.ToUnixTimestamp() + "&recvWindow=" + "50000" + "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript, 256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format("{0}{1}", base_url, api_name), heder, data_for_encript).Result;
            var jarrayRezalt = JObject.Parse(jsonRezalt);
            var rezalt = new List<BaseHistoryRecord>();
            if (jarrayRezalt["msg"] == null)
            {
                var jsonArrayRecords = JArray.Parse(jarrayRezalt["depositList"].ToString());
                foreach (var record in jsonArrayRecords) {
                    BaseHistoryRecord bhr = new BaseHistoryRecord();
                    bhr.Id = 0;
                    bhr.Provaider = record["txId"].ToString();
                    bhr.Status = record["status"].ToString();
                    bhr.Time = (new DateTime()).FromUnixTimestamp(record["insertTime"].ToObject<Int64>());
                    bhr.Txit = record["txId"].ToString();
                    bhr.Type = "deposit";
                    bhr.Account = record["address"].ToString(); ;
                    bhr.Amount = record["amount"].ToString(); ;
                    bhr.Currency = record["asset"].ToString(); ;
                    rezalt.Add(bhr);
                }
            }
            else
            {
                LastErrorInfo = jarrayRezalt["msg"].ToString();
                return new List<BaseHistoryRecord>();
            }
            api_name = "/wapi/v3/withdrawHistory.html";
            heder = new List<Tuple<string, string>>();
            jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            data_for_encript = "&startTime=" + dateTime.ToUnixTimestamp() + "&recvWindow=" + "50000" + "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript, 256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            jsonRezalt = wc.ReqwestPostAsync(string.Format("{0}{1}", base_url, api_name), heder, data_for_encript).Result;
            jarrayRezalt = JObject.Parse(jsonRezalt);
            if (jarrayRezalt["msg"] == null)
            {
                var jsonArrayRecords = JArray.Parse(jarrayRezalt["withdrawList"].ToString());
                foreach (var record in jsonArrayRecords)
                {
                    BaseHistoryRecord bhr = new BaseHistoryRecord();
                    bhr.Id = 0;
                    bhr.Provaider = record["txId"].ToString();
                    bhr.Status = record["status"].ToString();
                    bhr.Time = (new DateTime()).FromUnixTimestamp(record["insertTime"].ToObject<Int64>());
                    bhr.Txit = record["txId"].ToString();
                    bhr.Type = "withdraw";
                    bhr.Account = record["address"].ToString(); ;
                    bhr.Amount = record["amount"].ToString(); ;
                    bhr.Currency = record["asset"].ToString(); ;
                    rezalt.Add(bhr);
                }
            }
            else
            {
                LastErrorInfo += jarrayRezalt["msg"].ToString();
                return new List<BaseHistoryRecord>();
            }
            throw new NotImplementedException();
        }

        public List<BaseOrder> GetOrdersHistory(BaseCurrencyPair currencyPair, int top_count = 100)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return new List<BaseOrder>();
            }
            if (top_count > 1000)
                top_count = 1000;
            WebConector wc = new WebConector();
            string api_name = "/api/v3/allOrders";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            var jsontimestamp = wc.ReqwestGetAsync(string.Format("{0}/api/v3/time", base_url), new List<Tuple<string, string>>(), "").Result;
            string timestamp = (JObject.Parse(jsontimestamp))["serverTime"].ToString();
            heder.Add(new Tuple<string, string>("X-MBX-APIKEY", Username));
            string data_for_encript = "symbol=" + currencyPair.PairName + "&limit=" + top_count + "&recvWindow=" + "50000" + "&timestamp=" + timestamp;
            heder.Add(new Tuple<string, string>("signature", SignatureHelper.Sign(Password, data_for_encript, 256)));
            data_for_encript += "&signature=" + SignatureHelper.Sign(Password, data_for_encript, 256);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format("{0}{1}", base_url, api_name), heder, data_for_encript).Result;
            try
            {
                var jsonRezaltArray = JArray.Parse(jsonRezalt);
                LastErrorInfo = "";
                var rezalt = new List<BaseOrder>();
                foreach(var order in jsonRezaltArray)
                {
                    BaseOrder bo = new BaseOrder();
                    bo.Id = order["orderId"].ToObject<int>();
                    bo.Pair = new BaseCurrencyPair(order["symbol"].ToString());
                    bo.Quantity = order["origQty"].ToObject<double>();
                    bo.Price = order["price"].ToObject<double>();
                    bo.Type = BinanceOrderType.SetValue(order["type"].ToString());
                    bo.Amount = order["executedQty"].ToObject<int>();
                    rezalt.Add(bo);
                }
                return rezalt;
            }
            catch(Exception ex)
            {
                LastErrorInfo = jsonRezalt;
                return new List<BaseOrder>();
            }
        }

        public IOrderStatus GetOrderStatus(int order_id)
        {
            LastErrorInfo = "Set currency pair";
            throw new NotImplementedException();
        }
        public IOrderStatus GetOrderStatus(BaseCurrencyPair cp, int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return BinanceOrderStatus.Error;
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
            string jsonRezalt = wc.ReqwestGetAsync(string.Format("{0}{1}", base_url, api_name), heder, data_for_encript).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["msg"] == null)
            {
                LastErrorInfo = "";
                if(jsonRezaltArray["status"] != null)
                    return BinanceOrderStatus.SetValue(jsonRezaltArray["status"].ToString());
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["msg"].ToString();
                return BinanceOrderStatus.NoExsist;
            }
            return BinanceOrderStatus.Error;
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
