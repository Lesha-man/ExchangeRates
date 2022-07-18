using ExchangeRates.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ExchangeRates.Services
{
    public partial class ClientService : IClientService
    {
        #region Instance

        private static IClientService _instance;

        public static IClientService Instance => _instance ??= new ClientService();

        #endregion Instance

        public async Task<T> GetAsync<T>(string uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
                request.Timeout = 5000;

                using HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return default(T);
                }

                using Stream resStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(resStream))
                {
                    return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public async Task<List<Currency>> GetOrderAsync(DateTime date)
        {
            var url = $"https://www.nbrb.by/api/exrates/rates?periodicity=0&ondate={date}";

            return await GetAsync<List<Currency>>(url);
        }
    }
}