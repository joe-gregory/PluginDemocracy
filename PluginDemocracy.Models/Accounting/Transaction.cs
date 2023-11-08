namespace PluginDemocracy.Models
{
    public class Transaction : IRedFlaggable
    {
        public int Id { get; set; }
        public Guid ID { get; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public string? Category { get; private set; }
        public List<TransactionHistoryItem> History { get; }
        //IREDFLAGGABLE:
        public List<RedFlag> RedFlags { get; }
        public Type Type => typeof(Transaction);
        //END IREDFLAGGABLE

        public Transaction(User accountant, decimal amount, string description, string? category = null)
        {
            ID = new();
            Amount = amount;
            Description = description;
            if(category != null) Category = category;
            History = new();
            RedFlags = new();
            TransactionHistoryItem transactionHistoryItem = new(accountant, ActionType.Created, this);
            History.Add(transactionHistoryItem);
        }
        public Transaction(BaseDictamen dictamen, decimal amount, string description, string? category = null)
        {
            ID = new();
            Amount = amount;
            Description = description;
            if (category != null) Category = category;
            History = new();
            RedFlags = new();
            TransactionHistoryItem transactionHistoryItem = new(dictamen, ActionType.Created, this);
            History.Add(transactionHistoryItem);
        }
        public Transaction (Transaction transactionSnapShot)
        {
            ID = transactionSnapShot.ID; 
            Amount = transactionSnapShot.Amount;
            Description = transactionSnapShot.Description;
            Category = transactionSnapShot.Category;
            History = new List<TransactionHistoryItem>(transactionSnapShot.History);
            RedFlags = new List<RedFlag>(transactionSnapShot.RedFlags);
        }
        public void Edit(User accountant, decimal? amount = null, string? description = null, string? category = null)
        {
            Amount = amount ?? Amount;
            Description = description ?? Description;
            Category = category ?? Category;
            TransactionHistoryItem transactionHistoryItem = new(accountant, ActionType.Edited, this);
            History.Add(transactionHistoryItem);
        }
    }
}
