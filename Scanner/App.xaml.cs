using Prism.Unity;
using System.Windows;
using Scanner.Views;
using Prism.Ioc;
using Scanner.ViewModels;
using Scanner.Constants;

namespace Scanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public App() { }

        protected override Window CreateShell() => Container.Resolve<ShellWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectDiskPage, SelectDiskViewModel>(PageKeys.SelectDisk);
            containerRegistry.RegisterForNavigation<ScanPage, ScanViewModel>(PageKeys.Scan);
            containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();
        }
    }
}
