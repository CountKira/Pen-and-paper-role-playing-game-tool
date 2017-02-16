﻿using DCOM.WPF.MVVM;
using Pen_and_paper_role_playing_tool;
using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace WpfApplication
{
	class ServerSetupViewModel : INotifyPropertyChanged, IDialogRequestClose
	{
		public Servers Servers { get; set; }
		private string chatName;
		public string ChatName
		{
			get => chatName;
			set { chatName = value; OnPropertyChanged(nameof(ChatName)); }
		}

		private string ipAddresses;
		public string IpAddresses { get => ipAddresses; set { ipAddresses = value; OnPropertyChanged(nameof(IpAddresses)); } }
		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public ICommand ConnectCommand { get; set; }
		public ICommand CancelCommand { get; set; }
		public ServerSetupViewModel()
		{
			ConnectCommand = new ActionCommand(Connect_Click);
			CancelCommand = new ActionCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
			var addresses = Dns.GetHostAddresses(Dns.GetHostName());
			var stringBuilder = new StringBuilder();
			foreach (var address in addresses)
				stringBuilder.Append(address.ToString()).Append(Environment.NewLine);
			ipAddresses = stringBuilder.ToString();
		}

		private void Connect_Click(object sender)
		{
			//TODO: Add an entry into the UPnP when selecting over Internet.
			Servers = new Servers();
			CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
		}
		public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
		public event PropertyChangedEventHandler PropertyChanged;
	}
}