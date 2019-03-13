using TimeStudy.ViewModels;

namespace TimeStudyApp.Services.StateMachine
{
    public class StateFactory
    {
        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public StateFactory(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public BaseState GetCurrentState()
        {
            var currentStatus = stateservice.GetApplicationState();
            BaseState currentState = null;

            switch (currentStatus.CurrentState)
            {
                case Model.Status.ElementRunning:
                    currentState = new ElementRunningState(viewModel);
                    break;
                case Model.Status.ForeignElementRunning:
                    currentState = new ForeignElementRunningState(viewModel);
                    break;
                case Model.Status.NoElementRunning:
                    currentState = new NoElementRunningState(viewModel);
                    break;
                default:
                    break;
            }

            return currentState;

        }
    }
}
