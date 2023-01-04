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

namespace ReferenceTimer.ViewModels.Files
{
    public interface IFileListViewModel : IViewModelBase, IDisposable {}

    internal sealed class FileListViewModel : ViewModelBase, IFileListViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private readonly IOpenFilesDialogAdapter _openFileDialogAdapter;
        private readonly IReferenceContainer _referenceContainer;
        private readonly Func<string, IReference> _referenceFactory;

        public ReadOnlyObservableCollection<IReferenceFileViewModel> ReferenceFiles { get; }

        public IReferenceFileViewModel? SelectedReferenceFile
        {
            get => GetViewModel(_referenceContainer.SelectedReference);
            set
            {
                _referenceContainer.SelectedReference = value == null
                    ? null
                    : value.Reference;

                this.RaisePropertyChanged();
            }
        }

        public ICommand AddReferencesCommand { get; }
        public ICommand RemoveSelectedReferencesCommand { get; }

        public FileListViewModel(
            IReferenceContainer referenceContainer,
            IOpenFilesDialogAdapter openFilesDialogAdapter,
            Func<string, IReference> referenceFactory,
            Func<IReference, IReferenceFileViewModel> referenceFileViewModelFactory)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));
            _openFileDialogAdapter = openFilesDialogAdapter
                ?? throw new ArgumentNullException(nameof(openFilesDialogAdapter));
            _referenceFactory = referenceFactory
                ?? throw new ArgumentNullException(nameof(referenceFactory));

            AddReferencesCommand = ReactiveCommand
                .Create(AddReferences)
                .DisposeWith(_disposables);

            RemoveSelectedReferencesCommand = ReactiveCommand
                .Create(RemoveSelectedReferences)
                .DisposeWith(_disposables);

            _referenceContainer.References
                .Connect()
                .Transform(reference => referenceFileViewModelFactory(reference))
                .Bind(out var referenceFiles)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposables);

            _referenceContainer
                .WhenAnyValue(container => container.SelectedReference)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedReferenceFile)))
                .DisposeWith(_disposables);

            ReferenceFiles = referenceFiles;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void AddReferences()
        {
            var referencePaths = _openFileDialogAdapter.OpenFiles();
            if (referencePaths is null)
                return;

            _referenceContainer.References
                .Edit(innerList => AddReferences(referencePaths, innerList));
        }

        private void AddReferences(
            IEnumerable<string> referencePaths,
            IExtendedList<IReference> referenceList)
        {
            var newReferences = referencePaths
                .Select(path => _referenceFactory(path));

            referenceList.AddRange(newReferences);
        }

        private void RemoveSelectedReferences()
        {
            var selectedReferences = ReferenceFiles
                .Where(reference => reference.IsSelected)
                .Select(reference => reference.Reference);

            _referenceContainer.References.RemoveMany(selectedReferences);
        }

        private IReferenceFileViewModel? GetViewModel(IReference? selectedReference)
        {
            return ReferenceFiles.SingleOrDefault(viewModel => viewModel.Reference == selectedReference);
        }
    }
}
