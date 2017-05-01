using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MVVM_Framework;
using WpfApplication.Annotations;

namespace WpfApplication.ViewModel
{
    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        private string header;
        private string content;

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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class CharacterSheetViewModel : INotifyPropertyChanged
    {
        public CharacterSheetViewModel()
        {
            Items = new List<Item>
            {
                new Item { Header="Name", Content="" },
                new Item { Header="Class", Content="" },
                new Item { Header="Armor Class", Content="" },
                new Item { Header="Strength", Content="10" },
                new Item { Header="Dexterity", Content="10" },
                new Item { Header="Constitution", Content="10" },
                new Item { Header="Wisdom", Content="10" },
                new Item { Header="Intelligence", Content="10" },
                new Item { Header="Charisma", Content="10" },
                new Item { Header="Items", Content="" },
            };
            foreach (var item in Items)
            {
                item.PropertyChanged += (s, e) =>
                {
                    PropertyChanged?.Invoke(s, e);
                };
            }
        }

        public List<Item> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void SetItem(Item item)
        {
            var index = Items.FindIndex(i => i.Header == item.Header);
            if (index < 0) return;
            Items[index].Content = item.Content;
        }
    }
}