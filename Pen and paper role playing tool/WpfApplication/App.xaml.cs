using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
#if DEBUG
			foreach (var arg in e.Args)
			{
				//TODO: Remove setting the culture with arguments after debug
				try
				{
					Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(arg);
				}
				catch (Exception)
				{
				}
			}
#endif
		}
	}
}
