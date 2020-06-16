using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgLevelTeamCapacity.Models
{
    public class TeamDetails
    {
        public List<TeamParam> value { get; set; }
        public int count { get; set; }

    }
    public class TeamParam
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string identityUrl { get; set; }
        public string projectName { get; set; }
        public string projectId { get; set; }
        public int capacity { get; set; }
    }
   
}