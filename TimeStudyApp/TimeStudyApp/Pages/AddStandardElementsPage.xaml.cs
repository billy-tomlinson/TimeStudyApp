using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class AddStandardElementsPage : ContentPage
    {
        public AddStandardElementsPage()
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
            if (Utilities.RatedTimeStudyTableUpdated)
            {
                if (!Utilities.StandardElementsPageHasUpdatedRatedTimeStudyChanges)
                {
                    Utilities.StandardElementsPageHasUpdatedRatedTimeStudyChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new AddStandardElementsViewModel
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
