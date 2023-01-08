using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReferenceTimer.ViewModels.Referencer
{
    public interface ISettingsViewModel : IDisposable
    {
        uint LimitInMinutes { get; set; }
        uint LimitInSeconds { get; set; }
    }

    internal sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Reactive]
        public uint LimitInMinutes { get; set; }

        [Reactive]
        public uint LimitInSeconds { get; set; }

        public ICommand Add30SecondsCommand { get; }
        public ICommand Add60SecondsCommand { get; }
        public ICommand Add300SecondsCommand { get; }

        public ICommand Remove30SecondsCommand { get; }
        public ICommand Remove60SecondsCommand { get; }
        public ICommand Remove300SecondsCommand { get; }

        public SettingsViewModel()
        {
            var limitObservable = this.WhenAnyValue(x => x.LimitInMinutes, x => x.LimitInSeconds);
            var canMinus30SecondsObservable = limitObservable
                .Select(_ => LimitInMinutes * 60 + LimitInSeconds >= 30);
            var canMinus60SecondsObservable = limitObservable
                .Select(_ => LimitInMinutes * 60 + LimitInSeconds >= 60);
            var canMinus300SecondsObservable = limitObservable
                .Select(_ => LimitInMinutes * 60 + LimitInSeconds >= 300);

            Add30SecondsCommand = ReactiveCommand.Create(() => AddSecconds(30));
            Add60SecondsCommand = ReactiveCommand.Create(() => AddSecconds(60));
            Add300SecondsCommand = ReactiveCommand.Create(() => AddSecconds(300));

            Remove30SecondsCommand = ReactiveCommand
                .Create(() => RemoveSecconds(30), canMinus30SecondsObservable).DisposeWith(_disposables);
            Remove60SecondsCommand = ReactiveCommand
                .Create(() => RemoveSecconds(60), canMinus60SecondsObservable).DisposeWith(_disposables);
            Remove300SecondsCommand = ReactiveCommand
                .Create(() => RemoveSecconds(300), canMinus300SecondsObservable).DisposeWith(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void AddSecconds(uint seconds)
        {
            var total = LimitInMinutes * 60 + LimitInSeconds;

            var newTotal = total + seconds;

            LimitInMinutes = newTotal / 60;
            LimitInSeconds = newTotal % 60;
        }

        private void RemoveSecconds(uint seconds)
        {
            var total = LimitInMinutes * 60 + LimitInSeconds;

            var newTotal = total - seconds;

            LimitInMinutes = newTotal / 60;
            LimitInSeconds = newTotal % 60;
        }
    }
}
