using MVVM_Framework;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using WpfApplication.ViewModel;

namespace WpfApplication.Windows
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window, IDialog
    {
        private List<FrameworkElement> frameworkElements = new List<FrameworkElement>();
        private BoardViewModel BoardViewModel => DataContext as BoardViewModel;

        public BoardWindow()
        {
            InitializeComponent();
        }

        private FrameworkElement capturedElement;
        private Point offset;

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            Panel.SetZIndex(element, 1);
            offset = e.GetPosition(element);
            element.CaptureMouse();
            capturedElement = element;
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            Panel.SetZIndex(element, 0);
            element.ReleaseMouseCapture();
            capturedElement = null;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (capturedElement == sender)
            {
                var position = e.GetPosition(this);
                var endposition = Point.Subtract(position, (Vector)offset);
                BoardViewModel.SetNewPosition(capturedElement.DataContext, endposition);
            }
        }
    }
}