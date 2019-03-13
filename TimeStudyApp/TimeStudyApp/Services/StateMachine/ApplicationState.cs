using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public class ApplicationState : BaseViewModel
    {
        public int StoreState(State state)
        {
            return SaveApplicationState(state);
        }

        public State GetState()
        {
            return GetApplicationState();
        }
    }
}
