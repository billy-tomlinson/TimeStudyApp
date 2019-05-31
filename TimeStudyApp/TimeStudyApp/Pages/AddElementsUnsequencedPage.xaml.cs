using TimeStudy.Custom;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace TimeStudy.Pages
{
    public partial class AddElementsUnsequencedPage : ContentPage
    {
        public AddElementsUnsequencedPage()
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
            if (Utilities.ActivitySampleTableUpdated)
            {
                if (!Utilities.ActivityPageHasUpdatedActivitySampleChanges)
                {
                    Utilities.ActivityPageHasUpdatedActivitySampleChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new AddElementsUnsequencedViewModel
                    {
                        CommentsVisible = false
                    };
                    BindingContext = viewModel;
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();
           
        }
    }
}
