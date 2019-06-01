using TimeStudy.ViewModels;

namespace TimeStudyApp.Services.StateMachine
{
    public class StateFactory
    {
        TimeStudyMainPageViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public StateFactory(TimeStudyMainPageViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public BaseState GetCurrentState()
        {
            var currentStatus = stateservice.GetApplicationState();
            BaseState currentState = null;

            switch (currentStatus.CurrentState)
            {
                case Model.Status.NoElementRunning:
                    currentState = new NoElementRunningState(viewModel);
                    break;
                case Model.Status.ElementRunning:
                    currentState = new ElementRunningState(viewModel);
                    break;
                case Model.Status.InterruptElementRunning:
                    currentState = new InterruptElementRunningState(viewModel);
                    break;
                case Model.Status.UnratedInterruptElementRunning:
                    currentState = new UnratedInterruptElementRunningState(viewModel);
                    break;
                case Model.Status.OccassionalElementRunning:
                    currentState = new OccassionalElementRunningState(viewModel);
                    break;
                case Model.Status.UnratedOccassionalElementRunning:
                    currentState = new UnratedOccassionalElementRunningState(viewModel);
                    break;
                default:
                    break;
            }

            return currentState;

        }
    }
}
