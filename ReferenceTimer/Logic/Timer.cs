using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReferenceTimer.Logic
{
    public interface ITimer : IDisposable
    {
        uint CountDownInMilliseconds { get; }
        uint LimitInSeconds { get; set; }
        TimerState TimerState { get; }

        void PlayPauseTimer();
        void PauseTimer();
        void ResumeTimer();
        void StopTimer();
        void ResetTimer();
    }

    internal sealed class Timer : ReactiveObject, ITimer
    {
        private CompositeDisposable _timerDisposables = new CompositeDisposable();

        [Reactive]
        public uint CountDownInMilliseconds { get; private set; }

        [Reactive]
        public uint LimitInSeconds { get; set; }

        [Reactive]
        public TimerState TimerState { get; private set; } = TimerState.Stopped;

        public Timer(uint limitInSeconds)
        {
            LimitInSeconds = limitInSeconds;
            CountDownInMilliseconds = LimitInSeconds * 1000;
        }

        public void Dispose()
        {
            _timerDisposables.Dispose();
        }

        public void PlayPauseTimer()
        {
            switch (TimerState)
            {
                case TimerState.Stopped:
                case TimerState.Finished:
                case TimerState.Paused:
                    ResumeTimer();
                    break;
                case TimerState.Running:
                    PauseTimer();
                    break;
            }
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

        public void ResetTimer()
        {
            var timerState = TimerState;

            StopTimer();

            if (timerState == TimerState.Running)
                ResumeTimer();
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
