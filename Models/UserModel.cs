using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayUsersWebApp.Models
{
    public class User
    {
        public string subjectKind { get; set; }
        public string metaType { get; set; }
        public string domain { get; set; }
        public string principalName { get; set; }
        public string mailAddress { get; set; }
        public string origin { get; set; }
        public string originId { get; set; }
        public string displayName { get; set; }        
        public string url { get; set; }
        public string descriptor { get; set; }

    }
    public class AccessLevel
    {
        public string licensingSource { get; set; }
        public string accountLicenseType { get; set; }
        public string msdnLicenseType { get; set; }
        public string licenseDisplayName { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string assignmentSource { get; set; }

    }
    public class Value
    {
        public string id { get; set; }
        public User user { get; set; }
        public AccessLevel accessLevel { get; set; }
        public DateTime lastAccessedDate { get; set; }
        public string LastAccessDate { get; set; }
        public DateTime dateCreated { get; set; }
        public string DateCreated { get; set; }
        public bool AccesedRecently { get; set; }

    }
    public class UserModel
    {
        public int count { get; set; }
        public IList<Value> value { get; set; }
        public Dictionary<string ,int> AccessLevels { get; set; }

    }
}