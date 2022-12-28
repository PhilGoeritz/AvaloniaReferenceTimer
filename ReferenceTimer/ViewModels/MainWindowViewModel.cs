using ReferenceTimer.Model;
using ReferenceTimer.ViewModels.Files;
using System;

namespace ReferenceTimer.ViewModels
{
    public interface IMainWindowViewModel : IViewModelBase
    {
    }

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public IFileListViewModel FileList { get; }

        public MainWindowViewModel(
            Func<IReferenceContainer, IFileListViewModel> fileListViewModelFactory)
        {
            var referenceContainer = new ReferenceContainer();
            FileList = fileListViewModelFactory(referenceContainer);
        }
    }
}
