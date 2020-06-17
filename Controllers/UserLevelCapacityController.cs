using CustomReport.Models;
using CustomReport.Models.OrgModel;
using CustomReport.Models.ProjectModel;
using DisplayUsersWebApp.Models.AccessDetails;
using DisplayUsersWebApp.Models.AccountsResponse;
using DisplayUsersWebApp.Models.ProfileDetails;
using DisplayUsersWebApp.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrgLevelTeamCapacity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace DisplayUsersWebApp.Controllers
{

    public class UserLevelCapacityController : Controller
    {

        public AccountService service = new AccountService();
        readonly string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
        string BaseURLvsrm = ConfigurationManager.AppSettings["BaseURLvsrm"];
        string version = ConfigurationManager.AppSettings["ApiVersion"];
        string version1 = "5.1-preview";
        // GET: UserLevelCapacity
        public ActionResult Index()
        {
            if (Session["visited"] == null)
                return RedirectToAction("../Account/Verify");

            if (Session["PAT"] == null)
            {
                try
                {
                    AccessDetails _accessDetails = new AccessDetails();
                    AccountsResponse.AccountList accountList = null;
                    string code = Session["PAT"] == null ? Request.QueryString["code"] : Session["PAT"].ToString();
                    string redirectUrl = ConfigurationManager.AppSettings["RedirectUri"];
                    string clientId = ConfigurationManager.AppSettings["ClientSecret"];
                    string accessRequestBody = string.Empty;
                    accessRequestBody = service.GenerateRequestPostData(clientId, code, redirectUrl);
                    _accessDetails = service.GetAccessToken(accessRequestBody);
                    ProfileDetails profile = service.GetProfile(_accessDetails);
                    if (!string.IsNullOrEmpty(_accessDetails.access_token))
                    {
                        Session["PAT"] = _accessDetails.access_token;
                        if (profile.displayName != null || profile.emailAddress != null)
                        {
                            Session["User"] = profile.displayName ?? string.Empty;
                            Session["Email"] = profile.emailAddress ?? profile.displayName.ToLower();
                        }
                    }
                    accountList = service.GetAccounts(profile.id, _accessDetails);
                    Session["AccountList"] = accountList;
                    string pat = Session["PAT"].ToString();
                    List<SelectListItem> OrganizationList = new List<SelectListItem>();
                    foreach (AccountsResponse.Value i in accountList.value)
                    {
                        OrganizationList.Add(new SelectListItem { Text = i.accountName, Value = i.accountName });
                    }
                    ViewBag.OrganizationList = OrganizationList;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return View();
        }

        public JsonResult UserLevelCapacity(string organ)
        {
            OrgModel org = new OrgModel();
            OrgLevelUsers orgLevelUsers = new OrgLevelUsers();
            orgLevelUsers.OrgUsersByTeam = new Dictionary<string, orgUserByTeam>();

            try
            {
                var req = new APIRequest(Session["PAT"].ToString());
                string url;
                string response;
                string url1;

                List<string> MemberCount = new List<string>();
                url = BaseURL + organ + "/_apis/projects?api-version=" + version1;
                response = req.ApiRequest(url);
                org = JsonConvert.DeserializeObject<OrgModel>(response);
                TeamDetails teamDetail = new TeamDetails();


                org.OrgUsers = new List<UserDetails>();
                foreach (var project in org.Value)
                {
                    List<IterationDetails> ListIterateDetails = new List<IterationDetails>();
                    ProjectDetails projectDet;//= new ProjectDetails() { Id = project.Id, Name = project.Name, Groups = new List<Group>() };

                    url = "https://vssps.dev.azure.com/" + organ + "/_apis/graph/descriptors/" + project.Id + "?api-version=" + version1 + ".1";
                    response = req.ApiRequest(url);
                    JObject jobj = JObject.Parse(response);
                    url = "https://vssps.dev.azure.com/" + organ + "/_apis/graph/groups?scopeDescriptor=" + jobj["value"] + "&api-version=" + version1;
                    response = req.ApiRequest(url);
                    GroupList Grp = JsonConvert.DeserializeObject<GroupList>(response);
                    foreach (var group in Grp.value)
                    {
                        url = "https://vsaex.dev.azure.com/" + organ + "/_apis/GroupEntitlements/" + group.originId + "/members?api-version=" + version1 + ".1";
                        response = req.ApiRequest(url);
                        UsersModel MembersList = JsonConvert.DeserializeObject<UsersModel>(response);
                        if (MembersList.members != null)
                        {
                            if (MembersList.members.Count > 0)
                            {
                                foreach (var member in MembersList.members)
                                {
                                    if (!MemberCount.Contains(member.id))
                                    {
                                        MemberCount.Add(member.id);
                                        org.OrgUsers.Add(new UserDetails { user = member, Projects = new List<ProjectDetails>() });

                                    }
                                    foreach (var usr in org.OrgUsers)
                                    {
                                        if (member.id == usr.user.id)
                                        {
                                            projectDet = new ProjectDetails() { Id = project.Id, Name = project.Name, Groups = project.Groups };
                                            var Projexist = false;
                                            foreach (var proj in usr.Projects)
                                            {
                                                if (proj.Id == projectDet.Id)
                                                {
                                                    Projexist = true;
                                                    continue;
                                                }
                                            }
                                            if (Projexist != true)
                                                usr.Projects.Add(projectDet);
                                        }
                                    }

                                }

                            }
                        }
                    }
                    url1 = "https://dev.azure.com/" + organ + "/_apis/projects/" + project.Name + "/teams?api-version=5.1";
                    response = req.ApiRequest(url1);
                    teamDetail = JsonConvert.DeserializeObject<TeamDetails>(response);
                    CapacityDetails capacitydetails = new CustomReport.Models.CapacityDetails();
                    if (teamDetail.value != null && teamDetail.value.Count > 0)
                    {
                        IterationDetails iterationDetails = new IterationDetails();

                        string teamName = "";
                        
                        foreach (var team in teamDetail.value)
                        {
                            teamName = team.name;

                            string url2 = "https://dev.azure.com/" + organ + "/" + project.Name + "/" + teamName + "/_apis/work/teamsettings/iterations?api-version=5.1";
                            response = req.ApiRequest(url2);
                            iterationDetails = JsonConvert.DeserializeObject<IterationDetails>(response);
                            iterationDetails.teamname = teamName;
                            ListIterateDetails.Add(iterationDetails);
                        }
                        //    capacity = GetTeamCapacityDetails(orgVal, ProjName, ListIterateDetails);
                    }
                    foreach (var ListIterations in ListIterateDetails)
                    {
                        foreach (var item in ListIterations.value)
                        {
                            if (item.attributes.timeFrame == "current")
                            {

                                string url3 = "https://dev.azure.com/" + organ + "/" + project.Name + "/" + ListIterations.teamname + "/_apis/work/teamsettings/iterations/" + item.id + "/capacities?api-version=5.1";
                                response=req.ApiRequest(url3);
                                capacitydetails=JsonConvert.DeserializeObject<CustomReport.Models.CapacityDetails>(response);
                                foreach(var userCap in capacitydetails.value)
                                {
                                   if(!orgLevelUsers.OrgUsersByTeam.ContainsKey(userCap.teamMember.uniqueName))
                                    {
                                       

                                        orgLevelUsers.OrgUsersByTeam.Add(userCap.teamMember.uniqueName, new orgUserByTeam() { name = userCap.teamMember.displayName, id = userCap.teamMember.id, mailID = userCap.teamMember.uniqueName, UserProjects = new Dictionary<string, userProject>() });
                                    }
                                   
                                    orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].capacity +=float.Parse(userCap.activities[0].capacityPerDay);
                                   
                                    if (!orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects.ContainsKey(project.Name))
                                    {
                                        orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects.Add(project.Name, new userProject() { name = project.Name, ProjectTeams = new Dictionary<string, userTeam>() });
                                    }
                                        orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects[project.Name].capacity += float.Parse(userCap.activities[0].capacityPerDay);
                                    

                                   if (!orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects[project.Name].ProjectTeams.ContainsKey(ListIterations.teamname))
                                    {
                                        orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects[project.Name].ProjectTeams.Add(ListIterations.teamname, new userTeam() { name = ListIterations.teamname});
                                    }

                                    orgLevelUsers.OrgUsersByTeam[userCap.teamMember.uniqueName].UserProjects[project.Name].ProjectTeams[ListIterations.teamname].capacity += float.Parse(userCap.activities[0].capacityPerDay);

                                }
                            }
                        }
                    }



                }

                org.UsersByTeam = orgLevelUsers;


            }
            catch (Exception ex)
            {

            }


            return Json(org, JsonRequestBehavior.AllowGet);
        }
    }
}