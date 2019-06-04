using System;
using System.Linq;
using System.Windows.Input;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyDetailsViewModel : BaseViewModel
    {
        public TimeStudyDetailsViewModel(string conn) : base(conn) { ConstructorSetUp(); }

        public TimeStudyDetailsViewModel()
        {
            ConstructorSetUp(); 
        }

        public ICommand SubmitAndFocusActivities => new Command
        (
            (parameter) =>
            {
                ValidateValues();

                if (!IsInvalid)
                {
                    StudyPageOpacity = 0.5;
                    SampleStudy.IsRated = true;
                    Utilities.StudyId = RatedTimeStudyRepo.SaveItem(SampleStudy);
                    StudyNumber = Utilities.StudyId;
                    CreateUnratedActivities();

                    Utilities.RatedStudy = true;

                    var page = parameter as ContentPage;
                    var parentPage = page.Parent as TabbedPage;
                    parentPage.CurrentPage = parentPage.Children[1];

                    IsActive = false;
                }

                ShowClose = true;
                IsPageVisible = true;
            }
        );

        double studyPageOpacity = 1;
        public double StudyPageOpacity
        {
            get { return studyPageOpacity; }
            set
            {
                studyPageOpacity = value;
                OnPropertyChanged();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged();
            }
        }

        Model.RatedTimeStudy sampleStudy;
        public RatedTimeStudy SampleStudy
        {
            get { return sampleStudy; }
            set
            {
                sampleStudy = value;
                OnPropertyChanged();
            }
        }

        private void ConstructorSetUp()
        {
       
            IsActive = true;
            IsPageEnabled = true;
            Utilities.StudyId = 0;

            SampleStudy = new Model.RatedTimeStudy()
            {
                IsRated = true,
                Date = DateTime.Now,
                Time = DateTime.Now.TimeOfDay
            };

            IsPageVisible = true;

            int lastStudyId = 0;
            var studies = RatedTimeStudyRepo.GetItems()?.ToList();

            if (studies.Count > 0)
                lastStudyId = studies.OrderByDescending(x => x.Id)
                                        .FirstOrDefault().Id;

            lastStudyId = lastStudyId + 1;

            SampleStudy.StudyNumber = lastStudyId;
            CloseView = new Command(CloseValidationView);

        }

        private void ValidateValues()
        {
            ValidationText = "Please enter all study details";
            ShowClose = true;
            IsInvalid = true;
            Opacity = 0.2;
            IsPageEnabled = false;

            if ((SampleStudy.Department != null && SampleStudy.Department?.Trim().Length > 0) &&
                (SampleStudy.Name != null && SampleStudy.Name?.Trim().Length > 0) &&
                (SampleStudy.StudiedBy != null && SampleStudy.StudiedBy?.Trim().Length > 0))
            {
                Opacity = 1;
                IsInvalid = false;
                IsPageEnabled = true;
            }
        }

        public void CreateUnratedActivities()
        {
            var activityName = WorkElementNameRepo.GetItems().FirstOrDefault(x => x.Name == "INEFFECTIVE");

            if(activityName == null)
                activityName = new WorkElementName { Name = "INEFFECTIVE" };
                
            var unrated1 = new WorkElement()
            {
                ActivityName = activityName,
                IsEnabled = true,
                Rated = false,
                StudyId = Utilities.StudyId,
                DeleteIcon = string.Empty,
                ItemColour = Utilities.InactiveColour,
                ObservedColour = Utilities.InactiveColour,
                IsValueAdded = false//,
                //IsForeignElement = true

            };

            activityName = WorkElementNameRepo.GetItems().FirstOrDefault(x => x.Name == "LOST TIME");

            if (activityName == null)
                activityName = new WorkElementName { Name = "LOST TIME" };
                
            var unrated2 = new WorkElement()
            {
                ActivityName = activityName,
                IsEnabled = true,
                Rated = false,
                StudyId = Utilities.StudyId,
                DeleteIcon = string.Empty,
                ItemColour = Utilities.InactiveColour,
                ObservedColour = Utilities.InactiveColour,
                IsValueAdded = false//,
                //IsForeignElement = true
            };

            WorkElementNameRepo.SaveItem(unrated1.ActivityName);
            WorkElementRepo.SaveItem(unrated1);
            WorkElementRepo.UpdateWithChildren(unrated1);

            WorkElementNameRepo.SaveItem(unrated2.ActivityName);
            WorkElementRepo.SaveItem(unrated2);
            WorkElementRepo.UpdateWithChildren(unrated2);
        }
    }
}