using MVVM_Framework;
using TCP_Framework;

namespace WpfApplication.ViewModel
{
    internal interface IClientServerViewModel : IDialogRequestClose
    {
        IClientServer ClientServer { get; set; }
        string ChatName { get; set; }
    }
}