using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class TimeStudyPage : ContentPage
    {
        public TimeStudyPage()
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
            if (Utilities.ActivityTableUpdated || Utilities.OperatorTableUpdated || Utilities.ObservationTableUpdated)
            {
                if (!Utilities.TimeStudyPageHasUpdatedActivityChanges
                    || !Utilities.TimeStudyPageHasUpdatedOperatorChanges
                    || !Utilities.TimeStudyPageHasUpdatedObservationChanges)
                {
                    Utilities.TimeStudyPageHasUpdatedActivityChanges = true;
                    Utilities.TimeStudyPageHasUpdatedOperatorChanges = true;
                    Utilities.TimeStudyPageHasUpdatedObservationChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new TimeStudyViewModel
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
