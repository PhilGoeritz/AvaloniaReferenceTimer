using ReactiveUI;

namespace ReferenceTimer.ViewModels
{
    public interface IViewModelBase
        : IReactiveObject, IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors
    { }

    public class ViewModelBase : ReactiveObject, IViewModelBase
    {
    }
}
