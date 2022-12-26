using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ReferenceTimer.ViewModels
{
    internal class FileListViewModel : ViewModelBase
    {
        private readonly IOpenFilesDialogAdapter _openFileDialogAdapter;

        [Reactive]
        public IReadOnlyList<IReferenceFileViewModel> ReferenceFiles { get; set; }

        public ICommand LoadReferencesCommand { get; }

        public FileListViewModel()
        {
            _openFileDialogAdapter = new OpenFilesDialogAdapter();

            ReferenceFiles = new List<IReferenceFileViewModel>();

            LoadReferencesCommand = ReactiveCommand.Create(LoadReferences);
        }

        private void LoadReferences()
        {
            var referencePaths = _openFileDialogAdapter.OpenFiles();
            if (referencePaths is null)
                return;

            ReferenceFiles = referencePaths
                .Select(path => new Reference(path))
                .Select(reference => new ReferenceFileViewModel(reference))
                .ToList();
        }
    }
}
