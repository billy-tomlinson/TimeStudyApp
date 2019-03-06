﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyViewModel : BaseViewModel
    {

        public Command StartTimer { get; set; }
        public Command LapTimer { get; set; }
        public Command StopTimer { get; set; }
        public Command ClearLaps { get; set; }
        public Command RatingSelected { get; set; }
        public Command ItemClickedCommand { get; set; }
        public Command RunningItemClickedCommand { get; set; }
        public Command ShowForeignElements { get; set; }
        public Command CloseForeignElements { get; set; }
        public Command ForeignElementSelected { get; set; }

        private bool IsRunning;
        private bool HasBeenStopped;
        public double TimeWhenLapButtonClicked { get; set; }
        public double TimeWhenStopButtonClicked { get; set; }
        public double LapTime { get; set; }
        public double CurrentTicks { get; set; }
        public TimeSpan StartTime { get; set; }

        ObservableCollection<Activity> AllForeignElements;

        List<Activity> SelectedForeignElements;

        static int ActivitiesCount;
        static int ActivitiesCounter;
        static int CycleCount;

        LapTime CurrentLapTime;
        LapTime CurrentWithoutLapTime;

        public TimeStudyViewModel()
        {

            StartTimer = new Command(StartTimerEvent);
            StopTimer = new Command(StopTimerEvent);
            LapTimer = new Command(LapTimerEvent);
            ClearLaps = new Command(ClearLapsEvent);
            Override = new Command(OverrideEvent);
            RatingSelected = new Command(RatingSelectedEvent);
            ShowForeignElements = new Command(ShowForeignElementsEvent);
            CloseForeignElements = new Command(CloseForeignElementsEvent);
            ForeignElementSelected = new Command(ForeignElementSelectedEvent);
            ItemClickedCommand = new Command(ShowForeignElementsEvent);
            RunningItemClickedCommand = new Command(RunningItemClickedCommandEvent);

            LapTimes = new ObservableCollection<LapTime>();
            LapTimesList = new List<LapTime>();
            SelectedForeignElements = new List<Activity>();

            Activities = Get_Rated_Enabled_Activities();

            AllForeignElements = Get_UnRated_Enabled_Activities();

            IEnumerable<Activity> obsCollection = AllForeignElements;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            };

            AllForeignElements = ConvertListToObservable(list1);

            GroupActivities = Utilities.BuildGroupOfActivities(AllForeignElements);

            IsPageVisible = IsStudyValid();

            LapTime = 0;
            CurrentTicks = 0;
            StopWatchTime = "0.000";
            IsRunning = false;
            IsStartEnabled = true;
            IsLapEnabled = false;
            IsStopEnabled = false;
            IsClearEnabled = false;
            ActivitiesCount = Activities.Count;
            CycleCount = 1;
        }

        private bool IsStudyValid()
        {

            if (Utilities.StudyId == 0 || Utilities.IsCompleted)
                return false;

            if (Activities.Count == 0)
            {
                InvalidText = $"Please add at least one element to study {Utilities.StudyId.ToString()}";
                return false;
            }

            return true;
        }

        public void StartTimerEvent()
        {
            IsRunning = true;
            IsStartEnabled = false;
            IsLapEnabled = true;
            IsStopEnabled = true;
            IsClearEnabled = false;
            StartTime = DateTime.Now.TimeOfDay;
            StartTimeFormatted = StartTime.ToString(@"c");
            CurrentElement = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter + 1);
            CurrentElementName = CurrentElement?.Name;

            RunTimer();
            if(!HasBeenStopped)
                AddCurrentWithoutLapTimeToList();

            HasBeenStopped = false;
        }

        public void StopTimerEvent()
        {
            IsRunning = false;
            IsStartEnabled = true;
            IsLapEnabled = false;
            IsStopEnabled = false;
            IsClearEnabled = true;
            IsStartEnabled = true;
            HasBeenStopped = true;

            TimeWhenStopButtonClicked = RealTimeTicks;
        }

        public void ClearLapsEvent()
        {
            ValidationText = "Are you sure you want to clear and reset the stop watch?";
            ShowOkCancel = true;
            IsOverrideVisible = false;
            ShowClose = false;
            Opacity = 0.2;
            CloseColumnSpan = 1;
            IsInvalid = true;
        }

        void OverrideEvent(object sender)
        {
            LapTimes = new ObservableCollection<LapTime>();
            OnPropertyChanged("LapTimes");

            LapTimesList = new List<LapTime>();
            StopWatchTime = "0.000";
            IsLapEnabled = false;
            IsStopEnabled = false;
            IsClearEnabled = false;
            IsStartEnabled = true;

            TimeWhenStopButtonClicked = 0;
            TimeWhenLapButtonClicked = 0;
            LapTime = 0;

            IsInvalid = false;
            Opacity = 1;
        }

        void RunningItemClickedCommandEvent(object sender)
        {
            Opacity = 0.2;
            ActivitiesVisible = true;
        }

        void RatingSelectedEvent(object sender)
        {
            var button = sender as Custom.CustomButton;
            CurrentLapTime.Rating = button.Rating;

            LapTimesList.Add(CurrentLapTime);

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .OrderByDescending(x => x.Cycle)
                .ThenByDescending(x => x.Sequence));

            Opacity = 1;
            RatingsVisible = false;

            if (ActivitiesCounter == ActivitiesCount)
            {
                CurrentElementName = Activities.FirstOrDefault(x => x.Sequence == 1).Name;
            }
            else
                CurrentElementName = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter + 1).Name;

            AddCurrentWithoutLapTimeToList();

            ForeignElementCount = 0;
        }

        void ForeignElementSelectedEvent(object sender)
        {
            Opacity = 1;

            var value = (int)sender;

            var foreign = AllForeignElements.Where(x => x.Id == value).FirstOrDefault();
            SelectedForeignElements.Add(foreign);
            ForeignElementCount = SelectedForeignElements.Count;
            AddForeignElementWithoutLapTimeToList(foreign);

            //CurrentElementWithoutLapTimeName = foreign.Name;
            //CurrentSequence = foreign.Sequence;
            //CurrentCycle = CycleCount;

            ActivitiesVisible = false;
        }

        void ShowForeignElementsEvent(object sender)
        {
            Opacity = 0.2;
            ActivitiesVisible = true;

            //var value = (LapTime)sender;

            //if (value.IndividualLapTime == null)
            //{
            //    Opacity = 0.2;
            //    ActivitiesVisible = true;
            //}
            //else
            //{
            //    if(value.ForeignElements.Any())
            //    {
            //        Opacity = 0.2;
            //        LapTimeSelected = value;
            //        ForeignElements = new ObservableCollection<Activity>(value.ForeignElements);
            //        ForeignElementsVisible = true;
            //    }
            //}
        }

        void CloseForeignElementsEvent(object sender)
        {
            ForeignElementsVisible = false;
            Opacity = 1.0;
        }

        public void LapTimerEvent()
        {

            SetUpButtonsAndTimeVariables();

            ForceRoundingToLapTime();

            string lapTimeTimeFormatted = LapTime.ToString("0.000");

            Activity element;

            if (ActivitiesCounter == 0) 
                ActivitiesCounter = 1;
            else
                ActivitiesCounter = ActivitiesCounter + 1;

            if (ActivitiesCounter > ActivitiesCount)
            {
                ActivitiesCounter = 1;
                CycleCount = CycleCount + 1;
            }

            element = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter);

            LapTimesList.Remove(CurrentWithoutLapTime);

            CurrentLapTime = CurrentWithoutLapTime;
            CurrentLapTime.IndividualLapTime = lapTimeTimeFormatted;
            CurrentLapTime.TotalElapsedTime = CurrentTimeFormattedDecimal;
            CurrentLapTime.ElementColour = Color.Gray;
            CurrentLapTime.ForeignElements = SelectedForeignElements;

            if (SelectedForeignElements.Count > 0)
                CurrentLapTime.ElementColour = Color.Orange;

            SelectedForeignElements = new List<Activity>();

            TimeWhenLapButtonClicked = RealTimeTicks;

            CheckIfRatedStudy();
        }

        public void RunTimer()
        {

            TimeSpan TotalTime;
            TimeSpan TimeElement = new TimeSpan();
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () =>
            {
                if (!IsRunning) return false;

                TotalTime = TotalTime + TimeElement.Add(new TimeSpan(0, 0, 0, 1));

                var timeElaspedSinceStart = DateTime.Now.TimeOfDay - StartTime;

                var realTicks = timeElaspedSinceStart.Ticks / 1000000;

                RealTimeTicks = TimeWhenStopButtonClicked + (double)realTicks / 600;

                StopWatchTime = RealTimeTicks.ToString("0.000");

                return IsRunning;
            });
        }


        private void AddForeignElementWithoutLapTimeToList(Activity foreign)
        {

            var currentForeign = new LapTime
            {
                Cycle = CycleCount,
                Element = foreign.Name,
                TotalElapsedTime = "Running",
                //Sequence = CurrentSequence + 0.01,
                IsForeignElement = true,
                ParentIdentity = CurrentWithoutLapTime.LapIdentity
            };

            LapTimesList.Remove(CurrentWithoutLapTime);

            LapTimesList.Add(currentForeign);

            var currentPausedLap = CurrentWithoutLapTime;

            currentPausedLap.TotalElapsedTime = RealTimeTicks.ToString("0.000");
            currentPausedLap.IndividualLapTime = "Paused";

            LapTimesList.Add(currentPausedLap);

            PausedLapTime = CurrentWithoutLapTime;

            CurrentElementWithoutLapTimeName = currentForeign.Element;
            CurrentSequence = 0;
            CurrentCycle = CycleCount;

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
                //.ThenByDescending(x => x.Sequence));
        }

        private void AddCurrentWithoutLapTimeToList()
        {
            if (ActivitiesCounter == 0) ActivitiesCounter = 1;

            var element = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter);

            if (ActivitiesCounter <= ActivitiesCount)
            {
                CurrentWithoutLapTime = new LapTime
                {
                    Cycle = CycleCount,
                    Element = element.Name,
                    TotalElapsedTime = "Running",
                    Sequence = element.Sequence,
                    ElementColour = Color.Silver
                };
            }

            CurrentElementWithoutLapTimeName = element.Name;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimesList.Add(CurrentWithoutLapTime);

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.Cycle)
                .ThenByDescending(x => x.Sequence));
        }


        private void AddPausedWithoutLapTimeToList()
        {

            CurrentWithoutLapTime = PausedLapTime;

            CurrentElementWithoutLapTimeName = PausedLapTime.Element;
            CurrentSequence = (int)PausedLapTime.Sequence;
            CurrentCycle = CycleCount;

            LapTimesList.Add(CurrentWithoutLapTime);

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
                //.OrderByDescending(x => x.Cycle)
                //.ThenByDescending(x => x.Sequence));
        }

        private void SetUpButtonsAndTimeVariables()
        {
            IsStartEnabled = false;
            IsLapEnabled = true;
            IsStopEnabled = true;
            IsClearEnabled = false;

            var timeElaspedSinceStart = DateTime.Now.TimeOfDay - StartTime;

            var timeElaspedSinceStartDecimal = timeElaspedSinceStart.Ticks / 1000000;
            RealTimeTicks = TimeWhenStopButtonClicked + (double)timeElaspedSinceStartDecimal / 600;
            CurrentTimeFormatted = timeElaspedSinceStart.ToString();
            CurrentTimeFormattedDecimal = RealTimeTicks.ToString("0.000");
        }

        private void CheckIfRatedStudy()
        {
            if (!Utilities.RatedStudy)
            {
                LapTimesList.Add(CurrentLapTime);

                AddCurrentWithoutLapTimeToList();

                ForeignElementCount = 0;
            }
            else
            {
                Opacity = 0.2;
                RatingsVisible = true;
            }
        }

        private void ForceRoundingToLapTime()
        {
            LapTime = RealTimeTicks - TimeWhenLapButtonClicked;

            double randomToForceRounding;

            Random r = new Random();
            int rInt = r.Next(0, 9);
            if (rInt > 0)
            {
                randomToForceRounding = (double)rInt / 10000;
                LapTime = LapTime + randomToForceRounding;
            }
        }

        static LapTime lapTimeSelected;
        public LapTime LapTimeSelected
        {
            get => lapTimeSelected;
            set
            {
                lapTimeSelected = value;
                OnPropertyChanged();
            }
        }

        static LapTime pausedLapTime;
        public LapTime PausedLapTime
        {
            get => pausedLapTime;
            set
            {
                pausedLapTime = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<Activity> foreignElements;
        public ObservableCollection<Activity> ForeignElements
        {
            get => foreignElements;
            set
            {
                foreignElements = value;
                OnPropertyChanged();
            }
        }

        static string stopWatchTime = "0.000";
        public string StopWatchTime
        {
            get => stopWatchTime;
            set
            {
                stopWatchTime = value;
                OnPropertyChanged();
            }
        }

        static string startTimeFormatted;
        public string StartTimeFormatted
        {
            get => startTimeFormatted;
            set
            {
                startTimeFormatted = value;
                OnPropertyChanged();
            }
        }
        static string currentTimeFormatted;
        public string CurrentTimeFormatted
        {
            get => currentTimeFormatted;
            set
            {
                currentTimeFormatted = value;
                OnPropertyChanged();
            }
        }

        static string currentTimeFormattedDecimal;
        public string CurrentTimeFormattedDecimal
        {
            get => currentTimeFormattedDecimal;
            set
            {
                currentTimeFormattedDecimal = value;
                OnPropertyChanged();
            }
        }

        static double realTimeTicks;
        public double RealTimeTicks
        {
            get => realTimeTicks;
            set
            {
                realTimeTicks = value;
                OnPropertyChanged();
            }
        }

        static bool isStartEnabled;
        public bool IsStartEnabled
        {
            get => isStartEnabled;
            set
            {
                isStartEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isStopEnabled;
        public bool IsStopEnabled
        {
            get => isStopEnabled;
            set
            {
                isStopEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isLapEnabled;
        public bool IsLapEnabled
        {
            get => isLapEnabled;
            set
            {
                isLapEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isClearEnabled;
        public bool IsClearEnabled
        {
            get => isClearEnabled;
            set
            {
                isClearEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool ratingsVisible;
        public bool RatingsVisible
        {
            get => ratingsVisible;
            set
            {
                ratingsVisible = value;
                OnPropertyChanged();
            }
        }

        static bool activitiesVisible;
        public bool ActivitiesVisible
        {
            get => activitiesVisible;
            set
            {
                activitiesVisible = value;
                OnPropertyChanged();
            }
        }

        static bool foreignElementsVisible;
        public bool ForeignElementsVisible
        {
            get => foreignElementsVisible;
            set
            {
                foreignElementsVisible = value;
                OnPropertyChanged();
            }
        }

        static Activity currentElement;
        public Activity CurrentElement
        {
            get => currentElement;
            set
            {
                currentElement = value;
                OnPropertyChanged();
            }
        }

        static string currentElementName;
        public string CurrentElementName
        {
            get => currentElementName;
            set
            {
                currentElementName = value;
                OnPropertyChanged();
            }
        }


        static string currentElementWithoutLapTimeName;
        public string CurrentElementWithoutLapTimeName
        {
            get => currentElementWithoutLapTimeName;
            set
            {
                currentElementWithoutLapTimeName = value;
                OnPropertyChanged();
            }
        }

        static int foreignElementCount;
        public int ForeignElementCount
        {
            get => foreignElementCount;
            set
            {
                foreignElementCount = value;
                OnPropertyChanged();
            }
        }

        static int? currentSequence;
        public int? CurrentSequence
        {
            get => currentSequence;
            set
            {
                currentSequence = value;
                OnPropertyChanged();
            }
        }

        static int currentCycle;
        public int CurrentCycle
        {
            get => currentCycle;
            set
            {
                currentCycle = value;
                OnPropertyChanged();
            }
        }

        static List<LapTime> LapTimesList = new List<LapTime>();

        static ObservableCollection<LapTime> lapTimes;
        public ObservableCollection<LapTime> LapTimes
        {
            get
            {
                return new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
                //.OrderByDescending(x => x.Cycle)
                //.ThenByDescending(x => x.Sequence));
            }
            set
            {
                lapTimes = value;
                OnPropertyChanged();
            }
        }
    }
}
