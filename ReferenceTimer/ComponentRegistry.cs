using Autofac;
using ReferenceTimer.Model;
using ReferenceTimer.ViewModels;

namespace ReferenceTimer
{
    internal class ComponentRegistry
    {
        public ContainerBuilder RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<Reference>().As<IReference>().InstancePerDependency();

            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<FileListViewModel>().As<IFileListViewModel>().InstancePerDependency();
            builder.RegisterType<ReferenceFileViewModel>().As<IReferenceFileViewModel>().InstancePerDependency();

            return builder;
        }
    }
}
