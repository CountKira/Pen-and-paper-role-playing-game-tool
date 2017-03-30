using MVVM_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApplication.ViewModel;

namespace WpfApplication.Windows
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window, IDialog
    {
        public TableWindow()
        {
            InitializeComponent();
        }

        private object sender;
        private Point offset;

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            offset = e.GetPosition(element);
            element.CaptureMouse();
            this.sender = sender;
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            element.ReleaseMouseCapture();
            this.sender = null;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.sender == sender)
            {
                var element = sender as FrameworkElement;
                var position = e.GetPosition(this);
                var endposition = Point.Subtract(position, (Vector)offset);
                var tableElement = element.DataContext as TableElement;
                var dataContext = DataContext as TableViewModel;
                dataContext.SetTableElementPosition(endposition, tableElement);
            }
        }
    }
}