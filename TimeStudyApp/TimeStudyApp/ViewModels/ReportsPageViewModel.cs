using System;
using System.Threading.Tasks;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class ReportsPageViewModel : BaseViewModel
    {
        public Command SendEmail { get; set; }

        public ReportsPageViewModel()
        {
            ConstructorSetUp();
        }

        private void ConstructorSetUp()
        {
            SendEmail = new Command(SendEmailDetails);
            IsPageVisible = (Utilities.StudyId > 0);
        }

        private async void SendEmailDetails()
        {
            IsBusy = true;
            IsEnabled = false;
            Opacity = 0.2;

            Utilities.MoveLapsToHistoryTable();

            try 
            {
                Task emailTask = Task.Run(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var spreadsheet = new SpreadsheetService().CreateExcelWorkBook();
                        Utilities.SendEmail(spreadsheet);
                    });
                });

                 await emailTask;
            }
            catch (Exception ex)
            {
                IsEnabled = true;
                IsBusy = false;
                Opacity = 1;

                ValidationText = "Unable to generate report.Please check STORAGE permissions.";
                Opacity = 0.2;
                IsInvalid = true;
                ShowClose = true;
            }

            IsBusy = false;
            IsEnabled = true;
            Opacity = 1;
        }
    }
}
