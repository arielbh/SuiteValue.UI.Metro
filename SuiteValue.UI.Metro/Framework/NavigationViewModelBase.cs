using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace CodeValue.SuiteValue.UI.Metro.Framework
{
    public class NavigationViewModelBase<TNavigationParameter> : AsyncViewModelBase, INavigationViewModel
    {
        #region INavigationViewModel

        public bool RegisteredForNavigation { get; set; }

        void INavigationViewModel.OnNavigatedTo(NavigationMode mode, object parameter)
        {
            TNavigationParameter obj = default(TNavigationParameter);
            if (typeof(TNavigationParameter) == typeof(object))
            {
                OnNavigatedTo(mode, (TNavigationParameter)parameter);
                return;
            }

            if (parameter != null)
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(parameter as string)))
                {
                    {
                        var serializer = new DataContractSerializer(typeof(TNavigationParameter));
                        obj = (TNavigationParameter)serializer.ReadObject(stream);
                    }
                }
            }
            OnNavigatedTo(mode, obj);
        }

        bool INavigationViewModel.OnNavigatingFrom(NavigationMode mode)
        {
            return OnNavigatingFrom(mode);
        }

        void INavigationViewModel.OnNavigatedFrom(NavigationMode mode)
        {
            OnNavigatedFrom(mode);
        } 
        #endregion

        #region Protected and Overrides
        protected virtual void OnNavigatedTo(NavigationMode mode, TNavigationParameter parameter)
        {
        }

        protected virtual bool OnNavigatingFrom(NavigationMode mode)
        {
            return false;
        }

        protected virtual void OnNavigatedFrom(NavigationMode mode)
        {
        } 
        #endregion        
    
        protected virtual void Navigate(Type viewType, object parameter)
        {
            if (RequestNavigateTo != null)
            {
                var serializedObject = SerializeParameter(parameter);
                RequestNavigateTo(this, new NavigationEventArgs(viewType, serializedObject));
            }
        }

        private static string SerializeParameter(object parameter)
        {
            string serializedObject = null;
            if (parameter != null)
            {
                using (var stream = new MemoryStream())
                {
                    //MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(parameter));
                    var serializer =
                        new DataContractSerializer(parameter.GetType());
                    serializer.WriteObject(stream, parameter);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        serializedObject = reader.ReadToEnd();
                    }
                }
            }
            return serializedObject;
        }

        protected virtual void NavigateTo(Type viewType, object parameter)
        {
            if (RequestNavigateTo != null)
            {
                var serializedObject = SerializeParameter(parameter);
                RequestNavigateTo(this, new NavigationEventArgs(viewType, serializedObject));
            }
        }

        protected virtual void NavigateBack()
        {
            if (RequestNavigateBack != null)
            {
                RequestNavigateBack(this, EventArgs.Empty);
            }
        }

        public event EventHandler<NavigationEventArgs> RequestNavigateTo;
        public event EventHandler<EventArgs> RequestNavigateBack;

        private ICommand _navigateBackCommand;
        public ICommand NavigateBackCommand
        {
            get
            {
                return _navigateBackCommand ?? (_navigateBackCommand = new DelegateCommand(NavigateBack));
            }
        }

        //private ICommand _navigateHomeCommand;
        //public ICommand NavigateHomeCommand
        //{
        //    get
        //    {
        //        return _navigateHomeCommand ?? (_navigateHomeCommand = new DelegateCommand(() =>
        //            NavigateTo(typeof(CountdownDashboardView), null)));
        //    }
        //}

        //ICommand _openRegisterViewCommand;
        //public ICommand OpenRegisterViewCommand
        //{
        //    get
        //    {
        //        return _openRegisterViewCommand ?? (_openRegisterViewCommand = new DelegateCommand(() =>
        //        {
        //            NavigateTo(typeof(RegistrationView), null);
        //        }));
        //    }
        //}

        //ICommand _openLoginViewCommand;
        //public ICommand OpenLoginViewCommand
        //{
        //    get
        //    {
        //        return _openLoginViewCommand ?? (_openLoginViewCommand = new DelegateCommand(() =>
        //        {
        //            NavigateTo(typeof(LoginView), null);
        //        }));
        //    }
        //}


        //public IdentityService IdentityService
        //{
        //    get
        //    {
        //        return IdentityService.Instance;
        //    }
        //}

        
    }
}
