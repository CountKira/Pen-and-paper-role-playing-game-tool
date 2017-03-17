namespace MVVM_Framework
{
    public interface IDialogService
    {
        void Register<TViewModel, TView>() where TView : IDialog;

        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;

        void Show<TViewModel>(TViewModel viewModel);
    }
}