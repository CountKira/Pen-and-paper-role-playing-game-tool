using Pen_and_paper_role_playing_tool;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//TODO: Check all check(Exception) blocks to prevent harmful exceptions from not being detected
		//TODO: Messages send to the server in quick succession are printed on the same line
		public MainWindow()
		{
			InitializeComponent();
		}
	}
}