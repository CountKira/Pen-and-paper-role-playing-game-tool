using DCOM.WPF.MVVM;
using Pen_and_paper_role_playing_tool;

namespace WpfApplication.ViewModel
{
    internal interface IClientServerViewModel : IDialogRequestClose
    {
        IClientServer ClientServer { get; set; }
        string ChatName { get; set; }
    }
}