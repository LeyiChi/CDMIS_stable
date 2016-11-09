using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.ViewModels;
using CDMIS.Models;
using System.Data;
using CDMIS.ServiceReference;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;


namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class PatientInfoController : Controller
    {
        #region <" 私有变量 ">
        public static DataTable PatientAlertInfoList = new DataTable();//WF PatientAlertInfoList
        public static PatientAlertViewModel PatientAlertViewData = new PatientAlertViewModel();//WF PatientAlertInfoList
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();  //webservice统一变量
        static string _ErrorMSG { get; set; }
        #endregion


        //首页-基本信息和tab
        public ActionResult Index(string PatientId, string TabNo)
        {
            //PatientId = "P4444";
            Session["PatientId"] = PatientId;
            ServiceReference.PatientBasicInfo pbi = _ServicesSoapClient.GetPatBasicInfo(PatientId);  //获取基本信息

            PatientBasicInfoViewModel pbiModel = new PatientBasicInfoViewModel();
            pbiModel.PatientBasicInfo.UserId = pbi.UserId;
            pbiModel.PatientBasicInfo.UserName = pbi.UserName;
            pbiModel.PatientBasicInfo.Gender = pbi.Gender;
            pbiModel.PatientBasicInfo.Age = Convert.ToInt32(pbi.Age);
            pbiModel.PatientBasicInfo.BloodType = pbi.BloodType;
            pbiModel.PatientBasicInfo.Module = pbi.Module;
            pbiModel.PatientBasicInfo.AlertNumber = _ServicesSoapClient.GetUntreatedAlertAmount(PatientId);   //获取警报数
            ViewBag.TabNo = TabNo;
            return View(pbiModel);
        }

        public ActionResult PatientBasicInfo(string PointedPatient)
        {
            //PatientId = "P4444";
            string PatientId = PointedPatient;
            if (PointedPatient == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            ServiceReference.PatientBasicInfo pbi = _ServicesSoapClient.GetPatBasicInfo(PatientId);  //获取基本信息

            PatientBasicInfoViewModel pbiModel = new PatientBasicInfoViewModel();
            pbiModel.PatientBasicInfo.UserId = pbi.UserId;
            pbiModel.PatientBasicInfo.UserName = pbi.UserName;
            pbiModel.PatientBasicInfo.Gender = pbi.GenderText;
            pbiModel.PatientBasicInfo.Age = Convert.ToInt32(pbi.Age);
            pbiModel.PatientBasicInfo.BloodType = pbi.BloodTypeText;
            pbiModel.PatientBasicInfo.InsuranceType = pbi.InsuranceTypeText;
            pbiModel.PatientBasicInfo.Module = pbi.Module;
            pbiModel.PatientBasicInfo.AlertNumber = _ServicesSoapClient.GetUntreatedAlertAmount(PatientId);   //获取警报数
            return View(pbiModel);
        }

        #region 详细信息
        //个人信息（不可编辑）
        public ActionResult PatientDetailInfo(string PatientId, string Category)
        {
            //PatientId = "P4444";
            if (PatientId == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            else
            {
                Session["PatientId"] = PatientId;
            }
            PatientDetailInfoViewModel pbiModel = new PatientDetailInfoViewModel();
            pbiModel.UserId = PatientId;
            List<ModuleInfo> ModuleInfo = new List<Models.ModuleInfo>();
            DataSet ModulesInfo = _ServicesSoapClient.GetModulesBoughtByPId(PatientId);
            foreach (DataTable item in ModulesInfo.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    ModuleInfo NewLine = new Models.ModuleInfo();
                    NewLine.Category = row[0].ToString();
                    NewLine.ModuleName = row[1].ToString();
                    ModuleInfo.Add(NewLine);
                }
            }
            pbiModel.ModuleBoughtInfo = ModuleInfo;
            pbiModel.ModuleDetailList = PDCHPFunctions.GetPatientDetailInfo(_ServicesSoapClient, PatientId, Category);
            return View(pbiModel);
        }

        //保存编辑的信息
        [HttpPost]
        public ActionResult PatientDetailInfo(PatientDetailInfoViewModel model)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string Category = Request.Form["ModuleDetailList[0].CategoryCode"];
            List<ModuleInfo> ModuleInfo = new List<Models.ModuleInfo>();
            DataSet ModulesInfo = _ServicesSoapClient.GetModulesBoughtByPId(model.UserId);
            foreach (DataTable item in ModulesInfo.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    ModuleInfo NewLine = new Models.ModuleInfo();
                    NewLine.Category = row[0].ToString();
                    NewLine.ModuleName = row[1].ToString();
                    ModuleInfo.Add(NewLine);
                }
            }
            model.ModuleBoughtInfo = ModuleInfo;
            List<PatientDetailInfo> ItemInfo = PDCHPFunctions.GetPatientDetailInfo(_ServicesSoapClient, model.UserId, Category);
            bool flag = false;
            foreach (PatientDetailInfo Row in ItemInfo)
            {
                if ((Row.ItemCode != "InvalidFlag") && (Row.ItemCode != "Doctor"))
                {
                    string CategoryCode = "M";  //主键  
                    string Value = Request.Form[Row.ItemCode];   //只更改了Value
                    if (Row.ControlType != "7")
                    {
                        flag = _ServicesSoapClient.SetPatBasicInfoDetail(model.UserId, CategoryCode, Row.ItemCode, Row.ItemSeq, Value, Row.Description, Row.ItemSeq, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    }
                    if (flag == false)
                    {
                        break;
                    }
                }
            }
            model.ModuleDetailList = PDCHPFunctions.GetPatientDetailInfo(_ServicesSoapClient, model.UserId, Category);
            return View(model);
        }

        //个人信息（可编辑）（现在用不着了）
        public ActionResult PatientDetailInfoEdit(string PatientId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            PatientDetailInfoViewModel pbiModel = new PatientDetailInfoViewModel();

            pbiModel.PatientDetailInfo = PDCHPFunctions.GetPatientDetailInfoEdit(_ServicesSoapClient, PatientId, DoctorId);
            pbiModel.UserId = PatientId;
            return View(pbiModel);
        }

        //个人信息（编辑提交）（现在用不着了）
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
                    int DeleteFlag = 0;
                    string CategoryCode = ta.Rows[0][1].ToString();
                    while (DeleteFlag == 0)
                    {
                        DeleteFlag = _ServicesSoapClient.DeleteModuleDetail(PatientId, CategoryCode);
                    }
                    foreach (System.Data.DataRow row in ta.Rows)
                    {
                        if ((row[3].ToString() != "InvalidFlag") && (row[3].ToString() != "Doctor") && (row[4].ToString() != "伴随疾病"))
                        {
                            CategoryCode = row[1].ToString();  //主键  
                            string ItemCode = row[3].ToString();     //主键 
                            int ItemSeq = Convert.ToInt32(row[10]);   //主键 

                            string Description = row[9].ToString();
                            int SortNo = Convert.ToInt32(row[10]);

                            string Value = Request.Form[row[3].ToString()];   //只更改了Value
                            string ControlType = row[11].ToString();

                            //插入数据
                            if (ControlType != "7")
                            {
                                flag = _ServicesSoapClient.SetPatBasicInfoDetail(PatientId, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                            }
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
                return RedirectToAction("PatientDetailInfo", "PatientInfo", new { PatientId = ei.UserId }); //成功跳转至详细
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

        //个人信息（编辑-取消）（现在用不着了）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EditCancel")]
        public ActionResult EditCancel(PatientDetailInfoViewModel ei, FormCollection formCollection)
        {
            string PatientId = ei.UserId;
            return RedirectToAction("PatientDetailInfo", "PatientInfo", new { PatientId = ei.UserId }); //跳转至详细
        }
        #endregion

        #region 临床信息

        //同步临床信息 生成医院下拉框
        public JsonResult GetHospitalList()
        {
            var res = new JsonResult();
            List<string> HospitalList = new List<string>();

            DataSet HospitalDs = _ServicesSoapClient.GetHospitalList();
            if (HospitalDs != null)
            {
                DataTable HospitalDT = HospitalDs.Tables[0];

                foreach (DataRow DR in HospitalDT.Rows)
                {
                    HospitalList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());                
                }
            }
            res.Data = HospitalList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //临床-时间轴
        public ActionResult ClinicInfo(string PatientId)
        {
            //PatientId = "P4444";
            if (PatientId == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            else
            {
                Session["PatientId"] = PatientId;
            }
            //AdmissionDate ClinicDate Num
            CDMIS.ServiceReference.Clinic Clinic = _ServicesSoapClient.GetClinicalNew(PatientId, Convert.ToDateTime("1897-01-01 00:00:00"), Convert.ToDateTime("1897-01-01 00:00:00"), 10);
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
        public ActionResult HealthParameters(string PatientId)
        {
            //UserId = "P4444";
            if (PatientId == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            else
            {
                Session["PatientId"] = PatientId;
            }
            string UserId = Session["PatientId"] as string;
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

        //健康参数-获取某生理参数图的生理数据
        public JsonResult GetPicture(string PatientId, string Itemcode)
        {
            var res = new JsonResult();
            res.Data = PDCHPFunctions.GetPictureList(_ServicesSoapClient, PatientId, Itemcode);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //健康参数-获取某生理参数图的推荐阈值
        public JsonResult GetBasicAlert(string ItemCode)
        {
            var res = new JsonResult();

            //string temp = DateTime.Now.ToString("yyyyMMdd");
            //int today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

            WnMstAlert WnMstAlert = _ServicesSoapClient.GetWnMstBasicAlert(ItemCode);

            //WnMstAlert来自webservice类，Min、Max、Units、Remarks

            res.Data = WnMstAlert;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //健康参数-设置阈值 
        public JsonResult SetAlert(string PatientId, string ItemCode, string Min, string Max, string Unit, string StartTime, string EndTime, string Remarks)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();

            string StartDate = StartTime.Replace("-", "");  //日期字符串处理
            string EndDate = EndTime.Replace("-", "");

            int SortNo = _ServicesSoapClient.GetMPAMaxSortNo(PatientId, ItemCode);
            //int SortNo = _ServicesSoapClient.GetMPAMaxSortNo(UserId, ItemCode)

            bool flag = _ServicesSoapClient.SetWnMstPersonalAlert(PatientId, ItemCode, SortNo + 1, Convert.ToDecimal(Min), Convert.ToDecimal(Max), Unit, Convert.ToInt32(StartDate), Convert.ToInt32(EndDate), Remarks, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region  健康计划
        //数据操作主要在js上
        public ActionResult HealthPlan(string PatientId)
        {
            //UserId = "P4444";
            if (PatientId == null)
            {
                PatientId = Session["PatientId"] as String;
            }
            else
            {
                Session["PatientId"] = PatientId;
            }
            string UserId = Session["PatientId"] as string;

            HealthPlanViewModel ei = new HealthPlanViewModel();
            ei.UserId = UserId;

            return View(ei);
        }

        //健康参数-获取某生理参数图的生理数据
        public JsonResult GetImplementationForWebFirst(string PatientId, string Module)
        {
            var res = new JsonResult();
            res.Data = _ServicesSoapClient.GetImplementationForWebFirst(PatientId, Module);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult GetImplementationForWebSecond(string PatientId, string PlanNo)
        {
            var res = new JsonResult();
            res.Data = _ServicesSoapClient.GetImplementationForWebSecond(PatientId, PlanNo);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult GetSignInfoByCodeWeb(string PatientId, string PlanNo, string ItemCode, int StartDate, int EndDate)
        {
            var res = new JsonResult();
            res.Data = _ServicesSoapClient.GetSignInfoByCodeWeb(PatientId, PlanNo, ItemCode, StartDate, EndDate);
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region 症状管理
        // GET: /PatientInfo/SymptomsManagement/
        public ActionResult SymptomsManagement(string PatientId)
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
            SymptomsViewModel SymptomsViewData = new SymptomsViewModel();
            SymptomsViewData.UserId = user.UserId;
            SymptomsViewData.PId = PatientId;
            SymptomsViewData.SymptomsTypeSelected = "0";
            SymptomsViewData.SymptomsNameSelected = "0";
            SymptomsViewData.RecordTime = null;
            OtherCs.SYFunctions.GetSymptomsList(_ServicesSoapClient, ref SymptomsViewData);
            return View(SymptomsViewData);
        }

        // POST: /PatientInfo/SymptomsManagement/
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

        #region 治疗方案
        // GET: /PatientInfo/TreatmentPlan/
        public ActionResult TreatmentPlan(string PatientId)
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
            TreatmentViewModel TreatmentData = new TreatmentViewModel();
            TreatmentData.UserId = user.UserId;
            TreatmentData.PId = PatientId;
            OtherCs.TreatFunctions.GetTreatmentList(_ServicesSoapClient, TreatmentData.UserId, PatientId, TreatmentData.TreatmentList);

            return View(TreatmentData);
        }

        // POST: /PatientInfo/TreatmentPlan/
        [HttpPost]
        public ActionResult TreatmentPlan(TreatmentViewModel piTreatmentViewData)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            if (ModelState.IsValid)
            {

                //往数据库插数据
                //
                bool SetTreatmentInfoFlag = OtherCs.TreatFunctions.SetTreatmentInfo(_ServicesSoapClient, piTreatmentViewData, user);
                if (SetTreatmentInfoFlag)
                {
                    return RedirectToAction("TreatmentPlan");
                }
                else
                {

                }
            }

            return View(piTreatmentViewData);
        }

        public JsonResult DeleteTreatmentInfo(string PId, string SortNo)
        {
            var res = new JsonResult();
            var flag = 2;
            flag = _ServicesSoapClient.DeleteTreatmentInfo(PId, Convert.ToInt32(SortNo));
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region 警报记录
        // GET: /PatientInfo/PatientAlert/
        public ActionResult PatientAlert(string PatientId)
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

            PatientAlertViewData.UserId = PatientId;
            PatientAlertViewData.DoctorId = user.UserId;
            PatientAlertInfoList.Clear();
            PatientAlertInfoList = new DataTable();
            OtherCs.TrnFunctions.GetTrnList(_ServicesSoapClient, ref PatientAlertInfoList, ref PatientAlertViewData, false);
            return View(PatientAlertViewData);
        }

        //警报列表（查看）
        [HttpPost]
        public ActionResult PatientAlert(PatientAlertViewModel AlertViewData)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            PatientAlertViewData.AlertStatusSelected = AlertViewData.AlertStatusSelected;
            OtherCs.TrnFunctions.GetTrnList(_ServicesSoapClient, ref PatientAlertInfoList, ref AlertViewData, true);
            return View(AlertViewData);
        }

        public JsonResult SetTrnProcessFlag(string PId, string SortNo)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            var flag = false;
            flag = _ServicesSoapClient.SetTrnProcessFlag(PId, Convert.ToInt32(SortNo), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region 每日任务
        public ActionResult EverydayTask(string PatientId)
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