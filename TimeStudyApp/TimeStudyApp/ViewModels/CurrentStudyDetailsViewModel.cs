﻿using System;
using TimeStudy.Model;
using TimeStudy.Services;

namespace TimeStudy.ViewModels
{
    public class CurrentStudyDetailsViewModel : BaseViewModel
    {
        public CurrentStudyDetailsViewModel()
        {
            IsPageVisible = IsStudyValid();

            SampleStudy = SampleRepo.GetItem(Utilities.StudyId);
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

        ActivitySampleStudy sampleStudy;
        public ActivitySampleStudy SampleStudy
        {
            get { return sampleStudy; }
            set
            {
                sampleStudy = value;
                OnPropertyChanged();
            }
        }
    }
}
