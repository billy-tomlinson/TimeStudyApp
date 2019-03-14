using TimeStudy.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public abstract class BaseState
    {
        public abstract void ShowForeignElements();
        public abstract void ShowNonForeignElements();
        public abstract void ElementSelectedEvent();
        public abstract void RatingSelectedEvent();
        public abstract void AddElementWithoutLapTimeToList();
    }
}
