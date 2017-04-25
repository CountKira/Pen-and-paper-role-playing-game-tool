using Microsoft.Win32;

namespace WpfApplication.ViewModel
{
    internal static class PictureChanger
    {
        public static (bool, string) ChangePictureUrl()
        {
            var dialog = new OpenFileDialog { Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg" };
            var result = dialog.ShowDialog() ?? false;
            return (result, dialog.FileName);
        }
    }
}