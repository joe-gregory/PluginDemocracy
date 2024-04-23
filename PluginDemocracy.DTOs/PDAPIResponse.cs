namespace PluginDemocracy.DTOs
{
    public class PDAPIResponse
    {
        #region PROPERTIES
        public UserDto? User { get; set; }
        public List<Alert> Alerts { get; set; }
        public DateTime LastRefreshed { get; set; } 
        public string? RedirectTo { get; set; }
        public Dictionary<string, string> RedirectParameters { get; set; }
        public string? SessionJWT { get; set; }
        public bool? LogOut { get; set; }
        public List<CommunityDto> AllCommunities { get; set; } = [];
        public CommunityDto? Community { get; set; }
        public List<PostDto> Posts { get; set; } = [];
        #endregion
        #region METHODS
        /// <summary>
        /// Constructor Method. Initializes Alerts and RedirectParameters.
        /// </summary>
        public PDAPIResponse()
        {
            Alerts = [];
            RedirectParameters = [];
            LastRefreshed = DateTime.UtcNow;
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
        #endregion
        #region ALERT CLASS AND ENUM
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
        #endregion
    }
}
