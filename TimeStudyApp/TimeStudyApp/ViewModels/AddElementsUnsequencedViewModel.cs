﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeStudy.Model;
using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class AddElementsUnsequencedViewModel : BaseViewModel
    {
        public Command SaveActivity { get; set; }
        public Command SaveComment { get; set; }
        public Command CancelComment { get; set; }
        public Command ItemSelected { get; set; }
        public Command MoveUpSelected { get; set; }
        public Command MoveDownSelected { get; set; }
        public Command CloseCategories { get; set; }
        public Command SaveCategory { get; set; }
        public Command SettingsSelected { get; set; }
        public Command DeleteSelected { get; set; }
        public Activity Activity;
        public int ActivitiesCount;
       
        public AddElementsUnsequencedViewModel()
        {
            ConstructorSetUp();
        }

        public AddElementsUnsequencedViewModel(string conn) : base(conn)
        {
            ConstructorSetUp();
        }

        static ObservableCollection<Activity> itemsCollection;
        public ObservableCollection<Activity> ItemsCollection
        {
            get => itemsCollection;
            set
            {
                itemsCollection = value;
                OnPropertyChanged();
            }
        }

        private string comment;
        public string Comment
        {
            get => comment;
            set
            {
                comment = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        static bool commentsVisible;
        public bool CommentsVisible
        {
            get => commentsVisible;
            set
            {
                commentsVisible = value;
                OnPropertyChanged();
            }
        }

        static bool categoriesVisible;
        public bool CategoriesVisible
        {
            get => categoriesVisible;
            set
            {
                categoriesVisible = value;
                OnPropertyChanged();
            }
        }

        bool _isNonValueAdded;
        public bool IsNonValueAdded
        {
            get => _isNonValueAdded;
            set
            {
                _isNonValueAdded = value;
                OnPropertyChanged();
                Switch_Toggled();
            }
        }

        void Switch_Toggled()
        {
            ActivityType = IsNonValueAdded == false ? "VALUE ADDED" : "NON VALUE ADDED";
        }

        string activityType = "VALUE ADDED";
        public string ActivityType
        {
            get { return activityType; }
            set
            {
                activityType = value;
                OnPropertyChanged();
            }
        }

        void SaveActivityDetails()
        {
            ValidateValues();

            if (!IsInvalid)
            {
                var duplicatesCheck = ActivityNameRepo.GetItems()
                    .FirstOrDefault(_ => _.Name?.ToUpper() == Name.ToUpper().Trim());

                if (duplicatesCheck == null)
                {
                    var activities = Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().Count();

                    var activityName = new ActivityName()
                    {
                        Name = Name.ToUpper().Trim()
                    };
                    var activity = new Activity
                    {
                        ActivityName = activityName,
                        IsEnabled = true,
                        ObservedColour = Utilities.ValueAddedColour,
                        IsValueAdded = true,
                        Sequence = activities + 1,
                        IsForeignElement = false
                    };

                    SaveActivityDetails(activity);
                    Utilities.ActivityPageHasUpdatedActivityChanges = true;
                }
                else
                {
                    ValidationText = $"{Name.ToUpper().Trim()} is a duplicate. Please add a unique element or select from list.";
                    Opacity = 0.2;
                    IsInvalid = true;
                    ShowClose = true;
                }

                ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
                ActivitiesCount = ItemsCollection.Count;
                Name = string.Empty;
            }
        }

        void SaveCommentDetails()
        {
            if (Comment != null)
            {
                Activity.Comment = Comment.ToUpper();
                ActivityRepo.SaveItem(Activity);
                Utilities.ActivityPageHasUpdatedActivityChanges = true;
            }

            Opacity = 1;
            CommentsVisible = false;
            Comment = string.Empty;
        }

        void CancelCommentDetails()
        {
            Opacity = 1;
            CommentsVisible = false;
            Comment = string.Empty;
        }

        public override void SubmitDetailsAndNavigate()
        {
            ValidateActivitiesAdded();

            if (!IsInvalid)
            {
                Utilities.Navigate(new AddForeignElementsPage());
            }
        }

        public ICommand ItemClickedCommand
        {
            get { return ShowComments(); }
        }

        Command ShowComments()
        {
            return new Command((item) =>
            {
                Activity = item as Activity;
                Comment = Activity.Comment;
                Opacity = 0.2;
                CommentsVisible = true;
            });
        }

        public void ValidateValues()
        {
            ValidationText = "Please Enter a valid Name";

            IsInvalid = true;
            ShowClose = true;
            Opacity = 0.2;

            if ((Name != null && Name?.Trim().Length > 0))
            {
                Opacity = 1;
                IsInvalid = false;
            }
        }

        private void ValidateActivitiesAdded()
        {

            ValidationText = "Please add at least one Element";

            IsInvalid = true;
            ShowClose = true;
            Opacity = 0.2;

            var activities = Get_Rated_Enabled_Activities();

            if ((activities.Count > 0))
            {
                Opacity = 1;
                IsInvalid = false;
            }
        }

        void CloseCategoriesEvent(object sender)
        {
            Opacity = 1.0;
            CategoriesVisible = false;
        }

        void SaveCategoryEvent(object sender)
        {
            if (IsNonValueAdded)
            {
                Activity.ItemColour = Utilities.NonValueAddedColour;
                Activity.ObservedColour = Utilities.NonValueAddedColour;
            }
            else
            {
                Activity.ItemColour = Utilities.ValueAddedColour;
                Activity.ObservedColour = Utilities.ValueAddedColour;
            }

            Activity.IsValueAdded = !IsNonValueAdded;
            ActivityRepo.SaveItem(Activity);
            Opacity = 1.0;
            ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren()
                .OrderByDescending(x => x.Id));

            CategoriesVisible = false;
            Utilities.ActivityPageHasUpdatedActivityChanges = true;
        }

        void MoveElementUpOnePlace(object sender)
        {
            if (Activity.Sequence == 1 || Activity.Sequence <= 0) return;

            int sequenceNumber;

            var activity1 = Activity;
            sequenceNumber = activity1.Sequence;
            activity1.Sequence = sequenceNumber - 1;

            var activity2 = ActivityRepo.GetItems()
                .FirstOrDefault(x => x.Sequence == sequenceNumber - 1 && x.StudyId == Utilities.StudyId);
            activity2.Sequence = sequenceNumber;
            ActivityRepo.SaveItem(activity1);
            ActivityRepo.SaveItem(activity2);

            ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
        }

        void MoveElementDownOnePlace(object sender)
        {
            if (Activity.Sequence >= ActivitiesCount || Activity.Sequence <= 0) return;

            int sequenceNumber;

            var activity1 = Activity;
            sequenceNumber = activity1.Sequence;

            activity1.Sequence = sequenceNumber + 1;

            var activity2 = ActivityRepo.GetItems()
                .FirstOrDefault(x => x.Sequence == sequenceNumber + 1 && x.StudyId == Utilities.StudyId);
            activity2.Sequence = sequenceNumber;
            ActivityRepo.SaveItem(activity1);
            ActivityRepo.SaveItem(activity2);

            ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
        }

        void ActivitySelectedEvent(object sender)
        {
            SetElementsColour();

            var value = (int)sender;
            Activity = ActivityRepo.GetItem(value);
            Activity.ItemColour = Utilities.NonValueAddedColour;
            Activity.ObservedColour = Utilities.NonValueAddedColour;
            ActivityRepo.SaveItem(Activity);

            ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
        }

        private void SetElementsColour()
        {
            var activities = Get_All_Enabled_Activities();

            foreach (var item in activities)
            {
                item.ItemColour = Utilities.ValueAddedColour;
                item.ObservedColour = Utilities.ValueAddedColour;
                ActivityRepo.SaveItem(item);
            }
        }

        void AddSelectedEvent(object sender)
        {
            var value = (int)sender;
            Activity = ActivityRepo.GetItem(value);
            Comment = Activity.Comment;
            Opacity = 0.2;
            CommentsVisible = true;
        }

        async void DeleteSelectedEvent(object sender)
        {
            var value = (int)sender;

            if (!StudyInProcess)
            {
                Activity = ActivityRepo.GetItem(value);

                var activitiesToBeSequenced = ActivityRepo.GetItems()
                    .Where(x => x.Sequence > Activity.Sequence && x.StudyId == Utilities.StudyId);

                if (!activitiesToBeSequenced.Any())
                {
                    await DeleteActivity(value);
                    return;
                }

                foreach (var item in activitiesToBeSequenced)
                {
                    item.Sequence = item.Sequence - 1;
                    ActivityRepo.SaveItem(item);
                }

                await DeleteActivity(value);
            }

            else
            {
                var obs = ObservationRepo.GetItems().Where(x => x.ActivityId == value
                          && x.StudyId == Utilities.StudyId);

                if (obs.Any())
                {
                    ValidationText = "Cannot delete an element once used in Study.";
                    Opacity = 0.2;
                    IsInvalid = true;
                    ShowClose = true;
                }
                else
                {
                    await DeleteActivity(value);
                }

            }
            Utilities.ActivityPageHasUpdatedActivityChanges = true;
        }

        private async Task DeleteActivity(int value)
        {
            IsBusy = true;
            IsEnabled = false;
            Opacity = 0.2;
            Task deleteTask = Task.Run(() =>
            {
                Activity = ActivityRepo.GetWithChildren(value);
                ActivityRepo.DeleteItem(Activity);

                var activities = ActivityRepo
                                    .GetAllWithChildren()
                                    .Where(x => x.ActivityName.Name == Activity.ActivityName.Name
                                     && x.StudyId != Utilities.StudyId);

                if (!activities.Any())
                    ActivityNameRepo.DeleteItem(Activity.ActivityName);

                SetElementsColour();
                ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
                ActivitiesCount = ItemsCollection.Count;
            });

            await deleteTask;

            IsEnabled = true;
            IsBusy = false;
            Opacity = 1;
        }

        private void ConstructorSetUp()
        {
            SaveActivity = new Command(SaveActivityDetails);
            SaveComment = new Command(SaveCommentDetails);
            CancelComment = new Command(CancelCommentDetails);
            ItemSelected = new Command(ActivitySelectedEvent);
            MoveUpSelected = new Command(MoveElementUpOnePlace);
            MoveDownSelected = new Command(MoveElementDownOnePlace);
            CloseCategories = new Command(CloseCategoriesEvent);
            SaveCategory = new Command(SaveCategoryEvent);
            SettingsSelected = new Command(AddSelectedEvent);
            DeleteSelected = new Command(DeleteSelectedEvent);

            Name = string.Empty;
            CheckActivitiesInUse();
            SetElementsColour();
            ItemsCollection = new ObservableCollection<Activity>(Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren().OrderBy(x => x.Sequence));
            ActivitiesCount = ItemsCollection.Count;
            var count = ItemsCollection.Count;
            Activity = new Activity
            {
                SettingsIcon = Utilities.CommentsImage
            };
        }

        private void CheckActivitiesInUse()
        {
            var activities = Get_All_Enabled_Activities();

            foreach (var item in activities)
            {
                var obs = ObservationRepo.GetItems()
                                         .Where(x => x.ActivityId == item.Id || x.AliasActivityId == item.Id
                                          && x.StudyId == Utilities.StudyId)
                                         .ToList();

                var merged = MergedActivityRepo.GetItems()
                                               .Where(x => x.ActivityId == item.Id || x.MergedActivityId == item.Id)
                                               .ToList();

                var deleteIcon = item.Rated ? Utilities.DeleteImage : string.Empty;

                if (obs.Any() || merged.Any())
                {
                    deleteIcon = string.Empty;
                }

                var activity = ActivityRepo.GetWithChildren(item.Id);
                activity.DeleteIcon = deleteIcon;

                SaveActivityDetails(activity);
                Utilities.ActivityPageHasUpdatedActivityChanges = true;
            }
        }
    }
}

