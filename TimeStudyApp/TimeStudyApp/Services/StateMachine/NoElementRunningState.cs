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

            viewModel.Opacity = 1.0;
            viewModel.IsPageEnabled = true;
        }

        public override void ShowForeignElements()
        {
            viewModel.IsForeignEnabled = false;
            viewModel.Opacity = 0.2;
            viewModel.ActivitiesVisible = true;
            return;
        }

        public override void ShowStandardElements()
        {
            viewModel.IsForeignEnabled = false;
            viewModel.Opacity = 0.2;
            viewModel.ActivitiesVisible = true;
            return;
        }

        public override void CloseActivitiesViewEvent()
        {
            viewModel.IsForeignEnabled = false;
        }
    }
}
