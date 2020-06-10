using System.Collections.Generic;


namespace CustomReport.Models
{
    public class TeamCapacity
    {
        public List<CurrentTeamCapacity> currentTeamCapacities { get; set; }
        public List<TotalTeamCapacity> totalTeamCapacities { get; set; }
        public List<CapacitybyTeamMember> capacitybyTeamMembers { get; set; }
        public Dictionary<string, List<LeavesbyTeamMember>> leavesbyTeamMembers { get; set; }
        public List<CapByTeamMember> CapByTeamMember { get; set; }
        public Dictionary<string, int> TotalLeavesperMember { get; set; }
        public List<CapacityOfTeam> TotalCapacityOfTeam { get; set; }

        public Dictionary<string, List<CapacityDetails>> projectCapacityModel;

        public Dictionary<string, int> projectCapacity;

        public Dictionary<string, Dictionary<string, int>> ProjectTeamCapacity = new Dictionary<string, Dictionary<string, int>>();
    }

    public class CurrentTeamCapacity
    {
        public string iterationPath { get; set; }
        public string teamName { get; set; }
        public string currentCapacity { get; set; }
        public string currentWorkingDays { get; set; }
    }
    public class TotalTeamCapacity
    {
        public string iterationPath { get; set; }
        public string teamName { get; set; }
        public string totalCapacity { get; set; }
        public string iterationStart { get; set; }
        public string iterationEnd { get; set; }
        public string totalWorkingDays { get; set; }
    }

    public class CapacityOfTeam
    {
        public string TotolCapacityperTeam { get; set; }
        public string teamName { get; set; }

    }

    public class CapacityCalculator
    {
        public string capacityPerDay { get; set; }
        public string iterationStart { get; set; }
        public string iterationEnd { get; set; }
    }
    public class CapacitybyTeamMember
    {
        public string teamMember { get; set; }
        public string capacityPerDay { get; set; }
        public string iterationPath { get; set; }
        public string teamName { get; set; }
        public string iterationStart { get; set; }
        public string iterationEnd { get; set; }
    }

    public class CapByTeamMember
    {
        public Dictionary<string, string> TeamCapacityPerDay { get; set; }
        public double UserTotalCapacity { get; set; }
        public string teamMember { get; set; }
        public string iterationPath { get; set; }
        public string iterationStart { get; set; }
        public string iterationEnd { get; set; }
    }
    public class LeavesbyTeamMember
    {
        public string teamMember { get; set; }
        public int NoOfdaysLeave { get; set; }
        public string iterationPath { get; set; }
        public string teamName { get; set; }
        public int TotalLeaves { get; set; }
        public List<DaysOffLeaves> TotalDaysOff { get; set; }
    }

    public class DaysOffLeaves
    {
        public string LeaveFrom { get; set; }
        public string LeaveTo { get; set; }
        public int NoOfDaysDayOff { get; set; }

    }
    public class CapacityDetails
    {
        public string teamName { get; set; }
        public string IterationPath { get; set; }
        public int count { get; set; }
        public List<Capacity> value { get; set; }
    }

    public class TeamTotalCapacity
    {
        public string teamName { get; set; }
        public int totalteamcap { get; set; }
    }
    public class Capacity
    {
        public TeamMember teamMember { get; set; }
        public List<Activities> activities { get; set; }
        public List<Leaves> daysOff { get; set; }
        public string url { get; set; }
    }

    public class Leaves
    {
        public string start { get; set; }
        public string end { get; set; }
    }
    public class TeamMember
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }
    public class Links
    {
        public Avatar avatar { get; set; }
    }
    public class Avatar
    {
        public string href { get; set; }
    }

    public class Activities
    {
        public string capacityPerDay { get; set; }
        public string name { get; set; }
    }
}