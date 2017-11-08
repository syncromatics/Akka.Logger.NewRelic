using System;
using Akka.Actor;

namespace Akka.Logger.NewRelic.Demo
{
    public class QuickResponder : ReceiveActor
    {
        public QuickResponder()
        {
            Receive<Message>(message =>
            {
                var response = new Message
                {
                    Text = $"Re: {message.Text.Substring(0, Math.Min(message.Text.Length, 30))}"
                };

                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Sender, response, Self);
            });
        }
    }
}