using System;
using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using TimeStudy.Droid.DependencyServices;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;
using Android.Runtime;
using Android.Support.V4.App;

[assembly: Dependency(typeof(LocalNotificationService))]
namespace TimeStudy.Droid.DependencyServices
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void LocalNotification(string title, string body, int id, DateTime notifyTime, double repeatInterval)
        {
            //BootReceiver.ReminderInterval = (long)repeatInterval;
            //Intent alarmIntent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
            //alarmIntent.AddFlags(ActivityFlags.SingleTop);
            //PendingIntent pending = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            //AlarmManager alarmManager = Android.App.Application.Context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();

            ////AlarmType.RtcWakeup – it will fire up the pending intent at a specified time, waking up the device
            //alarmManager.SetRepeating(AlarmType.RtcWakeup, BootReceiver.FirstReminder(), BootReceiver.ReminderInterval, pending);
            //PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, alarmIntent, 0);
        }

        public void DisableLocalNotification(string title, string body, int id, DateTime notifyTime)
        {
            //Intent alarmIntent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
            //PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, alarmIntent, 0); 
            //AlarmManager alarmManager = Android.App.Application.Context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();

            //alarmManager.Cancel(pendingIntent);
            //var notificationManager = NotificationManagerCompat.From(Android.App.Application.Context);
            //notificationManager.CancelAll();
        }
    }
}

