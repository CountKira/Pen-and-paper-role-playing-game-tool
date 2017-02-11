using Pen_and_paper_role_playing_tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for ServerSetup.xaml
	/// </summary>
	public partial class ServerSetup : Window
	{
		public Server Server { get; set; }
		public ServerSetup()
		{
			InitializeComponent();
			var addresses = Dns.GetHostAddresses(Dns.GetHostName());
			var stringBuilder = new StringBuilder();
			foreach (var address in addresses)
			{
				stringBuilder.Append(address.ToString()).Append(Environment.NewLine);
			}
			ipAddresses.Text = stringBuilder.ToString();
			SetIsWorkingVisibility(Visibility.Hidden);
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			//TODO: Add an entry into the UPnP when selecting over internet.
			Server = new Server();
			ConnectToServer(Server);
			SetIsWorkingVisibility(Visibility.Visible);
		}

		private async void ConnectToServer(Server server)
		{
			await server.EstablishConnection(8888);
			SetIsWorkingVisibility(Visibility.Hidden);
			Close();
		}

		private void SetIsWorkingVisibility(Visibility visibility)
		{
			tryingToConnectLabel.Visibility = visibility;
			connectionProgress.Visibility = visibility;
		}
	}
}