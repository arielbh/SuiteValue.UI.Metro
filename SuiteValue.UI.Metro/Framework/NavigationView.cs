using System;
using Windows.UI.Xaml;

namespace CodeValue.SuiteValue.UI.Metro.Framework
{
    public class NavigationView : LayoutAwarePage
    {
        /// <summary>
        /// Gets or sets the view-model attached to this view.
        /// </summary>
        public object ViewModel
        {
            get { return (INavigationViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(object),
                typeof(NavigationView),
                new PropertyMetadata(
                    null,
                    (d, e) => ((NavigationView)d).ViewModelChanged((INavigationViewModel)e.OldValue, (INavigationViewModel)e.NewValue)));

        private void ViewModelChanged(INavigationViewModel oldViewModel, INavigationViewModel newViewModel)
        {
            if (oldViewModel != null)
            {
                oldViewModel.RequestNavigateTo -= ViewModel_RequestNavigateTo;
                oldViewModel.RequestNavigateBack -= ViewModel_RequestNavigateBack;
            }

            if (newViewModel != null && !newViewModel.RegisteredForNavigation)
            {
                newViewModel.RequestNavigateTo += ViewModel_RequestNavigateTo;
                newViewModel.RequestNavigateBack += ViewModel_RequestNavigateBack;
                newViewModel.RegisteredForNavigation = true;
            }

            DataContext = newViewModel;
        }

        void ViewModel_RequestNavigateBack(object sender, EventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        void ViewModel_RequestNavigateTo(object sender, NavigationEventArgs e)
        {
            e.Cancel = Frame.Navigate(e.ViewType, e.Parameter);
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (ViewModel != null)
            {
                (ViewModel as INavigationViewModel).OnNavigatedTo(e.NavigationMode, e.Parameter);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            if (ViewModel != null)
            {
                e.Cancel = (ViewModel as INavigationViewModel).OnNavigatingFrom(e.NavigationMode);
            }

            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (ViewModel != null)
            {
                (ViewModel as INavigationViewModel).OnNavigatedFrom(e.NavigationMode);
            }

            base.OnNavigatedFrom(e);
        }
    }
}
