using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace WpfApplication
{
    internal static class PictureHelper
    {
        private static string RelativeBasePath { get; } = "Pictures";
        private static string FullBasePath => Path.GetFullPath(RelativeBasePath);

        public static (bool, string) ChangePictureUrl()
        {
            var dialog = new OpenFileDialog { Filter = "BackgroundImageUrl files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg" };
            var result = dialog.ShowDialog() ?? false;
            if (!result) return (false, null);

            var fullPath = Path.GetFullPath(RelativeBasePath);
            var sourceImageUrl = dialog.FileName;
            var destFileName = $"{fullPath}\\{Path.GetFileName(sourceImageUrl)}";
            var directoryName = Path.GetDirectoryName(sourceImageUrl);
            Debug.WriteLine($"{sourceImageUrl} {destFileName}");
            if (!File.Exists(destFileName) && directoryName != fullPath)
                File.Copy(sourceImageUrl, destFileName);
            var fileName = Path.GetFileName(sourceImageUrl);
            return (true, fileName);
        }

        public static string GetFullPictureUrl(string pictureName) => $"{FullBasePath}\\{pictureName}";

        public static byte[] GetPictureData(string pictureName)
        {
            var pictureUrl = GetFullPictureUrl(pictureName);
            return File.ReadAllBytes(pictureUrl);
        }

        public static void SavePicture(byte[] bytes, string imageName)
        {
            File.WriteAllBytes(GetFullPictureUrl(imageName), bytes);
        }
    }
}