using Pen_and_paper_role_playing_tool;
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

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for ClientSetup.xaml
	/// </summary>
	public partial class ClientSetup : Window
	{
		public string ChatName { get; set; }
		public Client Client { get; set; }

		public ClientSetup()
		{
			InitializeComponent();
		}

		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			var client = new Client(8888, ipAddressBox.Text);
			if (client.TryConnectingToServer())
			{
				Client = client;
				ChatName = chatName.Text;
				Close();
			}
			else
			{
				MessageBox.Show("Could not establish a connection with the server.");
			}

		}
	}
}
