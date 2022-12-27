using ReferenceTimer.Model;
using System;

namespace ReferenceTimer.ViewModels
{
    public interface IMainWindowViewModel : IViewModelBase
    {
        IViewModelBase Content { get; }
    }

    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly IReferenceContainer _referenceContainer;

        public IViewModelBase Content { get; }

        public MainWindowViewModel(
            Func<IReferenceContainer, IFileListViewModel> fileListViewModelFactory)
        {
            _referenceContainer = new ReferenceContainer();
            Content = fileListViewModelFactory(_referenceContainer);
        }
    }
}
