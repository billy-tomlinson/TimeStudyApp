using System.Windows.Input;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyAboutPageViewModel : BaseViewModel
    {
        public ICommand ClickCommand => new Command<string>((url) =>
        {
            Device.OpenUri(new System.Uri(url));
        });

        public ICommand DemoCommand => new Command<string>((url) =>
        {
            string demoUrl = string.Empty;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    demoUrl = "https://youtu.be/DJzYKROHQFY";
                    break;
                case Device.Android:
                    demoUrl = "https://youtu.be/Nyu9tgsonEM";
                    break;
            }

            Device.OpenUri(new System.Uri(demoUrl));
        });
    }
}
