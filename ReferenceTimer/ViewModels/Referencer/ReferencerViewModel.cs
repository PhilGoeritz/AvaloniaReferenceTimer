using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        [Reactive]
        public int SecondCounter { get; private set; }

        public ReferencerViewModel(IReferenceContainer referenceContainer)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;
            SecondCounter = 0;

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

        private void TriggerInterval()
        {
            SecondCounter = 0;

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => SecondCounter++)
                .DisposeWith(_disposables);
        }
    }
}
