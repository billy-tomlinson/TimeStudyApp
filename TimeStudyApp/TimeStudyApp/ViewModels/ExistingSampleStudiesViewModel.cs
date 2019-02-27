using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class ExistingSampleStudiesViewModel : BaseViewModel
    {
        bool completed;
        public ExistingSampleStudiesViewModel(bool completed)
        {
            this.completed = completed;
            ActivitySamples = new ObservableCollection<ActivitySampleStudy>(SampleRepo.GetItems()
                                  .Where(_ => _.Completed == completed));
        }

        static ObservableCollection<ActivitySampleStudy> activitySamples;
        public ObservableCollection<ActivitySampleStudy> ActivitySamples
        {
            get => activitySamples;
            set
            {
                activitySamples = value;
                OnPropertyChanged();
            }
        }

        public Command ItemClickedCommand
        {
            get { return Navigate(); }
        }

        public Command Navigate()
        {
            return new Command((item) =>
            {
                var study = item as ActivitySampleStudy;
                Utilities.StudyId = study.Id;
                Utilities.RatedStudy = study.IsRated;
                Utilities.IsCompleted = completed;

                if(!completed)
                    Utilities.Navigate(new MainPageTabbedPage());
                else
                    Utilities.Navigate(new ReportsPage());
            });
        }
    }
}
