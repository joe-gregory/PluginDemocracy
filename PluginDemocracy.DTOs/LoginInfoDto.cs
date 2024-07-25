using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class LoginInfoDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public CultureInfo Culture { get => new(CultureCode); set => CultureCode = value.Name; }
        public string CultureCode { get; set; } = "en-US";
    }
}
