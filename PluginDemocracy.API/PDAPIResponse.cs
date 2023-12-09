using PluginDemocracy.API.Models;

namespace PluginDemocracy.API
{
    public class PDAPIResponse
    {
        public UserDto? User;
        public List<Message> Messages;
        public string? RedirectTo;
        public Dictionary<string, string> RedirectParameters;
        public PDAPIResponse()
        {
            Messages = new();
            RedirectParameters = new();
        }
        public class Message
        {
            //Normal, Info, Success, Warning, Error
            SeverityLevel Severity;
            string Body;
            public Message(SeverityLevel severity, string body)
            {
                Severity = severity;
                Body = body;
            }
        }
        public enum SeverityLevel
        {
            Normal,
            Info,
            Success,
            Warning,
            Error
        }
    }
}
