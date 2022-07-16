using ExchangeRates.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace ExchangeRates.Helpers
{
    public static class LocalSettingsHelper
    {
        public static Order Order
        {
            get => Get<Order>();

            set => Preferences.Set(nameof(Order), JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        public static T Get<T>() => JsonConvert.DeserializeObject<T>(Preferences.Get(nameof(T), string.Empty));

        public static void Set<T>(T value) => Preferences.Set(nameof(T), JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
    }
}
