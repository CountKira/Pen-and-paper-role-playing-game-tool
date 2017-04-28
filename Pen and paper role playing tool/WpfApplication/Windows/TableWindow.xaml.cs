using MVVM_Framework;
using System.Windows;
using System.Windows.Input;
using WpfApplication.ViewModel;

namespace WpfApplication.Windows
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow : IDialog
    {
        public TableWindow()
        {
            InitializeComponent();
        }

        private FrameworkElement capturedElement;
        private Point offset;

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            offset = e.GetPosition(element);
            element?.CaptureMouse();
            capturedElement = element;
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            capturedElement?.ReleaseMouseCapture();
            capturedElement = null;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ReferenceEquals(capturedElement, sender)) return;
            var position = e.GetPosition(Canvas);
            var endposition = Point.Subtract(position, (Vector)offset);
            var tableElement = capturedElement.DataContext as TableElement;
            TableViewModel.SetTableElementPosition(endposition, tableElement);
        }
    }
}