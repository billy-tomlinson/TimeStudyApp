using TimeStudy.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TimeStudy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TimeStudyWelcomePage : ContentPage
	{
		public TimeStudyWelcomePage ()
		{
			InitializeComponent ();
            timeStudy.Source = ImageSource.FromFile("timestudyicon.png");
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
            Navigate();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Utilities.ClearNavigation();
        }

	    async void Navigate()
	    {
	        await System.Threading.Tasks.Task.Delay(3000);
	        await Navigation.PushAsync(new TimeStudyMenuPage());
	    }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}