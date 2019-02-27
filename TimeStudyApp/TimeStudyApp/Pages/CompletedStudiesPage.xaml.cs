using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class CompletedStudiesPage : ContentPage
    {
        public CompletedStudiesPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");

            listOfStudies.BindingContext = new ExistingStudiesViewModel(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Utilities.ClearNavigation();
        }
    }
}
