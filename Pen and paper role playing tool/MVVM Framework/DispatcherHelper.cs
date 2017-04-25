using System;
using System.Windows.Threading;

namespace WpfApplication
{
    public static class DispatcherHelper
    {
        private static Dispatcher currentDispatchter;

        public static void Initialize(Dispatcher dispatcher)
        {
            currentDispatchter = dispatcher;
        }

        public static void Invoke(Action action)
        {
            if (currentDispatchter == null)
                action();
            else
                currentDispatchter?.Invoke(action);
        }
    }
}