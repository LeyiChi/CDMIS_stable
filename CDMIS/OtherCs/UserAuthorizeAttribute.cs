using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Data;
using System.Security.Policy;
using CDMIS.ServiceReference;
using CDMIS.Models;
namespace CDMIS.OtherCs
{
    /// <summary>
    /// 自定义AuthorizeAttribute
    /// </summary>
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
       
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.Session["CurrentUser"] as UserAndRole;     
            var controller = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            //var isAllowed = this.IsAllowed(user, controller, action);
            bool AuthorityFlag = false;
            if (controller == "Home" && action == "GotoLogin")
            {
                 AuthorityFlag = true;
            }
            else
            {
                if (user != null)
                {
                    //AuthorityFlag = (bool)Cm.MstUser.GetUserAuthority(dc._clsCache, user.Name, controller, action);
                    AuthorityFlag = true;
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Account/GotoLogin?control=" + controller + "&page=" + action);
                }
            }
            if (!AuthorityFlag)
            {
                filterContext.Result = new RedirectResult("/Account/GotoLogin?control=" + controller + "&page=" + action);

            }
        }
       
    }
}