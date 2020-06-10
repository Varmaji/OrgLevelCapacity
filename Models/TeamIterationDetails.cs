using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrgLevelTeamCapacity.Models
{

    public class TeamIterationDetails
    {
        public string teamname { get; set; }
        public int count { get; set; }
        public List<IterationDetails> value { get; set; }

    }
       public class IterationDetails
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public Attributes attributes { get; set; }
        public string url { get; set; }

    }

    public class Attributes
    {
        public DateTime? startDate { get; set; }
        public DateTime? finishDate { get; set; }
        public string timeFrame { get; set; }

    }

}