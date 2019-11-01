using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CriptoExchengLib.Classes.Helper;
using CriptoExchengLib.Interfaces;
using Nancy.Json.Simple;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CriptoExchengLib.Classes
{
    class KrakenCryptoExchenge : ICryptoExchenge
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastErrorInfo { get; set; }

        private string base_url;
        private string api_version;
        public KrakenCryptoExchenge()
        {
            base_url = "https://api.kraken.com/0/{0}";
            api_version = "0";
        }

        //Kraken not suport get input info 
        public List<BaseAccount> GetAccountsList()
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return null;
            }
            WebConector wc = new WebConector();
            string api_name = "private/Balance";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = string.Format("nonce={0}", nonce);
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));

            var jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                List<BaseAccount> rezalt = new List<BaseAccount>();
                foreach (var record in jsonRezaltArray)
                {
                    rezalt.Add(new BaseAccount(record.Key,double.Parse(record.Value.ToString())));
                }
                return rezalt;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return null;
            }
        }

        public List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pairs, int limit)
        {
            if (pairs.Count > 1)
            {
                LastErrorInfo = "Suport only one pairs";
                return new List<BaseBookWarrant>();
            }
            WebConector wc = new WebConector();
            string api_name = "public/Depth?pair=";
            foreach (ICurrencyPair pair in pairs)
            {
                api_name += pair.PairName;
            }
            if (limit > 0 && limit <= 1000)
            {
                api_name += "&count=" + limit.ToString();
            }
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            var books_json = JObject.Parse(jsonRezaltArray["result"].ToString());
            List<BaseBookWarrant> rezalt = new List<BaseBookWarrant>();
            if (books_json.Count > 0)
            {
                foreach (ICurrencyPair cp in pairs)
                {
                        JToken jwarant = books_json[cp.PairName];
                        var jasks = JArray.Parse(jwarant["asks"].ToString());
                        var jbids = JArray.Parse(jwarant["bids"].ToString());
                        BaseBookWarrant bookWarrant = new BaseBookWarrant();
                        bookWarrant.Name = cp.PairName;
                        bookWarrant.Ask = JArray.Parse(jasks.ToString()).ToObject<double[,]>();
                        bookWarrant.Bid = JArray.Parse(jbids.ToString()).ToObject<double[,]>();
                        rezalt.Add(bookWarrant); 
                }
            }

            return rezalt;
        }

        public List<BaseCurrencyPair> GetCurrencyPair()
        {
            WebConector wc = new WebConector();
            string api_name = "public/AssetPairs";
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            List<BaseCurrencyPair> rezalt = new List<BaseCurrencyPair>();

            if (jsonRezaltArray.Count > 0)
            {
                var pair_json = JObject.Parse(jsonRezaltArray["result"].ToString());
                foreach (var cp_json in pair_json)
                {
                    BaseCurrencyPair cp = new BaseCurrencyPair(cp_json.Key);
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
                return null;
            }
            List<BaseHistoryRecord> rezalt = new List<BaseHistoryRecord>();
            WebConector wc = new WebConector();
            string api_name = "private/WithdrawStatus";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = string.Format("nonce={0}", nonce);
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));
            var jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                foreach (var record in jsonRezaltArray.Children<JObject>())
                {
                    BaseHistoryRecord bhr = new BaseHistoryRecord();
                    bhr.Time = UnixTimestampToDateTime(double.Parse(record["time"].ToString()));
                    bhr.Type = "Withdraw";
                    bhr.Id = int.Parse(record["refid"].ToString());
                    bhr.Currency = "";
                    bhr.Status = record["status"].ToString();
                    bhr.Provaider = record["info "].ToString();
                    bhr.Amount = record["amount"].ToString();
                    bhr.Account = "";
                    bhr.Txit = record["txid"].ToString();
                }
                
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return null;
            }
            api_name = "private/DepositStatus";
            nonce = DateTime.Now.Ticks;
            data_transmit = string.Format("nonce={0}", nonce);
            signature = SignatureFormat(api_name, data_transmit, nonce);
            heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));
            jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            jsonRezaltArray = JArray.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                foreach (var record in jsonRezaltArray.Children<JObject>())
                {
                    BaseHistoryRecord bhr = new BaseHistoryRecord();
                    bhr.Time = UnixTimestampToDateTime(double.Parse(record["time"].ToString()));
                    bhr.Type = "Deposit";
                    bhr.Id = int.Parse(record["refid"].ToString());
                    bhr.Currency = "";
                    bhr.Status = record["status"].ToString();
                    bhr.Provaider = record["info "].ToString();
                    bhr.Amount = record["amount"].ToString();
                    bhr.Account = "";
                    bhr.Txit = record["txid"].ToString();
                }

            }
            else
            {
                LastErrorInfo += jsonRezaltArray["error"].ToString();
                return null;
            }
            return rezalt;
        }

        public List<KrakenOrder> GetOrdersHistory(BaseCurrencyPair currencyPair, int top_count = -1)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return null;
            }

            List<KrakenOrder> rezalt = new List<KrakenOrder>();

            WebConector wc = new WebConector();
            string api_name = "private/OpenOrders";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = string.Format("nonce={0}", nonce);
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));

            var jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                foreach (JObject record in jsonRezaltArray.Children<JObject>())
                {
                    KrakenOrder ko = new KrakenOrder();
                    BaseOrderStatus os = BaseOrderStatus.Exsist;
                    os.Value = record["order_id"].ToString().FirstCharToUpper();
                    ko.Status = os;
                    ko.Pair = new BaseCurrencyPair(record["descr"]["pair"].ToString());
                    ko.Type = KrakenOrderType.SetValue(record["descr"]["type"].ToString().FirstCharToUpper());
                    ko.Ordertype = KrakenOrderType.SetValue(record["descr"]["ordertype"].ToString().FirstCharToUpper());
                    ko.Price = double.Parse(record["descr"]["price"].ToString());
                    ko.Quantity = double.Parse(record["vol"].ToString());
                    ko.OpenTm = UnixTimestampToDateTime(double.Parse(record["opentm "].ToString()));
                    rezalt.Add(ko);
                }
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
            }

            api_name = "private/OpenOrders";
            nonce = DateTime.Now.Ticks;
            data_transmit = string.Format("nonce={0}", nonce);
            signature = SignatureFormat(api_name, data_transmit, nonce);
            heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));

            jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            jsonRezaltArray = JArray.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                foreach (JObject record in jsonRezaltArray.Children<JObject>())
                {
                        KrakenOrder ko = new KrakenOrder();
                        BaseOrderStatus os = BaseOrderStatus.Exsist;
                        os.Value = record["order_id"].ToString().FirstCharToUpper();
                        ko.Status = os;
                        ko.Pair = new BaseCurrencyPair(record["descr"]["pair"].ToString());
                        ko.Type = KrakenOrderType.SetValue(record["descr"]["type"].ToString().FirstCharToUpper());
                        ko.Ordertype = KrakenOrderType.SetValue(record["descr"]["ordertype"].ToString().FirstCharToUpper());
                        ko.Price = double.Parse(record["descr"]["price"].ToString());
                        ko.Quantity = double.Parse(record["vol"].ToString());
                        ko.Amount = double.Parse(record["count"].ToString());
                        ko.OpenTm = UnixTimestampToDateTime(double.Parse(record["opentm "].ToString()));
                        rezalt.Add(ko);
                }
            }
            else
            {
                LastErrorInfo += jsonRezaltArray["error"].ToString();
            }
            rezalt = rezalt.Where(x => x.Pair.PairName == currencyPair.PairName).ToList();
            rezalt.OrderBy(x=>x.OpenTm);
            return (top_count>0)?rezalt.Take(top_count).ToList():rezalt;
        }

        List<BaseOrder> ICryptoExchenge.GetOrdersHistory(BaseCurrencyPair currencyPair, int top_count)
        {
            return GetOrdersHistory(currencyPair, top_count).Cast<BaseOrder>().ToList();
        }

        public IOrderStatus GetOrderStatus(int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return null;
            }

            WebConector wc = new WebConector();
            string api_name = "private/QueryOrders";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = string.Format("nonce={0}&txid={1}", nonce, order_id);
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));

            var jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                BaseOrderStatus bos = BaseOrderStatus.Exsist;
                bos.Value = jsonRezaltArray[order_id]["status"].ToString();
                return bos;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return null;
            }
        }

        public int PostOrder(IOrder order)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return -1;
            }
            if (!(order is KrakenOrder))
            {
                LastErrorInfo = "Use KrakenOrder type";
                return -1;
            }
            WebConector wc = new WebConector();
            string api_name = "private/AddOrder";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = "nonce=" + nonce + "&pair=" + order.Pair.PairName + "&" + "type=" + order.Type.Value + "&"
               + "ordertype=" + ((KrakenOrder)order).Ordertype.Value + "&" + "volume=" + order.Quantity + "&" 
               + "price=" + order.Price;
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));

            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                return Int32.Parse(jsonRezaltArray["txid"].ToString());
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return -1;
            }
        }

        public bool PostOrders(List<IOrder> orders)
        {

            foreach (var order in orders)
            {
                if (PostOrder(order) == -1)
                    return false;
            }
            return true;
            
        }

        public bool CanselOrder(int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return false;
            }

            WebConector wc = new WebConector();
            string api_name = "private/CancelOrder";
            Int64 nonce = DateTime.Now.Ticks;
            string data_transmit = string.Format("nonce={0}&txid={1}", nonce,order_id);
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));
        
            var jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, data_transmit).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] == null)
            {
                LastErrorInfo = "";
                return true;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return false;
            }
        }

        public bool SetAutentification(string user, string password)
        {
            this.Username = user;
            this.Password = password;
            return true;
        }

        private string SignatureFormat(string api_name,string data_transmit, Int64 nonce)
        {
            //data_transmit = "nonce=" + nonce + data_transmit;
            string path = string.Format("/{0}/{1}", api_version, api_name);
            byte[] base64DecodedSecred = Convert.FromBase64String(Password);
            var np = nonce + Convert.ToChar(0) + data_transmit;
            var pathBytes = Encoding.UTF8.GetBytes(path);
            var hash256Bytes = SignatureHelper.Sha256_hash(np);
            var z = new byte[pathBytes.Length + hash256Bytes.Length];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Length);
            var signature = SignatureHelper.Sign(base64DecodedSecred, z);
            return Convert.ToBase64String(signature);
        }

        private NameValueCollection ToNameValue(object objectItem)
        {
            Type type = objectItem.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            NameValueCollection propNames = new NameValueCollection();

            foreach (PropertyInfo propertyInfo in objectItem.GetType().GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    var pName = propertyInfo.Name.ToLower();
                    var pValue = propertyInfo.GetValue(objectItem, null);
                    if (pValue != null)
                    {
                        propNames.Add(pName, pValue.ToString());
                    }
                }
            }
            return propNames;
        }

        private static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }
    }
}
