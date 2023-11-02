namespace PluginDemocracy.Models
{
    public class Accounting
    {
        public Community Community { get; set; }
        public List<Transaction> Transactions { get; }
        public decimal Balance => Transactions.Sum(t => t.Amount);
        public Accounting(Community community)
        {
            Community = community;
            Transactions = new();
        }
        public void AddTransaction(decimal amount, string description, User accountant, string? category = null)
        {
            Transaction transaction = new(amount, description, accountant, category);
            Transactions.Add(transaction);
        }
        public void AddTransactin(decimal amount, string description, BaseDictamen dictamen, string? category = null)
        {
            Transaction transaction = new(amount, description, dictamen, category);
            Transactions.Add(transaction);
        }
    }
}
