using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudyApp.Pages
{
    public partial class TimeStudyAboutPage : ContentPage
    {
        public TimeStudyAboutPage()
        {
            InitializeComponent();
            timeStudy.Source = ImageSource.FromFile("stopwatch.png");
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override void OnAppearing()
        {
            Utilities.ClearNavigation();
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
