using ExchangeRates.Models;
using ExchangeRates.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ExchangeRates.Pages
{
    public partial class SettingsListPage : ContentPage
    {
        private readonly SettingsListViewModel _viewModel;

        public SettingsListPage(INavigation navigation, List<Currency> currencies)
        {
            InitializeComponent();

            BindingContext = _viewModel = new SettingsListViewModel(navigation, currencies);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.InitializeAsync();
        }
    }
}
