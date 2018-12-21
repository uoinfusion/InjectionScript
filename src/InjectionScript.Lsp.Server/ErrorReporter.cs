using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Threading.Tasks;

namespace InjectionScript.Lsp.Server
{
    internal sealed class ErrorReporter
    {
        private readonly TelemetryClient client;

        public ErrorReporter(string apiKey)
        {
            client = new TelemetryClient(new TelemetryConfiguration(apiKey));
            var version = GetType().Assembly?.GetName()?.Version?.ToString(3);
            if (version != null)
                client.Context.Component.Version = version;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => Report(e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            client.TrackPageView("started");
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
            => Report(args.Exception.Flatten().InnerException);

        internal void Report(object exceptionObject)
        {
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
    }
}
