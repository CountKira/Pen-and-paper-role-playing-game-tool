using System;
using System.Collections.Generic;
using System.Windows;

namespace MVVM_Framework
{
    public class DialogService : IDialogService
    {
        private readonly Window owner;

        public DialogService(Window owner)
        {
            this.owner = owner;
            Mappings = new Dictionary<Type, Type>();
        }

        public IDictionary<Type, Type> Mappings { get; }

        public void Register<TViewModel, TView>() where TView : IDialog
        {
            if (Mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped to type {typeof(TView)}");
            }

            Mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            var viewType = Mappings[typeof(TViewModel)];

            var dialog = (IDialog)Activator.CreateInstance(viewType);

            void Handler(object sender, DialogCloseRequestedEventArgs e)
            {
                viewModel.CloseRequested -= Handler;

                if (e.DialogResult.HasValue)
                    dialog.DialogResult = e.DialogResult;
                else
                    dialog.Close();
            }

            viewModel.CloseRequested += Handler;

            dialog.DataContext = viewModel;
            dialog.Owner = owner;

            return dialog.ShowDialog();
        }

        public void Show<TViewModel>(TViewModel viewModel)
        {
            var viewType = Mappings[typeof(TViewModel)];

            var dialog = (IDialog)Activator.CreateInstance(viewType);

            dialog.DataContext = viewModel;
            dialog.Owner = owner;

            dialog.Show();
        }
    }
}