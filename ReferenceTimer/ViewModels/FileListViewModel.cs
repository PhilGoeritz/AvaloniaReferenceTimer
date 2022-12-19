using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceTimer.ViewModels
{
    internal class FileListViewModel
    {
        [Reactive]
        public IReadOnlyList<IReferenceFileViewModel> ReferenceFiles { get; set; }

        public ICommand LoadReferencesCommand { get; }

        public FileListViewModel()
        {
            ReferenceFiles = new List<IReferenceFileViewModel>();

            LoadReferencesCommand = ReactiveCommand.Create(LoadReferences);
        }

        private void LoadReferences()
        {

        }
    }
}
