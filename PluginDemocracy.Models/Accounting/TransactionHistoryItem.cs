namespace PluginDemocracy.Models
{
    public enum ActionType
    {
        Created,
        Edited,
        Deleted
    }

    /// <summary>
    /// Which transaction, which property did they edit, what was the previous value, what is the new value, who edited it
    /// </summary>
    public class TransactionHistoryItem
    {
        /// <summary>
        /// Who made the change
        /// </summary>
        public IAccountant Accountant { get; }
        public ActionType Action { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public string? Category { get; }

        public TransactionHistoryItem(IAccountant accountant, ActionType action, decimal amount, string description, string? category = null)
        {
            Date = DateTime.UtcNow;
            Action = action;
            Accountant = accountant;
            Amount = amount;
            Description = description;
            Category = category;
        }
    }
}
