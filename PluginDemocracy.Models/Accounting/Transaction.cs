namespace PluginDemocracy.Models
{
    public class Transaction
    {
        public Guid ID { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public string? Category { get; }
        public List<TransactionHistoryItem> History { get; }
        public Transaction(decimal amount, string description, User accountant, string? category = null)
        {
            ID = new();
            Amount = amount;
            Description = description;
            if(category != null) Category = category;
            History = new();

            TransactionHistoryItem transactionHistoryItem = new(accountant, ActionType.Created, amount, description, category);
            History.Add(transactionHistoryItem);
        }
        public Transaction(decimal amount, string description, BaseDictamen dictamen, string? category = null)
        {
            ID = new();
            Amount = amount;
            Description = description;
            if (category != null) Category = category;
            History = new();

            TransactionHistoryItem transactionHistoryItem = new(dictamen, ActionType.Created, amount, description, category);
            History.Add(transactionHistoryItem);
        }
    }
}
