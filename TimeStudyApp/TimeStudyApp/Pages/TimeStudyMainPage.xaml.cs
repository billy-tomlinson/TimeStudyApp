using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class TimeStudyMainPage : ContentPage
    {
        public TimeStudyMainPage()
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

            if (Utilities.WorkElementTableUpdated)
            {
                if (!Utilities.RatedTimeStudyPageHasUpdatedWorkElementChanges)
                {
                    Utilities.TimeStudyMainPageHasUpdatedWorkElementChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new TimeStudyMainPageViewModel()
                    {
                        RatingsVisible = false,
                        ActivitiesVisible = false,
                    };

                    BindingContext = viewModel;
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();
        }
    }
}
