using System;

internal interface IProposal
{
    IProposalOrigin Origin { get; set; }
    string Title { get; set; }

}
