using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CodeValue.SuiteValue.UI.Metro.Controls
{
    public enum PeriodKind
    {
        H12, H24
    }

    public sealed class TimePicker : Control, INotifyPropertyChanged
    {
        public TimePicker()
        {
            this.DefaultStyleKey = typeof(TimePicker);
        }


        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime), typeof(TimePicker),
                                        new PropertyMetadata(DateTime.Now, UpdateValue));

        private static void UpdateValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                (d as TimePicker).UpdateByDateTime((DateTime)e.NewValue);
        }

        private void UpdateByDateTime(DateTime newValue)
        {

            if (Kind == PeriodKind.H12)
            {
                HoursOptions = Enumerable.Range(1, 12);
                ShowPeriod = Visibility.Visible;
                SelectedHour = int.Parse(newValue.ToString("hh"));

            }
            else
            {
                HoursOptions = Enumerable.Range(0, 24);
                ShowPeriod = Visibility.Collapsed;
                SelectedHour = int.Parse(newValue.ToString("HH"));
            }

            SelectedMinutes = newValue.Minute;
            SelectedPeriod = newValue.ToString("tt");


            //SelectedPeriod = newValue.

        }



        private void UpdateToValue()
        {
            if (_minutes != null)
            {
                var date = new DateTime(Value.Year, Value.Month, Value.Day, SelectedHour, SelectedMinutes, Value.Second,
                                        Value.Kind);
                Value = date;
            }
        }

        private int _selectedHour;

        public int SelectedHour
        {
            get { return _selectedHour; }
            set
            {
                if (value != _selectedHour)
                {
                    _selectedHour = value;
                    UpdateToValue();
                }
                OnPropertyChanged("SelectedHour");

            }
        }


        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TimePicker),
                                        new PropertyMetadata(Orientation.Horizontal));



        public PeriodKind Kind
        {
            get { return (PeriodKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Kind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(PeriodKind), typeof(TimePicker),
                                        new PropertyMetadata(PeriodKind.H12, KindChanged));

        private static void KindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as TimePicker;
            self.UpdateByDateTime(self.Value);

        }


        private ComboBox _minutes;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _minutes = GetTemplateChild("Minutes") as ComboBox;
            _minutes.ItemsSource = Enumerable.Range(0, 60);

            UpdateByDateTime(Value);

            OnPropertyChanged("SelectedMinutes");

        }

        private IEnumerable<int> _hoursOptions;

        public IEnumerable<int> HoursOptions
        {
            get { return _hoursOptions; }
            set
            {
                if (value != _hoursOptions)
                {
                    _hoursOptions = value;
                    OnPropertyChanged("HoursOptions");
                }
            }
        }

        private int _selectedMinutes;

        public int SelectedMinutes
        {
            get { return _selectedMinutes; }
            set
            {
                if (value != _selectedMinutes)
                {
                    _selectedMinutes = value;
                    UpdateToValue();

                }
                OnPropertyChanged("SelectedMinutes");

            }
        }

        private Visibility _showPeriod;

        public Visibility ShowPeriod
        {
            get { return _showPeriod; }
            set
            {
                if (value != _showPeriod)
                {
                    _showPeriod = value;
                    OnPropertyChanged("ShowPeriod");
                }
            }
        }

        private string _selectedPeriod;

        public string SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (value != _selectedPeriod)
                {
                    _selectedPeriod = value;
                    OnPropertyChanged("SelectedPeriod");
                }
            }
        }

        public string[] TimeOptions
        {
            get
            {
                return new[] { "AM", "PM" };
            }

        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
