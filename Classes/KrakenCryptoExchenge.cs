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

        public KrakenCryptoExchenge()
        {
            base_url = "https://api.kraken.com/0/{0}";
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
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            string data_for_encript = "nonce=" + nonce + Convert.ToChar(0) + "pair=" + order.Pair.PairName + "&" + "type=" + order.Type.Value + "&" + "ordertype=" + ((KrakenOrder)order).Ordertype.Value + "&" + "volume=" + order.Quantity + "&" + "price=" + order.Price;
            data_for_encript = SignatureHelper.ComputeSha256Hash(data_for_encript);
            heder.Add(new Tuple<string, string>("API-Sign", Convert.ToBase64String(Encoding.UTF8.GetBytes(SignatureHelper.Sign(Password, (string.Format("0/", api_name)+data_for_encript),512).ToCharArray()))));
            var body = "{pair="+ order.Pair.PairName +"," + "type=" + order.Type.Value + ","
               + "ordertype=" + ((KrakenOrder)order).Ordertype.Value + "," + "volume=" + order.Quantity + "," 
                + "price=" + order.Price + "}"; 
            //body.Add("nonce", nonce);
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


            //string api_name = "private/CancelOrder";
            //List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            //string nonce = DateTime.Now.Ticks.ToString();
            //heder.Add(new Tuple<string, string>("API-Key", Username));
            //string data_for_encript = nonce + Convert.ToChar(0) + "txid=" + order_id;
            //byte[] base64DecodedSecred = Convert.FromBase64String(Password);
            //var data_hash = SignatureHelper.Sha256_hash(data_for_encript);
            //var pathBytes = Encoding.UTF8.GetBytes(string.Format(api_version + "/" + api_name));
            //var data_for_signature = new byte[pathBytes.Length + data_hash.Length];
            //pathBytes.CopyTo(data_for_signature, 0);
            //data_hash.CopyTo(data_for_signature, pathBytes.Length);
            //var signature_byte = SignatureHelper.Sign(base64DecodedSecred, data_for_signature);
            //var signature = Convert.ToBase64String(signature_byte);
            //heder.Add(new Tuple<string, string>("API-Sign", signature));


            WebConector wc = new WebConector();
            string api_name = "private/CancelOrder";
            //List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            //string nonce = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            //heder.Add(new Tuple<string, string>("API-Key", Username));
            //string data_for_encript = "nonce=" + nonce + "&txid=" + order_id;
            //data_for_encript = SignatureHelper.ComputeSha256Hash(data_for_encript);
            //heder.Add(new Tuple<string, string>("API-Sign", Convert.ToBase64String(Encoding.UTF8.GetBytes(SignatureHelper.Sign(Password, (string.Format("0/", api_name) + data_for_encript), 512).ToCharArray()))));

            // generate a 64 bit nonce using a timestamp at tick resolution
            Int64 nonce = DateTime.Now.Ticks;
            var props = "nonce=" + nonce + "&txid=" + order_id;
            string path = string.Format(base_url, "0/private/api_name");
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            byte[] base64DecodedSecred = Convert.FromBase64String(Password);
            var np = nonce + Convert.ToChar(0) + props;
            var pathBytes = Encoding.UTF8.GetBytes(path);
            var hash256Bytes = SignatureHelper.Sha256_hash(np);
            var z = new byte[pathBytes.Length + hash256Bytes.Length];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Length);
            var signature = SignatureHelper.Sign(base64DecodedSecred, z);
            heder.Add(new Tuple<string, string>("API-Sign", Convert.ToBase64String(signature)));
            string jsonRezalt = wc.ReqwestPostAsync(string.Format(base_url, api_name), heder, props).Result;

            QueryPrivate("CancelOrder", "&txid=" + order_id);

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

        private JsonObject QueryPrivate(string a_sMethod, string props = null)
        {
            //RateGate _rateGate = new RateGate(1, TimeSpan.FromSeconds(5)); ;
            string _url = "https://api.kraken.com/";
            string _version = "0";

            // generate a 64 bit nonce using a timestamp at tick resolution
            Int64 nonce = DateTime.Now.Ticks;
            props = "nonce=" + nonce + props;


            string path = string.Format("/{0}/private/{1}", _version, a_sMethod);
            string address = _url + path;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Headers.Add("API-Key", Username);


            byte[] base64DecodedSecred = Convert.FromBase64String(Password);

            var np = nonce + Convert.ToChar(0) + props;

            var pathBytes = Encoding.UTF8.GetBytes(path);
            var hash256Bytes = SignatureHelper.Sha256_hash(np);
            var z = new byte[pathBytes.Length + hash256Bytes.Length];
            pathBytes.CopyTo(z, 0);
            hash256Bytes.CopyTo(z, pathBytes.Length);

            var signature = SignatureHelper.Sign(base64DecodedSecred, z);

            webRequest.Headers.Add("API-Sign", Convert.ToBase64String(signature));

            WebConector wc = new WebConector();
            List<Tuple<string, string>> heder = new List<Tuple<string, string>>();
            heder.Add(new Tuple<string, string>("API-Key", Username));
            heder.Add(new Tuple<string, string>("API-Sign", Convert.ToBase64String(signature)));
           var rezzzz =  wc.ReqwestPostAsync(address,heder, props).Result;

            if (props != null)
            {

                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(props);
                }
            }

            //Make the request
            try
            {
                //Wait for RateGate
                //_rateGate.WaitToProceed();

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            var v = JsonConvert.DeserializeObject(sr.ReadToEnd());
                            return null;
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            if (response.StatusCode != HttpStatusCode.InternalServerError)
                            {
                                throw;
                            }
                            return (JsonObject)JsonConvert.DeserializeObject(sr.ReadToEnd());
                        }
                    }
                }

            }
        }

        private byte[] sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;

                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                return result;
            }
        }

        private byte[] getHash(byte[] keyByte, byte[] messageBytes)
        {
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {

                Byte[] result = hmacsha512.ComputeHash(messageBytes);

                return result;

            }
        }

    }
}
