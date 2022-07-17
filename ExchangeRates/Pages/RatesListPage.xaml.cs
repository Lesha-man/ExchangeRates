using ExchangeRates.ViewModels;
using Xamarin.Forms;

namespace ExchangeRates.Pages
{
    public partial class RatesListPage : ContentPage
    {
        private readonly RatesListViewModel _viewModel;

        public RatesListPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new RatesListViewModel(Navigation);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.InitializeAsync();
        }
    }
}
