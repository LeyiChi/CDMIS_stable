using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CDMIS.CommonLibrary;

namespace CDMIS
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // 参数默认值
            );

        }

        protected void Application_Start()
        {
            //注册 log4net
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Config\\log4net.config"));

            //在应用程序启动时运行的代码        
            //初始日志的配置
            LoggerHelper.SetConfig();        

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

        
        }

        //void Application_Error(object sender, EventArgs e)
        //{
        //    //在出现未处理的错误时运行的代码
        //    Exception objExp = HttpContext.Current.Server.GetLastError();
        //    string username = "";
        //    string userid = "";
        //    if (Session["ulogin"] != null)
        //    {
        //        string[] uinfo = Session["ulogin"].ToString().Split('|');
        //        userid = uinfo[0];
        //        username = uinfo[1];
        //    }
        //    Aotain114.Public.LogHelper.WriteLog("\r\n用户ID:" + userid + "\r\n用户名:" + username + "\r\n客户机IP:" + Request.UserHostAddress + "\r\n错误地址:" + Request.Url + "\r\n异常信息:" + Server.GetLastError().Message, objExp);

        //} 
    }
}