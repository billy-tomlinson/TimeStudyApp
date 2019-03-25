﻿using System;
using System.Linq;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public class UnratedOccassionalElementRunningState : BaseState
    {

        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public UnratedOccassionalElementRunningState(TimeStudyUnsequencedViewModel viewModel)
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
                    element.Name, element.IsForeignElement, RunningStatus.Running, element.Rated);

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
            var current = viewModel.Get_Running_Unrated_LapTime();
            if (current != null)
            {
                current.Rating = 0;
                current.Status = RunningStatus.Completed;

                Utilities.LastRatedLapTimeId = current.Id;

                viewModel.LapTimeRepo.SaveItem(current);
            }

            SetCurrentApplicationState();
        }

        public override void RatingSelectedEvent()
        {
            throw new NotImplementedException();
        }

        public override void ShowForeignElements()
        {
            throw new NotImplementedException();
        }

        public override void ShowNonForeignElements()
        {
            viewModel.LapTimerEvent();
            viewModel.IsCancelEnabled = false;
            viewModel.IsForeignEnabled = false;
            viewModel.RatingsVisible = false;
            viewModel.CurrentApplicationState.CurrentState = Model.Status.UnratedOccassionalElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);

            viewModel.CollectionOfElements = viewModel.Get_All_Enabled_Activities_WithChildren();
            viewModel.GroupElementsForActivitiesView();

            var currentSelected = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            viewModel.ProcessForeignElementWithRating(currentSelected.Rated, currentSelected.Name,
                currentSelected.IsForeignElement, 0);

            viewModel.ActivitiesVisible = true;
            viewModel.Opacity = 0.2;
        }

        private void SetCurrentApplicationState()
        {
            var currentSelected = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);

            if (currentSelected.IsForeignElement && !currentSelected.Rated)
                viewModel.CurrentApplicationState.CurrentState = Model.Status.UnratedOccassionalElementRunning;

            else if (currentSelected.IsForeignElement && currentSelected.Rated)
                viewModel.CurrentApplicationState.CurrentState = Model.Status.OccassionalElementRunning;

            else
                viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;

            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }
    }
}