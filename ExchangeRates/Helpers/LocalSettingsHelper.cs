using ExchangeRates.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Diploma.Helpers
{
    public static class LocalSettingsHelper
    {
        public static User User
        {
            get => JsonConvert.DeserializeObject<User>(Preferences.Get(nameof(User), string.Empty));

            set => Preferences.Set(nameof(User), JsonConvert.SerializeObject(value,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
