using System;
using UserNotifications;
using TimeStudy.Services;

namespace TimeStudy.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, 
            UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            AlarmNotificationService.RestartAlarmCounter = true;

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }
    }
}
