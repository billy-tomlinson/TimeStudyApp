using System.Windows.Input;
using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public ICommand StudyMenu { get; set; }
        public ICommand StopWatch { get; set; }
        public ICommand AddActivities { get; set; }
        public ICommand MergeActivities { get; set; }
        public ICommand AddOperators { get; set; }
        public ICommand ExistingStudies { get; set; }
        public ICommand Reports { get; set; }
        public ICommand CurrentStudy { get; set; }
        public ICommand StudySetUp { get; set; }
        public ICommand CloseApplication { get; set; }

        public MenuPageViewModel()
        {
            StudyMenu = new Command(GoStudyMenu);
            StopWatch = new Command(GoStopWatch);
            AddActivities = new Command(GoActivities);
            MergeActivities = new Command(GoMergeActivities);
            AddOperators = new Command(GoOperators);
            ExistingStudies = new Command(GoExistingStudies);
            Reports = new Command(GoReports);
            CurrentStudy = new Command(GoCurrentStudy);
            StudySetUp = new Command(GoStudySetUp);
            CloseApplication = new Command(CloseApplicationEvent);
        }

        void CloseApplicationEvent(object obj)
        {
            DependencyService.Get<ITerminateApplication>()
                .CloseApplication();
            App.MenuIsPresented = false;
        }

        void GoStudyMenu(object obj)
        {
            Utilities.Navigate(new StudyMenuPage());
            App.MenuIsPresented = false;
        }

        void GoStopWatch(object obj)
        {
            Utilities.Navigate(new TimeStudyUnsequencedPage());
            App.MenuIsPresented = false;
        }

        void GoCurrentStudy(object obj)
        {
            if (Utilities.StudyId == 0) return;
            Utilities.Navigate(new MainPageTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoStudySetUp(object obj)
        {
            Utilities.StudyId = 0;
            Utilities.Navigate(new StudySetUpTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoActivities(object obj)
        {
            Utilities.Navigate(new AddElementsPage()); 
            App.MenuIsPresented = false;
        }

        void GoMergeActivities(object obj)
        {
            Utilities.Navigate(new EditActivitiesPage());
            App.MenuIsPresented = false;
        }

        void GoOperators(object obj)
        {
            Utilities.Navigate(new AddForeignElementsPage());
            App.MenuIsPresented = false;
        }

        void GoExistingStudies(object obj)
        {
            Utilities.Navigate(new ExistingStudiesTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoReports(object obj)
        {
            Utilities.Navigate(new ReportsPage());
            App.MenuIsPresented = false;
        }
    }
}
