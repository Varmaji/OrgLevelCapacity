using CustomReport.Models.ProjectModel;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;

namespace CustomReport.Models.OrgModel
{
    public class OrgModel
    {
        public int Count { get; set; }
        public List<ProjectDetails> Value { get; set; }
        public List<string> WiTypes { get; set; }
        public orgCounts counts { get; set; }
        public List<UserDetails> OrgUsers { get; set; }
        public Dictionary<string, int> AccessLevels { get; set; }
        public OrgLevelUsers UsersByTeam { get; set; }

    }
    public class countGen
    {
        public int Count { get; set; }
    }
    public class orgCounts
    {
        public int buildDefCount { get; set; }
        public int releaseDefCount { get; set; }
        public int repoCount { get; set; }
        public int UserCount { get; set; }
        public int processCount { get; set; }
        public int WIcountOrg { get; set; }
        public int WIcountType { get; set; }
        public int ProjWIcountByType { get; set; }

    }

    public class MembersMod
    {
        public List<Member> members { get; set; }
        public string continuationToken { get; set; }
        public int totalCount { get; set; }
    }
    public class Member
    {
        public string id { get; set; }

    }

    //

    public class User
    {
        public string subjectKind { get; set; }
        public string directoryAlias { get; set; }
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
    public class Members
    {
        public string id { get; set; }
        public User user { get; set; }
        public AccessLevel accessLevel { get; set; }
        public DateTime lastAccessedDate { get; set; }
        public string LastAccessDate { get; set; }
        public DateTime dateCreated { get; set; }
        public string DateCreated { get; set; }

    }

    public class UsersModel
    {
        public IList<Members> members { get; set; }
        public string continuationToken { get; set; }
        public int totalCount { get; set; }
    }
    public class UserDetails
    {
        public Members user { get; set; }
        public List<ProjectDetails> Projects { get; set; }
        public int capacity { get; set; }
        /*public List<Group> Groups { get; set; }*/
    }
  
    public class GroupList
    {
        public int count { get; set; }
        public List<Group> value { get; set; }
    }

    public class Group
    {
        public string principalName { get; set; }
        public string originId { get; set; }
        public string displayName { get; set; }
        public string descriptor { get; set; }
    }

}