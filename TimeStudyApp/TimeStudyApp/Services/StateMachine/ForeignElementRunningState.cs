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

        public override void AddElementWithoutLapTimeToList(Activity element)
        {
            var currentForeignLap = new LapTime
            {
                Cycle = viewModel.CycleCount,
                Element = element.Name,
                TotalElapsedTime = "Running",
                IsForeignElement = element.IsForeignElement,
                StudyId = Utilities.StudyId
            };

            viewModel.CurrentWithoutLapTime = currentForeignLap;

            var id = viewModel.LapTimeRepo.SaveItem(viewModel.CurrentWithoutLapTime);
            viewModel.CurrentWithoutLapTime = viewModel.Get_Running_LapTime(id);

            viewModel.Opacity = 0.2;
            viewModel.RatingsVisible = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ElementSelectedEvent(int id)
        {
            viewModel.CurrentSelectedElement = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == id);
            viewModel.IsForeignEnabled = false;
        }

        public override void RatingSelectedEvent()
        {
            viewModel.ProcessForeignElementWithRating(viewModel.CurrentSelectedElement.Rated, viewModel.CurrentSelectedElement.Name,
                viewModel.CurrentSelectedElement.IsForeignElement, viewModel.RatingButton.Rating);

            if (viewModel.LapButtonText != "   Lap   ")
            {
                viewModel.ActivitiesVisible = true;
                viewModel.RatingsVisible = false;
                viewModel.Opacity = 0.2;
            }
            else
            {
                viewModel.CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();
                viewModel.ActivitiesVisible = false;
                viewModel.RatingsVisible = false;
                viewModel.Opacity = 1.0;
            }

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowForeignElements()
        {
            if (viewModel.CurrentWithoutLapTime.Rating == null)
            {
                viewModel.CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();

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
            if (viewModel.CurrentWithoutLapTime.Rating == null)
            {
                viewModel.RatingsVisible = true;
                viewModel.Opacity = 0.2;
            }
        }
    }
}
