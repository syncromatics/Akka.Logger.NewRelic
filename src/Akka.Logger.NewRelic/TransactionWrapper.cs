using System;

namespace Akka.Logger.NewRelic
{
    /// <summary>
    /// Delegate that is attributed for tracing New Relic transactions
    /// </summary>
    /// <param name="invocation">Action that must be invoked within the body of the delegate</param>
    /// <remarks>
    /// The delegate must be defined in the assembly that is monitored by the New Relic agent
    /// </remarks>
    /// <example>
    /// Define a method to act as the delegate:
    /// <code>
    /// [NewRelic.Api.Agent.Transaction]
    /// private static void TransactionWrapper(Action invoke)
    /// {
    ///     invoke();
    /// }
    /// </code>
    /// 
    /// Then register the delegate in the IoC container (Autofac, etc.):
    /// <code>
    /// builder.RegisterInstance&lt;TransactionWrapper&gt;(TransactionWrapper);
    /// </code>
    /// </example>
    public delegate void TransactionWrapper(Action invocation);
}