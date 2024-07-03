namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; init; }
        public IAvatar? Author { get; private set; }
        public string? Body { get; private set; }
        public DateTime PublishedDate { get; init; }
        public DateTime? LatestActivity { get; private set; } = null;

        private readonly List<PostComment> _comments;
        public IReadOnlyList<PostComment> Comments
        {
            get
            {
                return _comments.AsReadOnly();
            }
        }
        private readonly List<string> _images;
        public IReadOnlyList<string> Images
        {
            get
            {
                return _images.AsReadOnly();
            }
        }
        private readonly List<PostReaction> _reactions;
        public IReadOnlyList<PostReaction> Reactions
        {
            get
            {
                return _reactions.AsReadOnly();
            }
        }
        #pragma warning disable CS8618
        protected Post() { }
        #pragma warning restore CS8618
        public Post(IAvatar author, string body, List<string>? imagesLinks = null)
        {
            Author = author;
            Body = body;
            PublishedDate = DateTime.UtcNow;
            _comments = [];
            if(imagesLinks != null) _images = imagesLinks;
            else _images = [];
            _reactions = [];
        }
        public void AddImage(string imageLink)
        {
            _images.Add(imageLink);
            LatestActivity = DateTime.UtcNow;
        }
        public void AddImages(List<string> imageLinks)
        {
            _images.AddRange(imageLinks);
            LatestActivity = DateTime.UtcNow;
        }
        public void RemoveImage(string imageLink)
        {
            _images.Remove(imageLink);
        }
        public void AddComment(User commenter, string comment)
        {
            PostComment newComment = new(commenter, this, comment);
            _comments.Add(newComment);
            LatestActivity = DateTime.UtcNow;
        }
        public void RemoveComment(PostComment comment)
        {
            _comments.Remove(comment);
        }
        /// <summary>
        /// If the reaction being passed is the same as the user's current reaction, remove the reaction. 
        /// If the user has already reacted to the post but the new reaction is a new type, remove the previous reaction and add the new reaction.
        /// </summary>
        /// <param name="user">The user reacting</param>
        /// <param name="reaction">Reaction of type <see cref="ReactionType"/></param>
        public void React(User user, ReactionType reaction)
        {
            //if the user already has a reaction of the same type, remove the reaction and return
            if(Reactions.Any(r => r.User == user && r.ReactionType == reaction))
            {
                _reactions.RemoveAll(r => r.User == user && r.ReactionType == reaction);
                return;
            }
            //If the user has already reacted to the post, remove the previous reaction
            _reactions.RemoveAll(r => r.User == user);
            _reactions.Add(new PostReaction(user, reaction));
        }
    }
}
