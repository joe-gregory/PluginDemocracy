using System;

internal class Proposal : IProposal
{
    private IProposalOrigin Origin { get; set; }
    private string Title { get; set; }

    private string _description;
    string Description
    {
        get { return _description; }
        set
        {
            _description = SanitizeHtml(value);
        }
    }
    private string SanitizeHtml(string html)
    {
        //var sanitizer = new Ganss.XSS.HtmlSanitizer();
        //return sanitizer.Sanitize(html);
    }



}