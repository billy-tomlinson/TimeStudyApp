﻿using System.Collections.Generic;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudy.ViewModels;

namespace TimeStudyApp.Services.StateMachine
{
    public class ElementRunningState : BaseState
    {
        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public ElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {
            var element = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            if (element.IsForeignElement)
            {
                viewModel.SelectedForeignElements.Add(element);
                viewModel.ForeignElementCount = viewModel.SelectedForeignElements.Count;

                var current = viewModel.Get_Running_LapTime();
                current.Status = Model.RunningStatus.Paused;
                current.TotalElapsedTimeDouble = viewModel.RealTimeTicks;
                current.TotalElapsedTime = viewModel.RealTimeTicks.ToString("0.000");

                viewModel.LapTimeRepo.SaveItem(current);

                viewModel.TimeWhenForiegnButtonClicked = viewModel.RealTimeTicks;

                var currentForeignLap = new LapTime
                {
                    Cycle = viewModel.CycleCount,
                    Element = element.Name,
                    Status = Model.RunningStatus.Running,
                    IsForeignElement = element.IsForeignElement,
                    StudyId = Utilities.StudyId
                };

                viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
            else 
            {
                var current = viewModel.Get_Running_LapTime();
                if(current == null)
                {
                    current = new LapTime
                    {
                        Cycle = viewModel.CycleCount,
                        Element = element.Name,
                        Status = Model.RunningStatus.Running,
                        IsForeignElement = element.IsForeignElement,
                        StudyId = Utilities.StudyId
                    };

                    viewModel.LapTimeRepo.SaveItem(current);

                    viewModel.CurrentElementWithoutLapTimeName = current.Element;
                    viewModel.CurrentSequence = null;
                    viewModel.CurrentCycle = viewModel.CycleCount;
                }
            }
        }

        public override void ElementSelectedEvent()
        {

            var current = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            if (current.IsForeignElement)
            {
                viewModel.IsForeignEnabled = false;
                viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
            else
            {
                viewModel.IsForeignEnabled = true;
                viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
        }

        public override void RatingSelectedEvent()
        {
            viewModel.AllForiegnLapTimes = new List<LapTime>();
            viewModel.TimeWhenForiegnButtonClicked = 0;

            viewModel.LapTimes = viewModel.Get_All_LapTimes_Not_Running();

            if (viewModel.ActivitiesCounter == viewModel.ActivitiesCount)
            {
                viewModel.CurrentElementName = viewModel.Activities.FirstOrDefault(x => x.Sequence == 1).Name;
            }
            else
                viewModel.CurrentElementName = viewModel.Activities.FirstOrDefault(x => x.Sequence == viewModel.ActivitiesCounter + 1).Name;
                
            viewModel.ForeignElementCount = 0;

            viewModel.Opacity = 1;
            viewModel.RatingsVisible = false;
            viewModel.ActivitiesVisible = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowForeignElements()
        {
            viewModel.Opacity = 0.2;
            viewModel.RatingsVisible = false;
            viewModel.ActivitiesVisible = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowNonForeignElements()
        {
            viewModel.LapTimerEvent();

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }
    }
}
