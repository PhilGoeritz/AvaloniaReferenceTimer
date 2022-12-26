using System.Collections.Generic;
using System.Linq;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia;

namespace ReferenceTimer.Logic
{
    public interface IOpenFilesDialogAdapter
    {
        string OpenFile();
        IList<string> OpenFiles();
    }

    internal class OpenFilesDialogAdapter : IOpenFilesDialogAdapter
    {
        public string OpenFile()
        {
            // ToDo: Replace with service locator
            var applicationLifetime = Application.Current?.ApplicationLifetime
                as IClassicDesktopStyleApplicationLifetime;

            if (applicationLifetime == null)
                return string.Empty;

            var dialog = new OpenFileDialog();
            return dialog.ShowAsync(applicationLifetime.MainWindow)?.Result?.FirstOrDefault()
                ?? string.Empty;
        }

        public IList<string> OpenFiles()
        {
            // ToDo: Replace with service locator
            var applicationLifetime = Application.Current?.ApplicationLifetime
                as IClassicDesktopStyleApplicationLifetime;

            if (applicationLifetime == null)
                return new List<string>();

            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = true;

            return dialog.ShowAsync(applicationLifetime.MainWindow)?.Result?.ToList()
                ?? new List<string>();
        }
    }
}
