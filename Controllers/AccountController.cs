using DisplayUsersWebApp.Models.AccountsResponse;
using DisplayUsersWebApp.Models.LoginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DisplayUsersWebApp.Services;
using OrgLevelTeamCapacity.Models;

namespace CustomReport.Controllers
{
    public class AccountController : Controller
    {
        public AccountService service = new AccountService();
 
        public ActionResult Verify(LoginModel model) => View(model);
        public ActionResult Index()
        {
            try
            {
                Session["visited"] = "1";
                string url = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state=User1&scope={1}&redirect_uri={2}";
                string redirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
                string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
                string AppScope = System.Configuration.ConfigurationManager.AppSettings["appScope"];
                url = string.Format(url, clientId, AppScope, redirectUrl);
                return Redirect(url);
            }
            catch (Exception)
            {
                //logger.Debug(JsonConvert.SerializeObject(ex, Formatting.Indented) + Environment.NewLine);
            }
            return RedirectToAction("Verify", "Account");
        }
        public JsonResult AccountList()
        {
            AccountsResponse.AccountList accountList = new AccountsResponse.AccountList();
            if (Session["AccountList"] != null)
            {
                accountList = (AccountsResponse.AccountList)Session["AccountList"];
            }
            return Json(accountList.value, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProjectList(string ORG)
        {
            var pm = service.GetApi<ProjectModel>("https://dev.azure.com/" + ORG + "/_apis/projects?api-version=5.1");
            return Json(pm.value, JsonRequestBehavior.AllowGet);
        }

       

        
        public ActionResult SignOut()
        {
            Session.Clear();
            return Redirect("https://app.vssps.visualstudio.com/_signout");
        }
    }
}