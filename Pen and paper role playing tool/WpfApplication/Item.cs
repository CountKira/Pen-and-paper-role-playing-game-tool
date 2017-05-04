using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApplication.Annotations;

namespace WpfApplication
{
    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        private string header;
        private string content;
        private TextWrapping textWrapping = TextWrapping.NoWrap;
        public bool IsTextWrapping => TextWrapping == TextWrapping.Wrap;

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public TextWrapping TextWrapping
        {
            get => textWrapping; set
            {
                textWrapping = value;
                OnPropertyChanged(nameof(TextWrapping));
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}