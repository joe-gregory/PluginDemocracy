using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.Models.Tests
{
    public static class UserFactory
    {
        public static User GenerateUser()
        {
            return new User(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now, new CultureInfo("en-US"));
        }
    }
}
