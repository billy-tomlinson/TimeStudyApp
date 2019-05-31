using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Custom
{
    [DesignTimeVisible(true)]
    public class CustomListView : ListView
    {

        public static BindableProperty ItemClickedCommandProperty =
            BindableProperty.Create(nameof(ItemClickedCommand), typeof(ICommand), typeof(CustomListView), null);

        public CustomListView(ListViewCachingStrategy strategy) : base(strategy)
        {
            this.ItemTapped += OnItemTapped;
            this.ItemDisappearing += OnDisappearing;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                ItemClickedCommand?.Execute(e.Item);
                SelectedItem = null;
            }
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            CustomListView view = (CustomListView)sender;

            if (Utilities.LapButtonClicked == true)
            {
                var obs = (ObservableCollection<LapTime>)view.ItemsSource;
                var observations = obs.OrderByDescending(x => x.Id).FirstOrDefault();
                this.ScrollTo(observations, ScrollToPosition.End, true);

                Utilities.LapButtonClicked = false;
            }
        }

        public ICommand ItemClickedCommand
        {
            get
            {
                return (ICommand)this.GetValue(ItemClickedCommandProperty);
            }
            set
            {
                this.SetValue(ItemClickedCommandProperty, value);
            }
        }
    }
}
