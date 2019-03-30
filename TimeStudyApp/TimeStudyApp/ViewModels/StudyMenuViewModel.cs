using TimeStudy.Pages;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class StudyMenuViewModel : BaseViewModel
    {
        public Command NewStudy { get; set; }
        public Command ExistingStudy { get; set; }

        public StudyMenuViewModel()
        {
            NewStudy = new Command(NewStudyPage);
            ExistingStudy = new Command(ExistingStudyPage);
        }

        void NewStudyPage()
        {
            Utilities.Navigate(new StudySetUpTabbedPage());
        }

        void ExistingStudyPage()
        {
            Utilities.Navigate(new ExistingStudiesTabbedPage()); 
        }
    }
}
