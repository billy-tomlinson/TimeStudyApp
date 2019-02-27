using System;
using Foundation;
using TimeStudy.Services;
using TimeStudy.iOS.DependencyServices;
using Xamarin.Forms;
using UserNotifications;

[assembly: Dependency(typeof(LocalNotificationService))]
namespace TimeStudy.iOS.DependencyServices
{
    public class LocalNotificationService : ILocalNotificationService
    {
        const string NotificationKey = "LocalNotificationKey";

        public void LocalNotification(string title, string body, int id, DateTime notifyTime, double repeatInterval)
        {

            var content = new UNMutableNotificationContent
            {
                Title = title,
                Subtitle = body,
                Body = "Time for Next Observation.",
                Sound = UNNotificationSound.Default,
                Badge = 1
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(repeatInterval, true);

            var requestID = "sampleRequest";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, HandleAction);
        }

        void HandleAction(NSError obj){}


        public void DisableLocalNotification(string title, string body, int id, DateTime notifyTime)
        {
            var requests = new string[] { "sampleRequest" };
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(requests);
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(requests);
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        }
    }
}
