using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReferenceTimer.Logic
{
    public interface ITimer : IDisposable
    {
        int CountDownInMilliseconds { get; }
        int LimitInSeconds { get; set; }
        TimerState TimerState { get; }
        
        void PauseTimer();
        void ResumeTimer();
        void StopTimer();
    }

    internal sealed class Timer : ReactiveObject, ITimer
    {
        private CompositeDisposable _timerDisposables = new CompositeDisposable();

        [Reactive]
        public int CountDownInMilliseconds { get; private set; }

        [Reactive]
        public int LimitInSeconds { get; set; }

        [Reactive]
        public TimerState TimerState { get; private set; } = TimerState.Stopped;

        public Timer(int limitInSeconds)
        {
            LimitInSeconds = limitInSeconds;
            CountDownInMilliseconds = LimitInSeconds * 1000;
        }

        public void Dispose()
        {
            _timerDisposables.Dispose();
        }

        public void PauseTimer()
        {
            ResetTimerDisposables();

            TimerState = TimerState.Paused;
        }

        public void ResumeTimer()
        {
            ResetTimerDisposables();

            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(AdvanceTimer)
                .DisposeWith(_timerDisposables);

            TimerState = TimerState.Running;
        }

        public void StopTimer()
        {
            ResetTimerDisposables();

            CountDownInMilliseconds = LimitInSeconds * 1000;
            TimerState = TimerState.Stopped;
        }

        private void AdvanceTimer(long _)
        {
            CountDownInMilliseconds -= 100;
            if (CountDownInMilliseconds > 0)
                return;

            ResetTimerDisposables();
            CountDownInMilliseconds = LimitInSeconds * 1000;
            TimerState = TimerState.Finished;
        }

        private void ResetTimerDisposables()
        {
            _timerDisposables.Dispose();
            _timerDisposables = new CompositeDisposable();
        }
    }
}
