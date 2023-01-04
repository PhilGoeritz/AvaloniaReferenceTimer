using System.Windows.Input;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReferenceTimer.ViewModels.Referencer
{
    public interface ISettingsViewModel
    {
        uint LimitInMinutes { get; set; }
        uint LimitInSeconds { get; set; }
    }

    internal sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
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
            Add30SecondsCommand = ReactiveCommand.Create(() => AddSecconds(30));
            Add60SecondsCommand = ReactiveCommand.Create(() => AddSecconds(60));
            Add300SecondsCommand = ReactiveCommand.Create(() => AddSecconds(300));

            Remove30SecondsCommand = ReactiveCommand.Create(() => RemoveSecconds(30));
            Remove60SecondsCommand = ReactiveCommand.Create(() => RemoveSecconds(60));
            Remove300SecondsCommand = ReactiveCommand.Create(() => RemoveSecconds(300));
        }

        void AddSecconds(uint seconds)
        {
            var total = LimitInMinutes * 60 + LimitInSeconds;

            var newTotal = total + seconds;

            LimitInMinutes = newTotal / 60;
            LimitInSeconds = newTotal % 60;
        }

        void RemoveSecconds(uint seconds)
        {
            var total = LimitInMinutes * 60 + LimitInSeconds;

            var newTotal = total - seconds;

            LimitInMinutes = newTotal / 60;
            LimitInSeconds = newTotal % 60;
        }
    }
}
