using System;
using System.ComponentModel;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace PluginDemocracy.Models
{
    public class Dictamen
    {
        public Guid Guid { get; }
        public Proposal Origin { get; }
        public DateTime IssueDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Active;
        public Dictamen(Proposal proposal)
        {
            Guid = Guid.NewGuid();
            Origin = proposal;
        }
    }
}
