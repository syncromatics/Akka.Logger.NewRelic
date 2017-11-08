using System.Collections.Generic;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Event;

namespace Akka.Logger.NewRelic
{
    /// <summary>
    /// Logs all Akka <see cref="LogEvent"/>s to New Relic as custom events
    /// </summary>
    public class NewRelicLogger : ReceiveActor, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
    {
        public NewRelicLogger()
        {
            Receive<LogEvent>(logEvent => Handle(logEvent));
            Receive<InitializeLogger>(_ => Sender.Tell(new LoggerInitialized()));
        }

        private static void Handle(LogEvent logEvent)
        {
            var logLevel = logEvent.LogLevel().ToString().Replace("Level", "");
            var eventType = Regex.Replace(logEvent.LogSource, @"[^a-z\d:_ ]+", "_", RegexOptions.IgnoreCase).Trim('_');
            global::NewRelic.Api.Agent.NewRelic.RecordCustomEvent(eventType, new Dictionary<string, object>
            {
                { "Message", logEvent.Message },
                { "LogLevel", logLevel },
                { "LogClass", logEvent.LogClass.FullName },
            });
        }
    }
}
