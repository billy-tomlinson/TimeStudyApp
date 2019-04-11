using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using TimeStudy.Model;
using TimeStudy.Pages;
using TimeStudy.Services;
using TimeStudyApp.Model;
using TimeStudyApp.Services;
using TimeStudyApp.Services.StateMachine;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyUnsequencedViewModel : BaseViewModel
    {
        public Command StartTimer { get; set; }
        public Command LapTimer { get; set; }
        public Command StopTimer { get; set; }
        public Command ClearLaps { get; set; }
        public Command RatingSelected { get; set; }
        public Command ItemClickedCommand { get; set; }
        public Command ShowForeignElements { get; set; }
        public Command ShowForeignElementsTopButton { get; set; }
        public Command ShowStandardElements { get; set; }
        public Command CloseForeignElements { get; set; }
        public Command ResumePased { get; set; }
        public Command ElementSelected { get; set; }
        public Command CloseActivitiesView { get; set; }
        public Command CloseRatingsView { get; set; }
        public Command ShowRatingView { get; set; }
        public Command FinishStudyFromActivities { get; set; }
        public new Command CloseView { get; set; } 

        private bool SaveButtonClicked;
        private bool IsRunning;
        private bool cancelActivitiesView;
        private bool HasBeenStopped;

        public double TimeWhenStopButtonClicked { get; set; }
        public double LapTime { get; set; }
        public double CurrentTicks { get; set; }
        public double LastSuccesstulLapTime { get; set; }
        public TimeSpan StartTime { get; set; }


        public BaseState ApplicationState { get; set; }
        public StateFactory ApplicationStateFactory { get; set; }

        public State CurrentApplicationState { get; set; }
        public ApplicationState StateService { get; set; }

        public List<Activity> SelectedForeignElements;

        public List<LapTime> AllForiegnLapTimes = new List<LapTime>();

        public int CycleCount;

        public TimeStudyUnsequencedViewModel()
        {
            ConstructorSetUp();
        }

        private void ConstructorSetUp(bool generateNewVersionRecord = true)
        {

            StartTimer = new Command(StartTimerEvent);
            StopTimer = new Command(StopTimerEvent);
            LapTimer = new Command(LapTimerEvent);
            ClearLaps = new Command(ClearLapsEvent);
            Override = new Command(OverrideEvent);
            RatingSelected = new Command(RatingSelectedEvent);
            ShowForeignElements = new Command(ShowForeignElementsEvent);
            ShowForeignElementsTopButton = new Command(ShowForeignElementsTopButtonEvent);
            ShowStandardElements = new Command(ShowStandardElementsEvent);
            CloseForeignElements = new Command(CloseForeignElementsEvent);
            ResumePased = new Command(ResumePausedEvent);
            ElementSelected = new Command(ElementsSelectedEvent);
            ItemClickedCommand = new Command(ShowForeignElementsEvent);
            CloseActivitiesView = new Command(CloseActivitiesViewEvent);
            CloseRatingsView = new Command(CloseRatingsViewEvent);
            ShowRatingView = new Command(ShowRatingViewEvent);
            FinishStudyFromActivities = new Command(FinishStudyFromActivitiesEvent);
            CloseView = new Command(CloseValidationView);

            ApplicationStateFactory = new StateFactory(this);

            StateService = new ApplicationState();
            var state = StateService.GetApplicationState();

            if (state != null)
                CurrentApplicationState = new State()
                {
                    Id = state.Id,
                    CurrentState = Status.NoElementRunning
                };
            else
            {
                CurrentApplicationState = new State()
                {
                    CurrentState = Status.NoElementRunning
                };
            }

            StateService.SaveApplicationState(CurrentApplicationState);

            LapTimes = new ObservableCollection<LapTime>();

            SelectedForeignElements = new List<Activity>();

            Activities = Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren();

            CollectionOfElements = Get_All_Enabled_Activities();

            GroupElementsForActivitiesView();

            IsPageVisible = IsStudyValid();

            LapTime = 0;
            CurrentTicks = 0;
            StopWatchTime = "0.000";
            IsRunning = false;
            IsCancelEnabled = !IsRunning;
            IsStartEnabled = true;
            IsLapEnabled = true;
            IsStopEnabled = false;
            IsClearEnabled = false;
            IsForeignEnabled = false;
            IsPageEnabled = true;
            CycleCount = 1;
            Utilities.TimeWhenLapOrForiegnButtonClicked = 0;
            LapButtonText = "   Start   ";

            if (generateNewVersionRecord)
            {
                var studyHistoryVersion = new StudyHistoryVersion()
                {
                    StudyId = Utilities.StudyId,
                    Date = DateTime.Now,
                    Time = DateTime.Now.TimeOfDay
                };
                var version = StudyHistoryVersionRepo.SaveItem(studyHistoryVersion);
                var currentStudy = SampleRepo.GetWithChildren(Utilities.StudyId);
                Utilities.StudyVersion = version;
            }

            ResetAllGlobalVariables();

            TimerService.Stop();

            Utilities.MoveLapsToHistoryTable();
        }

        public void RunTimer()
        {
            TimerService.StartTimer(new TimeSpan(0, 0, 0, 0, 100), CallBackForTimer());
        }

        private Func<bool> CallBackForTimer()
        {
            TimeSpan TotalTime;
            TimeSpan TimeElement = new TimeSpan();

            return () =>
            {
                if (!IsRunning) return false;

                TotalTime = TotalTime + TimeElement.Add(new TimeSpan(0, 0, 0, 1));

                var timeElaspedSinceStart = DateTime.Now.TimeOfDay - StartTime;

                var realTicks = timeElaspedSinceStart.Ticks / 1000000;

                RealTimeTicks = TimeWhenStopButtonClicked + (double)realTicks / 600;

                StopWatchTime = RealTimeTicks.ToString("0.000");

                return IsRunning;
            };
        }

        public void GroupElementsForActivitiesView()
        {
            IEnumerable<Activity> obsCollection = CollectionOfElements;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            };

            CollectionOfElements = ConvertListToObservable(list1);

            GroupActivities = Utilities.BuildGroupOfActivities(CollectionOfElements);
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

        public void CloseActivitiesViewEvent()
        {
            cancelActivitiesView = true;
            Opacity = 1;
            ActivitiesVisible = false;
            IsPageEnabled = true;

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.CloseActivitiesViewEvent();

        }

        public void FinishStudyFromActivitiesEvent()
        {
            FinishStudyFromActivitiesClicked = true;
            CloseActivitiesViewEvent();
            StopTimerEvent();
        }

        public void CloseRatingsViewEvent()
        {
            var currentLapTime = Get_Running_LapTime_By_Id();
            currentLapTime.Status = RunningStatus.Running;
            Utilities.CurrentRunningElementId = LapTimeRepo.SaveItem(currentLapTime);

            Opacity = 1;
            RatingsVisible = false;
            IsPageEnabled = true;
        }

        public void ResumePausedEvent()
        {
            cancelActivitiesView = true;
            Opacity = 1;
            LapTimerEvent();
            ActivitiesVisible = false;
        }

        public void ShowRatingViewEvent()
        {
            RatingsVisible = true;
            ActivitiesVisible = false;
            Opacity = 0.2;
        }

        public void StartTimerEvent()
        {
            IsRunning = true;
            IsCancelEnabled = !IsRunning;
            IsStartEnabled = false;
            IsLapEnabled = true;
            IsStopEnabled = true;
            IsClearEnabled = false;
            StartTime = DateTime.Now.TimeOfDay;
            StartTimeFormatted = StartTime.ToString(@"c");

            RunTimer();
            if (!HasBeenStopped)
                AddCurrentWithoutLapTimeToList();

            HasBeenStopped = false;
        }

        public void StopTimerEvent()
        {
            ValidationText = "Are you sure you want to stop and save the study?";
            ShowOkCancel = true;
            IsOverrideVisible = false;
            ShowClose = false;
            Opacity = 0.2;
            CloseColumnSpan = 1;
            IsInvalid = true;
            IsPageEnabled = false;
            SaveButtonClicked = true;
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
            IsPageEnabled = false;
            SaveButtonClicked = false;
        }

        void OverrideEvent(object sender)
        {
            ConstructorSetUp(false);

            CurrentApplicationState.CurrentState = Status.NoElementRunning;
            StateService.SaveApplicationState(CurrentApplicationState);

            if (SaveButtonClicked)
            {
                var studyVersion = StudyHistoryVersionRepo.GetItems()
                            .FirstOrDefault(x => x.StudyId == Utilities.StudyId && x.Id == Utilities.StudyVersion);
                studyVersion.TimeStudyStarted = Utilities.TimeStudyStarted;
                studyVersion.TimeStudyFinished = DateTime.Now;

                StudyHistoryVersionRepo.SaveItem(studyVersion);
                Utilities.Navigate(new ReportsPage());
            }
                
        }

        private void ResetAllGlobalVariables()
        {
            IsPageEnabled = true;
            ShowOkCancel = false;
            IsInvalid = false;
            IsOverrideVisible = false;
            Opacity = 1.0;
            IsRunning = false;
            IsCancelEnabled = !IsRunning;
            cancelActivitiesView = false;
            HasBeenStopped = true;
            TimeWhenStopButtonClicked = 0;
            LapTime = 0;
            CurrentTicks = 0;
            LastSuccesstulLapTime = 0;
            CurrentCycle = 0;
            CurrentSequence = null;
            CurrentElementWithoutLapTimeName = null;
            IsPageEnabled = true;
            RealTimeTicks = 0;
            StartTime = new TimeSpan();
            Utilities.CurrentSelectedElementId = 0;
            Utilities.CurrentRunningElementId = 0;
            Utilities.LastRatedLapTimeId = 0;
            Utilities.TimeWhenLapOrForiegnButtonClicked = 0;

        }

        public Custom.CustomButton RatingButton;

        void RatingSelectedEvent(object sender)
        {
            RatingButton = sender as Custom.CustomButton;
            var current = Get_Running_Unrated_LapTime();
            if (current != null)
            {
                current.Rating = RatingButton.Rating;
                current.Status = RunningStatus.Completed;

                Utilities.LastRatedLapTimeId = current.Id;

                LapTimeRepo.SaveItem(current);
            }

            IsCancelEnabled = false;
            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.RatingSelectedEvent();
        }

        void ElementsSelectedEvent(object sender)
        {

            var value = (int)sender;

            Utilities.CurrentSelectedElementId = value;

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ElementSelectedEvent();

            Opacity = 1;

            var current = CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.AddElementWithoutLapTimeToList();

            IsPageEnabled = true;
            ActivitiesVisible = false;
            RatingsVisible = false;
            Opacity = 1.0;

            IsRunning = true;
            IsCancelEnabled = !IsRunning;

            LapButtonText = "   Lap   ";
        }

        void ShowForeignElementsEvent()
        {
            Utilities.IsForeignElement = true;
            IsPageEnabled = false;
            IsForeignEnabled = false;
            IsNonForeignEnabled = true;
            CollectionOfElements = Get_All_Enabled_Activities_WithChildren();
            GroupElementsForActivitiesView();

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ShowForeignElements();
        }

        void ShowForeignElementsTopButtonEvent()
        {
            Utilities.TimeWhenLapOrForiegnButtonClicked = RealTimeTicks;
            Utilities.IsForeignElement = true;
            IsCancelEnabled = true;
            ShowForeignElementsEvent();
            IsNonForeignEnabled = true;
        }

        void ShowStandardElementsEvent()
        {
            Utilities.TimeWhenLapOrForiegnButtonClicked = RealTimeTicks;
            Utilities.IsForeignElement = false;
            IsPageEnabled = false;
            IsForeignEnabled = true;
            IsNonForeignEnabled = false;
            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ShowStandardElements(); ;
        }

        void CloseForeignElementsEvent(object sender)
        {
            IsPageEnabled = true;
            ForeignElementsVisible = false;
            Opacity = 1.0;
        }

        public void LapTimerEvent()
        {
            LapButtonText = "   Lap   ";

            var pausedLap = Get_Paused_LapTime();

            if (pausedLap == null)
            {
                if (ActivitiesVisible) return;

                SetUpButtonsAndTimeVariables();

                ForceRoundingToLapTime(true);

                SetUpCurrentLapTime();

                Opacity = 0.2;
                RatingsVisible = true;

            }
            else
            {
                var current = Get_Running_LapTime();
                if (current.IsForeignElement && !cancelActivitiesView)
                {
                    SetUpCurrentLapTime();
                    ShowForeignElementsEvent();
                    return;
                }

                ReInstatePausedLapTimeToCurrentRunning();
            }

            cancelActivitiesView = false;

        }

        public void ProcessForeignElementWithRating(bool rated, string name, int? rating = null)
        {
            AddForeignLapTimetoListAsCompleted(rating);

            CurrentElementWithoutLapTimeName = name;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
        }

        public void ReInstatePausedLapTimeToCurrentRunning()
        {
            LapTime current;

            current = Get_Paused_LapTime();

            current.Status = RunningStatus.Running;

            Utilities.CurrentRunningElementId = LapTimeRepo.SaveItem(current);

            CurrentCycle = CycleCount;

            CurrentElementWithoutLapTimeName = current.Element;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
        }

        private void AddForeignLapTimetoListAsCompleted(int? rating = null)
        {

            ForceRoundingToLapTime();

            SetUpCurrentLapTime();

            var current = Get_Current_Foreign_LapTime();

            current.Rating = rating;
            current.Status = RunningStatus.Completed;
            LapTimeRepo.SaveItem(current);

            AllForiegnLapTimes.Add(current);
        }

        public void SetUpCurrentLapTime()
        {
            var currentLapTime = Get_Running_LapTime();

            if (currentLapTime != null)
            {
                SetUpCurrentLapProperties(currentLapTime);
            }
        }

        public void SetUpCurrentLapProperties(LapTime currentLapTime)
        {
            string lapTimeTimeFormatted = LapTime.ToString("0.000");

            currentLapTime.IndividualLapTime = LapTime;
            currentLapTime.IndividualLapTimeFormatted = lapTimeTimeFormatted;
            currentLapTime.TotalElapsedTimeDouble = Utilities.TimeWhenLapOrForiegnButtonClicked;
            currentLapTime.TotalElapsedTime = Utilities.TimeWhenLapOrForiegnButtonClicked.ToString("0.000");
            currentLapTime.ElementColour = Color.Gray;
            currentLapTime.ForeignElements = SelectedForeignElements;
            currentLapTime.Status = RunningStatus.Completed;
            LapTimeRepo.SaveItem(currentLapTime);
        }

        public void AddCurrentWithoutLapTimeToList()
        {
            double totalElapsedTime;
            var element = CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);

            var currentLapTime = Get_Running_LapTime();
            if (currentLapTime != null)
                totalElapsedTime = currentLapTime.TotalElapsedTimeDouble;
            else
                totalElapsedTime = Utilities.TimeWhenLapOrForiegnButtonClicked;

            var currentWithoutLapTime = Utilities.SetUpCurrentLapTime(CycleCount, element.Name,
                RunningStatus.Running, element.Rated, Color.Silver);

            Utilities.CurrentRunningElementId = LapTimeRepo.SaveItem(currentWithoutLapTime);

            CurrentElementWithoutLapTimeName = element.Name;
            CurrentSequence = element.Sequence;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
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

        private void ForceRoundingToLapTime(bool isLapTime = false)
        {
            var lastRecordedLapTime = Get_Running_LapTime();
            if (lastRecordedLapTime != null)
                if (!lastRecordedLapTime.HasBeenPaused)
                    LapTime = Utilities.TimeWhenLapOrForiegnButtonClicked - lastRecordedLapTime.TimeWhenLapStarted;
                else
                {

                    double combinedForeignTimes = 0;
                    var foriegnLaps = LapTimeRepo.GetItems().Where(x => x.Id > lastRecordedLapTime.Id);
                    foreach (var item in foriegnLaps)
                    {
                        combinedForeignTimes = combinedForeignTimes + item.IndividualLapTime;
                    }

                    LapTime = Utilities.TimeWhenLapOrForiegnButtonClicked - lastRecordedLapTime.TimeWhenLapStarted - combinedForeignTimes;
                }
            else
                LapTime = Utilities.TimeWhenLapOrForiegnButtonClicked;

            double randomToForceRounding;

            Random r = new Random();
            int rInt = r.Next(0, 9);
            if (rInt > 0)
            {
                randomToForceRounding = (double)rInt / 10000;
                LapTime = LapTime + randomToForceRounding;
            }
        }

        public new void CloseValidationView()
        {
            if (!FinishStudyFromActivitiesClicked)
            {
                Opacity = 1;
                IsInvalid = false;
                IsPageEnabled = true;
            }
            else
            {
                FinishStudyFromActivitiesClicked = false;
                IsForeignEnabled = false;
                IsInvalid = false;
                Opacity = 0.2;
                ActivitiesVisible = true;
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

        static bool isForeignEnabled;
        public bool IsForeignEnabled
        {
            get => isForeignEnabled;
            set
            {
                isForeignEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isNonForeignEnabled;
        public bool IsNonForeignEnabled
        {
            get => isNonForeignEnabled;
            set
            {
                isNonForeignEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isCancelEnabled;
        public bool IsCancelEnabled
        {
            get => isCancelEnabled;
            set
            {
                isCancelEnabled = value;
                IsFinishStudyEnabled = !isCancelEnabled;
                OnPropertyChanged();
            }
        }


        static bool isFinishStudyEnabled;
        public bool IsFinishStudyEnabled
        {
            get => isFinishStudyEnabled;
            set
            {
                isFinishStudyEnabled = value;
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

        static string lapButtonText;
        public string LapButtonText
        {
            get => lapButtonText;
            set
            {
                lapButtonText = value;
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

        static ObservableCollection<Activity> collectionOfElements;
        public ObservableCollection<Activity> CollectionOfElements
        {
            get => collectionOfElements;
            set
            {
                collectionOfElements = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<LapTime> lapTimes;
        public ObservableCollection<LapTime> LapTimes
        {
            get
            {
                return lapTimes;
            }
            set
            {
                lapTimes = value;
                OnPropertyChanged();
            }
        }
    }
}
