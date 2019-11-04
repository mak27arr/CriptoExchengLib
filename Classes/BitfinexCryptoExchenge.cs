using System;
using System.Collections.Generic;
using System.Text;
using CriptoExchengLib.Interfaces;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;

namespace CriptoExchengLib.Classes
{
    class BitfinexCryptoExchenge : ICryptoExchenge
    {
        private string base_url;
        private string abase_url;
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastErrorInfo { get; set; }

        public BitfinexCryptoExchenge()
        {
            base_url = "https://api-pub.bitfinex.com/v2/{0}";
            abase_url = "https://api.bitfinex.com/v2/{0}";
        }

        public bool CanselOrder(int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Autorizated";
                return false;
            }
            WebConector wc = new WebConector();
            string api_name = "auth/w/order/cancel";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            string body_jsonstr = "{id:" + order_id + "}";
            string data_for_encript = "/api/" + api_name + nonce.ToString() + body_jsonstr;
            heder.Add(new Tuple<string, string>("bfx-apikey", Username));
            heder.Add(new Tuple<string, string>("bfx-signature", SignatureHelper.Sign(Password, data_for_encript,384)));
            heder.Add(new Tuple<string, string>("bfx-nonce", nonce));

            string jsonRezalt = wc.ReqwestPostAsync(string.Format(abase_url, api_name), heder, body_jsonstr).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] != null)
            {
                LastErrorInfo = "";
                //return Int32.Parse(jsonRezaltArray["order_id"].ToString());
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                //return -1;
            }
            throw new NotImplementedException();
        }

        public List<BaseAccount> GetAccountsList()
        {
            throw new NotImplementedException();
        }
        //
        public List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pairs, int limit)
        {
            //if (pairs.Count > 1)
            //{
            //    LastErrorInfo = "Suport only one pairs";
            //    return new List<BaseBookWarrant>();
            //}
            WebConector wc = new WebConector();
            List<BaseBookWarrant> rezalt = new List<BaseBookWarrant>();
            foreach (ICurrencyPair pair in pairs)
            {
                string api_name = "book/";
                api_name += pair.PairName;
            
            api_name += "/P0";
            if (limit == 25 || limit == 100)
            {
                api_name += "?len=" + limit.ToString();
            }
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            
            if (jsonRezaltArray.Count > 0)
            {
                foreach (var cp in jsonRezaltArray)
                {
                    BaseBookWarrant bookWarrant = new BaseBookWarrant();
                    bookWarrant.Name = pairs.First().PairName;
                    var buff = JArray.Parse(cp.ToString()).ToObject<double[]>();
                    bookWarrant.Ask_amount = buff[2];
                    bookWarrant.Ask_quantity = buff[1];
                    bookWarrant.Ask_top = buff[0];
                    rezalt.Add(bookWarrant);
                }
            }
            }
            return rezalt;
        }
        //
        public List<BaseCurrencyPair> GetCurrencyPair()
        {
            WebConector wc = new WebConector();
            string api_name = "/conf/pub:list:pair:exchange";
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            List<BaseCurrencyPair> rezalt = new List<BaseCurrencyPair>();

            if (jsonRezaltArray.Count > 0)
            {
                foreach (var cp_json in jsonRezaltArray) { 
                    foreach (var cp_j in JArray.Parse(cp_json.ToString()))
                    {
                        BaseCurrencyPair cp = new BaseCurrencyPair(cp_j.ToString());
                        rezalt.Add(cp);
                    }
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
//        let signature = `/api/${apiPath}${nonce}${JSON.stringify(body)}` 
//const sig = CryptoJS.HmacSHA384(signature, apiSecret).toString()

        public int PostOrder(IOrder order)
        {
            if (Username == null || Password == null)
                return -1;
            WebConector wc = new WebConector();
            string api_name = "auth/w/order/submit";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            string body_jsonstr = "{type:'" + order.Type.Value + "',symbol:'" + order.Pair + "',price:'" + order.Price + "',amount:'" + order.Amount + "',flags:0}";
            string data_for_encript = "/api/" + api_name + nonce.ToString() + body_jsonstr;
            heder.Add(new Tuple<string, string>("bfx-apikey", Username));
            heder.Add(new Tuple<string, string>("bfx-signature", SignatureHelper.Sign(Password, data_for_encript,384)));
            heder.Add(new Tuple<string, string>("bfx-nonce", nonce));

           // var body = this.ToNameValue(order);
            //body.Add("bfx-nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(abase_url, api_name), heder, body_jsonstr).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["error"] != null)
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
