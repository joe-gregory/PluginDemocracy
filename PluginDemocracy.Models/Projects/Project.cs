namespace PluginDemocracy.Models
{
    public class Project
    {
        public int Id { get; set; }
        public Guid Guid { get; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; }
        public bool Active => DateTime.UtcNow < Deadline;
        public Community Community { get; }
        public decimal FundingGoal { get; set; }
        public decimal CurrentFunding => Accounting.Balance;
        public decimal RemainingFunding => FundingGoal - CurrentFunding;
        public DateTime Deadline { get; }
        public BaseDictamen Dictamen { get; }
        public User? Author { get; }
        public Accounting Accounting { get; }

        public Project(string title, string description, Community community, decimal fundingGoal, DateTime deadline, BaseDictamen dictamen)
        {
            Guid = new();
            Title = title;
            Description = description;
            CreatedOn = DateTime.UtcNow;
            Community = community;
            FundingGoal = fundingGoal;
            Deadline = deadline;
            Dictamen = dictamen;
            Author = dictamen.Proposal?.Author?? null;
            Accounting = new Accounting(community);
        }

        public void MakeDonation(User user, decimal amount)
        {
            Accounting.AddTransaction(amount, $"Donation to {Title} by {user.FullName}.", user, "Donation");
        }
    }
}
