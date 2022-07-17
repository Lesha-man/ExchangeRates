using ExchangeRates.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeRates.Services
{
    public partial interface IClientService
    {
        public Task<List<Currency>> GetOrderAsync(DateTime date);
    }
}