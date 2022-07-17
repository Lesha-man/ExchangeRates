using ExchangeRates.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Essentials;
using ExchangeRates.ViewModels;

namespace ExchangeRates.Helpers
{
    public static class LocalSettingsHelper
    {
        public static List<SettingsListViewModel.CurrencyVisualSettings> Order
        {
            get => Get<List<SettingsListViewModel.CurrencyVisualSettings>>();

            set => Set(value);
        }

        public static T Get<T>() => JsonConvert.DeserializeObject<T>(Preferences.Get(nameof(T), string.Empty));

        public static void Set<T>(T value) => Preferences.Set(nameof(T), JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
    }
}
