using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Scanner.Constants;
using System.Windows.Input;

namespace Scanner.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public ICommand LoadedCommand =>
            _loadedCommand ?? (_loadedCommand = new DelegateCommand(OnLoaded));

        public ICommand UnloadedCommand =>
            _unloadedCommand ?? (_unloadedCommand = new DelegateCommand(OnUnloaded));

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private void OnLoaded()
        {
            _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
            RequestNavigate(PageKeys.SelectDisk);
        }

        private void OnUnloaded()
        {
            _regionManager.Regions.Remove(Regions.Main);
        }

        private void RequestNavigate(string target)
        {
            if (_navigationService.CanNavigate(target))
            {
                _navigationService.RequestNavigate(target);
            }
        }
    }
}
