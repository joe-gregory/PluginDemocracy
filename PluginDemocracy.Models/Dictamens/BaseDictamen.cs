namespace PluginDemocracy.Models
{
    public abstract class BaseDictamen : IAccountant
    {
        public Guid Guid { get; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Community Community { get; set; }
        //Change this because the Origin of a Dictamen is either a Role or a Proposal
        public DateTime? IssueDate { get; private set; }
        /// <summary>
        /// Proposal that is running this Dictamen
        /// </summary>
        public Proposal? Proposal { get; set; }

        public Type Type => typeof(BaseDictamen);
        /// <summary>
        /// Strategies:
        /// </summary> 
        public BaseDictamen(string title, string description, Community community)
        {
            Guid = Guid.NewGuid();
            Title = title;
            Description = description;
            Community = community;
        }
        /// <summary>
        /// Community invokes Dictamen.Execute() which is the actual action the Dictamen takes. If it works, it returns true. 
        /// </summary>
        public void Issue()
        {
            if (Community == null) throw new Exception("This Dictamen's Community property is null.");
            if (Community.IssueDictamen(this))
            {
                IssueDate = DateTime.UtcNow;
            }
            else throw new Exception("Community.IssueDictamen did not return true.");
        }
        /// <summary>
        /// Execute() is the actual action the Dictamen takes and it is invoked by the Community running this Dictamne.
        /// </summary>
        abstract public void Execute();
    }
}
