using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class EditActivitiesPage : ContentPage
    {
        public EditActivitiesPage()
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
                if (!Utilities.MergePageHasUpdatedActivityChanges)
                {
                    Utilities.MergePageHasUpdatedActivityChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new EditActivitiesViewModel();

                    BindingContext = viewModel;
                }
            }

            //BindingContext = new EditActivitiesViewModel();
            base.OnAppearing();
            Utilities.ClearNavigation();
        }
    }
}
