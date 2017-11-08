using System;
using System.Threading;
using Akka.Actor;
using NewRelic.Api.Agent;

namespace Akka.Logger.NewRelic.Demo
{
    public class ExceptionThrowingActor : ReceiveActor
    {
        public ExceptionThrowingActor()
        {
            Receive<string>(_ => DoWork());

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(1), Self, "bump", Self);
        }

        [Transaction]
        private static void DoWork()
        {
            SpinWait.SpinUntil(() => false, TimeSpan.FromSeconds(1));

            throw new Exception("Hoark!", new Exception("Some work was attempted"));
        }
    }
}