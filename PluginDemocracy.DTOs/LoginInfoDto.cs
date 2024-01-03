﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class LoginInfoDto : BaseMessageDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
