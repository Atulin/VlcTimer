using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VlcTimer.Models
{
    public class VideoTimer : INotifyPropertyChanged
    {
        private string _elapsed = "00:00.000";

        public string Elapsed
        {
            get => _elapsed;
            set
            {
                _elapsed = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}