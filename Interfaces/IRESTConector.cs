using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CriptoExchengLib.Interfaces
{
    public interface IRESTConector
    {
        Task<string> ReqwestPostAsync(string url,List<Tuple<string,string>> heder,string body);
        Task<string> ReqwestGetAsync(string url,List<Tuple<string, string>> heder, string bodys);
    }
}
