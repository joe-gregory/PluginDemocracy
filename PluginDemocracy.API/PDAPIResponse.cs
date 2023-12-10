using PluginDemocracy.API.Models;

namespace PluginDemocracy.API
{
    public class PDAPIResponse
    {
        public UserDto? User;
        public List<Alert> Alerts;
        public string? RedirectTo;
        public Dictionary<string, string> RedirectParameters;
        public PDAPIResponse()
        {
            Alerts = new();
            RedirectParameters = new();
        }
        public void AddAlert(string severity, string message)
        {
            if (Enum.TryParse(severity, true, out Severity severityLevel))
            {
                Alert messageToAdd = new(severityLevel, message);
                Alerts.Add(messageToAdd);
            }
            else throw new Exception("Unable to add message to PDAPIResponse.Messages using AddMessage method. Likely Severity provided with typos.");
        }
        public class Alert
        {
            //Normal, Info, Success, Warning, Error
            public Severity Severity;
            public string Message;
            public Alert(Severity severity, string body)
            {
                Severity = severity;
                Message = body;
            }
        }
        public enum Severity
        {
            Normal,
            Info,
            Success,
            Warning,
            Error
        }
    }
}
