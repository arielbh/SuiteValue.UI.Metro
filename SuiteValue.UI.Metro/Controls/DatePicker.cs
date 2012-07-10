using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CodeValue.SuiteValue.UI.Metro.Controls
{

    public sealed class DatePicker : Control, INotifyPropertyChanged
    {
        public DatePicker()
        {
            this.DefaultStyleKey = typeof(DatePicker);
        }

        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime), typeof(DatePicker),
                                        new PropertyMetadata(DateTime.Now, UpdateValue));

        private static void UpdateValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                (d as DatePicker).UpdateByDateTime((DateTime)e.NewValue);
        }



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DatePicker),
                                        new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        private ComboBox _months;
        private ComboBox _days;
        private ComboBox _years;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _selectedMonth = Value.Month - 1;
            _selectedDay = Value.Day;
            _selectedYear = Value.Year;

            _months = GetTemplateChild("Months") as ComboBox;
            _months.ItemsSource = MonthsNames;
            _days = GetTemplateChild("Days") as ComboBox;
            _years = GetTemplateChild("Years") as ComboBox;
            _years.ItemsSource = Enumerable.Range(DateTime.Now.Year - 100, 200);

            UpdateByDateTime(Value);

            OnPropertyChanged("SelectedDay");
            OnPropertyChanged("SelectedMonthName");
            OnPropertyChanged("SelectedYear");

        }

        private void UpdateToValue()
        {
            if (_years != null && _months != null && _days != null)
            // && _years.SelectedItem != null && _months.SelectedItem != null && _days.SelectedItem != null)
            {
                var daysinMonth = DateTime.DaysInMonth(SelectedYear, SelectedMonth + 1);
                if (SelectedDay > daysinMonth)
                {
                    SelectedDay = daysinMonth;
                }
                var date = new DateTime(SelectedYear, SelectedMonth + 1, SelectedDay, Value.Hour, Value.Minute, Value.Second, Value.Kind);
                Value = date;
            }

        }

        private void UpdateByDateTime(DateTime date)
        {
            int days = DateTime.DaysInMonth(date.Year, date.Month);
            DaysOptions = Enumerable.Range(1, days);

            SelectedYear = date.Year;
            SelectedMonthName = MonthsNames[date.Month - 1];
            //SelectedMonth = date.Month - 1;
            SelectedDay = date.Day;
        }

        private string _selectedMonthName;

        public string SelectedMonthName
        {
            get { return _selectedMonthName; }
            set
            {
                if (value != _selectedMonthName)
                {
                    _selectedMonthName = value;
                    SelectedMonth = Array.IndexOf(MonthsNames, value);
                    UpdateToValue();
                }
                OnPropertyChanged("SelectedMonthName");

            }
        }

        private IEnumerable<int> _daysOptions;

        public IEnumerable<int> DaysOptions
        {
            get { return _daysOptions; }
            set
            {
                if (value != _daysOptions)
                {
                    _daysOptions = value;
                    OnPropertyChanged("DaysOptions");
                }
            }
        }

        private int _selectedYear = DateTime.Now.Year;

        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (value != _selectedYear)
                {
                    _selectedYear = value;
                    UpdateToValue();

                }
                OnPropertyChanged("SelectedYear");

            }
        }

        private int _selectedDay = 1;

        public int SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                if (value != _selectedDay)
                {
                    _selectedDay = value;
                    UpdateToValue();
                }
                OnPropertyChanged("SelectedDay");

            }
        }

        private int _selectedMonth = 0;

        public int SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (value > MonthsNames.Length || value < 0) return;
                if (value != _selectedMonth)
                {
                    _selectedMonth = value;

                    UpdateToValue();
                }
                OnPropertyChanged("SelectedMonth");

            }
        }

        public readonly string[] MonthsNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
