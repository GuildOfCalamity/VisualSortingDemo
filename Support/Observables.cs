using System;
using System.Collections;
using System.ComponentModel;

namespace VisualSortingItems
{
    /// <summary>
    /// Helper class for binding observable properties in XAML.
    /// </summary>
    public class Observables : INotifyPropertyChanged
    {
        private bool _isBusy;
        private bool _isNotBusy;

        public Observables()
        {
            _isBusy = false;
            _isNotBusy = true;
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                _isNotBusy = !value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool IsNotBusy
        {
            get { return _isNotBusy; }
            set
            {
                _isNotBusy = value;
                _isBusy = !value;
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public override string ToString() => $"{nameof(IsBusy)}: {IsBusy},  {nameof(IsNotBusy)}: {IsNotBusy}";
    }
}
