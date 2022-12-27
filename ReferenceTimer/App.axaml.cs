using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReferenceTimer.ViewModels;
using ReferenceTimer.Views;

namespace ReferenceTimer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var componentRegristry = new ComponentRegistry();

            var container = componentRegristry
                .RegisterComponents(new ContainerBuilder())
                .Build();

            container.BeginLifetimeScope();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = container.Resolve<IMainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
