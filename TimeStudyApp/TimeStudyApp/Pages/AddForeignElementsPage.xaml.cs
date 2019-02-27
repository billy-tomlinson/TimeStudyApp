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
            if (Utilities.ActivityTableUpdated || Utilities.OperatorTableUpdated || Utilities.ObservationTableUpdated || Utilities.ActivitySampleTableUpdated)
            {
                if (!Utilities.ForeignElementsPageHasUpdatedActivityChanges
                        || !Utilities.ForeignElementsPageHasUpdatedOperatorChanges
                        || !Utilities.ForeignElementsPageHasUpdatedActivitySampleChanges)
                {
                    Utilities.ForeignElementsPageHasUpdatedActivityChanges = true;
                    Utilities.ForeignElementsPageHasUpdatedOperatorChanges = true;
                    Utilities.ForeignElementsPageHasUpdatedActivitySampleChanges = true;

                    Utilities.UpdateTableFlags();

                    BindingContext = new AddForeignElementsViewModel();
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();
        }
    }
}

