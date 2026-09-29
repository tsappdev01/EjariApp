using System;
using System.Collections.Generic;
using System.Text;

namespace DIP.Settings
{
    public class SmtpSettingDto
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string DefaultFromAddress { get; set; }
        public string DefaultFromDisplayName { get; set; }

    }
}
