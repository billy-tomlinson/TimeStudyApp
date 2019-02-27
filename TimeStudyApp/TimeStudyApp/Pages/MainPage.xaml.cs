﻿using TimeStudy.Services;
using TimeStudy.ViewModels;
using Xamarin.Forms;

namespace TimeStudy.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            ConstructorSetUp();
        }

        private void ConstructorSetUp()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "");
            BindingContext = new MainPageViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnAppearing()
        {
            AlarmNotificationService.CheckIfAlarmHasExpiredWhilstInBackgroundOrAlarmOff();

            if (Utilities.ActivityTableUpdated || Utilities.OperatorTableUpdated || Utilities.ObservationTableUpdated)
            {
                if(!Utilities.MainPageHasUpdatedActivityChanges 
                    || !Utilities.MainPageHasUpdatedOperatorChanges
                    || !Utilities.MainPageHasUpdatedObservationChanges)
                {
                    Utilities.MainPageHasUpdatedActivityChanges = true;
                    Utilities.MainPageHasUpdatedOperatorChanges = true;
                    Utilities.MainPageHasUpdatedObservationChanges = true;

                    Utilities.UpdateTableFlags();

                    var viewModel = new MainPageViewModel
                    {
                        RatingsVisible = false,
                        ActivitiesVisible = false,
                    };
                    BindingContext = viewModel;
                }
            }

            Utilities.ClearNavigation();
            base.OnAppearing();
        }
    }
}
