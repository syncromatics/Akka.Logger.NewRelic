using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.Core;
using Autofac;
using Autofac.Extras.DynamicProxy;
using NewRelic.Api.Agent;

namespace Akka.Logger.NewRelic.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Reference the New Relic logger in the Akka configuration
            var config = ConfigurationFactory.FromObject(new
            {
                akka = new
                {
                    loggers = new[]
                    {
                        typeof(NewRelicLogger).AssemblyQualifiedName,
                    },
                },
            });
            var system = ActorSystem.Create("akka-logger-newrelic-demo", config);

            // Build an Autofac container with the AroundReceiveInterceptor registered for all actors to be instrumented
            var builder = new ContainerBuilder();
            builder.RegisterType<AroundReceiveInterceptor>();
            builder.RegisterAssemblyTypes(typeof(QuickResponder).Assembly)
                .Where(t => t.IsSubclassOf(typeof(ActorBase)))
                .EnableClassInterceptors()
                .InterceptedBy(typeof(AroundReceiveInterceptor));
            
            // Register the TransactionWrapper to enable the New Relic agent to instrument the intercepted actors
            builder.RegisterInstance<TransactionWrapper>(TransactionWrapper);

            // Register the Autofac DI resolver
            var container = builder.Build();
            new Akka.DI.AutoFac.AutoFacDependencyResolver(container, system);

            // Create and interact with actors as needed
            StartActors(system);

            // Allow the actor system to run for a short bit
            var delay = TimeSpan.FromMinutes(2);
            Console.WriteLine($"Waiting {delay} before terminating actor system...");
            Thread.Sleep(delay);

            // Stop the actor system
            system.Terminate().Wait();
            Console.Write("Press any key to quit");
            Console.ReadKey();
        }

        [Transaction]
        private static void TransactionWrapper(Action invoke)
        {
            invoke();
        }

        private static void StartActors(ActorSystem system)
        {
            var thinker = system.ActorOf(system.DI().Props<Thinker>());
            var quickResponder = system.ActorOf(system.DI().Props<QuickResponder>());
            system.ActorOf(system.DI().Props<ExceptionThrowingActor>());
            quickResponder.Tell(new Message {Text = "Hi QuickResponder!"}, thinker);
        }
    }
}
