using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using CodeValue.SuiteValue.UI.Metro.Authentications;
using CodeValue.SuiteValue.UI.Metro.Framework;
using GalaSoft.MvvmLight;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using NavigationEventArgs = Windows.UI.Xaml.Navigation.NavigationEventArgs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AuthenticationSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = new MainViewModel();
        }
    }

    class MainViewModel : ViewModelBase
    {

        private DelegateCommand _findProvidersCommand;

        public DelegateCommand FindProvidersCommand
        {
            get
            {
                return _findProvidersCommand ?? (_findProvidersCommand = new DelegateCommand(
                                                     FindProviders));
            }
        }

        private async void FindProviders()
        {
            var authManager = new AuthManager();
            authManager.QueryProviders(await AuthManager.GetAssemblyListAsync());
            var config = PrepareConiguration();
            authManager.ConfigureProviders(config);
            Providers = authManager.Providers.Select(p => new ProviderViewModel { Provider = p});
        }

        public Visibility DisplayDetails
        {
            get { return SelectedProvider == null ? Visibility.Collapsed : Visibility.Visible; }
    
        }

        private ProviderViewModel _selectedProvider;

        public ProviderViewModel SelectedProvider
        {
            get { return _selectedProvider; }
            set
            {
                if (value != _selectedProvider)
                {
                    _selectedProvider = value;
                    RaisePropertyChanged(() => SelectedProvider);
                    RaisePropertyChanged(() => DisplayDetails);
                    
                }
            }
        }

        private IEnumerable<ProviderViewModel> _providers;

        public IEnumerable<ProviderViewModel> Providers
        {
            get { return _providers; }
            set
            {
                if (value != _providers)
                {
                    _providers = value;
                    RaisePropertyChanged(() => Providers);
                }
            }
        }

        private dynamic PrepareConiguration()
        {

            dynamic result = new ExpandoObject();

            result.TwitterClientId = "YOUR_APP_ID";
            result.TwitterClientSecret = "YOUR_APP_CLIENT_SECRET";
            result.TwitterRedirectUrl = "http://contoso.net";

            result.GoogleClientId = "YOUR_APP_ID";
            result.GoogleRedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
            result.GoogleClientSecret = "YOUR_APP_CLIENT_SECRET";

            result.FacebookClientId = "YOUR_APP_ID";
            result.FacebookRedirectUrl = "http://contoso.net";
            return result;

        }
    }

    class ProviderViewModel : ViewModelBase
    {
        public IAuthProvider Provider { get; set; }

        private DelegateCommand _authenticateCommand;

        public DelegateCommand AuthenticateCommand
        {
            get
            {
                return _authenticateCommand ?? (_authenticateCommand = new DelegateCommand(
                                                    async () =>
                                                         {
                                                             try
                                                             {
                                                                 UserInfo = await Provider.Authenticate();

                                                             }
                                                             catch (Exception ex)
                                                             {

                                                                 Message = "Error authenticating " + Provider.Name;
                                                             }
                                                             
                                                         }));
            }
        }

        private UserInfo _userInfo;

        public UserInfo UserInfo
        {
            get { return _userInfo; }
            set
            {
                if (value != _userInfo)
                {
                    _userInfo = value;
                    RaisePropertyChanged(() => UserInfo);
                }
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
        }



    }
}
