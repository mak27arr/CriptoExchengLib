using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using CriptoExchengLib.Interfaces;
using Newtonsoft.Json.Linq;

namespace CriptoExchengLib.Classes
{
    class ExmoCryptoExchenge : ICryptoExchenge
    {
        private string base_url;
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastErrorInfo { get; protected set; }
        string ICryptoExchenge.Username { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string ICryptoExchenge.Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string ICryptoExchenge.LastErrorInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ExmoCryptoExchenge()
        {
            base_url = "https://api.exmo.com/v1/{0}";
        }

        public bool SetAutentification(string user, string password)
        {
            this.Username = user;
            this.Password = password;
            return true;
        }

        public List<BaseAccount> GetAccountsList()
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Authenticated";
                return new List<BaseAccount>();
            }
            WebConector wc = new WebConector();
            string api_name = "user_info";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key", Username));
            string data_for_encript = "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = new NameValueCollection();
            body.Add("nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray.Count != 0)
            {
                List < BaseAccount > rezaltlist = new List<BaseAccount>();
                foreach (var accountvalue in JObject.Parse(jsonRezaltArray["balances"].ToString()))
                {
                    rezaltlist.Add(new BaseAccount(accountvalue.Key,double.Parse(accountvalue.Value.ToString())));
                }

                LastErrorInfo = "";
                return rezaltlist;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return new List<BaseAccount>();
            }
        }

        public List<BaseBookWarrant> GetBookWarrants(List<BaseCurrencyPair> pairs,int limit)
        {
            WebConector wc = new WebConector();
            string api_name = "order_book/?pair=";
            foreach(ICurrencyPair pair in pairs)
            {
                api_name += pair.PairName + ",";
            }
            if(limit>0 && limit <= 1000)
            {
                api_name += "&limit=" + limit.ToString();
            }
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url,api_name),new List<Tuple<string, string>>(),"").Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            List<BaseBookWarrant> rezalt = new List<BaseBookWarrant>();
            if (jsonRezaltArray.Count > 0)
            {
                foreach (ICurrencyPair cp in pairs)
                {
                    BaseBookWarrant bookWarrant = new BaseBookWarrant();
                    bookWarrant.Name = cp.PairName;
                    JToken jwarant = jsonRezaltArray[cp.PairName];
                    bookWarrant.Ask_quantity = (double)jwarant["ask_quantity"];
                    bookWarrant.Ask_amount = (double)jwarant["ask_amount"];
                    bookWarrant.Ask_top = (double)jwarant["ask_top"];
                    bookWarrant.Bid_quantity = (double)jwarant["bid_quantity"];
                    bookWarrant.Bid_amount = (double)jwarant["bid_amount"];
                    bookWarrant.Bid_top = (double)jwarant["bid_top"];
                    bookWarrant.Ask = JArray.Parse(jwarant["ask"].ToString()).ToObject<double[,]>();
                    bookWarrant.Bid = JArray.Parse(jwarant["bid"].ToString()).ToObject<double[,]>();
                    rezalt.Add(bookWarrant);
                }
            }

            return rezalt;
        }

        public List<BaseCurrencyPair> GetCurrencyPair()
        {
            WebConector wc = new WebConector();
            string api_name = "pair_settings/";
            string jsonRezalt = wc.ReqwestGetAsync(string.Format(base_url, api_name), new List<Tuple<string, string>>(), "").Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            List<BaseCurrencyPair> rezalt = new List<BaseCurrencyPair>();

            if (jsonRezaltArray.Count > 0)
            {
                foreach (var cp_json in jsonRezaltArray)
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
                LastErrorInfo = "Not Authenticated";
                return new List<BaseHistoryRecord>();
            }
            WebConector wc = new WebConector();
            string api_name = "wallet_history";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key", Username));
            string data_for_encript = "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = new NameValueCollection();
            body.Add("nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray.Count != 0)
            {
                List<BaseHistoryRecord> rezaltlist = new List<BaseHistoryRecord>();
                foreach (var accountvalue in jsonRezaltArray["history"])
                {
                    BaseHistoryRecord bhr = new BaseHistoryRecord();
                    //bhr.Id = accountvalue[""];
                    bhr.Type = accountvalue["type"].ToString();
                    bhr.Time = Convert.ToDateTime(accountvalue["dt"].ToString());
                    bhr.Currency = accountvalue["curr"].ToString();
                    bhr.Status = accountvalue["status"].ToString();
                    bhr.Provaider = accountvalue["provider"].ToString();
                    bhr.Amount = accountvalue["amount"].ToString();
                    bhr.Account = accountvalue["account"].ToString();
                    bhr.Txit = accountvalue["txid"].ToString();
                    rezaltlist.Add(bhr);
                }

                LastErrorInfo = "";
                return rezaltlist;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return new List<BaseHistoryRecord>();
            }
        }

        public List<BaseOrder> GetOrdersHistory(BaseCurrencyPair currencyPair, int top_count = 100)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Authenticated";
                return new List<BaseOrder>();
            }
            WebConector wc = new WebConector();
            string api_name = "user_cancelled_orders";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key", Username));
            string data_for_encript = "limit=" + top_count + "&" + "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = new NameValueCollection();
            body.Add("limit", top_count.ToString());
            body.Add("nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JArray.Parse(jsonRezalt);
            if (jsonRezaltArray.Count != 0)
            {
                List<BaseOrder> rezaltlist = new List<BaseOrder>();
                foreach (JObject record in jsonRezaltArray.Children<JObject>())
                {
                        BaseOrder bhr = new BaseOrder();
                        bhr.Id = int.Parse(record["order_id"].ToString());
                        bhr.Type = BaseOrderType.SetValue(record["order_type"].ToString());
                        bhr.Pair = new BaseCurrencyPair(record["pair"].ToString());
                        bhr.Price = double.Parse(record["price"].ToString());
                        bhr.Quantity = double.Parse(record["quantity"].ToString());
                        bhr.Amount = double.Parse(record["amount"].ToString());
                    rezaltlist.Add(bhr);
                }

                LastErrorInfo = "";
                return rezaltlist;
            }
            else
            {
                return new List<BaseOrder>();
            }
        }

        public IOrderStatus GetOrderStatus(int order_id)
        {
            if (Username == null || Password == null)
            {
                LastErrorInfo = "Not Authenticated";
                return BaseOrderStatus.Error;
            }
            WebConector wc = new WebConector();
            string api_name = "order_trades";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key", Username));
            string data_for_encript = "order_id=" + order_id + "&" + "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = new NameValueCollection();
            body.Add("order_id", order_id.ToString());
            body.Add("nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["result"].ToString() == "true")
            {
                LastErrorInfo = "";
                return BaseOrderStatus.Exsist;
            }
            else
            {
                LastErrorInfo = jsonRezaltArray["error"].ToString();
                return BaseOrderStatus.NoExsist;
            }
        }

        public int PostOrder(IOrder order)
        {
            if (Username == null || Password == null)
                return -1;
            WebConector wc = new WebConector();
            string api_name = "order_create";
            List < Tuple<string, string> > heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key",Username));
            string data_for_encript = "pair="+order.Pair.PairName+"&"+ "quantity=" + order.Quantity + "&"+ "price=" + order.Price + "&"+ "type=" + order.Type.Value + "&" + "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = this.ToNameValue(order);
            body.Add("nonce",nonce);
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
            foreach (IOrder order in orders)
                PostOrder(order);
            return true;
        }

        public bool CanselOrder(int order_id)
        {
            if (Username == null || Password == null)
                return false;
            WebConector wc = new WebConector();
            string api_name = "order_cancel";
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("Key", Username));
            string data_for_encript = "order_id=" + order_id + "&" + "nonce=" + nonce;
            heder.Add(new Tuple<string, string>("Sign", SignatureHelper.Sign(Password, data_for_encript)));

            var body = new NameValueCollection();
            body.Add("order_id", order_id.ToString());
            body.Add("nonce", nonce);
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, body).Result;
            var jsonRezaltArray = JObject.Parse(jsonRezalt);
            if (jsonRezaltArray["result"].ToString() == "true")
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
