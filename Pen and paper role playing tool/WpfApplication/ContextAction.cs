using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WpfApplication
{
    public sealed class ContextAction : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get => name;
            set
            {
                if (value == name) return;
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool enabled = true;

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value == enabled) return;
                enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public ICommand Action { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}