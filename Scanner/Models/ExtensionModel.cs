using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Scanner.Models
{
    public class ExtensionModel : BindableBase
    {
        private double _ratio;
        private int _count;
        private long _totalSize;
        private bool _isSelected;

        public string Name { get; set; }

        public double Ratio
        {
            get { return _ratio; }
            set { SetProperty(ref _ratio, value); }
        }

        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        public long TotalSize
        {
            get { return _totalSize; }
            set { SetProperty(ref _totalSize, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public ObservableCollection<string> Files { get; set; }
    }
}
