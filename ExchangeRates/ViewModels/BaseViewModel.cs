using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExchangeRates.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected bool IsBusy;
        protected bool IsInitialized;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}