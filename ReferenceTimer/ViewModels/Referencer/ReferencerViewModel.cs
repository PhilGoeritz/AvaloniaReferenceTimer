using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels.Referencer
{
    public interface IReferencerViewModel : IDisposable {}

    internal sealed class ReferencerViewModel : ViewModelBase, IReferencerViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IReferenceContainer _referenceContainer;
        private readonly IReferenceContainerIterator _referenceContainerIterator;
        private readonly ITimer _timer;

        [Reactive]
        public string CurrentImagePath { get; private set; }

        [Reactive]
        public int SecondCounter { get; private set; }

        [Reactive]
        public TimerState TimerState { get; private set; }

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand PlayPauseTimerCommand { get; }
        public ICommand StopTimerCommand { get; }

        public ReferencerViewModel(
            IReferenceContainer referenceContainer,
            Func<int, ITimer> timerFactory,
            IReferenceContainerIterator referenceContainerIterator)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));
            _timer = timerFactory?.Invoke(5)
                ?? throw new ArgumentNullException(nameof(timerFactory));
            _referenceContainerIterator = referenceContainerIterator
                ?? throw new ArgumentNullException(nameof(referenceContainerIterator));

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;
            SecondCounter = 0;

            NextCommand = ReactiveCommand.Create(NextReference);
            PreviousCommand = ReactiveCommand.Create(PreviousReference);

            PlayPauseTimerCommand = ReactiveCommand.Create(PlayPauseTimer);
            StopTimerCommand = ReactiveCommand.Create(_timer.StopTimer);

            _timer
                .WhenAnyValue(x => x.CountDownInMilliseconds)
                .Subscribe(countDown => SecondCounter = countDown)
                .DisposeWith(_disposables);

            _timer
                .WhenAnyValue(x => x.TimerState)
                .Subscribe(TimerStateChanged)
                .DisposeWith(_disposables);

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
            if (_referenceContainer.References.Items
                    .Any(reference => reference.Path.Equals(CurrentImagePath)))
                return;

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path
                ?? string.Empty;
        }

        private void PlayPauseTimer()
        {
            switch (TimerState)
            {
                case TimerState.Stopped:
                case TimerState.Finished:
                case TimerState.Paused:
                    _timer.ResumeTimer();
                    break;
                case TimerState.Running:
                    _timer.PauseTimer();
                    break;
            }
        }

        private void NextReference()
        {
            CurrentImagePath = _referenceContainerIterator
                .GetNextPath(_referenceContainer, CurrentImagePath);

            ResetTimer();
        }

        private void PreviousReference()
        {
            CurrentImagePath = _referenceContainerIterator
                .GetPreviousPath(_referenceContainer, CurrentImagePath);

            ResetTimer();
        }

        private void ResetTimer()
        {
            var timerState = _timer.TimerState;

            _timer.StopTimer();

            if (timerState == TimerState.Running)
                _timer.ResumeTimer();
        }

        private void TimerStateChanged(TimerState _)
        {
            if (_timer.TimerState == TimerState.Finished)
            {
                NextReference();
                _timer.ResumeTimer();
            }

            TimerState = _timer.TimerState;
        }
    }
}
