using ReferenceTimer.Model;
using ReferenceTimer.ViewModels.Files;
using ReferenceTimer.ViewModels.Referencer;
using System;

namespace ReferenceTimer.ViewModels
{
    public interface IMainWindowViewModel : IViewModelBase
    {
    }

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public IFileListViewModel FileList { get; }
        public IReferencerViewModel Referencer { get; }

        public MainWindowViewModel(
            Func<IReferenceContainer, IFileListViewModel> fileListViewModelFactory,
            Func<IReferenceContainer, IReferencerViewModel> referencerViewModelFactory)
        {
            var referenceContainer = new ReferenceContainer();
            FileList = fileListViewModelFactory(referenceContainer);
            Referencer = referencerViewModelFactory(referenceContainer);
        }
    }
}
