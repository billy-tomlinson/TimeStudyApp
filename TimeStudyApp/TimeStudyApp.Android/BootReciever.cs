using System;

using Android.App;
using Android.Content;

namespace TimeStudy.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public static long ReminderInterval { get; set; }
        public static long FirstReminder()
        {
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            return calendar.TimeInMillis + ReminderInterval * 1000;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            var alarmIntent = new Intent(context, typeof(AlarmReceiver));
            alarmIntent.AddFlags(ActivityFlags.SingleTop);
            var pending = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            alarmManager.SetRepeating(AlarmType.RtcWakeup, FirstReminder(), ReminderInterval * 1000, pending);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, 0);
        }
    }
}
