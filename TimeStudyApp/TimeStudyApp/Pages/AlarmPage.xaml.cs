
using Plugin.Toasts;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class AlarmPage : ContentPage
    {
        public AlarmPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Utilities.ClearNavigation();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
