using ExchangeRates.Helpers;
using ExchangeRates.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace ExchangeRates.ViewModels
{
    public class SettingsListViewModel : BaseViewModel
    {
        public SettingsListViewModel(INavigation navigation, List<Currency> currencies)
        {
            ItemDragged = new Command<CurrencyVisualSettings>(OnItemDragged);
            ItemDraggedOver = new Command<CurrencyVisualSettings>(OnItemDraggedOver);
            ItemDragLeave = new Command<CurrencyVisualSettings>(OnItemDragLeave);
            ItemDropped = new Command<CurrencyVisualSettings>(i => OnItemDropped(i));

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
        public ICommand ItemDragged { get; }

        public ICommand ItemDraggedOver { get; }

        public ICommand ItemDragLeave { get; }

        public ICommand ItemDropped { get; }

        private void OnItemDragged(CurrencyVisualSettings item)
        {
            CurrencyVisualSettingsList.ForEach(i => i.IsBeingDragged = item == i);
        }

        private void OnItemDraggedOver(CurrencyVisualSettings item)
        {
            var itemBeingDragged = CurrencyVisualSettingsList.FirstOrDefault(i => i.IsBeingDragged);
            CurrencyVisualSettingsList.ForEach(i => i.IsBeingDraggedOver = item == i && item != itemBeingDragged);
        }

        private void OnItemDragLeave(CurrencyVisualSettings item)
        {
            CurrencyVisualSettingsList.ForEach(i => i.IsBeingDraggedOver = false);
        }

        private async Task OnItemDropped(CurrencyVisualSettings item)
        {
            var itemToMove = CurrencyVisualSettingsList.First(i => i.IsBeingDragged);
            var itemToInsertBefore = item;

            if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore)
                return;

            CurrencyVisualSettingsList.Remove(itemToMove);

            var insertAtIndex = CurrencyVisualSettingsList.IndexOf(itemToInsertBefore);
            CurrencyVisualSettingsList.Insert(insertAtIndex, itemToMove);
            itemToMove.IsBeingDragged = false;
            itemToInsertBefore.IsBeingDraggedOver = false;
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

        public class CurrencyVisualSettings : INotifyPropertyChanged
        {
            public int ID { get; set; }
            public string Abbreviation { get; set; }
            public int Scale { get; set; }
            public string Name { get; set; }
            public bool IsVisible { get; set; }

            private bool _isBeingDragged;
            public bool IsBeingDragged
            {
                get => _isBeingDragged;
                set
                {
                    _isBeingDragged = value;
                    OnPropertyChanged(nameof(IsBeingDragged));
                }
            }

            private bool _isBeingDraggedOver;
            public bool IsBeingDraggedOver
            {
                get => _isBeingDraggedOver;
                set
                {
                    _isBeingDraggedOver = value;
                    OnPropertyChanged(nameof(IsBeingDraggedOver));
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
