using TimeStudy.Custom;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace TimeStudy.Pages
{
    public partial class AddElementsPage : ContentPage
    {
        public AddElementsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnAppearing()
        {
            if (Utilities.ActivityTableUpdated || Utilities.OperatorTableUpdated || Utilities.ObservationTableUpdated || Utilities.ActivitySampleTableUpdated)
            {
                if (!Utilities.ActivityPageHasUpdatedActivityChanges 
                        || !Utilities.ActivityPageHasUpdatedOperatorChanges
                        || !Utilities.ActivityPageHasUpdatedObservationChanges
                        || !Utilities.ActivityPageHasUpdatedActivitySampleChanges)
                {
                    Utilities.ActivityPageHasUpdatedActivityChanges = true;
                    Utilities.ActivityPageHasUpdatedOperatorChanges = true;
                    Utilities.ActivityPageHasUpdatedObservationChanges = true;
                    Utilities.ActivityPageHasUpdatedActivitySampleChanges = true;


                    Utilities.UpdateTableFlags();

                    var viewModel = new AddElementsViewModel
                    {
                        CommentsVisible = false
                    };
                    BindingContext = viewModel;
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();
           
        }

        private static void UpdateTableFlags()
        {
            if (Utilities.MainPageHasUpdatedActivityChanges
                                    && Utilities.ActivityPageHasUpdatedActivityChanges)
                Utilities.ActivityTableUpdated = false;

            if (Utilities.MainPageHasUpdatedOperatorChanges
                && Utilities.ActivityPageHasUpdatedOperatorChanges)
                Utilities.OperatorTableUpdated = false;

            if (Utilities.MainPageHasUpdatedObservationChanges
                && Utilities.ActivityPageHasUpdatedObservationChanges)
                Utilities.ObservationTableUpdated = false;
        }
    }
}
