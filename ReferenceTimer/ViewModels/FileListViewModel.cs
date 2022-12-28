using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using DynamicData;
using ReactiveUI;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels
{
    public interface IFileListViewModel : IViewModelBase
    {
        ICommand LoadReferencesCommand { get; }
        ReadOnlyObservableCollection<IReferenceFileViewModel> ReferenceFiles { get; }
    }

    internal class FileListViewModel : ViewModelBase, IFileListViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IOpenFilesDialogAdapter _openFileDialogAdapter;
        private readonly IReferenceContainer _referenceContainer;
        private readonly Func<string, IReference> _referenceFactory;
        private readonly Func<IReference, IReferenceFileViewModel> _referenceFileViewModelFactory;

        public ReadOnlyObservableCollection<IReferenceFileViewModel> ReferenceFiles { get; }

        public ICommand LoadReferencesCommand { get; }

        public FileListViewModel(
            IReferenceContainer referenceContainer,
            Func<string, IReference> referenceFactory,
            Func<IReference, IReferenceFileViewModel> referenceFileViewModelFactory)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));
            _referenceFactory = referenceFactory
                ?? throw new ArgumentNullException(nameof(referenceFactory));
            _referenceFileViewModelFactory = referenceFileViewModelFactory
                ?? throw new ArgumentNullException(nameof(referenceFileViewModelFactory));

            _openFileDialogAdapter = new OpenFilesDialogAdapter();

            LoadReferencesCommand = ReactiveCommand
                .Create(LoadReferences)
                .DisposeWith(_disposables);

            _referenceContainer.References
                .Connect()
                .Transform(reference => referenceFileViewModelFactory(reference))
                .Bind(out var referenceFiles)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposables);

            ReferenceFiles = referenceFiles;
        }

        private void LoadReferences()
        {
            var referencePaths = _openFileDialogAdapter.OpenFiles();
            if (referencePaths is null)
                return;

            _referenceContainer.References
                .Edit(innerList => OverwriteReferences(referencePaths, innerList));
        }

        private void OverwriteReferences(
            IEnumerable<string> referencePaths,
            IExtendedList<IReference> referenceList)
        {
            var newReferences = referencePaths
                .Select(path => _referenceFactory(path));

            referenceList.Clear();
            referenceList.AddRange(newReferences);
        }
    }
}
