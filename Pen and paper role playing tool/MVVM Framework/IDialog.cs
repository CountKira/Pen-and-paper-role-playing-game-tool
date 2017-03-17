using System.Windows;

namespace MVVM_Framework
{
    public interface IDialog
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        Window Owner { get; set; }

        void Close();

        bool? ShowDialog();

        void Show();
    }
}