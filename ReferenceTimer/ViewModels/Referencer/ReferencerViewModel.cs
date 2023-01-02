using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels.Referencer
{
    public enum TimerState
    {
        Stopped,
        Running,
        Paused,
    }

    public interface IReferencerViewModel : IDisposable
    {
        string CurrentImagePath { get; }
    }

    internal sealed class ReferencerViewModel : ViewModelBase, IReferencerViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IReferenceContainer _referenceContainer;

        private CompositeDisposable _timerDisposables = new CompositeDisposable();

        [Reactive]
        public string CurrentImagePath { get; private set; }

        [Reactive]
        public int SecondCounter { get; private set; }

        [Reactive]
        public TimerState TimerState { get; private set; } = TimerState.Stopped;

        [Reactive]
        public int Limit { get; private set; } = 5;

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand PlayPauseTimerCommand { get; }
        public ICommand StopTimerCommand { get; }

        public ReferencerViewModel(IReferenceContainer referenceContainer)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;
            SecondCounter = 0;

            NextCommand = ReactiveCommand.Create(NextReference);
            PreviousCommand = ReactiveCommand.Create(PreviousReference);

            PlayPauseTimerCommand = ReactiveCommand.Create(PlayPauseTimer);
            StopTimerCommand = ReactiveCommand.Create(StopTimer);

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

        private void PlayPauseTimer()
        {
            switch (TimerState)
            {
                case TimerState.Paused:
                    ResumeTimer();
                    break;
                case TimerState.Stopped:
                    ResetAndRunTimer();
                    break;
                case TimerState.Running:
                    PauseTimer();
                    break;
            }
        }

        private void ResetAndRunTimer()
        {
            ResetTimerDisposables();

            SecondCounter = Limit;
            TimerState = TimerState.Running;

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(AdvanceTimer)
                .DisposeWith(_timerDisposables);
        }

        private void PauseTimer()
        {
            ResetTimerDisposables();

            TimerState = TimerState.Paused;
        }

        private void ResumeTimer()
        {
            ResetTimerDisposables();

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(AdvanceTimer)
                .DisposeWith(_timerDisposables);

            TimerState = TimerState.Running;
        }

        private void StopTimer()
        {
            ResetTimerDisposables();

            SecondCounter = Limit;
            TimerState = TimerState.Stopped;
        }

        private void AdvanceTimer(long _)
        {
            SecondCounter--;
            if (SecondCounter > 0)
                return;

            SecondCounter = Limit;
            NextReference();
        }

        private void ResetTimerDisposables()
        {
            _timerDisposables.Dispose();
            _timerDisposables = new CompositeDisposable();
        }

        private void PreviousReference()
        {
            var referenceList = _referenceContainer.References.Items.ToList();

            var reference = referenceList
                .FirstOrDefault(item => item.Path.Equals(CurrentImagePath));
            if (reference is null)
                return;

            var index = referenceList.IndexOf(reference);
            int nextIndex = index == 0
                ? referenceList.Count - 1
                : index - 1;

            CurrentImagePath = referenceList[nextIndex].Path;
            SecondCounter = Limit;
        }

        private void NextReference()
        {
            var referenceList = _referenceContainer.References.Items.ToList();

            var reference = referenceList
                .FirstOrDefault(item => item.Path.Equals(CurrentImagePath));
            if (reference is null)
                return;

            var index = referenceList.IndexOf(reference);
            int nextIndex = index + 1 == referenceList.Count
                ? 0
                : index + 1;

            CurrentImagePath = referenceList[nextIndex].Path;
            SecondCounter = Limit;
        }
    }
}
