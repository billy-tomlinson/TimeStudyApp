
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class StudySetUpTabbedPage : TabbedPage
    {
        public StudySetUpTabbedPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Utilities.ClearNavigation();
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
        }
    }
}
