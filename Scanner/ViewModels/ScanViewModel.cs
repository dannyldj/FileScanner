using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Scanner.Models;
using Scanner.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scanner.ViewModels
{
    public class ScanViewModel : BindableBase, INavigationAware
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private int _scannedAmount;
        private int _failToScanAmount;
        private ExtensionModel _selectedExtension;
        private DelegateCommand _stopCommand;
        private DelegateCommand<ExtensionModel> _selectCommand;
        private DelegateCommand<string> _openViewerCommand;

        public DiskModel SelectedDisk { get; set; }

        public int ScannedAmount
        {
            get { return _scannedAmount; }
            set { SetProperty(ref _scannedAmount, value); }
        }

        public int FailToScanAmount
        {
            get { return _failToScanAmount; }
            set { SetProperty(ref _failToScanAmount, value); }
        }

        public ExtensionModel SelectedExtension
        {
            get { return _selectedExtension; }
            set { SetProperty(ref _selectedExtension, value); }
        }

        public List<ExtensionModel> Extensions { get; set; }

        public IEnumerable<ExtensionModel> TopExtensions =>
            Extensions.OrderByDescending(e => e.Count).Take(10).Select(e =>
            {
                e.Ratio = (double)e.Count / ScannedAmount * 100;
                return e;
            });

        public ObservableCollection<Image> Images { get; set; }

        public DelegateCommand StopCommand =>
            _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand));

        public DelegateCommand<ExtensionModel> SelectCommand =>
            _selectCommand ?? (_selectCommand = new DelegateCommand<ExtensionModel>(ExecuteSelectCommand));

        public DelegateCommand<string> OpenViewerCommand =>
            _openViewerCommand ?? (_openViewerCommand = new DelegateCommand<string>(ExecuteOpenViewerCommand));

        public ScanViewModel()
        {
            Extensions = new List<ExtensionModel>();
            Images = new ObservableCollection<Image>();
        }

        private void ExecuteStopCommand()
        {
            _tokenSource.Cancel();
        }

        private void ExecuteSelectCommand(ExtensionModel extension)
        {
            if (extension.IsSelected)
            {
                extension.IsSelected = false;
                SelectedExtension = null;
            }
            else if (SelectedExtension != null)
            {
                SelectedExtension.IsSelected = false;
                extension.IsSelected = true;
                SelectedExtension = extension;
            }
            else
            {
                extension.IsSelected = true;
                SelectedExtension = extension;
            }
        }

        void ExecuteOpenViewerCommand(string fileName)
        {
            Process.Start(Path.GetDirectoryName(fileName));
        }

        private void GetAllExtensions(string root, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var files = Directory.GetFiles(root);
                foreach (var file in files)
                {
                    var imageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
                    var ext = Path.GetExtension(file);

                    var ex = Extensions.Where(e => e.Name == ext).FirstOrDefault();
                    if (imageExtensions.Contains(ext))
                    {
                        Task.Run(() =>
                        {
                            var thumbnail = Image.FromFile(file).GetThumbnailImage(64, 64, null, IntPtr.Zero);

                            DispatcherService.Invoke(() =>
                            {
                                if (Images.Count == 10)
                                {
                                    Images.RemoveAt(9);
                                }

                                Images.Insert(0, thumbnail);
                            });
                        });
                    }

                    if (ex != null)
                    {
                        ex.Count++;
                        ex.TotalSize += new FileInfo(file).Length;
                        DispatcherService.Invoke(() =>
                        {
                            ex.Files.Add(file);
                        });
                    }
                    else
                    {
                        Extensions.Add(new ExtensionModel
                        {
                            Name = ext,
                            Count = 1,
                            TotalSize = new FileInfo(file).Length,
                            Files = new ObservableCollection<string> { file },
                        });
                    }

                    ScannedAmount++;
                }
            }
            catch (IOException)
            {
                FailToScanAmount++;
            }
            catch (UnauthorizedAccessException)
            {
                FailToScanAmount++;
            }

            try
            {
                var directories = Directory.GetDirectories(root);
                if (directories.Length > 0)
                {
                    foreach (var dir in directories)
                    {
                        GetAllExtensions(dir, token);
                    }
                }
            }
            catch (IOException)
            {
                FailToScanAmount++;
            }
            catch (UnauthorizedAccessException)
            {
                FailToScanAmount++;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedDisk = navigationContext.Parameters.GetValue<DiskModel>("DiskInfo");
            
            Task.Run(() =>
            {
                GetAllExtensions(SelectedDisk.DName + "\\", _tokenSource.Token);

                _tokenSource.Cancel();
            });

            Task.Run(() =>
            {
                while (true)
                {
                    _tokenSource.Token.ThrowIfCancellationRequested();
                    RaisePropertyChanged(nameof(TopExtensions));
                    Thread.Sleep(100);
                }
            }, _tokenSource.Token);
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
