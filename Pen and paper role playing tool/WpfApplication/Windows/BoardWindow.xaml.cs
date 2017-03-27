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

        private bool captured;
        private Point offset;

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var index = frameworkElements.IndexOf(element);
            BoardViewModel.SelectedElementIndex = index;
            offset = e.GetPosition(element);
            element.CaptureMouse();
            captured = true;
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.ReleaseMouseCapture();
            captured = false;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                var element = sender as FrameworkElement;
                var position = e.GetPosition(this);
                var endposition = Point.Subtract(position, (Vector)offset);
                BoardViewModel.Left = endposition.X;
                BoardViewModel.Top = endposition.Y;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ellipse = new Ellipse()
            {
                Width = 30,
                Height = 30,
                Fill = System.Windows.Media.Brushes.Blue
            };
            ellipse.MouseDown += Ellipse_MouseDown;
            ellipse.MouseUp += Ellipse_MouseUp;
            ellipse.MouseMove += Ellipse_MouseMove;
            canvas.Children.Add(ellipse);
            frameworkElements.Add(ellipse);
        }
    }
}