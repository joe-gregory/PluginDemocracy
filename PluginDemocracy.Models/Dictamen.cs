using System;
using System.ComponentModel;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Dictamen
{
    public Guid Guid { get; }
    public Proposal Proposal { get; }
    public DateTime CreationDate { get; }
    virtual public string Type
    {
        get
        {
            return "Dictamen"
        }
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Valid
    {
        get
        {
            if (ValidityModifierDictamen == null)
            {
                return Proposal.Passed && ValiditySchema.IsValid()
            }
            else
            {
                return Proposal.Passed && ValiditySchema.IsValid() && !ValidityModifierDictamen.Valid
            }
        }
    }
    public IDictamenValiditySchema ValiditySchema { get; set; }
    public Dictamen ValidityModifierDictamen { get; set; }

    public Dictamen(Proposal proposal)
    {
        Guid = Guid.NewGuid();
        Proposal = proposal;
        CreationDate = DateTime.UtcNow;
    }
}
