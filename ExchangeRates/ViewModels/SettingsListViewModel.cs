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
    public class SettingsListViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private DateTime _firstDate;
        private DateTime _secondDate;

        public SettingsListViewModel(List<Currency> currencies)
        {
            var currenciesOrder = LocalSettingsHelper.Order;

            if (currenciesOrder?.FirstOrDefault() is null)
                return;

            CurrencyVisualSettingsList = new ObservableCollection<CurrencyVisualSettings>(currenciesOrder);

            foreach (var currency in currencies)
            {
                if (CurrencyVisualSettingsList.FirstOrDefault(cs => cs.ID == currency.Cur_ID) is null)
                {
                    CurrencyVisualSettingsList.Add(new CurrencyVisualSettings { ID = currency.Cur_ID, IsVisible = true});
                }
            }
        }

        public ObservableCollection<CurrencyVisualSettings> CurrencyVisualSettingsList { get; set; } = new();

        public ICommand SubmitCommand => new Command(Submit);
        public INavigation Navigation { get; set; }

        public async Task InitializeAsync()
        {
        }

        private async void Submit()
        {
            LocalSettingsHelper.Order = CurrencyVisualSettingsList.ToList();

            await Navigation.PopAsync();
        }
    }
}
