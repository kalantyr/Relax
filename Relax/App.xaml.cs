using System;
using System.Diagnostics;
using System.Windows;

namespace Relax
{
    public partial class App
    {
        public static void ShowError(Exception error)
        {
            var message = error.Message;
            if (Debugger.IsAttached)
                message += Environment.NewLine + Environment.NewLine + error.GetBaseException().StackTrace;
            MessageBox.Show(message, error.GetBaseException().GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
