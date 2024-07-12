namespace PluginDemocracy.Models
{
    public class Message
    {
        public int Id { get; init; }
        public User? Sender { get; init; }
        public DateTime Date { get; init; }
        /// <summary>
        /// The actual message content.
        /// </summary>
        public string Content { get; init; }
        /// <summary>
        /// Private constructor for the benefit of EFC
        /// </summary>
        private Message()
        {
            Content = string.Empty;
        }
        public Message(User author, string content)
        {
            Sender = author;
            Date = DateTime.UtcNow;
            Content = content;
        }
    }
}
