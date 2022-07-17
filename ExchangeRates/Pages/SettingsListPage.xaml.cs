using ExchangeRates.Models;
using ExchangeRates.ViewModels;
using System;
using System.Collections.Generic;
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

        private void DragGestureRecognizer_DragStarting(Object sender, DragStartingEventArgs e)
        {
            var label = (Label) ((Element) sender).Parent;

            e.Data.Properties["Label"] = label;
        }

        private void DropGestureRecognizer_Drop(Object sender, DropEventArgs e)
        {
            var label = (Label) ((Element) sender).Parent;
            var dropLabel = (Label) e.Data.Properties["Label"];
            if (label == dropLabel)
                return;

            var sourceContainer = (Grid) dropLabel.Parent;
            var targetContainer = (Grid) label.Parent;
            sourceContainer.Children.Remove(dropLabel);
            targetContainer.Children.Remove(label);
            sourceContainer.Children.Add(label);
            targetContainer.Children.Add(dropLabel);

            e.Handled = true;
        }

        private void DragGestureRecognizer_DragStarting_Collection(System.Object sender, Xamarin.Forms.DragStartingEventArgs e)
        {

        }

        private void DropGestureRecognizer_Drop_Collection(System.Object sender, Xamarin.Forms.DropEventArgs e)
        {
            e.Handled = true;
        }
    }
}
