using System;

using DynamicData;

namespace ReferenceTimer.Model
{
    public interface IReferenceContainer : IDisposable
    {
        ISourceList<IReference> References { get; }
    }

    internal sealed class ReferenceContainer : IReferenceContainer
    {
        public ISourceList<IReference> References { get; }

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
