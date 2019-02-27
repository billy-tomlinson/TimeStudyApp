using System.Threading;
using TimeStudy.Services;
using TimeStudy.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(TerminateApplication))]
namespace TimeStudy.iOS.DependencyServices
{
    public class TerminateApplication : ITerminateApplication
    {
        public void CloseApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}