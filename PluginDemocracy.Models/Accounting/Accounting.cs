namespace PluginDemocracy.Models
{
    public class Accounting
    {
        public int Id { get; set; }
        public HOACommunity? Community { get; set; }
        public List<Transaction> Transactions { get; private set; }
        public decimal Balance => Transactions?.Sum(t => t.Amount) ?? 0m;
        protected Accounting()
        {
            Transactions = new List<Transaction>();
        }
        public Accounting(HOACommunity community)
        {
            Community = community;
            Transactions = new();
        }
        public void AddTransaction(decimal amount, string description, User accountant, string? category = null)
        {
            Transaction transaction = new(accountant, amount, description, category);
            Transactions.Add(transaction);
        }
        public void AddTransactin(decimal amount, string description, BaseDictamen dictamen, string? category = null)
        {
            Transaction transaction = new(dictamen, amount, description, category);
            Transactions.Add(transaction);
        }
    }
}
