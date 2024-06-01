using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace PluginDemocracy.Models
{
    public class Petition(User originalAuthor)
    {
        #region DATA
        public int Id { get; set; }
        protected Community? _community;
        /// <summary>
        /// The community for which this petition is being created.
        /// </summary>
        [NotMapped]
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
        /// <summary>
        /// Protected field that indicates whether the Petition has been published.
        /// If the petition has been published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        protected bool _published = false;
        /// <summary>
        /// The date at which the Petition was published, and thus made available for signatures.
        /// Once published, it cannot be unpublished, edited, or deleted.
        /// </summary>
        public DateTime? PublishedDate { get; protected set; } = null;
        protected string? _title;
        /// <summary>
        /// A clear and concise title that summarizes the purpose of the petition.
        /// </summary>
        [NotMapped]
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
        [NotMapped]
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
        protected List<User> _authors = [originalAuthor];
        /// <summary>
        /// The name and contact information of the person or group who created the petition.
        /// </summary>
        [NotMapped]
        public IEnumerable<User> Authors
        {
            get
            {
                return _published ? _authors.AsReadOnly() : _authors;
            }
        }
        /// <summary>
        /// A clear statement of what the petitioners want to happen as a result of the petition.
        /// </summary>
        protected string? _actionRequested;
        [NotMapped]
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
        protected List<ESignature> _signatures = [];
        [NotMapped]
        public IReadOnlyList<ESignature> Signatures
        {
            get
            {
                return _signatures.AsReadOnly();
            }
        }
        protected DateTime? _deadlineForResponse;
        /// <summary>
        /// A suggested or required timeline for when the petition should be reviewed and responded to by the HOA board.
        /// </summary>
        [NotMapped]
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
        /// <summary>
        /// Returns the date of when the petition was published or the most recent e-signature was added.
        /// Whatever is the most recent date.
        /// </summary>
        [NotMapped]
        public DateTime? LastUpdated
        {
            get
            {
                ESignature? mostRecentSignature = _signatures.OrderByDescending(s => s.SignedDate).FirstOrDefault();
                return mostRecentSignature != null && mostRecentSignature.SignedDate > PublishedDate ? mostRecentSignature.SignedDate : PublishedDate;
            }
        }
        #endregion
        #region METHODS
        /// <summary>
        /// When it is published, the petition is made available to the public for signatures.
        /// Once published, no more changes can be made to a petition. 
        /// </summary>
        public void Publish()
        {
            IsGoodToPublish();
            _published = true;
            PublishedDate = DateTime.UtcNow;
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
