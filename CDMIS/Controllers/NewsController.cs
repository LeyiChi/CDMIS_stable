using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class NewsController : Controller
    {
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();//WebService
        string hostAddress = "";
        string hostport = "";
        //
        // GET: /News/
        #region <ActionResult>
        public ActionResult Index(string Module)
        {
            HealthEducationList HElist = new HealthEducationList();
            if (Module != null)
                HElist.selectedModuleId = Module;
            HElist.ModuleList = new List<SelectListItem>();
            DataSet ModuleInfo = _ServicesSoapClient.GetMstTaskByParentCode("TD0000");
            foreach (DataRow Row in ModuleInfo.Tables[0].Rows)
            {
                SelectListItem NewLine = new SelectListItem();
                NewLine.Value = Row[1].ToString();
                NewLine.Text = Row[2].ToString() + "模块";
                HElist.ModuleList.Add(NewLine);
            }
            if (HElist.selectedModuleId == "")
            {
                HElist.selectedModuleId = "TD0001";
            }
            DataSet info = _ServicesSoapClient.GetMstTaskByParentCode(HElist.selectedModuleId);
            SelectListItem SelectedModule = HElist.ModuleList.Find(
                delegate(SelectListItem x)
                {
                    return x.Value == HElist.selectedModuleId;
                });
            foreach (DataRow row in info.Tables[0].Rows)
            {
                HealthEducation news = new HealthEducation();
                news.Module = HElist.selectedModuleId;
                news.ModuleName = SelectedModule.Text;
                news.Id = row[1].ToString();
                news.Path = row[9].ToString();
                news.Title = row[2].ToString();
                news.CreateDateTime = row[10].ToString();
                news.Author = row[11].ToString();
                news.AuthorName = row[12].ToString();
                HElist.HEList.Add(news);
            }
            return View(HElist);
        }

        public ActionResult Create()
        {
            NewHealthEducationFile nhe = new NewHealthEducationFile();
            nhe.selectedModuleId = "TD0001";
            nhe.ModuleList = new List<SelectListItem>();
            DataSet ModuleInfo = _ServicesSoapClient.GetMstTaskByParentCode("TD0000");
            foreach (DataRow Row in ModuleInfo.Tables[0].Rows)
            {
                SelectListItem NewLine = new SelectListItem();
                NewLine.Value = Row[1].ToString();
                NewLine.Text = Row[2].ToString() + "模块";
                nhe.ModuleList.Add(NewLine);
            }
            return View(nhe);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(NewHealthEducationFile newhe)
        {
            if (ModelState.IsValid)
            {
                string dir = Server.MapPath("/");
                var user = Session["CurrentUser"] as UserAndRole;
                string servertime = _ServicesSoapClient.GetServerTime();
                newhe.news.Path = "/HealthEducation/" + newhe.selectedModuleId + "_" + servertime.Replace(':', '_') + ".html";
                //newhe.news.Path = dir + newhe.news.FileName;
                StreamReader sr = new StreamReader(dir + "HealthEducation\\head.txt", Encoding.Default);
                string head, temp;
                head = "";
                while ((temp = sr.ReadLine()) != null)
                {
                    head = head + temp;
                }
                temp = head + newhe.news.htmlContent + "</body></html>";
                sr.Close();

                System.IO.File.WriteAllText(dir + newhe.news.Path.Substring(1).Replace("/", "\\"), temp, Encoding.GetEncoding("UTF-8"));
                newhe.news.Author = user.UserId;
                //

                //保存数据
                hostAddress = Request.ServerVariables.Get("Local_Addr").ToString();
                if (hostAddress == "::1")
                {
                    hostAddress = "127.0.0.1";
                }
                hostport = Request.ServerVariables.Get("Server_Port").ToString();
                //newhe.news.Path = "http://" + hostAddress + ":" + hostport + "/HealthEducation/" + newhe.news.FileName;
                if (newhe.news.Title == null || newhe.news.Title == "")
                {
                    newhe.news.Title = "无主题";
                }
                int flag = _ServicesSoapClient.SetMstTask(newhe.selectedModuleId.Substring(0, 2), "", newhe.news.Title, newhe.selectedModuleId, "", -1, 99999999, 2, 1, newhe.news.Path, newhe.news.Author, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (flag == 1)
                {
                    return RedirectToAction("Index", new { Module = newhe.selectedModuleId });
                }
                else
                {
                    return View(newhe);
                }
            }
            return View(newhe);
        }

        public ActionResult Edit(string Module, string Id)
        {
            TaskDetailInfo info = _ServicesSoapClient.GetCmTaskItemInfo(Module.Substring(0, 2), Id);
            HealthEducation news = new HealthEducation();
            news.Module = Module;
            news.Id = Id;
            news.Path = info.OptionCategory;
            news.Title = info.Name;
            news.CreateDateTime = info.CreateDateTime.ToString();
            news.Author = info.Author;
            news.AuthorName = info.AuthorName;

            string dir = Server.MapPath("/");
            StreamReader sr = new StreamReader(dir + news.Path.Substring(1).Replace("/","\\"), Encoding.GetEncoding("GB2312"));

            string temp;
            news.htmlContent = "";
            if ((temp = sr.ReadLine()) != null)
            {
                Regex reg = new Regex(@"<body>([\s\S]*)</body>", RegexOptions.IgnoreCase);
                MatchCollection mc = reg.Matches(temp);
                news.htmlContent = mc[0].Value;
                news.htmlContent = news.htmlContent.Substring(6, news.htmlContent.Length - 13);
            }
            sr.Close();
            NewHealthEducationFile nhe = new NewHealthEducationFile();
            nhe.selectedModuleId = Module;
            nhe.news = news;
            nhe.ModuleList = new List<SelectListItem>();
            DataSet ModuleInfo = _ServicesSoapClient.GetMstTaskByParentCode("TD0000");
            foreach (DataRow Row in ModuleInfo.Tables[0].Rows)
            {
                SelectListItem NewLine = new SelectListItem();
                NewLine.Value = Row[1].ToString();
                NewLine.Text = Row[2].ToString() + "模块";
                nhe.ModuleList.Add(NewLine);
            }
            return View(nhe);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(NewHealthEducationFile newhe)
        {
            if (ModelState.IsValid)
            {
                string dir = Server.MapPath("/");
                var user = Session["CurrentUser"] as UserAndRole;
                string servertime = _ServicesSoapClient.GetServerTime();
                //newhe.news.FileName = newhe.selectedModuleId + "_" + servertime + ".html";
                //newhe.news.Path = dir + newhe.news.FileName;
                StreamReader sr = new StreamReader(dir + "HealthEducation\\head.txt", Encoding.Default);
                string head, temp;
                head = "";
                while ((temp = sr.ReadLine()) != null)
                {
                    head = head + temp;
                }
                temp = head + newhe.news.htmlContent + "</body></html>";
                sr.Close();

                if (System.IO.File.Exists(dir + newhe.news.Path.Substring(1).Replace("/", "\\")))
                {
                    System.IO.File.Delete(dir + newhe.news.Path.Substring(1).Replace("/", "\\"));
                }

                System.IO.File.WriteAllText(dir + newhe.news.Path.Substring(1).Replace("/", "\\"), temp, Encoding.GetEncoding("UTF-8"));
                newhe.news.Author = user.UserId;
                //
                //保存数据
                hostAddress = Request.ServerVariables.Get("Local_Addr").ToString();
                if (hostAddress == "::1")
                {
                    hostAddress = "127.0.0.1";
                }
                hostport = Request.ServerVariables.Get("Server_Port").ToString();
                //newhe.news.Path = "http://" + hostAddress + ":" + hostport + "/HealthEducation/" + newhe.news.FileName;
                if (newhe.news.Title == null || newhe.news.Title == "")
                {
                    newhe.news.Title = "无主题";
                }
                int flag = _ServicesSoapClient.SetMstTask(newhe.selectedModuleId.Substring(0, 2), newhe.news.Id, newhe.news.Title, newhe.selectedModuleId, "", -2, 99999999, 2, 1, newhe.news.Path, newhe.news.Author, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (flag == 1)
                {
                    return RedirectToAction("Index", new { Module = newhe.selectedModuleId });
                }
                else
                {
                    return View(newhe);
                }
            }
            return View(newhe);
        }

        public JsonResult DeleteHealthEducation(string Module, string Id)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteMstTask(Module.Substring(0, 2), Id);
            if (flag == 1)
            {
                res.Data = true;
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region function
        ////从Cm.MstHealthEducation表中读取所有健康教育资料的列表（倒序）
        //public DataTable GetHEFileList(string moduleId, string fileType)
        //{
        //    DataTable filedt = new DataTable();

        //    return filedt;
        //}
        #endregion
    }
}
