using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public interface IClientServer
	{
		EventHandler<WriterEventArgs> Writer { get; set; }
		void SendMessage(string input);
	}
}