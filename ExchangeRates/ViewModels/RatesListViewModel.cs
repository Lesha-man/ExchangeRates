using ExchangeRates.Helpers;
using ExchangeRates.Models;
using ExchangeRates.Pages;
using ExchangeRates.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

            if ((await tomorrowcurrencies).FirstOrDefault() is not null)
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

            if ((await firstDaycurrencies)?.FirstOrDefault() is null || (await secondDaycurrencies)?.FirstOrDefault() is null)
            {
                return;
            }

            UpdateRates();

            CompileRates(await firstDaycurrencies, await secondDaycurrencies);
        }

        private void UpdateRates()
        {
            var currenciesOrder = LocalSettingsHelper.Order;

            ExchangeRates.Clear();

            if (currenciesOrder?.FirstOrDefault() is null)
            {
                ExchangeRates.Add(new ExchangeRate { ID = 431 });
                ExchangeRates.Add(new ExchangeRate { ID = 451 });
                ExchangeRates.Add(new ExchangeRate { ID = 456 });

                return;
            }

            for (int i = 0; i < currenciesOrder.Count; i++)
            {
                if (currenciesOrder[i].IsVisible == false)
                    continue;

                ExchangeRates.Add(new ExchangeRate { ID = currenciesOrder[i].ID });
            }
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
                currencies.Add(new Currency
                {
                    Cur_Abbreviation = rate.Abbreviation,
                    Cur_ID = rate.ID,
                    Cur_Name = rate.Name,
                    Cur_Scale = rate.Scale
                });
            }

            await Navigation.PushAsync(new SettingsListPage(Navigation, currencies));
        }

        public class ExchangeRate : INotifyPropertyChanged
        {
            private int _iD;
            private string _abbreviation;
            private string _name;
            private int _scale;
            private double _firstDayOfficialRate;
            private double _secondDayOfficialRate;

            public int ID
            {
                get => _iD;
                set
                {
                    _iD = value;
                    OnPropertyChanged(nameof(ID));
                }
            }

            public string Abbreviation
            {
                get => _abbreviation;
                set
                {
                    _abbreviation = value;
                    OnPropertyChanged(nameof(Abbreviation));
                }
            }

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }

            public int Scale
            {
                get => _scale;
                set
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }

            public double FirstDayOfficialRate
            {
                get => _firstDayOfficialRate;
                set
                {
                    _firstDayOfficialRate = value;
                    OnPropertyChanged(nameof(FirstDayOfficialRate));
                }
            }

            public double SecondDayOfficialRate
            {
                get => _secondDayOfficialRate;
                set
                {
                    _secondDayOfficialRate = value;
                    OnPropertyChanged(nameof(SecondDayOfficialRate));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}