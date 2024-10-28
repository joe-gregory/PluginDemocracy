using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace PluginDemocracy.Models
{
    /// <summary>
    /// A petition is a document that is created by one or more users to request a change or action from the HOA board.
    /// The petition is esigned by users who agree with the request. The signatures that matter are the ones from home owners. 
    /// Any edit before it is published will clear the list Authors Ready to Publish. 
    /// </summary>
    public class Petition
    {
        #region DATA
        public int Id { get; init; }
        /// <summary>
        /// Protected field that indicates whether the Petition has been published.
        /// If the petition has been published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        public bool Published { get; private set; } = false;
        /// <summary>
        /// The date at which the Petition was published, and thus made available for signatures.
        /// Once published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        public DateTime? PublishedDate { get; protected set; } = null;
        private DateTime? _lastUpdated;
        /// <summary>
        /// If the petition has not been published, the LastUpdated date can be changed as changes are made to the draft. 
        /// However, once it is published, LastUpdated can only be set internally. It is set to the date it was published and afterwards
        /// after every new e-signature is added. 
        /// </summary>
        public DateTime? LastUpdated 
        {
            get
            {
                return _lastUpdated;
            }
            set 
            {
                _lastUpdated = value;
            } 
        }
        private ResidentialCommunity? _community;
        /// <summary>
        /// The community for which this petition is being created.
        /// </summary>
        public ResidentialCommunity? Community
        {
            get
            {
                return _community;
            }
            set
            {
                if (Published) throw new InvalidOperationException("Cannot change community of a published petition.");
                _community = value;
                _authorsReadyToPublish.Clear();
            }
        }
        private string? _title;
        /// <summary>
        /// A clear and concise title that summarizes the purpose of the petition.
        /// </summary>
        public string? Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (Published) throw new System.InvalidOperationException("Cannot change title of a published petition.");
                _authorsReadyToPublish.Clear();
                _title = value;
            }
        }
        private string? _description;
        /// <summary>
        /// A detailed description of the issue, including why it is important and what changes or actions are being requested.
        /// </summary>
        public string? Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (Published) throw new System.InvalidOperationException("Cannot change description of a published petition.");
                _authorsReadyToPublish.Clear();
                _description = value;
            }
        }
        /// <summary>
        /// A clear statement of what the petitioners want to happen as a result of the petition.
        /// </summary>
        private string? _actionRequested;
        public string? ActionRequested
        {
            get
            {
                return _actionRequested;
            }
            set
            {
                if (Published) throw new System.InvalidOperationException("Cannot change action requested of a published petition.");
                _authorsReadyToPublish.Clear();
                _actionRequested = value;
            }
        }
        private string? _supportingArguments;
        public string? SupportingArguments
        {
            get
            {
                return _supportingArguments;
            }
            set
            {
                if (Published) throw new System.InvalidOperationException("Cannot change supporting arguments of a published petition.");
                _authorsReadyToPublish.Clear();
                _supportingArguments = value;
            }
        }
        private DateTime? _deadlineForResponse;
        /// <summary>
        /// A suggested or required timeline for when the petition should be reviewed and responded to by the HOA board.
        /// </summary>
        public DateTime? DeadlineForResponse
        {
            get
            {
                return _deadlineForResponse;
            }
            set
            {
                if (Published) throw new System.InvalidOperationException("Cannot change deadline for response of a published petition.");
                _authorsReadyToPublish.Clear();
                _deadlineForResponse = value;
            }
        }
        private readonly List<string> _linksToSupportingDocuments = [];
        /// <summary>
        /// This documents will be stored in blob storage and the link to those documents will be stored here. 
        /// </summary>
        public List<string> LinksToSupportingDocuments => _linksToSupportingDocuments;
        private readonly List<User> _authors = [];
        /// <summary>
        /// The User(s) who created the petition and can modify it when not published.
        /// </summary>
        public IReadOnlyList<User> Authors => _authors.AsReadOnly();
        private readonly List<User> _authorsReadyToPublish = [];
        public IReadOnlyList<User> AuthorsReadyToPublish => _authorsReadyToPublish.AsReadOnly();
        private readonly List<ESignature> _signatures = [];
        public IReadOnlyList<ESignature> Signatures
        {
            get
            {
                return _signatures.AsReadOnly();
            }
        }
        #endregion
        #region METHODS
        /// <summary>
        /// Private constructor for the benefit of EFC
        /// </summary>
        private Petition() {}
        public Petition(User originalAuthor)
        {
            AddAuthor(originalAuthor);
        }
        /// <summary>
        /// When it is published, the petition is made available to the public for signatures.
        /// Once published, no more changes can be made to a petition. 
        /// </summary>
        protected void Publish()
        {
            IsItGoodToPublish();
            #pragma warning disable CS8602 // IsItGoodToPublish() checks if Community is null. 
            Community.AddPetition(this);
            #pragma warning restore CS8602
            PublishedDate = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            Published = true;
            //foreach (User author in _authors) author.RemovePetitionDraft(this);
        }
        /// <summary>
        /// If the petition is published, this method will throw an error.
        /// Otherwise, it adds the User to the list of author and adds the petition to the User's list of petition drafts.
        /// </summary>
        /// <param name="author">User to be added as author</param>
        /// <exception cref="System.InvalidOperationException">Exception thrown if petition is already published</exception>
        public void AddAuthor(User author)
        {
            if (Published) throw new System.InvalidOperationException("Cannot add an author to a published petition.");
            if (_authors.Contains(author)) throw new System.InvalidOperationException("Author is already added to the petition.");
            _authors.Add(author);
            author.AddPetitionDraft(this);
            LastUpdated = DateTime.UtcNow;
        }
        /// <summary>
        /// If published it throws an error. 
        /// Otherwise, it removes the User from the authors list and it removes
        /// the petition from <see cref="User.PetitionDrafts"/>
        /// </summary>
        /// <param name="author">The user to be removed as an author</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void RemoveAuthor(User author)
        {
            if (!_authors.Contains(author)) throw new System.InvalidOperationException("Author is not added to the petition.");
            _authors.Remove(author);
            author.RemovePetitionDraft(this);
            LastUpdated = DateTime.UtcNow;
        }
        public void AddLinkToSupportingDocument(string link)
        {
            if (Published) throw new System.InvalidOperationException("Cannot add a link to a supporting document to a published petition.");
            _linksToSupportingDocuments.Add(link);
            _authorsReadyToPublish.Clear();
            LastUpdated = DateTime.UtcNow;
        }
        public void RemoveLinkToSupportingDocument(string link)
        {
            if (Published) throw new System.InvalidOperationException("Cannot remove a link to a supporting document from a published petition.");
            _linksToSupportingDocuments.Remove(link);
            _authorsReadyToPublish.Clear();
            LastUpdated = DateTime.UtcNow;
        }
        /// <summary>
        /// When there are multiple authors, each author needs to mark the petition as ready to publish.
        /// When all the authors have marked it as published, then the petition publishes. 
        /// If there is only one author, then the petition will publish.
        /// </summary>
        /// <param name="author">The author saying he is ok with the current draft and is ready to publish</param>
        public void MarkAsReadyToPublish(User author)
        {
            IsItGoodToPublish();
            if (!_authors.Any(a => a.Id == author.Id)) throw new System.InvalidOperationException("Author must be added to the petition before it can mark it as ready to be published.");
            if (!_authorsReadyToPublish.Any(a => a.Id == author.Id))_authorsReadyToPublish.Add(author);
            if (_authorsReadyToPublish.Count == _authors.Count) Publish();
        }
        public void NotReadyToPublish(User author)
        {
            User? auth = _authors.FirstOrDefault(a => a.Id == author.Id) ?? throw new InvalidOperationException("No author found that matched Id");
            _authorsReadyToPublish.Remove(author);
            LastUpdated = DateTime.UtcNow;
        }
        /// <summary>
        /// Throws exceptions if the petition is not ready to be published.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void IsItGoodToPublish()
        {
            if (Published) throw new System.InvalidOperationException("Petition has already been published.");
            if (Community == null) throw new System.InvalidOperationException("Petition must be associated with a community.");
            if (_title == null) throw new System.InvalidOperationException("Petition must have a title.");
            if (_description == null) throw new System.InvalidOperationException("Petition must have a description.");
            if (_authors.Count == 0) throw new System.InvalidOperationException("Petition must have at least one author.");
            if (_actionRequested == null) throw new System.InvalidOperationException("Petition must have an action requested.");
        }
        public void Sign(ESignature signature)
        {
            if (!Published) throw new System.InvalidOperationException("Cannot sign an un-published petition.");
            if (_signatures.Any(s => s.Signer == signature.Signer)) throw new System.InvalidOperationException("Cannot sign a petition more than once.");
            _signatures.Add(signature);
            LastUpdated = DateTime.UtcNow;
        }
        public string ReturnHash()
        {
            var hashInput = $"{Title}{Description}{ActionRequested}{DeadlineForResponse}{string.Join(",", LinksToSupportingDocuments)}";
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashInput));
            return Convert.ToBase64String(hashBytes);
        }
        #endregion
    }
}
