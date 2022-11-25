using System;
using System.Windows;
using System.Windows.Threading;

namespace Scanner.Services
{
    public class DispatcherService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current?.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.Invoke(action);
        }
    }
}
