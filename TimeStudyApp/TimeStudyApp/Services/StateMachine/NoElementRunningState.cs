using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudy.ViewModels;

namespace TimeStudyApp.Services.StateMachine
{
    public class NoElementRunningState : BaseState
    {
        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public NoElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {
            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ElementSelectedEvent()
        {
            //Utilities.CurrentSelectedElementId = id;  //viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == id);
            viewModel.StartTimerEvent();
            viewModel.IsForeignEnabled = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void RatingSelectedEvent()
        {
            viewModel.SetUpCurrentLapTime();

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowForeignElements()
        {
            viewModel.Opacity = 0.2;
            viewModel.ActivitiesVisible = true;
            return;
        }

        public override void ShowNonForeignElements()
        {
            viewModel.Opacity = 0.2;
            viewModel.ActivitiesVisible = true;
            return;
        }
    }
}
