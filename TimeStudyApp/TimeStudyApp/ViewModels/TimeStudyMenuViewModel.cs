using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyMenuViewModel : BaseViewModel
    {
        public Command NewStudy { get; set; }
        public Command ExistingStudy { get; set; }

        public TimeStudyMenuViewModel()
        {
            NewStudy = new Command(NewStudyPage);
            ExistingStudy = new Command(ExistingStudyPage);
        }

        void NewStudyPage()
        {
            Utilities.Navigate(new TimeStudySetUpTabbedPage());
        }

        void ExistingStudyPage()
        {
            Utilities.Navigate(new ExistingStudiesTabbedPage()); 
        }
    }
}
