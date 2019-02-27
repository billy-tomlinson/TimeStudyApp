using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;

namespace TimeStudy.ViewModels
{
    public class BaseAlarmViewModel : BaseViewModel
    {
        public BaseAlarmViewModel(string conn = null, string alarmconn = null) : base(conn, alarmconn) { }

        public const string Interval = "CONSTANT";

        public int IntervalTime = 1;
        public bool PageLoading;

        public AlarmDetails AlarmDetails;

        static string alarmType = Interval;

        public string AlarmType
        {
            get => alarmType;
            set
            {
                alarmType = value;
                OnPropertyChanged();
            }
        }

        static bool isAlarmEnabled;
        public bool IsAlarmEnabled
        {
            get => isAlarmEnabled;
            set
            {
                isAlarmEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged("AlarmType");
                Switch_Toggled_Enabled();
                if (!PageLoading)
                {
                    OnPropertyChanged("IsPageEnabled");

                    SaveAlarmDetails();

                    if (value)
                    {
                        IsPageEnabled = false;
                    }
                    else
                    {
                        IsPageEnabled = true;
                    }
                }
            }
        }


        static bool isRandom;
        public bool IsRandom
        {
            get => isRandom;
            set
            {
                isRandom = value;
                OnPropertyChanged();
                Switch_Toggled_Type();
                if (!PageLoading)
                {
                    var success = int.TryParse(IntervalMinutes, out int result);
                    IntervalIsValid(success);
                    IsPageEnabled = true;
                    OnPropertyChanged("IsPageEnabled");
                }
            }
        }

        static bool isPageEnabled;
        public bool IsPageEnabled
        {
            get => !IsAlarmEnabled;
            set
            {
                isPageEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsAlarmSectionEnabled
        {
            get => Utilities.StudyId > 0;
        }

        public void Switch_Toggled_Type()
        {
            AlarmDetails = AlarmRepo.GetItems()
                .SingleOrDefault(x => x.StudyId == Utilities.StudyId) ?? new AlarmDetails();
            IntervalTime = AlarmDetails.Interval / 60;
            AlarmType = isRandom == false ? Interval : AlarmNotificationService.Random;
        }


        public void Switch_Toggled_Enabled()
        {
            AlarmDetails = AlarmRepo.GetItems()
                .SingleOrDefault(x => x.StudyId == Utilities.StudyId) ?? new AlarmDetails();
            IntervalTime = AlarmDetails.Interval / 60;
            if (IsAlarmEnabled)
                AlarmStatus = "ENABLED";
            else
                AlarmStatus = "DISABLED";

        }

        void ShowValidationForEmptyStudyID()
        {
            if(IsAlarmEnabled)
            {
                IsAlarmEnabled = false;
                ValidationText = $"Please submit study details before setting alarm.";
                Opacity = 0.2;
                IsInvalid = true;
                ShowClose = true;
            }
        }

        void SaveAlarmDetails()
        {
            if (IsAlarmEnabled)
            {
                var success = int.TryParse(IntervalMinutes, out int result);

                if (!IntervalIsValid(success)) return;

                IntervalTime = result * 60;
            }
            else
                IntervalTime = IntervalTime * 60;

            if (Utilities.StudyId != 0)
            {
                AlarmNotificationService.SaveNewAlarmDetails(IntervalTime, AlarmType, IsAlarmEnabled);
                AlarmNotificationService.AlarmSetFromAlarmPage = true;
            }
        }

        public bool IntervalIsValid(bool success)
        {
            if (!IsAlarmEnabled) return true;

            if (!success)
            {
                ValidationText = "Please enter interval minutes less than 99";
                Opacity = 0.2;
                IsInvalid = true;
                IsAlarmEnabled = false;
                ShowClose = true;
                Switch_Toggled_Enabled();
                return false;

            }
            return true;
        }
    }
}
