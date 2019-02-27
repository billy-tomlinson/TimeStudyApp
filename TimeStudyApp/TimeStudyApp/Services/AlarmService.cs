using System;
using TimeStudy.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkStudy.Services
{
    public static class AlarmService
    {
    
        public static bool CancelAlarm { get; set; }

        public static bool ContinueTimer { get; set; } = true;

        public static void TurnOffAlarm()
        {
            Vibration.Cancel();
            CancelAlarm = true;
        }

        public static void StartVibrateTimer(string intervalMinutes)
        {
            Random random = new Random();
            var interval = int.Parse(intervalMinutes);
            if (interval <= 0)
                interval = random.Next(3, 10);
            Device.StartTimer(TimeSpan.FromMinutes(interval), SetUpTimer());
        }

        public static Func<bool> SetUpTimer()
        {
            return () =>
            {
                if (!ContinueTimer)
                {
                    return false;
                }
                CancelAlarm = false;
                StartTimer();
                return ContinueTimer;
            };
        }

        public static void StartTimer()
        {
            if (CancelAlarm)
            {
                DependencyService.Get<ILocalNotificationService>()
                    .DisableLocalNotification("Local Notification", "Next Observation", 0, DateTime.Now);
            }
            else
            {
                DependencyService.Get<ILocalNotificationService>()
                    .LocalNotification("Alert", "Next Observation Round", 0, DateTime.Now, 60);
                CancelAlarm = false;
            }
        }
    }
}
