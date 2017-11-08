using System;
using System.Linq;
using Akka.Actor;
using Gbfs.Net.v1;
using NewRelic.Api.Agent;

namespace Akka.Logger.NewRelic.Demo
{
    public class Thinker : ReceiveActor, IWithTransactionCategory
    {
        public string TransactionCategory { get; set; } = "Default";

        public class Think
        {
            public Guid Id { get; set; }
            public Message Message { get; set; }
        }

        public Thinker()
        {
            Become(() => Waiting());
        }

        [Trace]
        void Waiting()
        {
            TransactionCategory = nameof(Waiting);

            Receive<Message>(message =>
            {
                var think = new Think
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                };

                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, think, Sender);

                Become(Thinking);
            });
        }

        [Trace]
        private void Thinking()
        {
            TransactionCategory = nameof(Thinking);

            ReceiveAsync<Think>(async think =>
            {
                var laMetroGbfsFeedUrl = "https://gbfs.bcycle.com/bcycle_lametro/gbfs.json";
                var client = GbfsClient.GetInstance(laMetroGbfsFeedUrl);
                var manifest = await client.GetManifest();
                var language = manifest.Data.ContainsKey("en") ? "en" : manifest.Data.Keys.First();
                var statuses = await manifest.GetStationStatus(client, language);
                var totalBikesAvailable = statuses.Data.Stations.Sum(s => s.NumBikesAvailable);
                Sender.Tell(new Message
                {
                    Text = $"Whoa, there are {totalBikesAvailable} bikes available! - {think.Message.Text}",
                });
                Become(Waiting);
            });
        }
    }
}