namespace PluginDemocracy.Models
{
    public class Accounting
    {
        public int Id { get; set; }
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
