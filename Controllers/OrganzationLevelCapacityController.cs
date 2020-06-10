using CustomReport.Models;
using DisplayUsersWebApp.Models.AccessDetails;
using DisplayUsersWebApp.Models.AccountsResponse;
using DisplayUsersWebApp.Models.ProfileDetails;
using Newtonsoft.Json;
using OrgLevelTeamCapacity.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using IterationDetails = CustomReport.Models.IterationDetails;
using DisplayUsersWebApp.Services;

namespace OrgLevelTeamCapacity.Controllers
{
    public class OrganzationLevelCapacityController : Controller
    {
      
        TeamCapacity capacity = new TeamCapacity();


        public AccountService service = new AccountService();
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
                    foreach (var i in accountList.value)
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

        public JsonResult ProjectTeamIterationLevel(string orgVal)
        {
            ProjectModel project = new ProjectModel();
            
            string responseBody = "";
            capacity.projectCapacityModel = new Dictionary<string, List<CapacityDetails>>();
            capacity.projectCapacity = new Dictionary<string, int>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", Session["PAT"]))));

                    using (HttpResponseMessage response = client.GetAsync("https://dev.azure.com/" + orgVal + "/_apis/projects?api-version=5.1").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        responseBody = response.Content.ReadAsStringAsync().Result;
                        project = JsonConvert.DeserializeObject<ProjectModel>(responseBody);
                    }
                }
                if (project.value != null && project.value.Count > 0)
                {
                    foreach (var Projectitem in project.value)
                    {
                        TeamDetails teamDetail = new TeamDetails();
                        string ProjName = Projectitem.name;
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(
                                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                                        string.Format("{0}:{1}", "", Session["PAT"]))));
                            string url = "https://dev.azure.com/" + orgVal + "/_apis/projects/" + ProjName + "/teams?api-version=5.1";
                            using (HttpResponseMessage response = client.GetAsync(url).Result)
                            {
                                response.EnsureSuccessStatusCode();
                                responseBody = response.Content.ReadAsStringAsync().Result;
                                teamDetail = JsonConvert.DeserializeObject<TeamDetails>(responseBody);

                            }

                            if (teamDetail.value != null && teamDetail.value.Count > 0)
                            {

                                IterationDetails iterationDetails = new IterationDetails();
                                string teamName = "";
                                List<IterationDetails> ListIterateDetails = new List<IterationDetails>();
                                // Dictionary<string, List<TeamIterationDetails>> TeamIterationValues = new Dictionary<string, List<TeamIterationDetails>>();
                                foreach (var team in teamDetail.value)
                                {
                                    teamName = team.name;
                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        client1.DefaultRequestHeaders.Accept.Add(
                                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                        client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                                    string.Format("{0}:{1}", "", Session["PAT"]))));
                                        string url1 = "https://dev.azure.com/" + orgVal + "/" + ProjName + "/" + teamName + "/_apis/work/teamsettings/iterations?api-version=5.1";
                                        using (HttpResponseMessage response = client.GetAsync(url1).Result)
                                        {
                                            response.EnsureSuccessStatusCode();
                                            responseBody = response.Content.ReadAsStringAsync().Result;
                                            iterationDetails = JsonConvert.DeserializeObject<IterationDetails>(responseBody);
                                            iterationDetails.teamname = teamName;
                                            ListIterateDetails.Add(iterationDetails);
                                           
                                        }
                                    }

                                }                                
                               capacity= GetTeamCapacityDetails(orgVal, ProjName, ListIterateDetails);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(capacity,JsonRequestBehavior.AllowGet);
        }

        public TeamCapacity GetTeamCapacityDetails(string org, string project, List<IterationDetails> iterations)
        {
            string responseBody = "";
            capacity.CapByTeamMember = new List<CapByTeamMember>();
            capacity.currentTeamCapacities = new List<CurrentTeamCapacity>();
            capacity.totalTeamCapacities = new List<TotalTeamCapacity>();
            capacity.capacitybyTeamMembers = new List<CapacitybyTeamMember>();
            capacity.leavesbyTeamMembers = new Dictionary<string, List<LeavesbyTeamMember>>();
            List<CustomReport.Models.CapacityDetails> capacityList = new List<CapacityDetails>();
            capacity.TotalLeavesperMember = new Dictionary<string, int>();
            List<string> AddedTeamMember = new List<string>();
            CapacityCalculator capacityCalculator = new CapacityCalculator(); 


            foreach (var ListIterations in iterations)
            {
                foreach (var item in ListIterations.value)
                {
                    if (item.attributes.timeFrame == "current")
                    {

                        CapacityDetails capacitydetails = new CustomReport.Models.CapacityDetails();
                        string teamname = ListIterations.teamname;
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(
                                  new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                                        string.Format("{0}:{1}", "", Session["PAT"]))));
                            string url = "https://dev.azure.com/" + org + "/" + project + "/" + teamname + "/_apis/work/teamsettings/iterations/" + item.id + "/capacities?api-version=5.1";
                            using (HttpResponseMessage response = client.GetAsync(url).Result)
                            {
                                response.EnsureSuccessStatusCode();
                                responseBody = response.Content.ReadAsStringAsync().Result;
                                capacitydetails = JsonConvert.DeserializeObject<CapacityDetails>(responseBody);
                                capacitydetails.teamName = teamname;
                                capacitydetails.IterationPath = item.path;
                                capacityList.Add(capacitydetails);
                            }
                        }
                    }
                }

            }
            var totalCapacity = 0.0;
            var individualTeamCapacity = 0.0;
            Dictionary<string, int> TeamWiseTotalCapacity = new Dictionary<string, int>();
            foreach (var i in capacityList)
            {
                individualTeamCapacity = 0;
                foreach (var j in i.value)
                {
                    
                    if (j.activities.Count > 0)
                    {
                        float.TryParse(j.activities[0].capacityPerDay, out float val);
                        totalCapacity +=val ;
                        individualTeamCapacity += val;

                    }

                }
                TeamWiseTotalCapacity.Add(i.teamName, (int)individualTeamCapacity);
            }
            capacity.ProjectTeamCapacity.Add(project, TeamWiseTotalCapacity);
            capacity.projectCapacity.Add(project,(int)totalCapacity);
            capacity.projectCapacityModel.Add(project, capacityList);
            
            
            return capacity;
        }
         public static double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }

    }
}