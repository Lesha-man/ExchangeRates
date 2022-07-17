﻿using ExchangeRates.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream resStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(resStream))
            {
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
            }
        }

        public async Task<List<Currency>> GetOrderAsync(DateTime date)
        {
            var url = $"https://www.nbrb.by/api/exrates/rates?periodicity=0&ondate={date}";

            return await GetAsync<List<Currency>>(url);
        }
    }
}