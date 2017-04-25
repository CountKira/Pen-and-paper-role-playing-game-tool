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
            var element = sender as FrameworkElement;
            element?.ReleaseMouseCapture();
            capturedElement = null;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ReferenceEquals(capturedElement, sender)) return;
            var position = e.GetPosition(this);
            var endposition = Point.Subtract(position, (Vector)offset);
            var tableElement = capturedElement.DataContext as TableElement;
            var dataContext = DataContext as TableViewModel;
            dataContext?.SetTableElementPosition(endposition, tableElement);
        }
    }
}