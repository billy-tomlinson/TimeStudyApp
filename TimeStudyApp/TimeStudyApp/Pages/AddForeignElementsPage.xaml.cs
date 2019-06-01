using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class AddForeignElementsPage : ContentPage
    {

        public AddForeignElementsPage()
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
                if (!Utilities.ForeignElementsPageHasUpdatedRatedTimeStudyChanges)
                {
                    Utilities.ForeignElementsPageHasUpdatedRatedTimeStudyChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new AddForeignElementsViewModel
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

