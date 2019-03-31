using System;
using System.Linq;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public class OccassionalElementRunningState : BaseState
    {

        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public OccassionalElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {
            var element = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);

            var current = viewModel.Get_Running_LapTime();
            if (current == null)
            {

                current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                    element.Name, RunningStatus.Running,  element.Rated, viewModel.RealTimeTicks);

                Utilities.CurrentRunningElementId = viewModel.LapTimeRepo.SaveItem(current);

                viewModel.CurrentElementWithoutLapTimeName = current.Element;
                viewModel.CurrentSequence = null;
                viewModel.CurrentCycle = viewModel.CycleCount;

                viewModel.IsForeignEnabled = false;
            }

            viewModel.LapTimes = viewModel.Get_All_LapTimes_Not_Running();

        }

        public override void CloseActivitiesViewEvent()
        {
            throw new NotImplementedException();
        }

        public override void ElementSelectedEvent()
        {
            throw new NotImplementedException();
        }

        public override void RatingSelectedEvent()
        {
            throw new NotImplementedException();
        }

        public override void ShowForeignElements()
        {
            throw new NotImplementedException();
        }

        public override void ShowStandardElements()
        {
            viewModel.LapTimerEvent();
            viewModel.IsCancelEnabled = false;
            viewModel.IsForeignEnabled = false;
            viewModel.CurrentApplicationState.CurrentState = Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);

            viewModel.CollectionOfElements = viewModel.Get_All_Enabled_Activities_WithChildren();
            viewModel.GroupElementsForActivitiesView();
        }
    }
}
