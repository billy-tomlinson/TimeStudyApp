using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class LocalNotificationPage : ContentPage
    {
        public LocalNotificationPage()
        {
            InitializeComponent();
            BindingContext = new LocalNotificationPageViewModel();
        }
    }
}
