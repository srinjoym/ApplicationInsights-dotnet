﻿namespace Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.Implementation
{
    using System;
#if !NET40
    using System.Diagnostics.Tracing;
#endif

#if NET40
    using Microsoft.Diagnostics.Tracing;
#endif

    [EventSource(Name = "Microsoft-ApplicationInsights-Extensibility-PerformanceCollector")]
    internal sealed class PerformanceCollectorEventSource : EventSource
    {
        private static readonly PerformanceCollectorEventSource Logger = new PerformanceCollectorEventSource();

        private PerformanceCollectorEventSource()
        {
            this.ApplicationName = this.GetApplicationName();
        }

        public static PerformanceCollectorEventSource Log
        {
            get
            {
                return Logger;
            }
        }

        public string ApplicationName { [NonEvent]get; [NonEvent]private set; }

        #region Infra init - success

        [Event(1, Level = EventLevel.Informational, Message = @"Performance counter infrastructure is being initialized. {0}")]
        public void ModuleIsBeingInitializedEvent(
            string message,
            string dummy = "dummy",
            string applicationName = "dummy")
        {
            this.WriteEvent(1, message, this.ApplicationName);
        }

        [Event(3, Level = EventLevel.Informational, Message = @"Performance counter {0} has been successfully registered with performance collector.")]
        public void CounterRegisteredEvent(string counter, string applicationName = "dummy")
        {
            this.WriteEvent(3, counter, this.ApplicationName);
        }

        [Event(4, Level = EventLevel.Informational, Message = @"Performance counters have been refreshed. Refreshed counters count is {0}.")]
        public void CountersRefreshedEvent(
            string countersRefreshedCount,
            string applicationName = "dummy")
        {
            this.WriteEvent(4, countersRefreshedCount, this.ApplicationName);
        }

        #endregion

        #region Infra init - failure

        [Event(5, Keywords = Keywords.UserActionable, Level = EventLevel.Warning, Message = @"Performance counter {1} has failed to register with performance collector. Please make sure it exists. Technical details: {0}")]
        public void CounterRegistrationFailedEvent(string e, string counter, string applicationName = "dummy")
        {
            this.WriteEvent(5, e, counter, this.ApplicationName);
        }

        [Event(6, Keywords = Keywords.UserActionable, Level = EventLevel.Warning, Message = @"Performance counter specified as {1} in ApplicationInsights.config was not parsed correctly. Please make sure that a proper name format is used. Expected formats are \category(instance)\counter or \category\counter. Technical details: {0}")]
        public void CounterParsingFailedEvent(string e, string counter, string applicationName = "dummy")
        {
            this.WriteEvent(6, e, counter, this.ApplicationName);
        }

        [Event(8, Keywords = Keywords.UserActionable, Level = EventLevel.Error,
            Message = @"Error collecting {0} of the configured performance counters. Please check the configuration.
{1}")]
        public void CounterCheckConfigurationEvent(
            string misconfiguredCountersCount,
            string e,
            string applicationName = "dummy")
        {
            this.WriteEvent(8, misconfiguredCountersCount, e, this.ApplicationName);
        }

        // Verbosity is Error - so it is always sent to portal; Keyword is Diagnostics so throttling is not applied.
        [Event(15, Level = EventLevel.Error,
            Keywords = Keywords.Diagnostics | Keywords.UserActionable,
            Message = @"Diagnostic message: Performance counters are unavailable when the application is running under IIS Express. Use EnableIISExpressPerformanceCounters element with a value of 'true' within the Performance Collector Module element to override this behavior.")]
        public void RunningUnderIisExpress(string applicationName = "dummy")
        {
            this.WriteEvent(15, this.ApplicationName);
        }

        #endregion

        #region Data reading - success

        [Event(9, Level = EventLevel.Verbose, Message = @"About to perform counter collection...")]
        public void CounterCollectionAttemptEvent(string applicationName = "dummy")
        {
            this.WriteEvent(9, this.ApplicationName);
        }

        [Event(10, Level = EventLevel.Verbose, Message = @"Counters successfully collected. Counter count: {0}, collection time: {1}")]
        public void CounterCollectionSuccessEvent(
            long counterCount,
            long operationDurationInMs,
            string applicationName = "dummy")
        {
            this.WriteEvent(10, counterCount, operationDurationInMs, this.ApplicationName);
        }

        #endregion

        #region Data reading - failure

        [Event(11, Level = EventLevel.Warning, Message = @"Performance counter {1} has failed the reading operation. Error message: {0}")]
        public void CounterReadingFailedEvent(string e, string counter, string applicationName = "dummy")
        {
            this.WriteEvent(11, e, counter, this.ApplicationName);
        }

        #endregion

        #region Data sending - success

        #endregion

        #region Data sending - failure

        [Event(12, Level = EventLevel.Warning, Message = @"Failed to send a telemetry item for performance collector. Error text: {0}")]
        public void TelemetrySendFailedEvent(string e, string applicationName = "dummy")
        {
            this.WriteEvent(12, e, this.ApplicationName);
        }

        #endregion

        #region Unknown errors

        [Event(13, Keywords = Keywords.UserActionable, Level = EventLevel.Warning, Message = @"Unknown error in performance counter infrastructure: {0}")]
        public void UnknownErrorEvent(string e, string applicationName = "dummy")
        {
            this.WriteEvent(13, e, this.ApplicationName);
        }

        #endregion

        #region Troubleshooting

        [Event(14, Message = "{0}", Level = EventLevel.Verbose)]
        public void TroubleshootingMessageEvent(string message, string applicationName = "dummy")
        {
            this.WriteEvent(14, message, this.ApplicationName);
        }

        [Event(16, Keywords = Keywords.UserActionable, Level = EventLevel.Error, Message = @"Performance counter is not available in the web app supported list. Counter is {0}.")]
        public void CounterNotWebAppSupported(
            string counterName,
            string applicationName = "dummy")
        {
            this.WriteEvent(16, counterName, this.ApplicationName);
        }

        [Event(17, Level = EventLevel.Warning, Message = @"Accessing environment variable - {0} failed with exception: {1}.")]
        public void AccessingEnvironmentVariableFailedWarning(
            string environmentVariable,
            string exceptionMessage,
            string applicationName = "dummy")
        {
            this.WriteEvent(17, environmentVariable, exceptionMessage, this.ApplicationName);
        }

        [Event(18, Keywords = Keywords.UserActionable, Level = EventLevel.Warning, Message = @"Web App Performance counter {1} has failed to register with performance collector. Please make sure it exists. Technical details: {0}")]
        public void WebAppCounterRegistrationFailedEvent(string e, string counter, string applicationName = "dummy")
        {
            this.WriteEvent(18, e, counter, this.ApplicationName);
        }

        [Event(19, Level = EventLevel.Error, Message = @"CounterName:{2} has unexpected (negative) value. Last Collected Value:{0} Previous Value:{1}. To avoid negative value, this will be reported as zero instead.")]
        public void WebAppCounterNegativeValue(double lastCollectedValue, double previouslyCollectedValue, string counterName, string applicationName = "dummy")
        {
            this.WriteEvent(19, lastCollectedValue, previouslyCollectedValue, counterName, this.ApplicationName);
        }

        #endregion

        [NonEvent]
        private string GetApplicationName()
        {
            string name;
            try
            {
                name = AppDomain.CurrentDomain.FriendlyName;
            }
            catch (Exception exp)
            {
                name = "Undefined " + exp.Message ?? exp.ToString();
            }

            return name;
        }

        public class Keywords
        {
            public const EventKeywords UserActionable = (EventKeywords)0x1;

            public const EventKeywords Diagnostics = (EventKeywords)0x2;
        }
    }
}