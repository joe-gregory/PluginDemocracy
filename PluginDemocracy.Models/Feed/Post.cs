﻿namespace PluginDemocracy.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User? Author { get; private set; }
        public string? Body { get; private set; }
        public DateTime PublishedDate { get; } = DateTime.UtcNow;
        public readonly List<PostComment> Comments;
        public DateTime? LatestActivity { get; private set; } = null;
        public List<string> Images { get; set; }
        public List<PostReaction> Reactions { get; set; }
        protected Post() 
        {
            Comments = [];
            Images = [];
            Reactions = [];
        }
        public Post(User user, string body, List<string>? imagesLinks = null)
        {
            Author = user;
            Body = body;
            Comments = [];
            if(imagesLinks != null) Images = imagesLinks;
            else Images = [];
            Reactions = [];
        }
        public void AddComment(PostComment comment)
        {
            Comments.Add(comment);
            LatestActivity = DateTime.UtcNow;
        }
        public void React(User user, ReactionType reaction)
        {
            //if the user already has a reaction of the same type, remove the reaction and return
            if(Reactions.Any(r => r.User == user && r.ReactionType == reaction))
            {
                Reactions.RemoveAll(r => r.User == user && r.ReactionType == reaction);
                return;
            }
            //If the user has already reacted to the post, remove the previous reaction
            Reactions.RemoveAll(r => r.User == user);
            Reactions.Add(new PostReaction(user, reaction));
        }
    }
}
