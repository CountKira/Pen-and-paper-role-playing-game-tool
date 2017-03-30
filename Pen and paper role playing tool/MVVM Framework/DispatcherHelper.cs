using System;
using System.Windows.Threading;

namespace WpfApplication
{
    public static class DispatcherHelper
    {
        static private Dispatcher currentDispatchter;

        static public void Initialize(Dispatcher dispatcher)
        {
            currentDispatchter = dispatcher;
        }

        static public void Invoke(Action action)
        {
            if (currentDispatchter == null)
                action();
            else
                currentDispatchter?.Invoke(action);
        }
    }
}