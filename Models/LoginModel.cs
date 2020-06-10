using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayUsersWebApp.Models.LoginModel
{
    public class LoginModel
    {
        public string AccountName { get; set; }

        public string PAT { get; set; }

        public string Message { get; set; }

        public string Event { get; set; }

        public string name { get; set; }

        public string EnableExtractor { get; set; }
        public string TemplateURL { get; set; }
    }
}