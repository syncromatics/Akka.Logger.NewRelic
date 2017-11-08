using System;
using System.Reflection;
using Akka.Actor;
using Castle.DynamicProxy;

namespace Akka.Logger.NewRelic
{
    public class AroundReceiveInterceptor : IInterceptor
    {
        /// <summary>
        /// Reference to the delegate that is attributed for tracing New Relic transactions
        /// </summary>
        private readonly TransactionWrapper _transactionWrapper;

        /// <summary>
        /// Reference to the protected internal <see cref="ActorBase.AroundReceive"/> method
        /// </summary>
        private static readonly MethodInfo ActorBaseReceiveMethod;

        static AroundReceiveInterceptor()
        {
            ActorBaseReceiveMethod = typeof(ActorBase).GetMethod("AroundReceive", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Initializes a new ActorBase.AroundReceive interceptor so it can be reported to the New Relic agent
        /// </summary>
        /// <param name="transactionWrapper">Delegate that is attributed for tracing New Relic transactions</param>
        public AroundReceiveInterceptor(TransactionWrapper transactionWrapper)
        {
            _transactionWrapper = transactionWrapper;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method != ActorBaseReceiveMethod)
            {
                invocation.Proceed();
                return;
            }

            var actorType = invocation.TargetType.Name;
            var messageType = invocation.Arguments[1]?.GetType().Name ?? "Unknown";

            var category = "Default";
            if (invocation.Proxy is IWithTransactionCategory withTransactionCategory)
            {
                category = withTransactionCategory.TransactionCategory ?? category;
            }

            var name = $"{actorType}/{category}/{messageType}";

            void Invoke()
            {
                global::NewRelic.Api.Agent.NewRelic.SetTransactionName("Actor", name);
                try
                {
                    invocation.Proceed();
                }
                catch (Exception e)
                {
                    global::NewRelic.Api.Agent.NewRelic.NoticeError(e, null);
                    throw;
                }
            }

            _transactionWrapper(Invoke);
        }
    }
}