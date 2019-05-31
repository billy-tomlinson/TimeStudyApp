using TimeStudy.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using TimeStudy.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TimeStudy
{
    public partial class App : Application
    {
        public static string DatabasePath = string.Empty;
        public App(){ }

        public App(string databasePath)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTAwMjk5QDMxMzYyZTMzMmUzMEdpQVpZS0g1RHZMN2RGVUEreFpGOXp3UW12dmt4RW00U09OSFdnOVd6SG89;MTAwMzAwQDMxMzYyZTMzMmUzMElsS3RQMk4ycm1mUlJFa3JYY1A3cUpzMnRsYmdkSTdoTVBGcHlWa1BQU1U9");

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");

            DatabasePath = databasePath;
            Utilities.Connection = DatabasePath;
            CallMain();
            //MainPage = new NavigationPage(new StopWatch());
        }

        public static NavigationPage NavigationPage { get; private set; }
        public static RootPage RootPage;
        public static bool MenuIsPresented
        {
            get
            {
                return RootPage.IsPresented;
            }
            set
            {
                RootPage.IsPresented = value;
            }
        }

        private void CallMain()
        {
            var menuPage = new HamburgerMenuPage(){ Title = "Main Page" , Icon="hamburger.png"  };
            NavigationPage = new NavigationPage(new WelcomePage());
            RootPage = new RootPage();
            RootPage.Master = menuPage;
            RootPage.Detail = NavigationPage;
            MainPage = RootPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            AppCenter.Start("ios=e79293ae-e7fe-4968-b897-6df32f2bf00a;" +
                  "android=4b7dad8c-e515-413d-9a8e-9e060c86a511;",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}



