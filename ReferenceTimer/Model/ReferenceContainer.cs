using System;

using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReferenceTimer.Model
{
    public interface IReferenceContainer : IDisposable
    {
        ISourceList<IReference> References { get; }
        IReference? SelectedReference { get; set; }
    }

    internal sealed class ReferenceContainer : ReactiveObject, IReferenceContainer
    {
        public ISourceList<IReference> References { get; }

        [Reactive]
        public IReference? SelectedReference { get; set; }

        public ReferenceContainer()
        {
            References = new SourceList<IReference>();
        }

        public void Dispose()
        {
            References.Dispose();
        }
    }
}
