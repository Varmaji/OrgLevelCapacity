using System.Collections.Generic;

namespace CustomReport.Models
{
    public class IterationDetails
    {
        public string teamname { get; set; }
        public int count { get; set; }
        public List<Iterations> value { get; set; }
    }
    public class Iterations
    {
        public string team { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public Attributes attributes { get; set; }
        public string url { get; set; }
    }
    public class Attributes
    {
        public string startDate { get; set; }
        public string finishDate { get; set; }
        public string timeFrame { get; set; }
    }
}