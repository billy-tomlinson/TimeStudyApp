using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class AllActivitiesViewModel : BaseViewModel
    {
        public Command Submit { get; set; }

        public AllActivitiesViewModel()
        {
            Submit = new Command(ActivitySelectedEvent);
            ItemsCollection = GetUnusedActivities();
        }

        private ObservableCollection<ActivityName> GetUnusedActivities()
        {
            var allActivities = Get_All_ActivityNames().Where(x => !x.IsMerge).ToList();

            var activityNames = allActivities.Select(x => x.Name).ToList();

            var activitiesInStudy = ActivityRepo.GetAllWithChildren().Where(x => x.StudyId == Utilities.StudyId);

            foreach (var item in activityNames)
            {
                foreach (var activity in activitiesInStudy)
                {
                    if (item == activity.ActivityName.Name)
                    {
                        var v = allActivities.FirstOrDefault(x => x.Name == item);
                        allActivities.Remove(v);
                    }
                }
            }

            return new ObservableCollection<ActivityName>(allActivities);
        }

        static ObservableCollection<ActivityName> itemsCollection;
        public ObservableCollection<ActivityName> ItemsCollection
        {
            get => itemsCollection;
            set
            {
                itemsCollection = value;
                OnPropertyChanged();
            }
        }

        async void ActivitySelectedEvent(object sender)
        {

            IsBusy = true;
            IsEnabled = false;
            Opacity = 0.2;

            var collection = new List<Activity>();
            var count = Get_All_Enabled_Activities().OrderByDescending(x => x.Id)
                .Count(z => z.StudyId == Utilities.StudyId);

            foreach (var item in ItemsCollection.Where(x => x.Selected))
            {
                var activity = new Activity
                {
                    ActivityName = item,
                    IsEnabled = true,
                    Rated = true,
                    ObservedColour = Utilities.ValueAddedColour
                };

                collection.Add(activity);
            }

            ActivityRepo.InsertAll(collection);

            ItemsCollection = GetUnusedActivities();

            //do this to ensure that everything has been written to the DB before navigating
            while (true)
            {
                await Task.Delay(500);
                var count1 = Get_All_Enabled_Activities().OrderByDescending(x => x.Id)
                .Count(z => z.StudyId == Utilities.StudyId);
                if (collection.Count() + count == count1)
                    break;
            }

            IsEnabled = true;
            IsBusy = false;
            Opacity = 1;

            var page = sender as ContentPage;
            var parentPage = page.Parent as TabbedPage;
            var activitiesPage = (Page)parentPage.Children[1];
            activitiesPage.BindingContext = new AddElementsViewModel();
            parentPage.CurrentPage = activitiesPage;
        }
    }
}