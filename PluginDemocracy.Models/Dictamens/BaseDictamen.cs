/// <summary>
/// Summary description for Class1
/// </summary>
namespace PluginDemocracy.Models
{
    /// <summary>
    /// A Dictamen can be Ran by a Proposal or a Role
    /// </summary>
    public abstract class BaseDictamen
    {
        public Guid Guid { get; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public BaseCommunity Community { get; }
        //Change this because the Origin of a Dictamen is either a Role or a Proposal
        public BaseDictamenOrigin Origin { get; }
        public DateTime? IssueDate { get; private set; }
        public BaseDictamen(BaseDictamenOrigin origin)
        {
            Guid = Guid.NewGuid();
            Community = origin.Community;
            Origin = origin;
        }
        public void Issue()
        {
            if (Community.IssueDictamen(this))
            {
                IssueDate = DateTime.Now;
                Execute();
            }
        }
        /// <summary>
        /// Execute() is the actual action the Dictamen takes
        /// </summary>
        public abstract void Execute();
    }
}
