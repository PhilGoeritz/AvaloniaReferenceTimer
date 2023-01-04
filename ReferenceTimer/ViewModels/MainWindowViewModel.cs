using System;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Model;
using ReferenceTimer.ViewModels.Files;
using ReferenceTimer.ViewModels.Referencer;

namespace ReferenceTimer.ViewModels
{
    public interface IMainWindowViewModel : IViewModelBase
    {
    }

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public IFileListViewModel FileList { get; }
        public IReferencerViewModel Referencer { get; }

        [Reactive]
        public bool FilesAreExpanded { get; private set; }

        public ICommand ExpanderCommand { get; }

        public MainWindowViewModel(
            Func<IReferenceContainer, IFileListViewModel> fileListViewModelFactory,
            Func<IReferenceContainer, IReferencerViewModel> referencerViewModelFactory)
        {
            var referenceContainer = new ReferenceContainer();
            FileList = fileListViewModelFactory(referenceContainer);
            Referencer = referencerViewModelFactory(referenceContainer);

            ExpanderCommand = ReactiveCommand.Create(() => FilesAreExpanded = !FilesAreExpanded);
        }
    }
}
