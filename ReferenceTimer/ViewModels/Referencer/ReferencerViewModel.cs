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
            Func<int, ITimer> timerFactory)
        {
            _referenceContainer = referenceContainer
                ?? throw new ArgumentNullException(nameof(referenceContainer));
            _timer = timerFactory?.Invoke(5)
                ?? throw new ArgumentNullException(nameof(timerFactory));

            CurrentImagePath = _referenceContainer.References.Items.FirstOrDefault()?.Path ?? string.Empty;
            SecondCounter = 0;

            NextCommand = ReactiveCommand.Create(() => ChangeReference(NextIndex));
            PreviousCommand = ReactiveCommand.Create(() => ChangeReference(PreviousIndex));

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

        private static int NextIndex(int listLength, int index)
        {
            return index + 1 == listLength
                ? 0
                : index + 1;
        }

        private static int PreviousIndex(int listLength, int index)
        {
            return index == 0
                ? listLength - 1
                : index - 1;
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

        private void ChangeReference(Func<int, int, int> getNextIndex)
        {
            var referenceList = _referenceContainer.References.Items.ToList();

            var reference = referenceList
                .FirstOrDefault(item => item.Path.Equals(CurrentImagePath));
            if (reference is null)
                return;

            var index = referenceList.IndexOf(reference);
            int nextIndex = getNextIndex(referenceList.Count, index);

            CurrentImagePath = referenceList[nextIndex].Path;

            if (TimerState == TimerState.Running)
            {
                _timer.StopTimer();
                _timer.ResumeTimer();
            }
        }

        private void TimerStateChanged(TimerState timerState)
        {
            if (_timer.TimerState == TimerState.Finished)
            {
                ChangeReference(NextIndex);
                _timer.ResumeTimer();
            }

            TimerState = _timer.TimerState;
        }
    }
}
