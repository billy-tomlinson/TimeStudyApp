using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudy.ViewModels;

namespace TimeStudyApp.Services.StateMachine
{
    public class ForeignElementRunningState : BaseState
    {
        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public ForeignElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {

            var currentRunning = viewModel.Get_Running_LapTime();
            currentRunning.TotalElapsedTime = "Paused";
            viewModel.LapTimeRepo.SaveItem(currentRunning);

            var currentSelected = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            var currentForeignLap = new LapTime
            {
                Cycle = viewModel.CycleCount,
                Element = currentSelected.Name,
                TotalElapsedTime = "Running",
                IsForeignElement = currentSelected.IsForeignElement,
                StudyId = Utilities.StudyId
            };

            var id = viewModel.LapTimeRepo.SaveItem(currentForeignLap);

            viewModel.CurrentElementWithoutLapTimeName = currentForeignLap.Element;
            viewModel.CurrentSequence = null;

            viewModel.LapTimes = viewModel.Get_All_LapTimes_Not_Running();

            viewModel.Opacity = 0.2;
            viewModel.RatingsVisible = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ElementSelectedEvent()
        {
            viewModel.IsForeignEnabled = false;
            viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void RatingSelectedEvent()
        {

            viewModel.CollectionOfElements = viewModel.Get_All_Foreign_Enabled_Activities_WithChildren();

            var currentSelected = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            viewModel.ProcessForeignElementWithRating(currentSelected.Rated, currentSelected.Name,
                currentSelected.IsForeignElement, viewModel.RatingButton.Rating);

            if (viewModel.LapButtonText != "   Lap   ")
            {
                viewModel.ActivitiesVisible = true;
                viewModel.RatingsVisible = false;
                viewModel.Opacity = 0.2;
            }
            else
            {
                viewModel.ReInstatePausedLapTimeToCurrentRunning();
                viewModel.ActivitiesVisible = false;
                viewModel.RatingsVisible = false;
                viewModel.Opacity = 1.0;
            }

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowForeignElements()
        {
            viewModel.CollectionOfElements = viewModel.Get_All_Foreign_Enabled_Activities_WithChildren();
            viewModel.GroupElementsForActivitiesView();

            var runningLapTime = viewModel.Get_Running_LapTime();
            if (runningLapTime.Rating == null)
            {
                viewModel.ReInstatePausedLapTimeToCurrentRunning();
                viewModel.RatingsVisible = false;
                viewModel.ActivitiesVisible = true;
                viewModel.Opacity = 0.2;
   
            }
            else
            {

                viewModel.Opacity = 0.2;
                viewModel.ActivitiesVisible = true;
            }
        }

        public override void ShowNonForeignElements()
        {
            var runningLapTime = viewModel.Get_Running_LapTime();
            if (runningLapTime.Rating == null)
            {
                viewModel.RatingsVisible = true;
                viewModel.Opacity = 0.2;
            }
        }
    }
}
