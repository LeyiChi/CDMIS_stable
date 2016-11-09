using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Data;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class PatientHomeController : Controller
    {
        #region <" 私有变量 ">
        public static DataTable PatientAlertInfoList = new DataTable(); //WF PatientAlertInfoList
        static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
        static string _ErrorMSG { get; set; }
        #endregion

        
        // GET: /PatientInfo/

        //首页-基本信息和tab
        //public ActionResult Index(string UserId)
        //{
        //    //UserId = "P4444";
        //    _PatientId = UserId;
        //    ServiceReference.PatientBasicInfo zn = _ServicesSoapClient.GetPatBasicInfo(UserId);  //获取基本信息

        //    PatientBasicInfoViewModel ei = new PatientBasicInfoViewModel();
        //    ei.PatientBasicInfo.UserId = zn.UserId;
        //    ei.PatientBasicInfo.UserName = zn.UserName;
        //    ei.PatientBasicInfo.Gender = zn.Gender;
        //    ei.PatientBasicInfo.Age = Convert.ToInt32(zn.Age);
        //    ei.PatientBasicInfo.BloodType = zn.BloodType;
        //    ei.PatientBasicInfo.Module = zn.Module;
        //    ei.PatientBasicInfo.AlertNumber = _ServicesSoapClient.GetUntreatedAlertAmount(UserId);   //获取警报数

        //    //CDMIS.Models.PatientBasicInfo zz = new CDMIS.Models.PatientBasicInfo();
        //    //ei.UserId= zn.UserId;
        //    //ei.UserName=zn.UserName;
        //    //ei.Gender=zn.Gender;
        //    //ei.Age=zn.Age;
        //    //ei.BloodType=zn.BloodType;
        //    //ei.Module=zn.Module;
        //    //ei.AlertNumber=_ServicesSoapClient.GetUntreatedAlertAmount(UserId);
        //    return View(ei);
        //}

        //public ActionResult PatientBasicInfo()
        //{
        //    //UserId = "P4444";
        //    var user = Session["CurrentUser"] as UserAndRole;
        //    _PatientId = user.UserId;
        //    ServiceReference.PatientBasicInfo zn = _ServicesSoapClient.GetPatBasicInfo(_PatientId);  //获取基本信息

        //    PatientBasicInfoViewModel ei = new PatientBasicInfoViewModel();
        //    ei.PatientBasicInfo.UserId = zn.UserId;
        //    ei.PatientBasicInfo.UserName = zn.UserName;
        //    ei.PatientBasicInfo.Gender = zn.GenderText;
        //    ei.PatientBasicInfo.Age = Convert.ToInt32(zn.Age);
        //    ei.PatientBasicInfo.BloodType = zn.BloodTypeText;
        //    ei.PatientBasicInfo.Module = zn.Module;
        //    ei.PatientBasicInfo.AlertNumber = _ServicesSoapClient.GetUntreatedAlertAmount(_PatientId);   //获取警报数

        //    //CDMIS.Models.PatientBasicInfo zz = new CDMIS.Models.PatientBasicInfo();
        //    //ei.UserId= zn.UserId;
        //    //ei.UserName=zn.UserName;
        //    //ei.Gender=zn.Gender;
        //    //ei.Age=zn.Age;
        //    //ei.BloodType=zn.BloodType;
        //    //ei.Module=zn.Module;
        //    //ei.AlertNumber=_ServicesSoapClient.GetUntreatedAlertAmount(UserId);
        //    return View(ei);
        //}

        #region 详细信息
        //个人信息（不可编辑）
        public ActionResult PatientDetailInfo(string UserId, string Category)
        {
            //UserId = "P4444";
            if (UserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                UserId = user.UserId;
            }
            PatientDetailInfoViewModel pbiModel = new PatientDetailInfoViewModel();
            pbiModel.UserId = UserId;
            List<ModuleInfo> ModuleInfo = new List<Models.ModuleInfo>();
            DataSet ModulesInfo = _ServicesSoapClient.GetModulesBoughtByPId(UserId);
            foreach (DataTable item in ModulesInfo.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    if (Convert.ToInt32(row[0].ToString().Substring(1)) < 4)
                    {
                        ModuleInfo NewLine = new Models.ModuleInfo();
                        NewLine.Category = row[0].ToString();
                        NewLine.ModuleName = row[1].ToString();
                        ModuleInfo.Add(NewLine);
                    }
                }
            }
            pbiModel.ModuleBoughtInfo = ModuleInfo;
            pbiModel.ModuleDetailList = PDCHPFunctions.GetPatientDetailInfo(_ServicesSoapClient, UserId, Category);
            return View(pbiModel);
        }

        //个人信息（可编辑）
        public ActionResult PatientDetailInfoEdit(string UserId)
        {
            PatientDetailInfoViewModel ei = new PatientDetailInfoViewModel();
            ei.UserId = UserId;
            ei.PatientDetailInfo = PDCHPFunctions.GetPatientDetailInfoEdit(_ServicesSoapClient, UserId, UserId);
            return View(ei);
        }


        //个人信息（编辑提交）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "PatientDetailInfoEdit")]
        public ActionResult PatientDetailInfoEdit(PatientDetailInfoViewModel ei, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string PatientId = ei.UserId;
            bool flag = false;

            DataSet set = _ServicesSoapClient.GetPatBasicInfoDtlList(PatientId);  //获取关注的详细信息

            if (set.Tables.Count > 0)
            {
                foreach (DataTable ta in set.Tables)
                {
                    foreach (System.Data.DataRow row in ta.Rows)
                    {
                        if ((row[3].ToString() != "InvalidFlag") && (row[3].ToString() != "Doctor") && (row[4].ToString() != "伴随疾病"))
                        {
                            string CategoryCode = row[1].ToString();  //主键  
                            string ItemCode = row[3].ToString();     //主键 
                            int ItemSeq = Convert.ToInt32(row[10]);   //主键 

                            string Description = row[9].ToString();
                            int SortNo = Convert.ToInt32(row[10]);

                            string Value = Request.Form[row[3].ToString()];   //只更改了Value

                            //插入数据
                            flag = _ServicesSoapClient.SetPatBasicInfoDetail(PatientId, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

                            if (flag == false)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            //flag为TRUE即全部插成功，FALSE不知道哪里断掉。。。
            if (flag == true)
            {
                return RedirectToAction("PatientDetailInfo", "PatientHome", new { UserId = ei.UserId }); //成功跳转至详细
                //return Content("ok");
            }
            else
            {
                //return Content("<script >alert('提交失败！');</script >", "text/html");
                Response.Write("<script>alert('部分插入失败');</script>");   //失败返回编辑，提示失败
                return View(ei);
                //return Content("fai");
            }

        }

        //个人信息（编辑-取消）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EditCancel")]
        public ActionResult EditCancel(PatientDetailInfoViewModel ei)
        {
            //string PatientId = ei.UserId;
            return RedirectToAction("PatientDetailInfo", "PatientHome", new { UserId = ei.UserId }); //跳转至详细
        }
        #endregion

        #region 临床信息
        //临床-时间轴
        public ActionResult ClinicInfo(string UserId)
        {
            //AdmissionDate ClinicDate Num
            if (UserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                UserId = user.UserId;
            }

            CDMIS.ServiceReference.Clinic Clinic = _ServicesSoapClient.GetClinicalNew(UserId, Convert.ToDateTime("1897-01-01 00:00:00"), Convert.ToDateTime("1897-01-01 00:00:00"), 10);

            //都放在了webservice

            return View(Clinic);
        }

        //临床-时间轴-继续加载
        public JsonResult GetMoreClinic(string PatientId, DateTime AdmissionDate, DateTime ClinicDate, int Num)
        {
            var res = new JsonResult();

            CDMIS.ServiceReference.Clinic Clinic = _ServicesSoapClient.GetClinicalNew(PatientId, AdmissionDate, ClinicDate, Num);

            res.Data = Clinic;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //临床-大类信息
        public ActionResult ClinicInfoDetail(string PatientId, string keycode)
        {
            //初始化
            ClinicInfoDetailViewModel ei = new ClinicInfoDetailViewModel();
            PDCHPFunctions.GetClinicInfoDetail(_ServicesSoapClient, ref ei, PatientId, keycode);
            return View(ei);
        }

        //临床-大类详细信息
        public ActionResult ClinicInfoDetailByType(string PatientId, string vid, string type, string sortno, string itemcode)
        {

            ClinicInfoDetailByTypeViewModel ei = new ClinicInfoDetailByTypeViewModel();

            PDCHPFunctions.GetClinicInfoDetailByType(_ServicesSoapClient, ref ei, PatientId, vid, type, sortno, itemcode);

            return View(ei);
        }
        #endregion

        #region 健康参数
        //健康参数，初始只添加关键主键，另外min/max/单位/code/name，后通过JsonResult获取图和相关参数，注意下拉框是根据不同病患加载
        public ActionResult HealthParameters(string UserId)
        {
            //UserId = "P4444";
            if (UserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                UserId = user.UserId;
                Session["PatientId"] = user.UserId;
            }
            else
            {
                Session["PatientId"] = UserId;
            }
            DataSet set = _ServicesSoapClient.GetPatient2VS(UserId);  //获取患者生理参数列表

            List<SelectListItem> sli = new List<SelectListItem>();
            foreach (System.Data.DataRow row in set.Tables[0].Rows)
            {
                sli.Add(new SelectListItem { Text = row["VitalSignsTypeName"].ToString() + "：" + row["VitalSignsName"].ToString(), Value = row[2].ToString() });
            }

            HealthParametersViewModel ei = new HealthParametersViewModel();
            ei.UserId = UserId;
            ei.VitalSignList = sli;//下拉框

            return View(ei);
        }

        //健康参数-获取某生理参数图的生理数据  太慢放webservice
        public JsonResult GetPicture(string PatientId, string Itemcode)
        {
            var res = new JsonResult();

            DataSet set = new DataSet();

            string[] s = Itemcode.Split(new char[] { '_' });
            string ItemType = s[0];

            set = _ServicesSoapClient.GetPatientVitalSignsAndThreshold(PatientId, ItemType, Itemcode);

            //set包含：RecordDate、RecordTime、Value、Unit、ThreholdMin、ThreholdMax

            List<Picture> PictureList = new List<Picture>();
            if (set != null)
            {
                foreach (System.Data.DataRow row in set.Tables[0].Rows)
                {
                    Picture Picture = new Picture();
                    Picture.date = row[0].ToString() + " " + row[1].ToString();
                    Picture.duration = Convert.ToDecimal(row[2]);
                    Picture.unit = row[3].ToString();
                    Picture.min = Convert.ToDecimal(row[4]);
                    Picture.max = Convert.ToDecimal(row[5]);

                    if ((Picture.duration >= Picture.min) && (Picture.duration <= Picture.max))
                    {
                        Picture.lineColor = "#b7e021";  //正常黄色
                    }
                    else
                    {
                        Picture.lineColor = "#FF2D2D";  //不正常红色
                    }

                    PictureList.Add(Picture);
                    //Picture.date = "2014-12-12 08:00";
                    //Picture.duration=101m;
                    //Picture.min =80m;
                }
            }
            res.Data = PictureList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //健康参数-获取某生理参数的单位等
        public JsonResult GetUnit(string Itemcode)
        {
            var res = new JsonResult();

            string[] s = Itemcode.Split(new char[] { '_' });
            string ItemType = s[0];
            VitalSigns VitalSign = new VitalSigns();

            VitalSign = _ServicesSoapClient.GetDataByTC(ItemType, Itemcode);

            res.Data = VitalSign;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //set参数图的生理数据  
        public JsonResult AddVitalSign(string UserId, string ItemCode, string Value, string Unit, string CompoundTime)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();

            string[] s = ItemCode.Split(new char[] { '_' });
            string ItemType = s[0];

            string CompoundTime1 = CompoundTime.Replace("时", "");
            string CompoundTime2 = CompoundTime1.Replace("-", "");  //时间字符串处理

            string CompoundTime3 = CompoundTime2.Replace(" ", "");
            int RecordDate = Convert.ToInt32(CompoundTime3.Substring(0, 8));  //日期
            int RecordTime = Convert.ToInt32(CompoundTime3.Substring(8, 2)); //时（两位）

            bool flag = _ServicesSoapClient.SetPatientVitalSigns(UserId, RecordDate, RecordTime, ItemType, ItemCode, Value, Unit, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            int AlertType = 5;
            WnMstAlert NowThreshold = _ServicesSoapClient.GetWnMstAlert(UserId, ItemCode, RecordDate);

            if (Convert.ToDecimal(Value) <= Convert.ToDecimal(NowThreshold.Min))
            {
                AlertType = 2;  //低于下限
            }
            if (Convert.ToDecimal(Value) >= Convert.ToDecimal(NowThreshold.Max))
            {
                AlertType = 1;  //超越上限
            }
            DateTime AlertDateTime = Convert.ToDateTime(CompoundTime1 + ":00");
            flag = _ServicesSoapClient.SetTrnAlertRecord(UserId, ItemCode, AlertDateTime, AlertType, 3, 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            int AlertNumber = -1;
            AlertNumber = _ServicesSoapClient.GetUntreatedAlertAmount(UserId);   //获取警报数
            res.Data = AlertNumber;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region 治疗方案
        // GET: /PatientHome/TreatmentPlan
        public ActionResult TreatmentPlan(string piUserId)
        {
            if (piUserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                piUserId = user.UserId;
            }
            TreatmentListViewModel TreatmentListData = new TreatmentListViewModel();
            TreatmentListData.UserId = piUserId;//TreatmentListData.UserId = piUserId;
            OtherCs.TreatFunctions.GetTreatmentList(_ServicesSoapClient, string.Empty, piUserId, TreatmentListData.TreatmentList);

            return View(TreatmentListData);
        }
        #endregion

        #region 症状管理
        // GET: /PatientHome/SymptomsManagement/
        public ActionResult SymptomsManagement(string piUserId)
        {
            if (piUserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                piUserId = user.UserId;
            }
            SymptomsViewModel SymptomsViewData = new SymptomsViewModel();
            SymptomsViewData.UserId = piUserId;
            SymptomsViewData.PId = piUserId;
            SymptomsViewData.SymptomsTypeSelected = "0";
            SymptomsViewData.SymptomsNameSelected = "0";
            SymptomsViewData.RecordTime = null;
            OtherCs.SYFunctions.GetSymptomsList(_ServicesSoapClient, ref SymptomsViewData);
            return View(SymptomsViewData);
        }

        // POST: /PatientHome/SymptomsManagement/
        [HttpPost]
        public ActionResult SymptomsManagement(SymptomsViewModel piSymptomsViewData)
        {
            if (ModelState.IsValid)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                DateTime? Time = piSymptomsViewData.RecordTime;
                //取数据库时间
                int RecordDate = 0;
                int RecordTime = 0;
                OtherCs.SYFunctions.SetRecordDateTime(_ServicesSoapClient, Time, ref RecordDate, ref RecordTime);
                //往数据库插数据
                bool SetSymptomsInfoFlag = OtherCs.SYFunctions.SetSymptomsInfo(_ServicesSoapClient, piSymptomsViewData, RecordDate, RecordTime, user);
                if (SetSymptomsInfoFlag)
                {
                    return RedirectToAction("SymptomsManagement");
                }
                else
                {

                }
            }

            return View(piSymptomsViewData);
        }

        //详细信息表根据SymptomsType动态加载GetSymptomsNameList下拉框
        public JsonResult GetListbySymptomsType(string SymptomsTypeSelected)
        {
            var res = new JsonResult();
            List<string> SymptomsNameList = new List<string>();
            if (SymptomsTypeSelected != "0")
            {
                DataSet SymptomsNameDS = _ServicesSoapClient.GetSymptomsNameList(SymptomsTypeSelected);
                DataTable SymptomsNameDT = SymptomsNameDS.Tables[0];

                foreach (DataRow DR in SymptomsNameDT.Rows)
                {
                    SymptomsNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = SymptomsNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //删除数据
        public JsonResult DeleteSymptomsInfo(string PId, string VId, string SyNo)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.DeleteSymptomsInfo(PId, VId, Convert.ToInt32(SyNo));
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region 警报记录
        // GET: /PatientHome/PatientAlert/
        public ActionResult PatientAlert(string piUserId)
        {
            if (piUserId == null)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                piUserId = user.UserId;
            }
            PatientAlertViewModel PatientAlertViewData = new PatientAlertViewModel();
            PatientAlertViewData.UserId = piUserId;//TreatmentListData.UserId = piUserId;
            OtherCs.TrnFunctions.GetTrnList(_ServicesSoapClient, ref PatientAlertInfoList, ref PatientAlertViewData, false);
            return View(PatientAlertViewData);
        }

        #endregion

        #region 每日任务
        public ActionResult EverydayTask(string UserId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            if (UserId == null)
            { 
                UserId = user.UserId;
            }
            string PatientId = UserId;
            Session["PatientId"] = UserId;
            EverydayTaskViewModel et = new EverydayTaskViewModel();
            et.PatientId = PatientId;
            ETFunctions.GetReminderList(_ServicesSoapClient, ref et, user.UserId);
            et.StartDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            et.ReminderNo = "0";
            et.CreatedBy = "";
            ViewBag.ErrorMSG = _ErrorMSG;
            _ErrorMSG = "";
            return View(et);
        }

        [HttpPost]
        //[HandleError(View = "EverydayTask", ExceptionType = typeof(Exception))]
        public ActionResult EverydayTask(EverydayTaskViewModel et, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string _PatientId = Session["PatientId"] as String;
            if (ModelState.IsValid)
            {
                bool flag = false;
                int type = 0;
                if (et.ReminderNo == "0")
                {
                    type = 1;
                }
                switch (et.AlertModeSelected)
                {
                    case "0": //没有选择提醒方式
                        flag = _ServicesSoapClient.SetReminder(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                et.Content, et.StartDateTime, user.UserId, et.CreatedBy, user.TerminalName, user.TerminalIP, user.DeviceType);
                        break;
                    case "1"://一次 
                        flag = _ServicesSoapClient.SetReminder_Once(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, et.OnceDateTime, et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        break;
                    case "2"://每天
                        for (int i = 0; i <= et.EveryDayNumber; i++)
                        {
                            if (Request.Form["EveryDayTime" + i] != null && Request.Form["EveryDayTime" + i] != "")
                            {
                                flag = _ServicesSoapClient.SetReminder_Everyday(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                        et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, Request.Form["EveryDayTime" + i], et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                                type = 1;
                                et.CreatedBy = user.UserId;
                            }
                        }
                        break;
                    case "3": //每周
                        for (int i = 0; i <= et.WeeklyNumber; i++)
                        {
                            if (Request.Form["WeeklyTime" + i] != null && Request.Form["WeeklyTime" + i] != "")
                            {
                                flag = _ServicesSoapClient.SetReminder_Weekly(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                        et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, Request.Form["WeeklyWeek" + i], Request.Form["WeeklyTime" + i], et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                                type = 1;
                                et.CreatedBy = user.UserId;
                            }
                        }
                        break;
                    case "4": //每月
                        for (int i = 0; i <= et.MonthlyNumber; i++)
                        {
                            if (Request.Form["MonthlyTime" + i] != null && Request.Form["MonthlyTime" + i] != "")
                            {                               
                                flag = _ServicesSoapClient.SetReminder_Monthly(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected), 
                                        et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, Request.Form["MonthlyDay" + i], Request.Form["MonthlyTime" + i], et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                                type = 1;
                                et.CreatedBy = user.UserId;
                            }
                        }
                        break;
                    case "5": //每年
                        DateTime annualStartDT = DateTime.Parse(et.StartDateTime);
                        for (int i = 0; i <=  et.AnnualNumber; i++)
                        {
                            if (Request.Form["AnnualTime" + i] != null && Request.Form["AnnualTime" + i] != "")
                            {
                                flag = _ServicesSoapClient.SetReminder_Annual(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                        et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, Request.Form["AnnualMonth" + i], Request.Form["AnnualDay" + i], Request.Form["AnnualTime" + i], et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                                type = 1;
                                et.CreatedBy = user.UserId;
                            }
                        }
                        break;
                    case "6": //间隔
                        flag = _ServicesSoapClient.SetReminder_Interval(type, _PatientId, et.ReminderNo, Convert.ToInt32(et.ReminderTypeSelected),
                                et.Content, Convert.ToInt32(et.AlertModeSelected), et.StartDateTime, et.FreqYear, et.FreqMonth, et.FreqDay, et.FreqHour, et.FreqMunite, et.CreatedBy, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        break;
                    default: break;

                }
                if (!flag)
                {
                    _ErrorMSG = "数据插入失败，请重试!";
                }
                return RedirectToAction("EverydayTask");
            }
            else
            {
                ETFunctions.GetReminderList(_ServicesSoapClient, ref et, user.UserId);
            }
            return View(et);
        }

        public JsonResult DeleteReminder(string pid, string reminderNo, string Creater)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            var flag = _ServicesSoapClient.DeleteReminder(pid, reminderNo, Creater, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public ActionResult TaskList(string PatientId)
        {
            if (PatientId == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            else
            {
                Session["PatientId"] = PatientId;
            }
            var user = Session["CurrentUser"] as UserAndRole;
            TaskListViewModel tl = new TaskListViewModel();
            tl.PatientId = PatientId;
            tl.Type = 0;
            TLFunctions.GetTaskList(_ServicesSoapClient, ref tl);
            return View(tl);
        }

        public JsonResult UpdateIsDone(string pid, string reminderNo, string taskDate, string taskTime)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            var flag = _ServicesSoapClient.UpdateIsDone(pid, reminderNo, Convert.ToInt32(taskDate), Convert.ToInt32(taskTime), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region OtherFunctions
        //下拉框生成（改变选中值）
        public List<SelectListItem> GetTypeList(string Type, string Value)
        {
            DataSet typeset = _ServicesSoapClient.GetTypeList(Type);   //字典表

            List<SelectListItem> sli = new List<SelectListItem>();
            foreach (System.Data.DataRow typerow in typeset.Tables[0].Rows)
            {
                sli.Add(new SelectListItem { Text = typerow[1].ToString(), Value = typerow[0].ToString() });
            }

            if (Value != "")
            {
                foreach (var item in sli)
                {
                    if (Value == item.Value)
                    {
                        item.Selected = true;
                    }
                }
            }
            return sli;
        }
        #endregion

    }


}
