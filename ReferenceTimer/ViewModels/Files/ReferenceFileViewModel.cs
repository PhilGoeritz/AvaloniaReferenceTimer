using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReferenceTimer.Model;

namespace ReferenceTimer.ViewModels.Files
{
    public interface IReferenceFileViewModel : IReactiveObject
    {
        bool IsSelected { get; set; }
        string Title { get; }
        IReference Reference { get; }
    }

    internal class ReferenceFileViewModel : ViewModelBase, IReferenceFileViewModel
    {
        [Reactive]
        public bool IsSelected { get; set; }

        public string Title { get; }
        public IReference Reference { get; }

        public ReferenceFileViewModel(IReference reference)
        {
            Title = reference.Title;
            Reference = reference;
        }
    }
}
