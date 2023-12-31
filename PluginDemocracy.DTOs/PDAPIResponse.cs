namespace PluginDemocracy.DTOs
{
    public class PDAPIResponse
    {
        public UserDto? User { get; set; }
        public bool LoggedIn { get {  return User != null; } }
        public List<Alert> Alerts { get; set; }
        public string? RedirectTo { get; set; }
        public Dictionary<string, string> RedirectParameters { get; set; }
        public PDAPIResponse()
        {
            Alerts = new();
            RedirectParameters = new();
        }
        public void AddAlert(string severity, string message)
        {
            if (Enum.TryParse(severity, true, out Severity severityLevel))
            {
                Alert messageToAdd = new();
                messageToAdd.Severity = severityLevel;
                messageToAdd.Message = message;
                Alerts.Add(messageToAdd);
            }
            else throw new Exception("Unable to add message to PDAPIResponse.Messages using AddMessage method. Likely Severity provided with typos.");
        }
        public class Alert
        {
            //Normal, Info, Success, Warning, Error
            public Severity Severity { get; set; }
            public string? Message { get; set; }
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
