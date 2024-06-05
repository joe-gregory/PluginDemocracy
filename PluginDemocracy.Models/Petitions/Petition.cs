using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace PluginDemocracy.Models
{
    public class Petition
    {
        #region DATA
        /// <summary>
        /// This is set by EFC
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Protected field that indicates whether the Petition has been published.
        /// If the petition has been published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        protected bool _published = false;
        public bool Published
        {
            get
            {
                return _published;
            }
        }
        /// <summary>
        /// The date at which the Petition was published, and thus made available for signatures.
        /// Once published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        public DateTime? PublishedDate { get; protected set; } = null;
        /// <summary>
        /// Returns the date of when the petition was published or the most recent e-signature was added.
        /// Whatever is the most recent date.
        /// </summary>
        public DateTime? LastUpdated
        {
            get
            {
                ESignature? mostRecentSignature = _signatures.OrderByDescending(s => s.SignedDate).FirstOrDefault();
                return mostRecentSignature != null && mostRecentSignature.SignedDate > PublishedDate ? mostRecentSignature.SignedDate : PublishedDate;
            }
        }
        protected Community? _community;
        /// <summary>
        /// The community for which this petition is being created.
        /// </summary>
        public Community? Community
        {
            get
            {
                return _community;
            }
            set
            {
                if (_published) throw new System.InvalidOperationException("Cannot change community of a published petition.");
                _community = value;
            }
        }
        protected string? _title;
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
                if (_published) throw new System.InvalidOperationException("Cannot change title of a published petition.");
                _title = value;
            }
        }
        protected string? _description;
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
                if (_published) throw new System.InvalidOperationException("Cannot change description of a published petition.");
                _description = value;
            }
        }
        /// <summary>
        /// A clear statement of what the petitioners want to happen as a result of the petition.
        /// </summary>
        protected string? _actionRequested;
        public string? ActionRequested
        {
            get
            {
                return _actionRequested;
            }
            set
            {
                if (_published) throw new System.InvalidOperationException("Cannot change action requested of a published petition.");
                _actionRequested = value;
            }
        }
        protected string? _supportingArguments;
        public string? SupportingArguments
        {
            get
            {
                return _supportingArguments;
            }
            set
            {
                if (_published) throw new System.InvalidOperationException("Cannot change supporting arguments of a published petition.");
                _supportingArguments = value;
            }
        }
        protected DateTime? _deadlineForResponse;
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
                if (_published) throw new System.InvalidOperationException("Cannot change deadline for response of a published petition.");
                _deadlineForResponse = value;
            }
        }
        protected List<string> _linksToSupportingDocuments = [];
        /// <summary>
        /// EF Core needs a property it can map to. It wont map protected fields. The property can be private/protected.
        /// </summary>
        protected string LinksToSupportingDocumentsSerialized
        {
            get => string.Join(';', _linksToSupportingDocuments);
            set => _linksToSupportingDocuments = [.. value.Split(';', StringSplitOptions.RemoveEmptyEntries)];
        }
        /// <summary>
        /// This documents will be stored in blob storage and the link to those documents will be stored here. 
        /// </summary>
        [NotMapped]
        public IEnumerable<string> LinksToSupportingDocuments
        {
            get
            {
                return _published ? _linksToSupportingDocuments.AsReadOnly() : _linksToSupportingDocuments;
            }
        }
        protected List<User> _authors;
        /// <summary>
        /// The name and contact information of the person or group who created the petition.
        /// </summary>
        public IEnumerable<User> Authors
        {
            get
            {
                return _published ? _authors.AsReadOnly() : _authors;
            }
        }
        protected List<User> _authorsReadyToPublish;
        public IEnumerable<User> AuthorsReadyToPublish
        {
            get
            {
                return _published ? _authorsReadyToPublish.AsReadOnly() : _authorsReadyToPublish;
            }
        }
        protected List<ESignature> _signatures = [];
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
        /// This constructor is for use by EFC
        /// </summary>
        protected Petition()
        {
            _authors = [];
            _authorsReadyToPublish = [];
        }
        public Petition(User originalAuthor)
        {
            _authors = [originalAuthor];
            _authorsReadyToPublish = [];
        }
        /// <summary>
        /// When it is published, the petition is made available to the public for signatures.
        /// Once published, no more changes can be made to a petition. 
        /// </summary>
        protected void Publish()
        {
            IsGoodToPublish();
            _published = true;
            PublishedDate = DateTime.UtcNow;
        }
        public void Publish(User author)
        {
            IsGoodToPublish();
            if (!_authors.Contains(author)) throw new System.InvalidOperationException("Author must be added to the petition before it can be published.");
            _authorsReadyToPublish.Add(author);
            if (_authorsReadyToPublish.Count == _authors.Count) Publish();
        }
        /// <summary>
        /// Throws exceptions if the petition is not ready to be published.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void IsGoodToPublish()
        {
            if (_published) throw new System.InvalidOperationException("Petition has already been published.");
            if (_community == null) throw new System.InvalidOperationException("Petition must be associated with a community.");
            if (_title == null) throw new System.InvalidOperationException("Petition must have a title.");
            if (_description == null) throw new System.InvalidOperationException("Petition must have a description.");
            if (_authors.Count == 0) throw new System.InvalidOperationException("Petition must have at least one author.");
            if (_actionRequested == null) throw new System.InvalidOperationException("Petition must have an action requested.");
        }
        public void Sign(ESignature signature)
        {
            if (!_published) throw new System.InvalidOperationException("Cannot sign an un-published petition.");
            if (_signatures.Any(s => s.Signer == signature.Signer)) throw new System.InvalidOperationException("Cannot sign a petition more than once.");
            _signatures.Add(signature);
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
