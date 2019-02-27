using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Custom;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class EditActivitiesViewModel : BaseViewModel
    {
        public Command SaveActivities { get; set; }
        public Command UnMergeActivities { get; set; }
        public Command CancelActivities { get; set; }
        public Command ActivitySelected { get; set; }

        public EditActivitiesViewModel(string conn): base(conn)
        {
            ConstructorSetUp();
        }

        public EditActivitiesViewModel()
        {
            ConstructorSetUp();
        }

        private List<Activity> mergedActivities;
        public List<Activity> MergedActivities
        {
            get => mergedActivities;
            set
            {
                mergedActivities = value;
                OnPropertyChanged();
            }
        }
        void ActivitySelectedEvent(object sender)
        {
            var value = (int)sender;
            ChangeButtonColour(value);
        }

        private void ChangeButtonColour(int sender)
        {
            IEnumerable<Activity> obsCollection = Activities;
            var list = new List<Activity>(obsCollection);
            var activity = list.Find(_ => _.Id == sender);

            activity.Colour = ChangeButtonColourForCategory(activity.Colour.GetShortHexString(), activity.ItemColour);

            list.RemoveAll(_ => _.Id == sender);
            list.Add(activity);
            Activities = ConvertListToObservable(list);
            GroupActivities = Utilities.BuildGroupOfActivities(Activities);

            var merged = MergedActivities.Find(_ => _.Id == sender);
            if (merged == null)
                MergedActivities.Add(activity);
            else
                MergedActivities.RemoveAll(_ => _.Id == sender);
        }

        private Color ChangeButtonColourForCategory(string currentColour, string originalColour)
        {
            Color activityColour;

            if (currentColour == Utilities.ClickedHex)
                activityColour = Color.FromHex(originalColour);
            else
                activityColour = Color.FromHex(Utilities.ClickedHex);

            return activityColour;
        }

        private void RefreshActivities()
        {

            MergedActivities = new List<Activity>();
            Activities = Get_All_Enabled_Activities_WithChildren();

            IEnumerable<Activity> obsCollection = Activities;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            }

            Activities = ConvertListToObservable(list1);
            GroupActivities = Utilities.BuildGroupOfActivities(Activities);
        }

        private ObservableCollection<MultipleActivities> ChangeButtonColourOnLoad()
        {
            IEnumerable<Activity> obsCollection = Activities;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            }

            Activities = ConvertListToObservable(list1);
            return Utilities.BuildGroupOfActivities(Activities);
        }

        public void SaveActivityDetails()
        {
            if (mergedActivities.Count < 2) return;

            var valueAdded = mergedActivities.Any(x => x.IsValueAdded && x.Rated);
            var nonValueAdded = mergedActivities.Any(x => !x.IsValueAdded && x.Rated);
            //var countRated = mergedActivities.Any(x => x.Rated);
            var countUnrated = mergedActivities.Any(x => !x.Rated);

            if (valueAdded && nonValueAdded && countUnrated || valueAdded && nonValueAdded || valueAdded && countUnrated || nonValueAdded && countUnrated)
            {
                ValidationText = "Cannot merge activities from different categories together.";
                Opacity = 0.2;
                RefreshActivities();
                IsInvalid = true;
                ShowClose = true;
                return;
            }

            var parentActivity = new Activity();
            var returnId = SaveActivityDetails(parentActivity);
            parentActivity = ActivityRepo.GetWithChildren(returnId);

            for (var i = 0; i < MergedActivities.Count; i++)
            {
                var existingObs = ObservationRepo.GetItems().Where(cw => cw.AliasActivityId == MergedActivities[i].Id);
                foreach (var item in existingObs)
                {
                    item.AliasActivityId = returnId;
                    ObservationRepo.SaveItem(item);
                }

                var merged = MergedActivities[i];

                merged.IsEnabled = false;

                SaveActivityDetails(merged);

                parentActivity.ActivityName.Name = parentActivity.ActivityName.Name + " " + merged.ActivityName.Name;
                parentActivity.ActivityName.IsMerge = true;
                parentActivity.IsEnabled = true;
                parentActivity.Rated = merged.Rated;
                parentActivity.DeleteIcon = merged.Rated ? Utilities.DeleteImage : string.Empty;
                parentActivity.IsValueAdded = merged.IsValueAdded;
                parentActivity.ItemColour = merged.ItemColour;
                parentActivity.ObservedColour = merged.ObservedColour;

                SaveActivityDetails(parentActivity);

                parentActivity.Activities.Add(merged);
            }

            //NOTE - have to do this in seperate loop as UpDateWithChildren deletes the merge record !!
            for (var i = 0; i < MergedActivities.Count; i++)
            {
                var merged = MergedActivities[i];
                MergedActivityRepo.SaveItem(new Model.MergedActivities() 
                { 
                    ActivityId = parentActivity.Id,
                    MergedActivityId = MergedActivities[i].Id 
                });
            }

            RefreshActivities();
        }

        public void UnMergeActivitiesEvent()
        {
            if (mergedActivities.Count != 1)
            {
                ValidationText = "Please select one activity only to un-merge";
                Opacity = 0.2;
                RefreshActivities();
                ShowClose = true;
                IsInvalid = true;
                return;
            }

            var mergedActivity = mergedActivities[0];

            var inUse = ObservationRepo.GetItems().Any(cw => cw.ActivityId == mergedActivity.Id);

            if (inUse)
            {
                ValidationText = "Activity cannot be un-merged. It has been used in the study";
                Opacity = 0.2;
                RefreshActivities();
                ShowClose = true;
                IsInvalid = true;
                return;
            }

            var mergedItems = MergedActivityRepo.GetItems()
                                                .Where(x => x.ActivityId == mergedActivity.Id)
                                                .ToList();

            foreach (var item in mergedItems)
            {
                var activity = ActivityRepo.GetWithChildren(item.MergedActivityId);
                activity.IsEnabled = true;
                SaveActivityDetails(activity);

                activity = ActivityRepo.GetWithChildren(item.ActivityId);
                activity.IsEnabled = false;
                SaveActivityDetails(activity);

                MergedActivityRepo.DeleteItem(item);
            }

            RefreshActivities();
        }

        void CancelActivityDetails()
        {
            IEnumerable<Activity> obsCollection = Activities;
            var list1 = new List<Activity>(obsCollection);
            var list = new List<Activity>(obsCollection);

            foreach (var item in list)
            {
                list1.RemoveAll(_ => _.Id == (int)item.Id);
                item.Colour = Color.FromHex(item.ItemColour);
                list1.Add(item);
            }

            Activities = ConvertListToObservable(list1);
            GroupActivities = Utilities.BuildGroupOfActivities(Activities);        
        }

        private void ConstructorSetUp()
        {
            SaveActivities = new Command(SaveActivityDetails);
            UnMergeActivities = new Command(UnMergeActivitiesEvent);
            CancelActivities = new Command(CancelActivityDetails);
            ActivitySelected = new Command(ActivitySelectedEvent);

            Activities = Get_All_Enabled_Activities_WithChildren();
            GroupActivities = ChangeButtonColourOnLoad();
            MergedActivities = new List<Activity>();
            var studyNumber = Utilities.StudyId > 0 ? Utilities.StudyId.ToString() : string.Empty;
            InvalidText = $"There are no activities to merge for study {studyNumber}";
            IsPageVisible = (Utilities.StudyId > 0 && !Utilities.IsCompleted && Activities.Count > 0);
        }
    }
}
