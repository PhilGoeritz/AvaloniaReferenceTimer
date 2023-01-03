using Autofac;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;
using ReferenceTimer.ViewModels;
using ReferenceTimer.ViewModels.Files;
using ReferenceTimer.ViewModels.Referencer;

namespace ReferenceTimer
{
    internal class ComponentRegistry
    {
        public ContainerBuilder RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<OpenFilesDialogAdapter>().As<IOpenFilesDialogAdapter>().SingleInstance();

            builder.RegisterType<Timer>().As<ITimer>().InstancePerDependency();
            builder.RegisterType<ReferenceContainerIterator>().As<IReferenceContainerIterator>().SingleInstance();

            // DTO
            builder.RegisterType<Reference>().As<IReference>().InstancePerDependency();

            // ViewModels
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<FileListViewModel>().As<IFileListViewModel>().InstancePerDependency();
            builder.RegisterType<ReferenceFileViewModel>().As<IReferenceFileViewModel>().InstancePerDependency();
            builder.RegisterType<ReferencerViewModel>().As<IReferencerViewModel>().InstancePerDependency();

            return builder;
        }
    }
}
