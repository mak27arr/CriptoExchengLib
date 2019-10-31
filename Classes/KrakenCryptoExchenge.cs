using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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

        public List<BaseAccount> GetAccountsList()
        {
            throw new NotImplementedException();
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
            if (!(order is KrakenOrder))
            {
                LastErrorInfo = "Use KrakenOrder type";
                return -1;
            }
            WebConector wc = new WebConector();
            string api_name = "private/AddOrder";
            Int64 nonce = DateTime.Now.Ticks;
            
            string data_transmit = "nonce=" + nonce + Convert.ToChar(0) + "pair=" + order.Pair.PairName + "&" + "type=" + order.Type.Value + "&" + "ordertype=" + ((KrakenOrder)order).Ordertype.Value + "&" + "volume=" + order.Quantity + "&" + "price=" + order.Price;
            var signature = SignatureFormat(api_name, data_transmit, nonce);
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", signature));
            var body = "{pair="+ order.Pair.PairName +"," + "type=" + order.Type.Value + ","
               + "ordertype=" + ((KrakenOrder)order).Ordertype.Value + "," + "volume=" + order.Quantity + "," 
                + "price=" + order.Price + "}"; 
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["result"].ToString() == "true")
            {
                LastErrorInfo = "";
                return Int32.Parse(jsonRezaltArray["order_id"].ToString());
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

        public NameValueCollection ToNameValue(object objectItem)
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
        
    }
}
