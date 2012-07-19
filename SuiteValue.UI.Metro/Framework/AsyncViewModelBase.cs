using GalaSoft.MvvmLight;
using Windows.UI.Xaml;

namespace CodeValue.SuiteValue.UI.Metro.Framework
{
    public class AsyncViewModelBase: ViewModelBase
    {
        private Visibility _showMainContent = Visibility.Visible;

        public Visibility ShowMainContent
        {
            get { return _showMainContent; }
            set
            {
                if (value != _showMainContent)
                {
                    _showMainContent = value;
                    RaisePropertyChanged(() => ShowMainContent);
                }
            }
        }

        private Visibility _showProgressBar = Visibility.Collapsed;

        public Visibility ShowProgressBar
        {
            get { return _showProgressBar; }
            set
            {
                if (value != _showProgressBar)
                {
                    _showProgressBar = value;
                    RaisePropertyChanged(() => ShowProgressBar);
                }
            }
        }

        private bool _isInAsync;

        public bool IsInAsync
        {
            get { return _isInAsync; }
            set
            {
                if (value != _isInAsync)
                {
                    _isInAsync = value;
                    ShowProgressBar = IsInAsync ? Visibility.Visible : Visibility.Collapsed;
                    ShowMainContent = IsInAsync ? Visibility.Collapsed : Visibility.Visible;
                    RaisePropertyChanged(() => IsInAsync);
                }
            }
        }

        private string _asyncMessage;

        public string AsyncMessage
        {
            get { return _asyncMessage; }
            set
            {
                if (value != _asyncMessage)
                {
                    _asyncMessage = value;
                    RaisePropertyChanged(() => AsyncMessage);
                }
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get { return _hasError; }
            set
            {
                if (value != _hasError)
                {
                    _hasError = value;
                    RaisePropertyChanged(() => HasError);
                }
            }
        }
    }
}
