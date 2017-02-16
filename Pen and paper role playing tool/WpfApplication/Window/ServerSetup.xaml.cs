using DCOM.WPF.MVVM;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for ServerSetup.xaml
	/// </summary>
	public partial class ServerSetup : Window, IDialog
	{
		public ServerSetup()
		{
			InitializeComponent();
		}
	}
}