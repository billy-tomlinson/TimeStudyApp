using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;
using TimeStudy.Custom;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TimeStudy.ViewModels
{
    public class ExistingStudiesViewModel : BaseViewModel
    {
        bool completed;
        public ExistingStudiesViewModel(bool completed)
        {
            this.completed = completed;

            ActivitySamples = new ObservableCollection<ActivitySampleStudy>(SampleRepo.GetAllWithChildren());

            if (completed)
            {
                var historic = StudyHistoryVersionRepo.GetItems();
                var historicStudies = new List<ActivitySampleStudy>();
                foreach (var item in historic)
                {
                    var study = SampleRepo.GetItem(item.StudyId);
                    if(study != null)
                    {
                        study.Version = item.Id;
                        study.Date = item.Date;
                        study.Time = item.Time;
                        historicStudies.Add(study);
                    }
                }

                ActivitySamples = new ObservableCollection<ActivitySampleStudy>(historicStudies);
            }
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

            return new  Command(async (item) =>
            {
                IsBusy = true;
                Task navigateTask = Task.Run( async () =>
                {
                    await Task.Delay(200);
                    Opacity = 0.2;

                    var study = item as ActivitySampleStudy;
                    study.ObservedColour = Xamarin.Forms.Color.Silver.GetShortHexString();
                    Utilities.StudyId = study.Id;
                    Utilities.StudyVersion = study.Version;
                    Utilities.RatedStudy = study.IsRated;
                    Utilities.IsCompleted = completed;

                    Device.BeginInvokeOnMainThread(async  () =>
                    {
                        if (!completed)
                           await Utilities.Navigate(new MainPageTabbedPage());
                        else
                            await Utilities.Navigate(new ReportsPage());
                    });
                });

                await navigateTask;

            });
        }
    }
}
