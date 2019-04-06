using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace TimeStudy.Custom
{
    [DesignTimeVisible(true)]
    public class CustomContentView : ContentView
    {
        public CustomContentView()
        {
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

    }
}
