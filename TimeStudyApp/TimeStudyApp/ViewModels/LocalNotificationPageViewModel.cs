using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class LocalNotificationPageViewModel : BaseViewModel
    {

        Command _saveCommand;
        public Command SaveCommand
        {
            get
            {
                return _saveCommand;
            }
            set
            {
                SetProperty(ref _saveCommand, value);
            }
        }

        Command _disableCommand;
        public Command DisableCommand
        {
            get
            {
                return _disableCommand;
            }
            set
            {
                SetProperty(ref _disableCommand, value);
            }
        }


        bool _notificationONOFF;
        public bool NotificationONOFF
        {
            get
            {
                return _notificationONOFF;
            }
            set
            {
                SetProperty(ref _notificationONOFF, value);
                Switch_Toggled();
            }
        }

        void Switch_Toggled()
        {

            if (NotificationONOFF == false)
            {

                MessageText = string.Empty;
                SelectedTime = DateTime.Now.TimeOfDay;
                SelectedDate = DateTime.Today;
                //DependencyService.Get<ILocalNotificationService>().Cancel(0);
            }
        }

        DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                SetProperty(ref _selectedDate, value);
            }
        }

        TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
        public TimeSpan SelectedTime
        {
            get
            {
                return _selectedTime;
            }
            set
            {
                SetProperty(ref _selectedTime, value);
            }
        }

        string _messageText;
        public string MessageText
        {
            get
            {
                return _messageText;
            }
            set
            {
                SetProperty(ref _messageText, value);
            }
        }

        public LocalNotificationPageViewModel()
        {
            SaveCommand = new Command(() => SaveLocalNotification());
            DisableCommand = new Command(() => DisableLocalNotification());
        }

        void SaveLocalNotification()
        {

            if (NotificationONOFF == true)
            {

                var date = (SelectedDate.Date.Month.ToString("00") + "-" + SelectedDate.Date.Day.ToString("00") + "-" + SelectedDate.Date.Year.ToString());

                var time = Convert.ToDateTime(SelectedTime.ToString()).ToString("HH:mm");

                var dateTime = date + " " + time;

                var selectedDateTime = DateTime.ParseExact(dateTime, "MM-dd-yyyy HH:mm", CultureInfo.InvariantCulture);

                if (!string.IsNullOrEmpty(MessageText))
                {
                
                    DependencyService.Get<ILocalNotificationService>()
                        .LocalNotification("Local Notification", MessageText, 0, selectedDateTime, 60);
                    Application.Current.MainPage.DisplayAlert("LocalNotificationDemo", "Notification details saved successfully ", "Ok");

                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("LocalNotificationDemo", "Please enter meassage", "OK");
                }

            }
            else
            {
                Application.Current.MainPage.DisplayAlert("LocalNotificationDemo", "Please switch on notification", "OK");
            }
        }


        void DisableLocalNotification()
        {
            DependencyService.Get<ILocalNotificationService>()
                .DisableLocalNotification("Local Notification", MessageText, 0, DateTime.Now);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
           [CallerMemberName]string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
