using System;
namespace TimeStudy.Services
{
    public interface ILocalNotificationService
    {
        void LocalNotification(string title, string body, int id, DateTime notifyTime, double repeatInterval);
        void DisableLocalNotification(string title, string body, int id, DateTime notifyTime);
    }
}
