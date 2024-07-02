namespace PluginDemocracy.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;   
        public string Message { get; set; } = string.Empty;
        public bool Read { get; set; } = false; 
        public DateTime Date { get; set; } = DateTime.UtcNow;
        protected Notification() { }
        public Notification(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
