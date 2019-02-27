using System;
using System.Linq;
using Xamarin.Forms;

namespace TimeStudy.Services
{
    public static class AlarmNotificationService
    {
        public static string Random = "RANDOM";

        static bool restartAlarmCounter;

        public static bool AlarmSetFromAlarmPage { get; set; }
        public static string AlarmType { get; set; }

        public static bool RestartAlarmCounter
        {
            get => restartAlarmCounter;
            set
            {
                restartAlarmCounter = value;
                UpdateAlarm();
            }
        }

        static int nextIntervalTime;
        public static int NextIntervalTime
        {
            get => nextIntervalTime;
            set
            {
                nextIntervalTime = value;
            }
        }

        static DateTime nextAlarmTime;
        public static DateTime NextAlarmTime
        {
            get => nextAlarmTime;
            set
            {
                nextAlarmTime = value;
            }
        }

        public static int GenerateRandomInterval(int interval)
        {
            Random r = new Random();
            var nextInterval = r.Next(0, interval * 2);
            nextInterval = nextInterval < 60 ? 61 : nextInterval;
            return nextInterval;
        }

        public static void SetNextRandomAlarmTime(int nextAlarm)
        {
            var service = DependencyService.Get<ILocalNotificationService>();
            NextAlarmTime = DateTime.Now.AddSeconds(nextAlarm);
            switch (Device.RuntimePlatform)
            { 
                case Device.iOS:
                    service.DisableLocalNotification("Alert", "Next Observation Round", 0, DateTime.Now);
                    service.LocalNotification("Alert", "Next Observation Round", 0, DateTime.Now, nextAlarm);
                    break;
                case Device.Android:
                    service.LocalNotification("Alert", "Next Observation Round", 0, DateTime.Now, nextAlarm);
                    break;
            }
        }

        public static void SetNextRandomAlarmTimeAndroid(int nextAlarm)
        {
            var service = DependencyService.Get<ILocalNotificationService>();

            service.LocalNotification("Alert", "Next Observation Round", 0, DateTime.Now, nextAlarm);
        }

        public static void DisableAlarm()
        {
            var service = DependencyService.Get<ILocalNotificationService>();

            service.DisableLocalNotification("Alert", "Next Observation Round", 0, DateTime.Now);
        }

        public static DateTime SaveNewAlarmDetails(int intervalTime, string alarmType, bool isAlarmEnabled)
        {
            var alarmDetails = Utilities.AlarmRepo.GetItems().SingleOrDefault(x => x.StudyId == Utilities.StudyId);

            alarmDetails.Interval = intervalTime;
            alarmDetails.Type = alarmType;
            alarmDetails.IsActive = isAlarmEnabled;
            alarmDetails.StudyId = Utilities.StudyId;

            var nextObsTime = alarmType == Random ? GenerateRandomInterval(intervalTime) : intervalTime;
            alarmDetails.NextNotificationTime = DateTime.Now.AddSeconds(nextObsTime);
            NextIntervalTime = nextObsTime;

            if (isAlarmEnabled)

                SetNextRandomAlarmTime(nextObsTime);

            if (!isAlarmEnabled)

                DisableAlarm();
           

            Utilities.AlarmRepo.SaveItem(alarmDetails);

            return alarmDetails.NextNotificationTime;
        }

        public static void DisableAlarmInDatabase()
        {
            var alarmDetails = Utilities.AlarmRepo.GetItems().SingleOrDefault(x => x.StudyId == Utilities.StudyId);

            alarmDetails.IsActive = false;

            Utilities.AlarmRepo.SaveItem(alarmDetails);
        }

        private static void UpdateAlarm()
        {

            if (restartAlarmCounter && Utilities.StudyId > 0)
            {
                var alarm = Utilities.AlarmRepo.GetItems().SingleOrDefault(x => x.StudyId == Utilities.StudyId);
                SaveNewAlarmDetails(alarm.Interval, alarm.Type, alarm.IsActive);
            }
        }

        public static DateTime CheckIfAlarmHasExpiredWhilstInBackgroundOrAlarmOff()
        {
            return UpdateAlarmAfterBeingInBackroundOrAlarmOff();
        }

        private static DateTime UpdateAlarmAfterBeingInBackroundOrAlarmOff()
        {

            DateTime newObsTime = new DateTime();
            if (Utilities.StudyId > 0)
            {
                var alarm = Utilities.AlarmRepo.GetItems().SingleOrDefault(x => x.StudyId == Utilities.StudyId);
                bool notificationExpired = alarm.NextNotificationTime < DateTime.Now;

                if (notificationExpired)
                {
                    newObsTime = SaveNewAlarmDetails(alarm.Interval, alarm.Type, alarm.IsActive);
                }
            }

            return newObsTime;
        }
    }
}
