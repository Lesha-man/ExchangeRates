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

        public SettingsListViewModel(INavigation navigation, List<Currency> currencies)
        {
            Navigation = navigation;

            var currenciesOrder = LocalSettingsHelper.Order;

            if (currenciesOrder?.FirstOrDefault() is not null)
                CurrencyVisualSettingsList = new ObservableCollection<CurrencyVisualSettings>(currenciesOrder);

            foreach (var currency in currencies)
            {
                if (CurrencyVisualSettingsList.FirstOrDefault(cs => cs.ID == currency.Cur_ID) is null)
                {
                    CurrencyVisualSettingsList.Add(new CurrencyVisualSettings
                    {
                        ID = currency.Cur_ID,
                        IsVisible = true,
                        Name = currency.Cur_Name,
                        Abbreviation = currency.Cur_Abbreviation,
                        Scale = currency.Cur_Scale
                    });
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

        public class CurrencyVisualSettings
        {
            public int ID { get; set; }
            public string Abbreviation { get; set; }
            public int Scale { get; set; }
            public string Name { get; set; }
            public bool IsVisible { get; set; }
        }
    }
}
