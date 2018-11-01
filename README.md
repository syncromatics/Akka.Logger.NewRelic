# Akka.Logger.NewRelic

[Logger implementation](http://getakka.net/articles/utilities/logging.html) in New Relic for [Akka.NET](https://github.com/akkadotnet/akka.net) actor systems.

## Quickstart

### Add the `Akka.Logger.NewRelic` package to your project:

```bash
dotnet add package Akka.Logger.NewRelic
```

### Ensure the [New Relic agent is installed](https://docs.newrelic.com/docs/agents/net-agent/getting-started/introduction-new-relic-net).

Also make sure the agent is running in order to collect and report transactions from your program.

### Enable the program to be monitored by the New Relic agent and name the app

From [app.config](src/Akka.Logger.NewRelic.Demo/app.config):
```xml
<appSettings>
    <add key="NewRelic.AgentEnabled" value="true" />
    <add key="NewRelic.AppName" value="Akka.Logger.NewRelic.Demo" />
</appSettings>
```

### Reference the New Relic logger in the Akka configuration

Example New Relic logger configuration inside your app.config or web.config:

```
akka {
    loggers = ["Akka.Logger.NewRelic.NewRelicLogger, Akka.Logger.NewRelic"]
}
```

(See [Program.cs](src/Akka.Logger.NewRelic.Demo/Program.cs) for an example of logger configuration directly in code.)

### Build an IoC container with proxied actors

Using Autofac, from [Program.cs](src/Akka.Logger.NewRelic.Demo/Program.cs):

```csharp
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
```

## Building

[![Travis](https://img.shields.io/travis/syncromatics/Akka.Logger.NewRelic.svg)](https://travis-ci.org/syncromatics/Akka.Logger.NewRelic)
[![NuGet](https://img.shields.io/nuget/v/Akka.Logger.NewRelic.svg)](https://www.nuget.org/packages/Akka.Logger.NewRelic/)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Akka.Logger.NewRelic.svg)](https://www.nuget.org/packages/Akka.Logger.NewRelic/)

The package targets .NET Standard 2.0 and can be built via [.NET Core](https://www.microsoft.com/net/core):

```bash
dotnet build
```

Because the standard New Relic agent (as of verion 6.18.139.0) does not (yet) support instrumenting .NET Core apps, the [demo program](src/Akka.Monitoring.NewRelic.Demo) targets .NET Framework 4.6.2.

## Code of Conduct

We are committed to fostering an open and welcoming environment. Please read our [code of conduct](CODE_OF_CONDUCT.md) before participating in or contributing to this project.

## Contributing

We welcome contributions and collaboration on this project. Please read our [contributor's guide](CONTRIBUTING.md) to understand how best to work with us.

## License and Authors

[![GMV Syncromatics Engineering logo](https://secure.gravatar.com/avatar/645145afc5c0bc24ba24c3d86228ad39?size=16) GMV Syncromatics Engineering](https://github.com/syncromatics)

[![license](https://img.shields.io/github/license/syncromatics/Akka.Logger.NewRelic.svg)](https://github.com/syncromatics/Akka.Logger.NewRelic/blob/master/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/syncromatics/Akka.Logger.NewRelic.svg)](https://github.com/syncromatics/Akka.Logger.NewRelic/graphs/contributors)

This software is made available by GMV Syncromatics Engineering under the MIT license.