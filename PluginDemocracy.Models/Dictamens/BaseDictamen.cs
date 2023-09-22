namespace PluginDemocracy.Models
{
    public abstract class BaseDictamen : IMultilingualDescriptor
    {
        public Guid Guid { get; }
        public MultilingualString Title { get; set; }
        public MultilingualString Description { get; set; }
        public Community? Community { get; }
        //Change this because the Origin of a Dictamen is either a Role or a Proposal
        public DateTime? IssueDate { get; private set; }
        /// <summary>
        /// Proposal that is running this Dictamen
        /// </summary>
        public Proposal? OriginProposal { get; set; }
        /// <summary>
        /// Strategies:
        /// </summary>
        public User? OriginUser { get; set; }
        private BaseDictamen()
        {
            Guid = Guid.NewGuid();
            Title = new();
            Description = new();
        }
        /// <summary>
        /// Community invokes Dictamen.Execute() which is the actual action the Dictamen takes. If it works, it returns true. 
        /// </summary>
        public void Issue()
        {
            if (Community != null && Community.IssueDictamen(this))
            {
                IssueDate = DateTime.Now;
            }
        }
        /// <summary>
        /// Execute() is the actual action the Dictamen takes and it is invoked by the Community running this Dictamne.
        /// </summary>
        abstract public void Execute();
    }
}
