using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Messaging;
using TimeStudy.Model;
using TimeStudyApp.Model;
using TimeStudyApp.Services;
using Xamarin.Forms;

namespace TimeStudy.Services
{
    public class Utilities
    {
        public static double TimeWhenLapOrForiegnButtonClicked { get; set; }
        public static int StudyId { get; set; }
        public static int StudyVersion { get; set; }
        public static int CurrentSelectedElementId { get; set; }
        public static int CurrentRunningElementId { get; set; }
        public static int LastRatedLapTimeId { get; set; }
        public static bool IsCompleted { get; set; }
        public static bool IsForeignElement { get; set; }
        public static bool IsRunning { get; set; } = false;
        public static bool RatedStudy { get; set; }
        public static string Connection { get; set; }
        public static DateTime TimeStudyStarted { get; set; }
        public static string Version { get => DependencyService.Get<IAppVersion>().GetVersion(); }
        public static string Build { get => DependencyService.Get<IAppVersion>().GetBuild(); }

        public static bool WorkElementTableUpdated { get; set; }
        public static bool RatedTimeStudyTableUpdated { get; set; }

        public static bool RatedTimeStudyPageHasUpdatedWorkElementChanges { get; set; }
        public static bool TimeStudyMainPageHasUpdatedWorkElementChanges { get; set; }

        public static bool StandardElementPageHasUpdatedStandardElementChanges { get; set; }
        public static bool StandardElementsPageHasUpdatedRatedTimeStudyChanges { get; set; }
        public static bool ForeignElementsPageHasUpdatedRatedTimeStudyChanges { get; set; }

        public static bool LapButtonClicked { get; set; }

        public static void UpdateTableFlags()
        {
            if (TimeStudyMainPageHasUpdatedWorkElementChanges)
                WorkElementTableUpdated = false;

            if (StandardElementsPageHasUpdatedRatedTimeStudyChanges && ForeignElementsPageHasUpdatedRatedTimeStudyChanges)
                RatedTimeStudyTableUpdated = false;
        }

        public static async Task Navigate(Page page)
        {
            await Task.Delay(500);
            await App.NavigationPage.Navigation.PushAsync(page);
        }

        public static Color Clicked = Color.FromHex("#CCCCCE");
        public static Color UnClicked = Color.FromHex("#4174f4");

        public const string ValueAddedColour = "#74D3AE";
        public const string NonValueAddedColour = "#F7CE5B";
        public const string InactiveColour = "#ED6A5A";
        public const string ValidColour = "#d5f0f1";
        public const string ClickedHex = "#CCCCCE";


        public const string DeleteImage = "delete.png";
        public static string UndoImage = "undo.png";
        public const string CommentsImage = "comments.png";

        public static void ClearNavigation()
        {
            var existingPages = App.NavigationPage?.Navigation.NavigationStack.ToList();

            for (int i = 0; i < existingPages?.Count; i++)
            {
                if (i != existingPages.Count - 1)
                    App.NavigationPage.Navigation.RemovePage(existingPages[i]);
            }
        }

        public static ObservableCollection<MultipleActivities> BuildGroupOfActivities(ObservableCollection<WorkElement> activites)
        {
            int counter = 0;
            bool added = false;
            var multipleActivities = new MultipleActivities();
            var groupedActivities = new ObservableCollection<MultipleActivities>();

            for (int i = 0; i < activites.Count; i++)
            {
                var activity = activites[i];

                if (counter == 0)
                {
                    multipleActivities.ActivityOne = activity;
                    added = false;
                    counter++;
                }

                else if (counter == 1)
                {
                    multipleActivities.ActivityTwo = activity;
                    added = false;
                    counter++;
                }

                else if (counter == 2)
                {
                    multipleActivities.ActivityThree = activity;
                    groupedActivities.Add(multipleActivities);
                    added = true;
                    multipleActivities = new MultipleActivities();
                    counter = 0;
                }
            }

            if (!added) groupedActivities.Add(multipleActivities);

            return groupedActivities;
        }

        public static void MoveLapsToHistoryTable()
        {
            var lapTimeRepo = new BaseRepository<LapTime>(Connection);
            var lapTimeHistoricRepo = new BaseRepository<LapTimeHistoric>(Connection);

            var laps = lapTimeRepo.GetAllWithChildren().ToList();
            var historicLaps = lapTimeHistoricRepo.GetAllWithChildren().ToList();
            var sqlCommand = "INSERT INTO LapTimeHistoric SELECT * FROM LapTime";
            lapTimeHistoricRepo.ExecuteSQLCommand(sqlCommand);
            sqlCommand = "DELETE FROM LapTime";
            lapTimeRepo.ExecuteSQLCommand(sqlCommand);
        }

        public static void SendEmail(TimeStudySpreadSheet spreadSheet)
        {
            var email = new EmailMessageBuilder()
                .Subject("Activity Sample Results")
                .Body($"Attached are the results for Study {StudyId}")
                .WithAttachment(Path.Combine(spreadSheet.FilePath, spreadSheet.FileName), "application/msexcel")
                .Build();

            var emailTask = CrossMessaging.Current.EmailMessenger;
            if (emailTask.CanSendEmail)
            {
                emailTask.SendEmail(email);
            }
        }

        public static LapTime SetUpCurrentLapTime(WorkElement activity, int cycleCount, RunningStatus status, 
            Color? colour = null)
        {
            LapTime lapTime;
            lapTime =  new LapTime
            {
                Cycle = cycleCount,
                Element = activity.Name,
                Status = RunningStatus.Running,
                IsForeignElement = IsForeignElement,
                StudyId = StudyId,
                ActivityId = CurrentSelectedElementId,
                IsRated = activity.Rated,
                Version = StudyVersion,
                TimeWhenLapStarted = TimeWhenLapOrForiegnButtonClicked,
                IsValueAdded = activity.IsValueAdded
            };

            if (colour != null) lapTime.ElementColour = (Color)colour;

            return lapTime;
        }
    }
}
