using ExchangeRates.Models;
using System;
using System.Collections.Generic;
using ExchangeRates.Helpers;
using System.Linq;
using ExchangeRates.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using ExchangeRates.Pages;

namespace ExchangeRates.ViewModels
{
    public class RatesListViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private DateTime _firstDate;
        private DateTime _secondDate;

        public RatesListViewModel(INavigation navigation)
        {
            Navigation = navigation;

            _clientService = ClientService.Instance;

            var currenciesOrder = LocalSettingsHelper.Order;

            if (currenciesOrder?.FirstOrDefault() is null)
                return;

            for (int i = 0; i < currenciesOrder.Count; i++)
            {
                if (currenciesOrder[i].IsVisible == false)
                    continue;

                ExchangeRates.Add(new ExchangeRate { ID = currenciesOrder[i].ID });
            }
        }

        public ObservableCollection<ExchangeRate> ExchangeRates { get; set; } = new();

        public DateTime FirstDate
        {
            set
            {
                if (_firstDate != value)
                {
                    _firstDate = value;
                    OnPropertyChanged(nameof(FirstDate));
                }
            }
            get
            {
                return _firstDate;
            }
        }

        public ICommand GoToSettingsCommand => new Command(GoToSettings);
        public INavigation Navigation { get; set; }
        public DateTime SecondDate
        {
            set
            {
                if (_secondDate != value)
                {
                    _secondDate = value;
                    OnPropertyChanged(nameof(SecondDate));
                }
            }
            get
            {
                return _secondDate;
            }
        }
        public async Task InitializeAsync()
        {

            var tomorrowcurrencies = _clientService.GetOrderAsync(DateTime.Today.AddDays(1));

            Task<List<Currency>> secondDaycurrencies;
            Task<List<Currency>> firstDaycurrencies;

            if ((await tomorrowcurrencies).Any())
            {
                secondDaycurrencies = tomorrowcurrencies;

                firstDaycurrencies = _clientService.GetOrderAsync(DateTime.Today);

                FirstDate = DateTime.Today;
                SecondDate = DateTime.Today.AddDays(1);
            }
            else
            {
                secondDaycurrencies = _clientService.GetOrderAsync(DateTime.Today);

                firstDaycurrencies = _clientService.GetOrderAsync(DateTime.Today.AddDays(-1));

                FirstDate = DateTime.Today.AddDays(-1);
                SecondDate = DateTime.Today;
            }

            CompileRates(await firstDaycurrencies, await secondDaycurrencies);
        }

        private void CompileRates(List<Currency> firstDaycurrencies, List<Currency> secondDaycurrencies)
        {
            FirstDate = DateTime.Today;
            SecondDate = DateTime.Today.AddDays(1);
            firstDaycurrencies.ForEach(fc =>
            {
                ExchangeRate rate = ExchangeRates.FirstOrDefault(rate => fc.Cur_ID == rate.ID);
                if (rate is null)
                {
                    ExchangeRates.Add(new ExchangeRate
                    {
                        ID = fc.Cur_ID,
                        Abbreviation = fc.Cur_Abbreviation,
                        Scale = fc.Cur_Scale,
                        Name = fc.Cur_Name,
                        FirstDayOfficialRate = fc.Cur_OfficialRate
                    });
                }
                else
                {
                    rate.Abbreviation = fc.Cur_Abbreviation;
                    rate.Scale = fc.Cur_Scale;
                    rate.Name = fc.Cur_Name;
                    rate.FirstDayOfficialRate = fc.Cur_OfficialRate;
                }
            });

            secondDaycurrencies.ForEach(sc =>
            {
                ExchangeRate rate = ExchangeRates.FirstOrDefault(rate => sc.Cur_ID == rate.ID);
                if (rate is null)
                {
                    ExchangeRates.Add(new ExchangeRate
                    {
                        SecondDayOfficialRate = sc.Cur_OfficialRate
                    });
                }
                else
                {
                    rate.SecondDayOfficialRate = sc.Cur_OfficialRate;
                }
            });
        }

        private async void GoToSettings()
        {
            List<Currency> currencies = new();

            foreach (var rate in ExchangeRates)
            {
                currencies.Add(new Currency { Cur_Abbreviation = rate.Abbreviation, Cur_ID = rate.ID });
            }

            await Navigation.PushAsync(new SettingsListPage(currencies));
        }

        public class ExchangeRate
        {
            public string Abbreviation { get; set; }
            public double FirstDayOfficialRate { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public int Scale { get; set; }
            public double SecondDayOfficialRate { get; set; }
        }

    }
}
