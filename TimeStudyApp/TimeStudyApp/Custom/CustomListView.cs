using System.Windows.Input;
using Xamarin.Forms;

namespace TimeStudy.Custom
{
    public class CustomListView : ListView
    {

        public static BindableProperty ItemClickedCommandProperty =
            BindableProperty.Create(nameof(ItemClickedCommand), typeof(ICommand), typeof(CustomListView), null);

        public CustomListView(ListViewCachingStrategy strategy) : base(strategy)
        {
            this.ItemTapped += OnItemTapped;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                ItemClickedCommand?.Execute(e.Item);
                SelectedItem = null;
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
