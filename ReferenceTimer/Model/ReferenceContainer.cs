using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace ReferenceTimer.Model
{
    public interface IReferenceContainer
    {
        ObservableCollection<IReference> References { get; set; }
    }

    internal class ReferenceContainer : ReactiveObject, IReferenceContainer
    {
        [Reactive]
        public ObservableCollection<IReference> References { get; set; }

        public ReferenceContainer()
        {
            References = new ObservableCollection<IReference>();
        }
    }
}
