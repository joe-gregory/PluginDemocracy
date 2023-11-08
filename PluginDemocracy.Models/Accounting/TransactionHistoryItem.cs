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
        public int Id { get; set; }
        /// <summary>
        /// Who made the change
        /// </summary>
        public User? Accountant { get; private set; }
        public BaseDictamen? Dictamen { get; private set; }
        public ActionType Action { get; private set; }
        public DateTime Date { get; private set; }
        /// <summary>
        /// Values of Transaction after changes
        /// </summary>
        public Transaction? TransactionSnapShot {get; private set; }
        /// <summary>
        /// Private parameterless constructor for EF
        /// </summary>
        private TransactionHistoryItem() { }

        public TransactionHistoryItem(User accountant, ActionType actionType, Transaction transactionSnapShot)
        {
            Accountant = accountant;
            Dictamen = null;
            Action = actionType;
            Date = DateTime.UtcNow;
            TransactionSnapShot = new Transaction(transactionSnapShot);
        }
        public TransactionHistoryItem(BaseDictamen dictamen, ActionType actionType, Transaction transactionSnapShot)
        {
            Accountant = null;
            Dictamen = dictamen;
            Action = actionType;
            Date = DateTime.UtcNow;
            TransactionSnapShot = new Transaction(transactionSnapShot);
        }
    }
}
