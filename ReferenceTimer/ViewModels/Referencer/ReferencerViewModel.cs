using System;
using System.Linq;
using System.Reactive.Disposables;

using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels.Referencer
{
    public interface IReferencerViewModel : IDisposable
    {
        string CurrentImagePath { get; }
    }

    internal sealed class ReferencerViewModel : ViewModelBase, IReferencerViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IReferenceContainer _referenceContainer;

        [Reactive]
        public string CurrentImagePath { get; private set; }

        public ReferencerViewModel(IReferenceContainer referenceContainer)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;

            _referenceContainer.References
                .Connect()
                .Subscribe(_ => UpdateCurrentImage())
                .DisposeWith(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void UpdateCurrentImage()
        {
            if (_referenceContainer.References.Items.Any(reference => reference.Path.Equals(CurrentImagePath)))
                return;

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;
        }
    }
}
