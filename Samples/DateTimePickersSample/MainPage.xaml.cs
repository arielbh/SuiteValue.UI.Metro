using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeValue.SuiteValue.UI.Metro.Controls;
using GalaSoft.MvvmLight;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
                                                                
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DateTimePickersSample
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ValueChanged(object sender, ValueChangedEventArgs e)
        {
            textblock.Text = e.NewValue.ToString();
        }
    }

    class MainViewModel : ViewModelBase
    {
        private DateTime _myValue = DateTime.Now;

        public DateTime MyValue
        {
            get { return _myValue; }
            set
            {
                if (value != _myValue)
                {
                    _myValue = value;
                    RaisePropertyChanged(() => MyValue);
                }
            }
        } 
    }
}
