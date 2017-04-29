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
            if (e.LeftButton == MouseButtonState.Released) return;
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
            var dataContext = DataContext as TableViewModel;
            dataContext?.SetTableElementPosition(endposition, tableElement);
        }

        private Point? viewOffset;

        private void DragView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released) return;
            var frameworkElement = sender as FrameworkElement;
            viewOffset = e.GetPosition(frameworkElement);
            frameworkElement?.CaptureMouse();
        }

        private void DragView_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewOffset == null) return;
            var currentPosition = e.GetPosition(sender as FrameworkElement);
            var distance = Point.Subtract(currentPosition, (Vector)viewOffset);

            SliderX.Value += distance.X;
            SliderY.Value += distance.Y;
            viewOffset = currentPosition;
        }

        private void DragView_MouseUp(object sender, MouseEventArgs e)
        {
            viewOffset = null;
            var frameworkElement = sender as FrameworkElement;
            frameworkElement?.ReleaseMouseCapture();
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double wheelMultiplier = 0.04;
            var delta = (double)e.Delta / 120 * wheelMultiplier;
            Slider.Value += delta;
        }
    }
}