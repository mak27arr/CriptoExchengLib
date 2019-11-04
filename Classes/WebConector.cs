using CriptoExchengLib.Classes;
using CriptoExchengLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CriptoExchengLib.Classes
{
    public class WebConector : IRESTConector
    {
        protected string baseUrl { get; set; }
        protected string username { get; set; }
        protected string password { get; set; }

        private HttpClient httpClient = new HttpClient();
        public async Task<string> ReqwestPostAsync(string url, List<Tuple<string, string>> heder, string body)
        {
            httpClient = new HttpClient();
            return await Task.Run(() => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                if (heder != null)
                {
                    foreach (Tuple<string, string> heder_field in heder)
                    {
                        requestMessage.Headers.Add(heder_field.Item1, heder_field.Item2);
                    }
                }
                StringContent content = new StringContent(body);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                requestMessage.Content = content;
                HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
                return response.Content.ReadAsStringAsync();
            });
        }

        public async Task<string> ReqwestDeleteAsync(string url, List<Tuple<string, string>> heder, string body)
        {
            httpClient = new HttpClient();
            return await Task.Run(() => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
                if (heder != null)
                {
                    foreach (Tuple<string, string> heder_field in heder)
                    {
                        requestMessage.Headers.Add(heder_field.Item1, heder_field.Item2);
                    }
                }
                StringContent content = new StringContent(body);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                requestMessage.Content = content;
                HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
                return response.Content.ReadAsStringAsync();
            });
        }

        public async Task<string> ReqwestGetAsync(string url, List<Tuple<string, string>> heder, string body)
        {
            httpClient = new HttpClient();
            return await Task.Run(() => {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                if (heder != null)
                {
                    foreach (Tuple<string, string> heder_field in heder)
                    {
                        requestMessage.Headers.Add(heder_field.Item1, heder_field.Item2);
                    }
                }
                StringContent content = new StringContent(body);
                requestMessage.Content = content;
                HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
                return response.Content.ReadAsStringAsync();
            });
        }

        public async Task<string> ReqwestPostAsync(string url, List<Tuple<string, string>> heder, NameValueCollection body)
        {
            return await Task.Run(() =>
            {
                using (var wb = new WebClient())
                {
                    if (heder != null)
                    {
                        foreach (Tuple<string, string> heder_field in heder)
                        {
                            wb.Headers.Add(heder_field.Item1, heder_field.Item2);
                        }
                    }
                    var response = wb.UploadValues(url, "POST", body);
                    return Encoding.UTF8.GetString(response);
                }
            });
        }
    }
}
