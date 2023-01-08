using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
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

        private TimerState _interruptTimerState;

        [Reactive]
        public string CurrentImagePath { get; private set; }

        [Reactive]
        public string SecondCounter { get; private set; }

        [Reactive]
        public TimerState TimerState { get; private set; }

        [Reactive]
        public bool AreSettingsOpen { get; set; }

        public ISettingsViewModel Settings { get; }

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand PlayPauseTimerCommand { get; }
        public ICommand StopTimerCommand { get; }

        public ReferencerViewModel(
            IReferenceContainer referenceContainer,
            Func<uint, ITimer> timerFactory,
            IReferenceContainerIterator referenceContainerIterator,
            ISettingsViewModel settingsViewModel)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));
            _timer = timerFactory?.Invoke(30)
                ?? throw new ArgumentNullException(nameof(timerFactory));
            _referenceContainerIterator = referenceContainerIterator
                ?? throw new ArgumentNullException(nameof(referenceContainerIterator));
            Settings = settingsViewModel
                ?? throw new ArgumentNullException(nameof(settingsViewModel));

            CurrentImagePath = _referenceContainer.SelectedReference?.Path ?? string.Empty;

            var anyReferencesAreLoaded = _referenceContainer.References.Connect()
                .Select(_ => _referenceContainer.References.Items.Any());
            var timerButtonsAreEnabled = Observable
                .CombineLatest(
                    anyReferencesAreLoaded,
                    this.WhenAnyValue(x => x.AreSettingsOpen),
                    (refLoaded, settingsOpen) => refLoaded && !settingsOpen);

            NextCommand = ReactiveCommand.Create(NextReference, timerButtonsAreEnabled);
            PreviousCommand = ReactiveCommand.Create(PreviousReference, timerButtonsAreEnabled);

            PlayPauseTimerCommand = ReactiveCommand.Create(_timer.PlayPauseTimer, timerButtonsAreEnabled);
            StopTimerCommand = ReactiveCommand.Create(_timer.StopTimer, timerButtonsAreEnabled);

            Settings.LimitInSeconds = 30;
            SecondCounter = string.Empty;

            Settings
                .WhenAnyValue(x => x.LimitInMinutes, x => x.LimitInSeconds)
                .Subscribe(_ => _timer.LimitInSeconds = Settings.LimitInSeconds + Settings.LimitInMinutes * 60)
                .DisposeWith(_disposables);

            _timer
                .WhenAnyValue(x => x.CountDownInMilliseconds)
                .Subscribe(countDown => SecondCounter = $"{countDown / 60000}:{countDown % 60000 / 1000},{countDown % 1000 / 100}")
                .DisposeWith(_disposables);

            _timer
                .WhenAnyValue(x => x.TimerState)
                .Subscribe(TimerStateChanged)
                .DisposeWith(_disposables);

            _referenceContainer.References
                .Connect()
                .Subscribe(_ => UpdateCurrentImage())
                .DisposeWith(_disposables);

            _referenceContainer
                .WhenAnyValue(x => x.SelectedReference)
                .Subscribe(_ => UpdateCurrentImage())
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.CurrentImagePath)
                .Subscribe(_ => UpdateSelectedReference())
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.AreSettingsOpen)
                .Subscribe(_ => InterruptResumeTimer())
                .DisposeWith(_disposables);
        }

        private void InterruptResumeTimer()
        {
            if (AreSettingsOpen)
            {
                _interruptTimerState = _timer.TimerState;
                if (_interruptTimerState == TimerState.Running)
                    _timer.PauseTimer();

                return;
            }

            if (_interruptTimerState == TimerState.Running)
                _timer.ResumeTimer();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void UpdateCurrentImage()
        {
            CurrentImagePath = _referenceContainer.SelectedReference?.Path
                ?? _referenceContainer.References.Items.FirstOrDefault()?.Path
                ?? string.Empty;

            if (string.IsNullOrEmpty(CurrentImagePath))
                _timer.StopTimer();
            else
                _timer.ResetTimer();
        }

        private void UpdateSelectedReference()
        {
            _referenceContainer.SelectedReference = _referenceContainer.References.Items
                .SingleOrDefault(reference => reference.Path.Equals(CurrentImagePath));
        }

        private void NextReference()
        {
            CurrentImagePath = _referenceContainerIterator
                .GetNextPath(_referenceContainer, CurrentImagePath);

            _timer.ResetTimer();
        }

        private void PreviousReference()
        {
            CurrentImagePath = _referenceContainerIterator
                .GetPreviousPath(_referenceContainer, CurrentImagePath);

            _timer.ResetTimer();
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
