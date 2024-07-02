using System.ComponentModel.DataAnnotations.Schema;

namespace PluginDemocracy.Models
{
    public abstract class BaseDictamen
    {
        public int Id { get; set; }
        /// <summary>
        /// This is the key used in PluginDemocracy.Translations on the frontend and not the actual Title of the Dictamen.
        /// </summary>
        abstract public string? TitleKey { get; set; }
        /// <summary>
        /// This is the key used in PluginDemocracy.Translations on the frontend and not the actual Description of the Dictamen.
        /// </summary>
        abstract public string? DescriptionKey { get; set; }
        public HOACommunity? Community { get; set; }
        //Change this because the Origin of a Dictamen is either a Role or a Proposal
        public DateTime? IssueDate { get; private set; }
        /// <summary>
        /// Proposal that is running this Dictamen
        /// </summary>
        public int ProposalId { get; set; }
        [ForeignKey("ProposalId")]
        public Proposal? Proposal { get; set; }
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
