using TimeStudy.Services;
using TimeStudy.Model;
using System.Linq;

namespace TimeStudy.ViewModels
{
    public class AlarmPageViewModel : BaseAlarmViewModel
    {
   
        ActivitySampleStudy sampleStudy;
        public ActivitySampleStudy SampleStudy
        {
            get { return sampleStudy; }
            set
            {
                sampleStudy = value;
                OnPropertyChanged();
            }
        }

        public string StudyType { get; set; }

        public AlarmPageViewModel()
        {
            if (!IsPageVisible) return;

            PageLoading = true;
            SampleStudy = SampleRepo.GetItem(Utilities.StudyId);

            StudyType = SampleStudy.IsRated == false ? "RATED" : "UNRATED";

            AlarmDetails = AlarmRepo.GetItems()
                .SingleOrDefault(x => x.StudyId == Utilities.StudyId) ?? new AlarmDetails();

            AlarmType = AlarmDetails.Type != string.Empty ? AlarmDetails.Type : Interval;
            IsRandom = AlarmDetails.Type != Interval;
            IsAlarmEnabled = AlarmDetails.IsActive;
            IntervalMinutes = (AlarmDetails.Interval / 60).ToString();
            PageLoading = false;
        }
    }
}
