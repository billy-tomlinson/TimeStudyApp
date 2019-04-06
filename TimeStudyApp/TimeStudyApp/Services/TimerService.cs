using System;
using System.Threading;
using Xamarin.Forms;

namespace TimeStudyApp.Services
{

    public class TimerService
    {

        private static CancellationTokenSource cancellation = new CancellationTokenSource();

        public static void StartTimer(TimeSpan timespan, Func<bool> callback)
        {

            CancellationTokenSource cts = cancellation; // safe copy
            Device.StartTimer(timespan,
                () => {
                    if (cts.IsCancellationRequested) return false;
                    return callback.Invoke();
            });
        }

        public static void Stop()
        {
            Interlocked.Exchange(ref cancellation, new CancellationTokenSource()).Cancel();
        }
    }
}
