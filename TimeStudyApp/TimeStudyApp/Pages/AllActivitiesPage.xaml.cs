using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class AllActivitiesPage : ContentPage
    {
        public AllActivitiesPage()
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
            if (Utilities.ActivityTableUpdated)
            {
                if (!Utilities.AllActivitiesPageHasUpdatedActivityChanges)
                {
                    Utilities.AllActivitiesPageHasUpdatedActivityChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new AllActivitiesViewModel();

                    BindingContext = viewModel;
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();

        }
    }
}
