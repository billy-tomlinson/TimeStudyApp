using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Command CloseView { get; set; }
        public Command Override { get; set; }

        private readonly string conn;
        private readonly string alarmconn;

        public Operator Operator;

        public BaseViewModel(string conn = null, string alarmconn = null)
        {
            this.conn = conn;
            this.alarmconn = alarmconn;
            SubmitDetails = new Command(SubmitDetailsAndNavigate);
            CloseView = new Command(CloseValidationView);
            EnsureTableCreation();
            InvalidText = "Please create a new study or select an existing one.";
            IsPageVisible = (Utilities.StudyId > 0);
        }


        public Command SubmitDetails { get; set; }

        public IBaseRepository<Operator> OperatorRepo => new BaseRepository<Operator>(conn);

        public IBaseRepository<AlarmDetails> AlarmRepo => new BaseRepository<AlarmDetails>(alarmconn);

        public IBaseRepository<Observation> ObservationRepo => new BaseRepository<Observation>(conn);

        public IBaseRepository<Activity> ActivityRepo => new BaseRepository<Activity>(conn);

        public IBaseRepository<ActivityName> ActivityNameRepo => new BaseRepository<ActivityName>(conn);

        public IBaseRepository<MergedActivities> MergedActivityRepo => new BaseRepository<MergedActivities>(conn);

        public IBaseRepository<ActivitySampleStudy> SampleRepo => new BaseRepository<ActivitySampleStudy>(conn);

        public IBaseRepository<ObservationRoundStatus> ObservationRoundStatusRepo => new BaseRepository<ObservationRoundStatus>(conn);

        static ObservableCollection<Activity> activities;
        public ObservableCollection<Activity> Activities
        {
            get => activities;
            set
            {
                activities = value;
                OnPropertyChanged();
            }
        }

        static string currentTime;
        public string CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged();
            }
        }

        static string timeOfNextObservation;
        public string TimeOfNextObservation
        {
            get => timeOfNextObservation;
            set
            {
                timeOfNextObservation = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<MultipleActivities> _groupActivities;
        public ObservableCollection<MultipleActivities> GroupActivities
        {
            get => _groupActivities;
            set
            {
                _groupActivities = value;
                OnPropertyChanged();
            }
        }

        bool isInvalid = false;
        public bool IsInvalid
        {
            get { return isInvalid; }
            set
            {
                isInvalid = value;
                OnPropertyChanged();
            }
        }


        double opacity = 1;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                OnPropertyChanged();
            }
        }

        string invalidText;
        public string InvalidText
        {
            get { return invalidText; }
            set
            {
                invalidText = value;
                OnPropertyChanged();
            }
        }

        int studyNumber;
        public int StudyNumber
        {
            get { return Utilities.StudyId; }
            set
            {
                studyNumber = value;
                OnPropertyChanged();
            }
        }

        int closeColumnSpan = 2;
        public int CloseColumnSpan
        {
            get { return closeColumnSpan; }
            set
            {
                closeColumnSpan = value;
                OnPropertyChanged();
            }
        }

        int totalObservationsRequired;
        public int TotalObservationsRequired
        {
            get { return totalObservationsRequired; }
            set
            {
                totalObservationsRequired = value;
                OnPropertyChanged();
            }
        }


        int totalObservationsTaken;
        public int TotalObservationsTaken
        {
            get { return totalObservationsTaken; }
            set
            {
                totalObservationsTaken = value;
                OnPropertyChanged();
            }
        }

        string totalOperatorPercentage;
        public string TotalOperatorPercentage
        {
            get { return totalOperatorPercentage; }
            set
            {
                totalOperatorPercentage = value;
                OnPropertyChanged();
            }
        }

        bool isPageVisible = false;
        public bool IsPageVisible
        {
            get { return isPageVisible; }
            set
            {
                isPageVisible = value;
                IsPageUnavailableVisible = !value;
                OnPropertyChanged();
            }
        }

        bool isOverrideVisible = false;
        public bool IsOverrideVisible
        {
            get { return isOverrideVisible; }
            set
            {
                isOverrideVisible = value;
                IsPageUnavailableVisible = !value;
                OnPropertyChanged();
            }
        }


        bool showOkCancel = false;
        public bool ShowOkCancel
        {
            get { return showOkCancel; }
            set
            {
                showOkCancel = value;
                OnPropertyChanged();
            }
        }

        bool showClose = false;
        public bool ShowClose
        {
            get { return showClose; }
            set
            {
                showClose = value;
                OnPropertyChanged();
            }
        }


        int isItemEnabled;
        public int IsItemEnabled
        {
            get { return isItemEnabled; }
            set
            {
                isItemEnabled = value;
                OnPropertyChanged();
            }
        }

        bool isPageUnavailableVisible = false;
        public bool IsPageUnavailableVisible
        {
            get { return isPageUnavailableVisible; }
            set
            {
                isPageUnavailableVisible = value;
                OnPropertyChanged();
            }
        }

        private string validationText;
        public string ValidationText
        {
            get => validationText;
            set
            {
                validationText = value;
                OnPropertyChanged();
            }
        }

        static string alarmStatus;
        public string AlarmStatus
        {
            get => alarmStatus;
            set
            {
                alarmStatus = value;
                OnPropertyChanged();
            }
        }

        static string intervalMinutes;
        public string IntervalMinutes
        {
            get => intervalMinutes;
            set
            {
                intervalMinutes = value;
                OnPropertyChanged();
            }
        }

        private bool busy = false;
        public bool IsBusy
        {
            get { return busy; }
            set
            {
                if (busy == value)
                    return;

                busy = value;
                OnPropertyChanged();
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool percentagesVisible;
        public bool PercentagesVisible
        {
            get => percentagesVisible;
            set
            {
                percentagesVisible = value;
                OnPropertyChanged();
            }
        }

       
        public bool StudyInProcess
        {
            get => Get_Observations_By_StudyId().Count > 0;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void SubmitDetailsAndNavigate() { }

        public ObservableCollection<Activity> Get_Rated_Enabled_Activities()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<Activity> Get_UnRated_Enabled_Activities()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && !x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<Activity> Get_All_Enabled_Activities()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && x.StudyId == Utilities.StudyId));
        }


        public ObservableCollection<Activity> Get_Previous_Enabled_Activities()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && x.StudyId != Utilities.StudyId)
                                         .OrderBy(y => y.Id));
        }

        public List<Observation> Get_Observations_By_StudyId()
        {
            return ObservationRepo.GetItems()
                               .Where(x => x.StudyId == Utilities.StudyId).ToList();
        }

        public ObservableCollection<Activity> Get_Enabled_Activities()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                .Where(x => x.IsEnabled && x.StudyId == Utilities.StudyId)
                .OrderBy(x => x.Sequence));
        }

        public ObservableCollection<Activity> Get_Rated_Enabled_Activities_WithChildren()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                                        .Where(x => x.IsEnabled && x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<Activity> Get_All_Enabled_Activities_WithChildren()
        {
            return new ObservableCollection<Activity>(ActivityRepo.GetAllWithChildren()
                .Where(x => x.IsEnabled && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<ActivityName> Get_All_ActivityNames()
        {
            return new ObservableCollection<ActivityName>(ActivityNameRepo.GetItems());

        }

        public ObservableCollection<Activity> ConvertListToObservable(List<Activity> list1)
        {
            return new ObservableCollection<Activity>(list1.OrderBy(x => x.Id).Where(x => x.IsEnabled));
        }

        public int SaveActivityDetails(Activity activity)
        {
            ActivityNameRepo.SaveItem(activity.ActivityName);
            var returnId = ActivityRepo.SaveItem(activity);
            ActivityRepo.UpdateWithChildren(activity);
            return returnId;
        }

        private void EnsureTableCreation()
        {
            AlarmRepo.CreateTable();
            OperatorRepo.CreateTable();
            ObservationRepo.CreateTable();
            ActivityRepo.CreateTable();
            ActivityNameRepo.CreateTable();
            MergedActivityRepo.CreateTable();
            SampleRepo.CreateTable();
            ObservationRoundStatusRepo.CreateTable();
        }

        public void CloseValidationView()
        {
            Opacity = 1;
            IsInvalid = false;
        }

        public List<OperatorRunningTotal> GetRunningTotals(Operator op)
        {
            var totals = new List<OperatorRunningTotal>();

            var observations = op.Observations;
            var totalObs = observations.Count;

            var observationsTaken = ObservationRepo.GetItems().Where(x => x.OperatorId == op.Id).ToList();

            if (observationsTaken.Count < 10)
            {
                PercentagesVisible = false;
                return totals;
            }

            PercentagesVisible = true;

            TotalObservationsTaken = totalObs;

            var activtyIds = observationsTaken.Select(x => new { Id = x.AliasActivityId })
                                              .Distinct().ToList();

            var distinctActivities = new List<dynamic>();

            foreach (var item in activtyIds)
            {
                distinctActivities.Add(item);
            }

            var totalRequiredForOperator = 0;

            foreach (var item in distinctActivities)
            {
                var count = observations.Count(x => x.AliasActivityId == item.Id);
                double percentage = count > 0 ? (double)count / totalObs : 0;
                percentage = Math.Round(percentage * 100, 1);

                var totalRequired = Utilities.CalculateObservationsRequired(percentage);

                totalRequiredForOperator = totalRequiredForOperator + totalRequired;

                var runningTotal = new OperatorRunningTotal()
                {
                    ActivityId = item.Id,
                    OperatorId = op.Id,
                    ActivityName = ActivityRepo.GetWithChildren(item.Id).ActivityName.Name,
                    NumberOfObservations = count,
                    ObservationsRequired = totalRequired,
                    Percentage = percentage,
                    PercentageFormatted = $"{percentage.ToString(CultureInfo.InvariantCulture)}%"
                };

                totals.Add(runningTotal);
            }

            TotalOperatorPercentage = string.Empty;
            TotalObservationsRequired = totalRequiredForOperator;
            double totalPercentage = 0;

            if (TotalObservationsRequired > 0)
            {
                totalPercentage = Math.Ceiling((double)TotalObservationsTaken / TotalObservationsRequired * 100);
                var percentage = totalPercentage < 100 ? totalPercentage : 100;
                TotalOperatorPercentage = $"{percentage.ToString(CultureInfo.InvariantCulture)}%";
            }

            return totals;
        }
    }
}
