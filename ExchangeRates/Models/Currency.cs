using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates.Models
{
    public class Currency
    {
        public int Amount { get; set; }

        public string Code { get; set; }

        public double ExchangeRate { get; set; }
    }
}
