using System.Windows.Input;
using TimeStudy.Pages;
using TimeStudy.Services;
using TimeStudyApp.Pages;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class MenuPageViewModel : BaseViewModel
    {
        public ICommand StudyMenu { get; set; }
        public ICommand ExistingStudies { get; set; }
        public ICommand Reports { get; set; }
        public ICommand CurrentStudy { get; set; }
        public ICommand StudySetUp { get; set; }
        public ICommand CloseApplication { get; set; }
        public ICommand About { get; set; }
        public ICommand CurrentStudyDetails { get; set; }

        public string VersionAndBuild => "version " + Utilities.Version + " build " + Utilities.Build;

        public MenuPageViewModel()
        {
            StudyMenu = new Command(GoStudyMenu);
            ExistingStudies = new Command(GoExistingStudies);
            Reports = new Command(GoReports);
            CurrentStudy = new Command(GoCurrentStudy);
            StudySetUp = new Command(GoStudySetUp);
            CloseApplication = new Command(CloseApplicationEvent);
            About = new Command(AboutEvent);
            CurrentStudyDetails = new Command(CurrentStudyDetailsEvent);
        }

        void CloseApplicationEvent(object obj)
        {
            DependencyService.Get<ITerminateApplication>()
                .CloseApplication();
            App.MenuIsPresented = false;
        }

        void GoStudyMenu(object obj)
        {
            Utilities.Navigate(new TimeStudyMenuPage());
            App.MenuIsPresented = false;
        }

        void CurrentStudyDetailsEvent(object obj)
        {
            Utilities.Navigate(new CurrentStudyDetailsPage());
            App.MenuIsPresented = false;
        }

        void GoCurrentStudy(object obj)
        {
            //if (Utilities.StudyId == 0) return;
            Utilities.Navigate(new TimeStudyMainPageTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoStudySetUp(object obj)
        {
            Utilities.StudyId = 0;
            Utilities.Navigate(new TimeStudySetUpTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoExistingStudies(object obj)
        {
            Utilities.Navigate(new ExistingStudiesTabbedPage());
            App.MenuIsPresented = false;
        }

        void GoReports(object obj)
        {
            Utilities.Navigate(new TimeStudyReportsPage());
            App.MenuIsPresented = false;
        }

        void AboutEvent(object obj)
        {
            Utilities.Navigate(new AboutPage());
            App.MenuIsPresented = false;
        }
    }
}
