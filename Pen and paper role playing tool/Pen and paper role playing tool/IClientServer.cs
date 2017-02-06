using System.Threading;
using System.Threading.Tasks;

namespace Pen_and_paper_role_playing_tool
{
	public interface IClientServer
	{
		Task<string> ReceiveMessage(CancellationToken token);
		void SendMessage(string input);
	}
}