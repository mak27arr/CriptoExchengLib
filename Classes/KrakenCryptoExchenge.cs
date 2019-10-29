using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using CriptoExchengLib.Interfaces;
using Newtonsoft.Json.Linq;

namespace CriptoExchengLib.Classes
{
    class KrakenCryptoExchenge : ICryptoExchenge
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastErrorInfo { get; set; }

        private string base_url;

        public KrakenCryptoExchenge()
        {
            base_url = "https://api.kraken.com/0/{0}";
        }

        public bool CanselOrder(int order_id)
        {
            throw new NotImplementedException();
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
                return -1;
            WebConector wc = new WebConector();
            string api_name = "private/AddOrder";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            string data_for_encript = "nonce=" + nonce + ;
            heder.Add(new Tuple<string, string>("API-Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = this.ToNameValue(order);
            body.Add("nonce", nonce);
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
            throw new NotImplementedException();
        }

        public bool SetAutentification(string user, string password)
        {
            this.Username = user;
            this.Password = password;
            return true;
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
