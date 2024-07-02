using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public class Project
    {
        public int Id { get; set; }
        public Guid Guid { get; private set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedOn { get; }
        public bool Active => DateTime.UtcNow < Deadline;
        public HOACommunity? Community { get; }
        public decimal? FundingGoal { get; set; }
        public decimal? CurrentFunding => Accounting?.Balance ?? 0m;
        public decimal? RemainingFunding => FundingGoal - CurrentFunding;
        public DateTime? Deadline { get; }
        public BaseDictamen? Dictamen { get; }
        public User? Author { get; }
        public Accounting? Accounting { get; }
        protected Project() {}
        public Project(string title, string description, HOACommunity community, decimal fundingGoal, DateTime deadline, BaseDictamen dictamen)
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
            Accounting?.AddTransaction(amount, $"Donation to {Title} by {user.FullName}.", user, "Donation");
        }
    }
}
