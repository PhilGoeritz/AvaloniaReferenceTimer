using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels
{
    public interface IFileListViewModel : IViewModelBase
    {
        ICommand LoadReferencesCommand { get; }
        ObservableCollection<IReferenceFileViewModel> ReferenceFiles { get; }
    }

    internal class FileListViewModel : ViewModelBase, IFileListViewModel
    {
        private readonly IOpenFilesDialogAdapter _openFileDialogAdapter;
        private readonly IReferenceContainer _referenceContainer;

        public ObservableCollection<IReferenceFileViewModel> ReferenceFiles { get; }

        public ICommand LoadReferencesCommand { get; }

        public FileListViewModel(
            IReferenceContainer referenceContainer,
            Func<string, IReference> referenceFactory,
            Func<IReference, IReferenceFileViewModel> referenceFileViewModelFactory)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));

            _openFileDialogAdapter = new OpenFilesDialogAdapter();

            ReferenceFiles = new ObservableCollection<IReferenceFileViewModel>();

            //_referenceContainer.References.CollectionChanged.Subscribe();

            LoadReferencesCommand = ReactiveCommand.Create(LoadReferences);
        }

        private void LoadReferences()
        {
            var referencePaths = _openFileDialogAdapter.OpenFiles();
            if (referencePaths is null)
                return;

            _referenceContainer.References.Clear();

            referencePaths
                .Select(path => new Reference(path))
                .ToList()
                .ForEach(_referenceContainer.References.Add);
        }
    }
}
