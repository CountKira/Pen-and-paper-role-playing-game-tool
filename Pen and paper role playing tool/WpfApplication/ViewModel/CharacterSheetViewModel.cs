using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApplication.Annotations;

namespace WpfApplication.ViewModel
{
    public class CharacterSheetViewModel : INotifyPropertyChanged
    {
        public CharacterSheetViewModel()
        {
            Items = new List<Item>
            {
                new Item { Header="Name"},
                new Item { Header="Class"},
                new Item { Header="Race"},
                new Item { Header="Alignment"},
                new Item { Header="Size"},
                new Item { Header="Initiative"},
                new Item { Header="Senses"},
                new Item { Header="Armor Class", Content="10"},
                new Item { Header="Touch", Content="10"},
                new Item { Header="Flat-footed", Content="10"},
                new Item { Header="Hit Points"},
                new Item { Header="Fortitude"},
                new Item { Header="Reflex"},
                new Item { Header="Willpower"},
                new Item { Header="Speed", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Melee", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Ranged", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Special Attacks", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Spells Prepared", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Strength", Content="10"},
                new Item { Header="Dexterity", Content="10"},
                new Item { Header="Constitution", Content="10"},
                new Item { Header="Intelligence", Content="10"},
                new Item { Header="Wisdom", Content="10"},
                new Item { Header="Charisma", Content="10"},
                new Item { Header="Base Attack"},
                new Item { Header="Combat Maneuver Bonus"},
                new Item { Header="Combat Maneuver Defense"},
                new Item { Header="Feats", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Skills", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Special Abilities", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Languages", TextWrapping=TextWrapping.Wrap},
                new Item { Header="Items", TextWrapping=TextWrapping.Wrap  },
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