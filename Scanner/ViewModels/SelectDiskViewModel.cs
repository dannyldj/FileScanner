using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Scanner.Constants;
using Scanner.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Scanner.ViewModels
{
    public class SelectDiskViewModel : BindableBase, INavigationAware
    {
        private IRegionNavigationService _navigationService;
        private DelegateCommand<DiskModel> _selectCommand;

        public ObservableCollection<DiskModel> Disks { get; set; }

        public DelegateCommand<DiskModel> SelectCommand =>
            _selectCommand ?? (_selectCommand = new DelegateCommand<DiskModel>(ExecuteSelectCommand));

        public SelectDiskViewModel(IRegionManager regionManager)
        {
            _navigationService = regionManager.Regions[Regions.Main].NavigationService;
            Disks = new ObservableCollection<DiskModel>();
        }

        private void ExecuteSelectCommand(DiskModel diskInfo)
        {
            RequestNavigate(PageKeys.Scan, new NavigationParameters
            {
                { "DiskInfo",  diskInfo }
            });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Disks.Clear();
            try
            {
                DriveInfo.GetDrives().ToList().ForEach(e =>
                {
                    Disks.Add(new DiskModel
                    {
                        DLabel = !string.IsNullOrEmpty(e.VolumeLabel) ? e.VolumeLabel : e.DriveType == DriveType.Removable ? Properties.Resources.DefaultPortableDisk : Properties.Resources.DefaultDisk,
                        DName = e.Name.Replace("\\", ""),
                        DUsage = 100 - (double)e.AvailableFreeSpace / e.TotalSize * 100,
                        IsPortable = e.DriveType == DriveType.Removable
                    });
                });
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        private void RequestNavigate(string target, NavigationParameters param)
        {
            if (_navigationService.CanNavigate(target))
            {
                _navigationService.RequestNavigate(target, param);
            }
        }
    }
}
