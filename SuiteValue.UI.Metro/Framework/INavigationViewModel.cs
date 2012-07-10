using System;
using System.ComponentModel;
using Windows.UI.Xaml.Navigation;

namespace CodeValue.SuiteValue.UI.Metro.Framework
{
    public class NavigationEventArgs : CancelEventArgs
    {
        public Type ViewType { get; private set; }
        public object Parameter { get; private set; }

        public NavigationEventArgs(Type viewType, object parameter)
	    {
            this.ViewType = viewType;
            this.Parameter = parameter;
	    }
    }

    public interface INavigationViewModel
    {
        event EventHandler<NavigationEventArgs> RequestNavigateTo;
        event EventHandler<EventArgs> RequestNavigateBack;

        void OnNavigatedTo(NavigationMode mode, object parameter);
        bool OnNavigatingFrom(NavigationMode mode);
        void OnNavigatedFrom(NavigationMode mode);
    }
}
