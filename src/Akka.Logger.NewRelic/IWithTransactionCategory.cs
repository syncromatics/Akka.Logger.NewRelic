namespace Akka.Logger.NewRelic
{
    /// <summary>
    /// Identifies an actor that may specify New Relic transaction categories
    /// </summary>
    public interface IWithTransactionCategory
    {
        /// <summary>
        /// Current transaction category to report to New Relic
        /// </summary>
        string TransactionCategory { get; set; }
    }
}