using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    internal sealed class ErrorReporter : IDisposable
    {
#pragma warning disable S3459 // Unassigned members should be removed
        private readonly TelemetryClient client;
#pragma warning restore S3459 // Unassigned members should be removed

        public ErrorReporter(string apiKey)
        {
#if !DEBUG
            client = new TelemetryClient(new TelemetryConfiguration(apiKey));
            var version = GetType().Assembly?.GetName()?.Version?.ToString(3);
            if (version != null)
                client.Context.Component.Version = version;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Report(e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            client.TrackPageView("started");
#endif
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
            => Report(args.Exception.Flatten().InnerException);

        internal void Report(object exceptionObject)
        {
            if (client == null)
                return;

            if (exceptionObject is Exception exception)
            {
                client.TrackException(exception);
            }
            else
            {
                client.TrackException(new Exception("Unsupported exception: " + exceptionObject.ToString()));
            }

            client.Flush();
        }

        public void Dispose()
        {
            if (client != null)
                client.Flush();
            Console.WriteLine("dispose");
        }
    }
}
