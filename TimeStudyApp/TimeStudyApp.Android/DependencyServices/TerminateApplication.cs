using TimeStudy.Droid.DependencyServices;
using TimeStudy.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TerminateApplication))]
namespace TimeStudy.Droid.DependencyServices
{
    public class TerminateApplication : ITerminateApplication
    {
        public void CloseApplication()
        {
            Java.Lang.JavaSystem.Exit(0);
        }
    }
}