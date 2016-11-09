using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using CDMIS.ViewModels;
using CDMIS.Models;
using CDMIS.ServiceReference;
using System.Data;
using System.Collections;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;
using System.Net.Http;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class DoctorHomeController : Controller
    {
        #region <" 私有变量 ">
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
        WebReferenceJC.BsWebService regionCenterWeb = new WebReferenceJC.BsWebService();

        //public static string user.UserId = "D003";  //合并时，用user.UserId替换_UserId
        public static DataSet _DS_PatientList = new DataSet();
        public static DataSet _DS_AlertList = new DataSet();
        DataTable _DT_Module = new DataTable();
        #endregion

        #region <" 医生首页 ">
        //
        // GET: /DoctorHome/
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        public DoctorHomeController()
        {
            DataSet ds = _ServicesSoapClient.GetModuleList();
            if (ds.Tables.Count != 0)
            {
                this._DT_Module = ds.Tables[0];
            }
        }

        #region <" 患者列表 ">
        //健康专员患者列表
        public ActionResult HealthCoachPatientList(string Patient)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            PatientListViewModel patientListView = new PatientListViewModel(user.UserId);
            return PartialView(patientListView);
        }

        //患者列表
        public ActionResult PatientList(string PatientId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            PatientListViewModel patientListView = new PatientListViewModel(user.UserId);
            _DS_PatientList = GetPatientListByDoctorId(user.UserId);
            DataTable dt = new DataTable();
            if (_DS_PatientList.Tables.Count != 0)
            {
                //Default: Get Patients from Fisrt Module ZAM 2015-1-20
                dt = _DS_PatientList.Tables[0];
            }
            patientListView.AdvancedSearchEnable = "0";
            if (PatientId != null && PatientId != "")
            {
                patientListView.PatientId = PatientId;
            }

            //GetPatientsBySearchConditions()
            string patientId = patientListView.PatientId == null ? "" : patientListView.PatientId;
            string patientName = patientListView.PatientName == null ? "" : patientListView.PatientName;
            int genderType = Convert.ToInt32(patientListView.GenderSelected);
            int careLevel = Convert.ToInt32(patientListView.CareLevelSelected);
            string moduleSelected = patientListView.ModuleSelected == null ? "" : patientListView.ModuleSelected; ;
            int alertStatus = Convert.ToInt32(patientListView.AlertStatusSelected);
            patientListView.DoctorId = user.UserId;

            //从网页端筛选患者
            dt = GetPatientListByConditions(patientId, patientName, genderType, careLevel, moduleSelected, alertStatus, patientListView.DoctorId);
            patientListView.PatientList = this.InitialPatientList(dt);
            return PartialView(patientListView);
        }

        //患者列表（查看）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "PatientListSearch")]
        public ActionResult PatientListSearch(PatientListViewModel patientListView, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            DataTable dt = new DataTable();

            //GetPatientsBySearchConditions()
            string patientId = patientListView.PatientId == null ? "" : patientListView.PatientId;
            string patientName = patientListView.PatientName == null ? "" : patientListView.PatientName;
            int genderType = Convert.ToInt32(patientListView.GenderSelected);
            int careLevel = Convert.ToInt32(patientListView.CareLevelSelected);
            string moduleSelected = patientListView.ModuleSelected == null ? "" : patientListView.ModuleSelected; ;
            int alertStatus = Convert.ToInt32(patientListView.AlertStatusSelected);
            patientListView.DoctorId = user.UserId;

            //从网页端筛选患者
            dt = GetPatientListByConditions(patientId, patientName, genderType, careLevel, moduleSelected, alertStatus, patientListView.DoctorId);
            patientListView.PatientList = this.InitialPatientList(dt);
            return PartialView("PatientList", patientListView);
        }

        //患者列表（刷新）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "PatientListRefresh")]
        public ActionResult PatientListRefresh(PatientListViewModel patientListView, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;

            //Get PatientList from database
            _DS_PatientList = GetPatientListByDoctorId(user.UserId);

            DataTable dt = new DataTable();
            //GetPatientsBySearchConditions()
            string patientId = patientListView.PatientId == null ? "" : patientListView.PatientId;
            string patientName = patientListView.PatientName == null ? "" : patientListView.PatientName;
            int genderType = Convert.ToInt32(patientListView.GenderSelected);
            int careLevel = Convert.ToInt32(patientListView.CareLevelSelected);
            string moduleSelected = patientListView.ModuleSelected == null ? "" : patientListView.ModuleSelected; ;
            int alertStatus = Convert.ToInt32(patientListView.AlertStatusSelected);
            patientListView.DoctorId = user.UserId;
            //从网页端筛选患者
            dt = GetPatientListByConditions(patientId, patientName, genderType, careLevel, moduleSelected, alertStatus, patientListView.DoctorId);
            patientListView.PatientList = this.InitialPatientList(dt);

            return PartialView("PatientList", patientListView);
        }


        //修改关注等级
        public JsonResult ChangeCareLevel(string patientId, string doctorId, int carelevel, string module)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            if (carelevel < 3)
            {
                ++carelevel;
            }
            else
            {
                carelevel = 1;
            }
            bool flag = _ServicesSoapClient.ChangCareLevel(module, patientId, doctorId, carelevel, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag)
            {
                res.Data = true;
                //PatientListViewModel patientListView =new PatientListViewModel();
                //RedirectToAction("PatientListRefresh", patientListView);
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region <" 警报记录 ">
        //警报记录      
        public ActionResult AlertList()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            AlertListViewModel alertListView = new AlertListViewModel(user.UserId);
            _DS_AlertList = GetAlertListByDoctorId(user.UserId);
            ////Default: GetAlertsBySearchConditions()
            //默认显示“未处理”警报记录 ZAM 2014-12-22
            DataTable dt = new DataTable();
            if (_DS_AlertList.Tables.Count != 0)
            {
                dt = SelectAlertsfuzzy(_DS_AlertList.Tables[0], "", "", 1);
            }
            alertListView.AlertRecordList = InitialAlertList(dt);
            alertListView.AlertStatusSelected = "1";
            return PartialView(alertListView);
        }

        //警报列表（查看）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AlertListSearch")]
        public ActionResult AlertListSearch(AlertListViewModel alertListView, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            DataTable dt = new DataTable();

            //GetAlertsBySearchConditions()
            string patientId = alertListView.PatientId == null ? "" : alertListView.PatientId;
            string patientName = alertListView.PatientName == null ? "" : alertListView.PatientName;
            int processFlag = Convert.ToInt32(alertListView.AlertStatusSelected);
            alertListView.DoctorId = user.UserId;

            //DataTable dt = new DataTable();
            //从网页端筛选警报
            if (_DS_AlertList.Tables.Count != 0)
            {
                dt = SelectAlertsfuzzy(_DS_AlertList.Tables[0], patientId, patientName, processFlag);
            }
            alertListView.AlertRecordList = InitialAlertList(dt);
            return PartialView("AlertList", alertListView);
        }

        //警报列表（刷新）
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AlertListRefresh")]
        public ActionResult AlertListRefresh(AlertListViewModel alertListView, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            DataTable dt = new DataTable();
            //Get AlertList from database
            _DS_AlertList = GetAlertListByDoctorId(user.UserId);

            //GetAlertsBySearchConditions()
            string patientId = alertListView.PatientId == null ? "" : alertListView.PatientId;
            string patientName = alertListView.PatientName == null ? "" : alertListView.PatientName;
            int processFlag = Convert.ToInt32(alertListView.AlertStatusSelected);
            alertListView.DoctorId = user.UserId;

            //DataTable dt = new DataTable();
            //从网页端筛选警报
            if (_DS_AlertList.Tables.Count != 0)
            {
                dt = SelectAlertsfuzzy(_DS_AlertList.Tables[0], patientId, patientName, processFlag);
            }
            alertListView.AlertRecordList = InitialAlertList(dt);
            return PartialView("AlertList", alertListView);
        }

        //修改警报处置状态
        public JsonResult ChangeAlertStatus(string pid, string sortNo)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = false;
            if (pid != null && sortNo != null)
            {
                //SetProcessFlag()
                //flag = _ServicesSoapClient.SetProcessFlag(pid, Convert.ToInt32(sortNo), user.UserId, "", "", 1);
                flag = _ServicesSoapClient.SetProcessFlag(pid, Convert.ToInt32(sortNo), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            if (flag)
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

        #region <" 筛选患者/警报 ">

        //从医生所负责的所有患者（_DS_PatientList）中搜索符合条件的患者
        public DataTable GetPatientListByConditions(string patientId, string patientName, int genderType, int careLevel, string moduleSelected, int alertStatus, string doctorId)
        {
            DataTable Patients = new DataTable();
            //GetSelectTypeName
            string genderText = genderType != 0 ? _ServicesSoapClient.GetMstTypeName("SexType", genderType) : "";

            //select patients from DataSet _DS_PatientList
            if (moduleSelected == "")
            {

                // search from all modules(Tables)
                foreach (DataTable dt in _DS_PatientList.Tables)
                {
                    string module = dt.TableName;
                    DataTable dt_patients = SelectPatientsfuzzy(dt, patientId, patientName, genderText, careLevel, alertStatus);
                    foreach (DataRow dr in dt_patients.Rows)
                    {
                        dr["Module"] = this.GetModuleName(module);
                    }
                    Patients.Merge(dt_patients);
                }
            }
            else
            {
                //search from single module (Table)
                DataTable dt = _DS_PatientList.Tables[moduleSelected];
                Patients.Merge(SelectPatientsfuzzy(dt, patientId, patientName, genderText, careLevel, alertStatus));
            }
            return Patients;
        }

        //患者列表的模糊检索 PID Name
        public DataTable SelectPatientsfuzzy(DataTable dt, string patientId, string patientName)
        {
            DataTable dt_SelectedPatients = dt.Clone();
            //DataRow[] drArr = dt.Select("PatientId LIKE 'PID20150107%'");
            DataRow[] drArr = dt.Select("PatientId LIKE \'" + patientId + "%' AND PatientName LIKE \'%" + patientName + "%'");
            for (int i = 0; i < drArr.Length; i++)
            {
                dt_SelectedPatients.ImportRow(drArr[i]);
            }
            return dt_SelectedPatients;
        }

        public DataTable SelectPatientsfuzzy(DataTable dt, string patientId, string patientName, string genderText, int careLevel, int alertStatus)
        {
            DataTable dt_SelectedPatients = new DataTable();
            if (dt != null)
            {
                dt_SelectedPatients = dt.Clone();
            }
            //Operator LIKE is used to include only values that match a pattern with wildcards. Wildcard character is * or %.
            string filterExpression = "";
            filterExpression += "PatientId LIKE \'%" + patientId + "%'";
            filterExpression += "AND PatientName LIKE \'%" + patientName + "%'";
            if (genderText != "")
            {
                filterExpression += "AND Gender LIKE \'%" + genderText + "%'";
            }
            if (careLevel != 0)
            {
                filterExpression += "AND CareLevel = " + careLevel;
            }
            if (alertStatus != 0)
            {
                if (alertStatus == 1) //1: 未处理
                {
                    filterExpression += "AND AlertNumber > 0";
                }
                else
                {
                    filterExpression += "AND AlertNumber = 0";
                }
            }
            DataRow[] drArr = null;
            if (dt != null)
            {
                drArr = dt.Select(filterExpression);
                for (int i = 0; i < drArr.Length; i++)
                {
                    dt_SelectedPatients.ImportRow(drArr[i]);
                }
            }
            //DataRow[] drArr = dt.Select("PatientId LIKE \'" + patientId + "%' AND PatientName LIKE \'%" + patientName + "%'");
            return dt_SelectedPatients;
        }

        //警报列表的模糊检索 PID Name alertstatus
        public DataTable SelectAlertsfuzzy(DataTable dt, string patientId, string patientName, int alertStatus)
        {
            DataTable dt_SelectedPatients = dt.Clone();
            //DataRow[] drArr = dt.Select("PatientId LIKE 'PID20150107%'");
            string filterExpression = "";
            filterExpression += "PatientId LIKE \'%" + patientId + "%'";
            filterExpression += "AND PatientName LIKE \'%" + patientName + "%'";
            if (alertStatus != 0)
            {
                if (alertStatus == 1) //1: 未处理
                {
                    filterExpression += "AND processFlag = 1";
                }
                else
                {
                    filterExpression += "AND processFlag = 2";
                }
            }

            DataRow[] drArr = dt.Select(filterExpression);
            //DataRow[] drArr = dt.Select("PatientId LIKE \'" + patientId + "%' AND PatientName LIKE \'%" + patientName + "%'");
            for (int i = 0; i < drArr.Length; i++)
            {
                dt_SelectedPatients.ImportRow(drArr[i]);
            }
            return dt_SelectedPatients;
        }

        #endregion

        #region <" 初始化患者/ 初始化警报 ">

        //从数据库获取医生所负责的所有患者，按模块分Table存放
        public DataSet GetPatientListByDoctorId(string doctorId)
        {
            try
            {
                DataSet ds = _ServicesSoapClient.GetPatientsByDoctorId(doctorId, "", "Doctor");
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //初始化ViewModel中的患者列表
        public List<Models.PatientBasicInfo> InitialPatientList(DataTable Patients)
        {
            List<Models.PatientBasicInfo> PatientList = new List<Models.PatientBasicInfo>();
            //string module = Patients.TableName;
            foreach (DataRow item in Patients.Rows)
            {
                Models.PatientBasicInfo patientbasic = new Models.PatientBasicInfo();
                patientbasic.UserName = item["PatientName"].ToString();
                patientbasic.UserId = item["PatientId"].ToString();
                if (item["Age"] != null && item["Age"].ToString() != "")
                {
                    patientbasic.Age = Convert.ToInt32(item["Age"].ToString());
                }
                patientbasic.Gender = item["Gender"].ToString();
                patientbasic.Diagnosis = item["Diagnosis"].ToString();
                patientbasic.Module = item["Module"].ToString();
                patientbasic.AlertNumber = Convert.ToInt32(item["AlertNumber"]);
                patientbasic.CareLevel = Convert.ToInt32(item["CareLevel"]);
                //patientbasic.singleModule = Patients.TableName != "" ? Patients.TableName : "";
                patientbasic.singleModule = item["ModuleType"].ToString();
                PatientList.Add(patientbasic);
            }
            return PatientList;
        }

        //从数据库获取医生所负责的所有患者警报
        public DataSet GetAlertListByDoctorId(string doctorId)
        {
            try
            {
                DataSet ds = _ServicesSoapClient.GetAlertsByDoctorId(doctorId);
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //初始化ViewModel中的警报列表
        public List<AlertInfo> InitialAlertList(DataTable AlertRecords)
        {
            List<AlertInfo> AlertRecordList = new List<AlertInfo>();
            {
                foreach (DataRow item in AlertRecords.Rows)
                {
                    AlertInfo alertRecord = new AlertInfo();
                    alertRecord.UserId = item["PatientId"].ToString();
                    alertRecord.UserName = item["PatientName"].ToString();
                    alertRecord.AlertTypeName = item["AlertTypeName"].ToString();
                    alertRecord.AlertItemName = item["AlertItem"].ToString();
                    alertRecord.AlertDateTime = item["AlertDateTime"].ToString();
                    alertRecord.ProcessFlag = Convert.ToInt32(item["ProcessFlag"]);
                    alertRecord.SortNo = Convert.ToInt32(item["SortNo"]);
                    AlertRecordList.Add(alertRecord);
                }
            }
            return AlertRecordList;
        }

        #endregion

        #region function
        //获取模块名称
        public string GetModuleName(string code)
        {
            if (code == "")
            {
                return "";
            }
            string modulename = "";
            DataRow[] drArry = this._DT_Module.Select("Code LIKE \'%" + code + "%'");
            foreach (DataRow item in drArry)
            {
                modulename = item["Name"].ToString();
            }
            return modulename;
        }

        //同一页面，多个提交
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultipleButtonAttribute : ActionNameSelectorAttribute
        {
            public string Name { get; set; }
            public string Argument { get; set; }

            public override bool IsValidName(ControllerContext controllerContext, string actionName, System.Reflection.MethodInfo methodInfo)
            {
                var isValidName = false;
                var keyValue = string.Format("{0}:{1}", Name, Argument);
                var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

                if (value != null)
                {
                    controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                    isValidName = true;
                }

                return isValidName;
            }
        }
        #endregion

     

        #region 建档--个人信息
        public ActionResult BasicProfile(string PatientId, string Role)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;
            Session["PatientId"] = PatientId;

            BasicProfileViewModel model = new BasicProfileViewModel();

            if (PatientId != null)
            {
                ViewBag.OperationInvalidFlag = "false";   //手机号输入框不显示



                if (Role == "Patient")
                {
                    //加载患者基本信息               
                    ServiceReference.PatientBasicInfo basicInfo = new ServiceReference.PatientBasicInfo();
                    basicInfo = _ServicesSoapClient.GetPatBasicInfo(PatientId);
                    model.Patient.UserId = basicInfo.UserId;
                    model.Patient.UserName = basicInfo.UserName;
                    model.Patient.Birthday = basicInfo.Birthday;
                    model.Patient.Gender = basicInfo.Gender;
                    model.Patient.BloodType = basicInfo.BloodType;
                    model.Patient.InsuranceType = basicInfo.InsuranceType;

                    //加载患者详细信息
                    GetPatientInfoDetail(ref model);
                }
                else
                {
                    DataSet basicInfoDs = _ServicesSoapClient.GetDoctorInfo(PatientId);
                    if (basicInfoDs.Tables.Count > 0)
                    {
                        DataTable basicInfoDt = basicInfoDs.Tables[0];
                        model.Patient.UserId = basicInfoDt.Rows[0]["DoctorId"].ToString();
                        model.Patient.UserName = basicInfoDt.Rows[0]["DoctorName"].ToString();
                        model.Patient.Birthday = basicInfoDt.Rows[0]["Birthday"].ToString();
                        model.Patient.Gender = basicInfoDt.Rows[0]["Gender"].ToString();
                        model.Patient.BloodType = "";
                        model.Patient.InsuranceType = "";
                    }

                    var DetailInfo = _ServicesSoapClient.GetDoctorInfoDetail(PatientId);
                    if (DetailInfo!=null)
                    {
                        model.Patient.IDNo = DetailInfo.IDNo;
                        model.Occupation = DetailInfo.Occupation;
                        model.Nationality = DetailInfo.Nationality;
                        model.Phone = DetailInfo.PhoneNumber;
                        model.Address = DetailInfo.HomeAddress;
                        model.EmergencyContact = DetailInfo.EmergencyContact;
                        model.EmergencyContactNumber = DetailInfo.EmergencyContactPhoneNumber;
                    }
                }
                

                //加载操作医生未负责患者列表      20150629
                //model.PatientList = GetPotentialPatientList(DoctorId);

                return View(model);
            }
            else
            {
                ViewBag.OperationInvalidFlag = "true";        //显示手机号输入框             

                //加载操作医生未负责患者列表           20150629  
                //model.PatientList = GetPotentialPatientList(DoctorId);

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult BasicProfile(BasicProfileViewModel model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                string DoctorId = user.UserId;
                string DoctorName = user.UserName;
                //操作标识符   页面所有操作只做一次判断
                bool flag = false;

                string UserId = model.Patient.UserId;
                Session["PatientId"] = model.Patient.UserId;
               

                #region 插入患者基本信息
                //插入患者基本信息         
                string UserName = model.Patient.UserName;
                string Birthday = "";
                if (model.Patient.Birthday != "")
                {
                    Birthday = System.Text.RegularExpressions.Regex.Replace(model.Patient.Birthday, @"[^0-9]+", "");
                }
                int Gender = Convert.ToInt32(model.Patient.Gender);
                int BloodType = Convert.ToInt32(model.Patient.BloodType);
                string IDNo = model.Patient.IDNo;           //患者身份证号码   2014/12/11 CSQ        
                string InsuranceType = model.Patient.InsuranceType;
                int InvalidFlag = 0;

                flag = _ServicesSoapClient.SetPatBasicInfo(UserId, UserName, Convert.ToInt32(Birthday), Gender, BloodType, IDNo, "", InsuranceType, InvalidFlag, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入MstUser
                //用户已存在
                if (_ServicesSoapClient.CheckUserExist(UserId) == true)
                {
                    DataSet ds = _ServicesSoapClient.GetUserInfoList(UserId, "");
                    if (ds.Tables.Count > 0)
                    {
                        User userInfo = new User();
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            //userInfo.UserName = row[1].ToString();
                            userInfo.Password = row[2].ToString();
                            userInfo.Class = row[3].ToString();
                            //userInfo.ClassName = row[4].ToString();
                            userInfo.StartDate = row[5].ToString();
                            userInfo.EndDate = row[6].ToString();
                            flag = _ServicesSoapClient.SetMstUser(UserId, UserName, userInfo.Password, "", "", 1, Convert.ToInt32(userInfo.StartDate), Convert.ToInt32(userInfo.EndDate), DateTime.Now, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        }
                    }
                    DataSet roleDs = _ServicesSoapClient.GetAllRoleMatch(UserId);
                    int patientFlag = 0;
                    if (roleDs.Tables.Count != 0)
                    {
                        DataTable roleDt = roleDs.Tables[0];
                        foreach (DataRow dr in roleDt.Rows)
                        {
                            if (dr["RoleClass"].ToString() == "Patient")
                            {
                                patientFlag = 1;
                            }
                        }
                    }
                    if (patientFlag == 0) 
                    {
                        flag = _ServicesSoapClient.SetPsRoleMatch(UserId, "Patient", "", "1", "") == 1? true : false;
                    }
                }
                else   //新增用户
                {
                    flag = _ServicesSoapClient.SetMstUser(UserId, UserName, "123456", "", "", 1, Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")), Convert.ToInt32(DateTime.Now.AddYears(1).ToString("yyyyMMdd")), DateTime.Now, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    //string test = Request["PhoneNo"].ToString();
                    if ((_ServicesSoapClient.SetPhoneNo(UserId, "PhoneNo", Request.Form["PhoneNo"].ToString(), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType)) == 1)
                    {
                        flag = true;
                    }
                    if (_ServicesSoapClient.SetPsRoleMatch(UserId, "Patient", "", "1", "") == 1)
                    {
                        flag = true;
                    }
                    //flag = _ServicesSoapClient.SetHUserId(UserId, model.Patient.HospitalId, model.Patient.HospitalCode, "", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                  
                }
                #endregion

                string Patient = UserId;

                #region 插入患者详细信息
                //插入 身份证号         
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact001_1", 1, IDNo, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 职业
                string Occupation = model.Occupation;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact001_2", 1, Occupation, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 国籍
                string Nationality = model.Nationality;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact001_3", 1, Nationality, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 手机号码
                string Phone = model.Phone;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact002_1", 1, Phone, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 家庭住址
                string Address = model.Address;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact002_2", 1, Address, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 紧急联系人
                string EmergencyContact = model.EmergencyContact;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact002_3", 1, EmergencyContact, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //插入 紧急联系人电话
                string EmergencyContactNumber = model.EmergencyContactNumber;
                flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, "Contact", "Contact002_4", 1, EmergencyContactNumber, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                #endregion

                if (flag == true)
                {
                    return RedirectToAction("ClinicalInfo", "DoctorHome", new { UserId = UserId });
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }

        public JsonResult AddNewPat()
        {
            var res = new JsonResult();
            string UserId = _ServicesSoapClient.GetNoByNumberingType(17);   //"1":患者Id的生成方式
            res.Data = UserId;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检查手机号码是否已存在
        public JsonResult checkPhoneNoRepeat(string PhoneNo)
        {
            var res = new JsonResult();
            int result = _ServicesSoapClient.CheckRepeat(PhoneNo, "PhoneNo");
            res.Data = result;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //根据手机号码获取Id
        public JsonResult GetIDByPhoneNo(string PhoneNo)
        {
            var res = new JsonResult();
            string UserId = _ServicesSoapClient.GetIDByInput("PhoneNo", PhoneNo);
            res.Data = UserId;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //根据Id获取角色
        public JsonResult GetRolesByUserId(string UserId)
        {
            var res = new JsonResult();
            List<string> RoleList = new List<string>();

            DataSet roleDs = _ServicesSoapClient.GetAllRoleMatch(UserId);
            if (roleDs.Tables.Count != 0)
            {
                DataTable roleDt = roleDs.Tables[0];
                foreach (DataRow dr in roleDt.Rows)
                {
                    RoleList.Add(dr["RoleClass"].ToString());
                }
            }
            res.Data = RoleList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //根据Id获取手机号码
        public JsonResult GetPhoneNoByUserId(string UserId)
        {
            var res = new JsonResult();
            string PhoneNo = _ServicesSoapClient.GetPhoneNoByUserId(UserId);
            if (PhoneNo == null)
            {
                PhoneNo = "";
            }
            res.Data = PhoneNo;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //插入用户相关信息和角色信息
        public JsonResult setMstUserPhoneNoRoleMatch(string UserId, string PhoneNo, string HospitalId, string HospitalCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;

            var res = new JsonResult();
            int result = 0;
           
            if ((_ServicesSoapClient.SetMstUser(UserId, "", "123456", "", "", 1, Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")), Convert.ToInt32(DateTime.Now.AddYears(1).ToString("yyyyMMdd")), DateTime.Now, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType)) == true)
            {
                if ((_ServicesSoapClient.SetPhoneNo(UserId, "PhoneNo", PhoneNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType)) == 1)
                {
                    if (_ServicesSoapClient.SetPsRoleMatch(UserId, "Patient", "", "1", "") == 1)
                    {

                        if (_ServicesSoapClient.SetHUserId(UserId, HospitalId, HospitalCode, "", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType)==true)
                        {
                            result = 1;
                        }
                    }
                }
            }
            //int result = _ServicesSoapClient.CheckRepeat(PhoneNo, "PhoneNo");
            res.Data = result;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region 建档--健康模块
        public ActionResult ModuleProfile(string PatientId, string Category)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            BasicProfileViewModel model = new BasicProfileViewModel();

            model.Patient.UserId = PatientId;

            //加载患者基本信息
            model.Patient = GetPatientBasicInfo(PatientId);

            #region 修改说明
            //1 获取患者已购买的模块
            //2 获取患者未购买的模块
            //3 之前已购买和未购买是分别调用Ps和Cm表的方法做的，现在可以都调用Ps表中的方法，获取问卷信息
            //4 不同模块的问卷分别加载，不要一次性加载，这样可以不用在前端做不同模块信息的同步，用JsonResult实现
            //5 把模块信息相关的三个页面用同一种方式实现，方便以后的维护
            //6 高血压和糖尿病模块的问卷只有二级标题，心衰模块还有三级标题以及显示控制的条目，需要再做修改
            #endregion


            List<ModuleInfo> ModuleInfo = new List<Models.ModuleInfo>();
            DataSet ModulesInfo = new DataSet();
            if (user.Role == "Doctor")
            {
                ModulesInfo = _ServicesSoapClient.GetModulesBoughtByPId(PatientId);
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
            }
            else
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                HttpResponseMessage response = client.GetAsync("Api/v1/Users/HModulesByID?PatientId=" + PatientId + "&DoctorId=" + DoctorId).Result;
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.ReadAsStringAsync().Result != "[]")
                    {
                        string[] Modules = response.Content.ReadAsStringAsync().Result.Split(new string[] { "},{", "[{\"", "\"}]" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Module in Modules)
                        {
                            string[] Detail = Module.Split(new string[] { "\",\"", "\":\"" }, StringSplitOptions.RemoveEmptyEntries);
                            ModuleInfo NewLine = new Models.ModuleInfo();
                            NewLine.Category = Detail[1].Substring(1);
                            NewLine.ModuleName = Detail[3];
                            ModuleInfo.Add(NewLine);
                        }
                    }
                }
            }
            model.ModuleBoughtInfo = ModuleInfo;
            ModuleInfo = new List<Models.ModuleInfo>();
            if (user.Role == "Doctor")
            {
                ModulesInfo = _ServicesSoapClient.GetModulesUnBoughtByPId(PatientId);
            }
            else
            {
                ModulesInfo = _ServicesSoapClient.GetModuleList();
            }
            DataSet DoctorModule = _ServicesSoapClient.GetDoctorModuleList(DoctorId);
            List<string> DoctorModules = new List<string>();
            foreach (DataRow item in DoctorModule.Tables[0].Rows)
            {
                DoctorModules.Add(item[0].ToString());
            }
            foreach (DataTable item in ModulesInfo.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    if ((user.Role == "Doctor" && DoctorModules.IndexOf(row[0].ToString()) != -1) || (user.Role == "HealthCoach" && row[0].ToString() != "M4" && row[0].ToString() != "M5" && model.ModuleBoughtInfo.Find(delegate(ModuleInfo x){return x.Category == row[0].ToString();}) == null))
                    {
                        ModuleInfo NewLine = new Models.ModuleInfo();
                        NewLine.Category = row[0].ToString();
                        NewLine.ModuleName = row[1].ToString();
                        ModuleInfo.Add(NewLine);
                    }
                }
            }
            model.ModuleUnBoughtInfo = ModuleInfo;
            List<PatientDetailInfo> ItemInfo = new List<PatientDetailInfo>();
            DataSet ItemInfoSet = _ServicesSoapClient.GetItemInfoByPIdAndModule(PatientId, Category);
            bool InvalidFlag = false;
            foreach (DataTable item in ItemInfoSet.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    if (row[3].ToString() != "InvalidFlag" && row[3].ToString() != "Patient")
                    {
                        if (row[3].ToString() == "Doctor")
                        {
                            PatientDetailInfo NewLine = new PatientDetailInfo()
                            {
                                CategoryCode = row[1].ToString(),
                                CategoryName = row[2].ToString(),
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = _ServicesSoapClient.GetUserName(row[7].ToString())
                            };
                            if (user.Role == "HealthCoach")
                            {
                                HttpClient client = new HttpClient();
                                client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                                HttpResponseMessage response = client.GetAsync("Api/v1/Users/BasicDtlValue?UserId=" + PatientId + "&CategoryCode=H" + Category + "&ItemCode=Doctor&ItemSeq=1").Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    NewLine.Value = response.Content.ReadAsStringAsync().Result.Split(new string[] { "{", ":", "}", "\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                }
                            }
                            if (NewLine.Value == DoctorId)
                            {
                                NewLine.EditDeleteFlag = "true";
                            }
                            else
                            {
                                NewLine.EditDeleteFlag = "false";
                            }
                            ItemInfo.Add(NewLine);
                        }
                        else
                        {
                            PatientDetailInfo NewLine = new PatientDetailInfo()
                            {
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ControlType = row[11].ToString(),
                                OptionCategory = row[12].ToString(),
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = row[8].ToString(),
                                GroupHeaderFlag = Convert.ToInt32(row[13])
                            };
                            if (NewLine.ControlType != "7")
                                NewLine.OptionList = GetTypeList(NewLine.OptionCategory, NewLine.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
                            ItemInfo.Add(NewLine);
                        }
                    }
                    else
                    {
                        if (row[3].ToString() == "InvalidFlag")
                        {
                            if (user.Role == "HealthCoach")
                            {
                                HttpClient client = new HttpClient();
                                client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                                HttpResponseMessage response = client.GetAsync("Api/v1/Users/BasicDtlValue?UserId=" + PatientId + "&CategoryCode=H" + Category + "&ItemCode=InvalidFlag&ItemSeq=1").Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    InvalidFlag = (response.Content.ReadAsStringAsync().Result.Split(new string[] { "{", ":", "}", "\"" }, StringSplitOptions.RemoveEmptyEntries)[1] == "0");
                                }
                            }
                            else
                                InvalidFlag = (row[7].ToString() != "");
                        }
                    }
                }
            }

            model.ModuleDetailList = ItemInfo;
            model.InvalidFlag = InvalidFlag;
            //#region 从病人详细信息表中加载模块关注详细信息
            //DataSet ItemInfoBoughtds = _ServicesSoapClient.GetPatBasicInfoDtlList(PatientId);
            //List<List<PatientDetailInfo>> ItemInfoBought = new List<List<PatientDetailInfo>>();
            //ArrayList modulesBoughtCode = new ArrayList();
            //ArrayList modulesBoughtName = new ArrayList();


            //if (ItemInfoBoughtds!=null)
            //{
            //    foreach (DataTable datatable in ItemInfoBoughtds.Tables)
            //    {
            //        List<PatientDetailInfo> items = new List<PatientDetailInfo>();

            //        foreach (DataRow row in datatable.Rows)
            //        {
            //            if (row[3].ToString() != "InvalidFlag" && row[3].ToString() != "Patient")
            //            {
            //                if (row[3].ToString() == "Doctor")
            //                {
            //                    PatientDetailInfo item = new PatientDetailInfo()
            //                    {
            //                        //PatientId = row[0].ToString(),
            //                        CategoryCode = row[1].ToString(),
            //                        CategoryName = row[2].ToString(),
            //                        ItemCode = row[3].ToString(),
            //                        ItemName = row[4].ToString(),
            //                        ParentCode = row[5].ToString(),
            //                        //ControlType = row[11].ToString(),
            //                        // OptionCategory = row[12].ToString(),
            //                        //OptionSelected = row[0].ToString(),
            //                        //OptionList = row[0],
            //                        ItemSeq = Convert.ToInt32(row[6]),
            //                        Value = row[7].ToString(),
            //                        //Content = row[9].ToString()
            //                        Content = _ServicesSoapClient.GetUserName(row[7].ToString())
            //                        //Description = row[9].ToString()
            //                    };

            //                    if (item.Value == DoctorId)
            //                    {
            //                        item.EditDeleteFlag = "true";
            //                    }
            //                    else
            //                    {
            //                        item.EditDeleteFlag = "false";
            //                    }
            //                    items.Add(item);
            //                }
            //                else
            //                {
            //                    PatientDetailInfo item = new PatientDetailInfo()
            //                    {
            //                        //PatientId = row[0].ToString(),
            //                        CategoryCode = row[1].ToString(),
            //                        CategoryName = row[2].ToString(),
            //                        ItemCode = row[3].ToString(),
            //                        ItemName = row[4].ToString(),
            //                        ParentCode = row[5].ToString(),
            //                        ControlType = row[11].ToString(),
            //                        OptionCategory = row[12].ToString(),
            //                        //OptionSelected = row[0].ToString(),
            //                        //OptionList = row[0],
            //                        ItemSeq = Convert.ToInt32(row[6]),
            //                        Value = row[7].ToString(),
            //                        Content = row[8].ToString()
            //                        //Description = row[9].ToString()
            //                    };
            //                    item.OptionList = GetTypeList(item.OptionCategory, item.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
            //                    items.Add(item);
            //                }
            //            }
            //        }
            //        modulesBoughtCode.Add(items[0].CategoryCode);
            //        modulesBoughtName.Add(items[0].CategoryName);
            //        ItemInfoBought.Add(items);
            //    }
            //}
            //model.PatientDetailInfo = ItemInfoBought;
            //#endregion
            return View(model);
        }

        [HttpPost]
        public ActionResult ModuleProfile(BasicProfileViewModel model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                string DoctorId = user.UserId;
                //string DoctorName = user.UserName;
                //操作标识符   页面所有操作只做一次判断
                bool flag = true;

                string UserId = model.Patient.UserId;
                string UserName = model.Patient.UserName;
                string CategoryCode = "";
                DataSet ItemInfoSet = new DataSet();
                CategoryCode = Request.Form["ModuleDetailList[0].CategoryCode"];
                List<PatientDetailInfo> ItemInfo = new List<PatientDetailInfo>();
                if (CategoryCode != null)
                {
                    ItemInfoSet = _ServicesSoapClient.GetItemInfoByPIdAndModule(UserId, CategoryCode);
                    foreach (DataTable Item in ItemInfoSet.Tables)
                    {
                        foreach (DataRow Row in Item.Rows)
                        {
                            if (Row[3].ToString() != "InvalidFlag" && Row[3].ToString() != "Patient" && Row[3].ToString() != "Doctor")
                            {
                                PatientDetailInfo NewLine = new PatientDetailInfo
                                {
                                    ItemCode = Row[3].ToString(),
                                    OptionCategory = Row[12].ToString()
                                };
                                ItemInfo.Add(NewLine);
                            }
                        }
                    }
                }
                List<ModuleInfo> ModuleInfo = new List<Models.ModuleInfo>();
                DataSet ModulesInfo = new DataSet();
                if (user.Role == "Doctor")
                {
                    ModulesInfo = _ServicesSoapClient.GetModulesBoughtByPId(UserId);
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
                }
                else
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                    HttpResponseMessage response = client.GetAsync("Api/v1/Users/HModulesByID?PatientId=" + UserId + "&DoctorId=" + DoctorId).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content.ReadAsStringAsync().Result != "[]")
                        {
                            string[] Modules = response.Content.ReadAsStringAsync().Result.Split(new string[] { "},{", "[{\"", "\"}]" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string Module in Modules)
                            {
                                string[] Detail = Module.Split(new string[] { "\",\"", "\":\"" }, StringSplitOptions.RemoveEmptyEntries);
                                ModuleInfo NewLine = new Models.ModuleInfo();
                                NewLine.Category = Detail[1].Substring(1);
                                NewLine.ModuleName = Detail[3];
                                ModuleInfo.Add(NewLine);
                            }
                        }
                    }
                }
                model.ModuleBoughtInfo = ModuleInfo;
                ModuleInfo = new List<Models.ModuleInfo>();
                if (user.Role == "Doctor")
                {
                    ModulesInfo = _ServicesSoapClient.GetModulesUnBoughtByPId(UserId);
                }
                else
                {
                    ModulesInfo = _ServicesSoapClient.GetModuleList();
                }
                DataSet DoctorModule = _ServicesSoapClient.GetDoctorModuleList(DoctorId);
                List<string> DoctorModules = new List<string>();
                foreach (DataRow item in DoctorModule.Tables[0].Rows)
                {
                    DoctorModules.Add(item[0].ToString());
                }
                foreach (DataTable item in ModulesInfo.Tables)
                {
                    foreach (DataRow row in item.Rows)
                    {
                        if ((user.Role == "Doctor" && DoctorModules.IndexOf(row[0].ToString()) != -1) || (user.Role == "HealthCoach" && row[0].ToString() != "M4" && row[0].ToString() != "M5" && model.ModuleBoughtInfo.Find(delegate(ModuleInfo x) { return x.Category == row[0].ToString(); }) == null))
                        {
                            ModuleInfo NewLine = new Models.ModuleInfo();
                            NewLine.Category = row[0].ToString();
                            NewLine.ModuleName = row[1].ToString();
                            ModuleInfo.Add(NewLine);
                        }
                    }
                }
                model.ModuleUnBoughtInfo = ModuleInfo;
                //#region 从病人详细信息表中加载模块关注信息
                //DataSet ItemInfoBoughtds = _ServicesSoapClient.GetPatBasicInfoDtlList(UserId);
                //List<List<PatientDetailInfo>> ItemInfoBought = new List<List<PatientDetailInfo>>();
                //ArrayList modulesBoughtCode = new ArrayList();
                //ArrayList modulesBoughtName = new ArrayList();

                //if (ItemInfoBoughtds != null)
                //{
                //    foreach (DataTable datatable in ItemInfoBoughtds.Tables)
                //    {
                //        List<PatientDetailInfo> items = new List<PatientDetailInfo>();
                //        foreach (DataRow row in datatable.Rows)
                //        {
                //            if (row[3].ToString() != "InvalidFlag")
                //            {
                //                PatientDetailInfo item = new PatientDetailInfo
                //                {
                //                    //PatientId = row[0].ToString(),
                //                    CategoryCode = row[1].ToString(),
                //                    CategoryName = row[2].ToString(),
                //                    ItemCode = row[3].ToString(),
                //                    ItemName = row[4].ToString(),
                //                    ParentCode = row[5].ToString(),
                //                    //ControlType = row[11].ToString(),
                //                    // OptionCategory = row[12].ToString(),
                //                    //OptionSelected = row[0].ToString(),
                //                    //OptionList = row[0],
                //                    //ItemSeq = Convert.ToInt32(row[6]),
                //                    Value = row[7].ToString(),
                //                    Content = row[8].ToString(),
                //                    //Description = row[9].ToString()
                //                };
                //                items.Add(item);
                //            }
                //        }
                //        modulesBoughtCode.Add(items[0].CategoryCode);
                //        modulesBoughtName.Add(items[0].CategoryName);
                //        ItemInfoBought.Add(items);
                //    }
                //}
                //model.PatientDetailInfo = ItemInfoBought;
                //#endregion

                //#region 从字典表中加载模块信息
                ////修改：只加载医生负责的模块&患者购买的模块
                ////DataTable Moduledt = _ServicesSoapClient.GetModuleList().Tables[0];
                //DataTable Moduledt = _ServicesSoapClient.GetDoctorModuleList(DoctorId).Tables[0];

                ////ArrayList moduleUnBought = new ArrayList();
                //int indicator = 0;
                //foreach (DataRow dr in Moduledt.Rows)
                //{
                //    //string dictCode = dr["Code"].ToString();
                //    //string dictName = dr["Name"].ToString();
                //    string dictCode = dr["CategoryCode"].ToString();
                //    string dictName = dr["CategoryName"].ToString();
                //    foreach (string modulesBt in modulesBoughtCode)
                //    {
                //        if (modulesBt == dictCode)
                //        {
                //            indicator = 1;                    //已购买
                //            break;
                //        }
                //    }
                //    if (indicator == 0)    //未购买
                //    {
                //        //mubt.CategoryCode = dictCode;
                //        //mubt.CategoryName = dictName;
                //        //model.moduleUnBought.Add(mubt);
                //        model.moduleUnBoughtCode.Add(dictCode);
                //        model.moduleUnBoughtName.Add(dictName);
                //        //indicator = 1;
                //    }
                //    else
                //    {
                //        indicator = 0;
                //    }
                //}

                ////string code = "";
                //DataTable dt = new DataTable();
                ////ArrayList selectedModule = new ArrayList();
                ////ViewBag.SelectedModule = selectedModule;
                ////foreach (DataRow dr in Moduledt.Rows)
                //foreach (string mubt in model.moduleUnBoughtCode)
                //{
                //    List<InfoItem> list = new List<InfoItem>();
                //    //code = mubt.CategoryCode;
                //    dt = _ServicesSoapClient.GetMstInfoItemByCategoryCode(mubt).Tables[0];
                //    foreach (DataRow InfoItemDr in dt.Rows)
                //    {
                //        InfoItem item = new InfoItem();
                //        item.Code = InfoItemDr["Code"].ToString();
                //        item.Name = InfoItemDr["Name"].ToString();
                //        item.ParentCode = InfoItemDr["ParentCode"].ToString();
                //        item.SortNo = Convert.ToInt32(InfoItemDr["SortNo"]);
                //        item.GroupHeaderFlag = Convert.ToInt32(InfoItemDr["GroupHeaderFlag"]);
                //        item.ControlType = InfoItemDr["ControlType"].ToString();
                //        item.OptionCategory = InfoItemDr["OptionCategory"].ToString();
                //        list.Add(item);

                //    }
                //    model.InfoItemList.Add(list);
                //}
                //#endregion

                string Patient = UserId;
                string ItemCode = "";
                int ItemSeq = 1;
                string Value = "";
                string Description = "";
                int SortNo = 1;
                string OptionCategory = "";
                
               


                #region 插入购买的模块关注详细信息
                int j = 0;
                if (CategoryCode != null)
                {
                    //是否购买
                    ModuleInfo ModuleFind = model.ModuleBoughtInfo.Find(delegate(ModuleInfo x)
                    {
                        return x.Category == CategoryCode;
                    });
                    if (ModuleFind == null)
                    {
                        //插入 医生详细信息表 负责患者信息
                        if (user.Role == "Doctor")
                        {
                            flag = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, CategoryCode, UserId, Description, 0, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        }
                        else
                        {
                            flag = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, "H" + CategoryCode, UserId, Description, 0, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        }
                        model.ModuleBoughtInfo.Add(new ModuleInfo
                        {
                            Category = CategoryCode,
                            ModuleName = model.ModuleUnBoughtInfo.Find(delegate(ModuleInfo x)
                            {
                                return x.Category == CategoryCode;
                            }).ModuleName
                        });
                        model.ModuleUnBoughtInfo.Remove(model.ModuleUnBoughtInfo.Find(delegate(ModuleInfo x)
                        {
                            return x.Category == CategoryCode;
                        }));
                    }
                    if (user.Role == "Doctor")
                    {
                        flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, CategoryCode, "InvalidFlag", ItemSeq, "0", Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        //插入 患者详细信息表 负责医生信息                           
                        flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, CategoryCode, "Doctor", ItemSeq, DoctorId, "", SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    }
                    else
                    {
                        flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, "H" + CategoryCode, "InvalidFlag", ItemSeq, "0", Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        //插入 患者详细信息表 负责医生信息                           
                        flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, "H" + CategoryCode, "Doctor", ItemSeq, DoctorId, "", SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    }

                    for (j = 0; j < ItemInfo.Count; j++)
                    {
                        ItemCode = ItemInfo[j].ItemCode;
                        OptionCategory = ItemInfo[j].OptionCategory;
                        Value = Request.Form[ItemInfo[j].ItemCode];

                        //插入患者详细信息表中的模块关注详细信息
                        if (OptionCategory != "Cm.MstHypertensionDrug" && OptionCategory != "Cm.MstDiabetesDrug" && OptionCategory != "Cm.MstLipidDrug" && OptionCategory != "Cm.MstUricAcidReductionDrug")
                        {
                            flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, "M", ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        }
                        //string[] Array = Value.Split(',');
                        //if (Value ==null)
                        //{
                        //插入患者详细信息表中的模块关注详细信息
                        //flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

                        //}
                        //else
                        //{
                        //string[] values = Value.Split(',');
                        //int vLength = values.Length;
                        //if (vLength > 1)
                        //{

                        //for (int vnum = 0; vnum < vLength; vnum++)
                        //{
                        //插入患者详细信息表中的模块关注详细信息
                        //flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, CategoryCode, ItemCode, ItemSeq, values[vnum].ToString(), Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        //SortNo++;
                        //ItemSeq++;
                        //}
                        //}
                        //else
                        //{
                        //插入患者详细信息表中的模块关注详细信息
                        // flag = _ServicesSoapClient.SetBasicInfoDetail(Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        //}
                        //}
                    }
                }
                #endregion
                if (CategoryCode != "")
                {
                    ItemInfo = new List<PatientDetailInfo>();
                    ItemInfoSet = _ServicesSoapClient.GetItemInfoByPIdAndModule(UserId, CategoryCode);
                    bool InvalidFlag = false;
                    foreach (DataTable item in ItemInfoSet.Tables)
                    {
                        foreach (DataRow row in item.Rows)
                        {
                            if (row[3].ToString() != "InvalidFlag" && row[3].ToString() != "Patient")
                            {
                                if (row[3].ToString() == "Doctor")
                                {
                                    PatientDetailInfo NewLine = new PatientDetailInfo()
                                    {
                                        CategoryCode = row[1].ToString(),
                                        CategoryName = row[2].ToString(),
                                        ItemCode = row[3].ToString(),
                                        ItemName = row[4].ToString(),
                                        ParentCode = row[5].ToString(),
                                        ItemSeq = Convert.ToInt32(row[6]),
                                        Value = row[7].ToString(),
                                        Content = _ServicesSoapClient.GetUserName(row[7].ToString())
                                    };
                                    if (user.Role == "HealthCoach")
                                    {
                                        HttpClient client = new HttpClient();
                                        client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                                        HttpResponseMessage response = client.GetAsync("Api/v1/Users/BasicDtlValue?UserId=" + UserId + "&CategoryCode=H" + CategoryCode + "&ItemCode=Doctor&ItemSeq=1").Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            NewLine.Value = response.Content.ReadAsStringAsync().Result.Split(new string[] { "{", ":", "}", "\"" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                        }
                                    }
                                    if (NewLine.Value == DoctorId)
                                    {
                                        NewLine.EditDeleteFlag = "true";
                                    }
                                    else
                                    {
                                        NewLine.EditDeleteFlag = "false";
                                    }
                                    ItemInfo.Add(NewLine);
                                }
                                else
                                {
                                    PatientDetailInfo NewLine = new PatientDetailInfo()
                                    {
                                        ItemCode = row[3].ToString(),
                                        ItemName = row[4].ToString(),
                                        ParentCode = row[5].ToString(),
                                        ControlType = row[11].ToString(),
                                        OptionCategory = row[12].ToString(),
                                        ItemSeq = Convert.ToInt32(row[6]),
                                        Value = row[7].ToString(),
                                        Content = row[8].ToString(),
                                        GroupHeaderFlag = Convert.ToInt32(row[13])
                                    };
                                    if (NewLine.ControlType != "7")
                                        NewLine.OptionList = GetTypeList(NewLine.OptionCategory, NewLine.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
                                    ItemInfo.Add(NewLine);
                                }
                            }
                            else
                            {
                                if (row[3].ToString() == "InvalidFlag")
                                {
                                    if (user.Role == "HealthCoach")
                                    {
                                        HttpClient client = new HttpClient();
                                        client.BaseAddress = new Uri("http://121.43.107.106:9000/");
                                        HttpResponseMessage response = client.GetAsync("Api/v1/Users/BasicDtlValue?UserId=" + UserId + "&CategoryCode=H" + CategoryCode + "&ItemCode=InvalidFlag&ItemSeq=1").Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            InvalidFlag = (response.Content.ReadAsStringAsync().Result.Split(new string[] { "{", ":", "}", "\"" }, StringSplitOptions.RemoveEmptyEntries)[1] == "0");
                                        }
                                    }
                                    else
                                        InvalidFlag = (row[7].ToString() != "");
                                }
                            }
                        }
                    }
                    model.ModuleDetailList = ItemInfo;
                    model.InvalidFlag = InvalidFlag;
                }
            }
            return View(model);
        }

        //加载患者关注的体征信息
        public JsonResult GetVitalSignsFocusedList(string UserId)
        {
            var res = new JsonResult();
            List<string> VitalSignsFocusedList = new List<string>();
            DataSet DS = _ServicesSoapClient.GetPatient2VS(UserId);
            if (DS.Tables.Count > 0)
            {
                DataTable DT = DS.Tables[0];
                foreach (DataRow DR in DT.Rows)
                {
                    VitalSignsFocusedList.Add(DR["VitalSignsType"].ToString() + "-" + DR["VitalSignsTypeName"].ToString() + "-" + DR["VitalSignsCode"].ToString() + "-" + DR["VitalSignsName"].ToString());

                }
            }
            res.Data = VitalSignsFocusedList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //增加
        public JsonResult VitalSignsFocusedAdd(string UserId, string VitalSignsType, string VitalSignsCode, int InvalidFlag)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            bool flag = false;
            flag = _ServicesSoapClient.SetPsPatient2VS(UserId, VitalSignsType, VitalSignsCode, InvalidFlag, 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult VitalSignsFocusedDelete(string UserId, string VitalSignsType, string VitalSignsCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteVitalSignsInfo(UserId, VitalSignsType, VitalSignsCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //编辑(现在用不着了)
        public ActionResult ModuleProfileEdit(string UserId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            BasicProfileViewModel model = new BasicProfileViewModel();

            //加载患者基本信息          
            model.Patient = GetPatientBasicInfo(UserId);

            //加载患者详细信息
            //GetPatientInfoDetail(ref model);                      

            #region 从病人详细信息表中加载模块信息
            DataSet ItemInfoBoughtds = _ServicesSoapClient.GetPatBasicInfoDtlList(UserId);
            List<List<PatientDetailInfo>> ItemInfoBought = new List<List<PatientDetailInfo>>();
            ArrayList modulesBoughtCode = new ArrayList();
            ArrayList modulesBoughtName = new ArrayList();

            foreach (DataTable datatable in ItemInfoBoughtds.Tables)
            {
                List<PatientDetailInfo> items = new List<PatientDetailInfo>();
                foreach (DataRow row in datatable.Rows)
                {
                    if (row[3].ToString() != "InvalidFlag")
                    {
                        if (row[3].ToString() == "Doctor")
                        {
                            PatientDetailInfo item = new PatientDetailInfo
                            {
                                //PatientId = row[0].ToString(),
                                CategoryCode = row[1].ToString(),
                                CategoryName = row[2].ToString(),
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ControlType = row[11].ToString(),
                                OptionCategory = row[12].ToString(),
                                //OptionSelected = row[0].ToString(),
                                //OptionList = row[0],
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = _ServicesSoapClient.GetUserName(row[7].ToString())
                                //Description = row[9].ToString()
                            };
                            item.OptionList = GetTypeList(item.OptionCategory, item.Value);
                            if (item.Value == DoctorId)
                            {
                                item.EditDeleteFlag = "true";
                            }
                            else
                            {
                                item.EditDeleteFlag = "false";
                            }
                            items.Add(item);
                        }
                        else
                        {
                            PatientDetailInfo item = new PatientDetailInfo
                            {
                                //PatientId = row[0].ToString(),
                                CategoryCode = row[1].ToString(),
                                CategoryName = row[2].ToString(),
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ControlType = row[11].ToString(),
                                OptionCategory = row[12].ToString(),
                                //OptionSelected = row[0].ToString(),
                                //OptionList = row[0],
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = row[8].ToString(),
                                //Description = row[9].ToString()
                            };
                            item.OptionList = GetTypeList(item.OptionCategory, item.Value);
                            items.Add(item);
                        }
                    }
                }
                modulesBoughtCode.Add(items[0].CategoryCode);
                modulesBoughtName.Add(items[0].CategoryName);
                ItemInfoBought.Add(items);
            }
            model.PatientDetailInfo = ItemInfoBought;

            #endregion

            return View(model);
        } 

        [HttpPost]//现在用不着了
        public ActionResult ModuleProfileEdit(BasicProfileViewModel model, FormCollection formCollection)
        {
            string UserId = model.Patient.UserId;
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            //加载患者基本信息
            model.Patient = GetPatientBasicInfo(UserId);

            //加载患者详细信息
            //GetPatientInfoDetail(ref model);                                        

            #region 从病人详细信息表中加载
            DataSet ItemInfoBoughtds = _ServicesSoapClient.GetPatBasicInfoDtlList(UserId);
            List<List<PatientDetailInfo>> ItemInfoBought = new List<List<PatientDetailInfo>>();

            bool flag = false;
            string Patient = UserId;
            string CategoryCode = "";
            string ItemCode = "";
            int ItemSeq = 1;
            string Value = "";
            string Content = "";
            string ControlType = "";
            int SortNo = 1;
            
            if (ItemInfoBoughtds.Tables.Count > 0)
            {
                foreach (DataTable datatable in ItemInfoBoughtds.Tables)
                {
                    if ((datatable.Rows[0][3].ToString() == "Doctor") && (datatable.Rows[0][7].ToString() == DoctorId))
                    {
                        int DeleteFlag = 0;
                        CategoryCode = datatable.Rows[0][1].ToString();
                        while (DeleteFlag == 0)
                        {
                            DeleteFlag = _ServicesSoapClient.DeleteModuleDetail(Patient, CategoryCode);
                        }
                        foreach (DataRow row in datatable.Rows)
                        {
                            if ((row[3].ToString() != "InvalidFlag") && (row[3].ToString() != "Doctor") && (row[4].ToString() != "伴随疾病"))
                            {
                                //PatientDetailInfo item = new PatientDetailInfo

                                //PatientId = row[0].ToString(),
                                CategoryCode = row[1].ToString();
                                //CategoryName = row[2].ToString(),
                                ItemCode = row[3].ToString();
                                ItemSeq = Convert.ToInt32(row[10]);
                                //Description = row[9].ToString();
                                Content = row[9].ToString();
                                SortNo = Convert.ToInt32(row[10]);
                                ControlType = row[11].ToString();

                                Value = Request.Form[row[3].ToString()];   //只更改了Value
                                if (ControlType != "7")
                                {
                                    flag = _ServicesSoapClient.SetPatBasicInfoDetail(Patient, CategoryCode, ItemCode, ItemSeq, Value, Content, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            //flag为TRUE即全部插成功
            if (flag == true)
            {
                return RedirectToAction("ModuleProfile", "DoctorHome", new { PatientId = UserId }); //成功跳转至详细
            }
            else
            {
                Response.Write("<script>alert('数据保存失败');</script>");   //失败返回编辑，提示失败
                return RedirectToAction("ModuleProfileEdit", "DoctorHome", new { UserId = UserId });
            }
        }

        //删除      
        public JsonResult ModuleProfileDelete(string UserId, string CategoryCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;
            var res = new JsonResult();
            /*对数据进行处理*/
            //string UserId = UserId;
            string ItemCode = "InvalidFlag";
            int ItemSeq = 1;   //BasicInfoDetail
            int flag = 2;
            if (user.Role == "Doctor")
            {
                flag = _ServicesSoapClient.DeleteModule(UserId, CategoryCode, ItemCode, ItemSeq);
            }
            else
            {
                flag = _ServicesSoapClient.DeleteModule(UserId, "H" + CategoryCode, ItemCode, ItemSeq);
            }
            if (user.Role == "Doctor")
            {
                flag = _ServicesSoapClient.DeletePatient(DoctorId, CategoryCode, UserId);
            }
            else
            {
                flag = _ServicesSoapClient.DeletePatient(DoctorId, "H" + CategoryCode, UserId);
            }
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

        //根据选中的体征类型加载体征项目名称下拉框
        public JsonResult GetListbyVitalSignsTypeName(string VitalSignsTypeNameSelected)
        {
            var res = new JsonResult();
            List<string> VitalSignsTypeNameList = new List<string>();
            if (VitalSignsTypeNameSelected != "0")
            {
                DataSet TypeNameDS = _ServicesSoapClient.GetVitalSignsNameListByType(VitalSignsTypeNameSelected);
                DataTable TypeNameDT = TypeNameDS.Tables[0];
                foreach (DataRow DR in TypeNameDT.Rows)
                {
                    VitalSignsTypeNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = VitalSignsTypeNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region 就诊信息
        public ActionResult ClinicalInfo(string UserId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;
        

            ClinicalInfoProfileViewModel ClinicalInfoModel = new ClinicalInfoProfileViewModel();
            ClinicalInfoModel.UserId = UserId;

            //加载患者基本信息
            //ClinicalInfoModel.PatientBasicInfo = GetPatientBasicInfo(UserId);

            //加载患者就诊信息
            ClinicalInfoModel.InPatientList = GetInPatientList(UserId, DoctorId);
            ClinicalInfoModel.OutPatientList = GetOutPatientList(UserId, DoctorId);
            ClinicalInfoModel.ClinicalInfoList = GetInPatientInfoList(UserId);

            return View(ClinicalInfoModel);
        }

        [HttpPost]
        public ActionResult ClinicalInfo(ClinicalInfoProfileViewModel model, FormCollection formCollection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            bool flag = false;
            string VisitType = Request.Form["VisitType"];

            string UserId = model.UserId;
            string VisitId = "";
            int SortNo = 1;
            string HospitalCode = model.ClinicalInfo.HospitalCode;
            string Department = model.ClinicalInfo.DepartmentCode;
            string Doctor = model.ClinicalInfo.Doctor;

            switch (VisitType)
            {
                case "1":                    //门诊
                    VisitId = _ServicesSoapClient.GetNoByNumberingType(8);
                    DateTime ClinicDate = model.ClinicalInfo.AdmissionDate;
                    flag = _ServicesSoapClient.SetOutPatientInfo(UserId, VisitId, ClinicDate, HospitalCode, Department, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    break;
                case "2":                 //急诊
                    VisitId = _ServicesSoapClient.GetNoByNumberingType(9);
                    DateTime EmergencyDate = model.ClinicalInfo.AdmissionDate;
                    flag = _ServicesSoapClient.SetOutPatientInfo(UserId, VisitId, EmergencyDate, HospitalCode, Department, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    break;
                case "3":                  //住院
                    VisitId = _ServicesSoapClient.GetNoByNumberingType(5);
                    DateTime AdmissionDate = model.ClinicalInfo.AdmissionDate;
                    DateTime DischargeDate = new DateTime();

                    if (Request.Form["DischargeDate"] == "")
                    {
                        DischargeDate = Convert.ToDateTime("9999/01/01 0:00:00");
                    }
                    else
                    {
                        DischargeDate = Convert.ToDateTime(Request.Form["DischargeDate"]);
                    }
                    flag = _ServicesSoapClient.SetInPatientInfo(UserId, VisitId, SortNo, AdmissionDate, DischargeDate, HospitalCode, Department, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    break;
                case "4":
                    string VisitId_SortNo_AdmissionDate = model.ClinicalInfo.VisitIdSelected;
                    VisitId = VisitId_SortNo_AdmissionDate.Split('_')[0];
                    SortNo = Convert.ToInt32(VisitId_SortNo_AdmissionDate.Split('_')[1]);
                    DateTime LastAdmissionDate = Convert.ToDateTime(VisitId_SortNo_AdmissionDate.Split('_')[2]);
                    DateTime TransforDate = model.ClinicalInfo.AdmissionDate;

                    //更新转科之前那条住院记录的出院日期为转科日期   不管转科之前的记录的出院日期是否已经填写，都重新写成转科日期
                    flag = _ServicesSoapClient.SetInPatientInfo(UserId, VisitId, SortNo, LastAdmissionDate, TransforDate, HospitalCode, Department, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

                    SortNo = SortNo + 1;
                    DateTime DischargeOutDate = new DateTime();
                    if (Request.Form["DischargeDate"] == "")
                    {
                        DischargeOutDate = Convert.ToDateTime("9999/01/01 0:00:00");
                    }
                    else
                    {
                        DischargeOutDate = Convert.ToDateTime(Request.Form["DischargeDate"]);
                    }
                    flag = _ServicesSoapClient.SetInPatientInfo(UserId, VisitId, SortNo, TransforDate, DischargeOutDate, HospitalCode, Department, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    break;
            }
            if (flag == true)
            {
                return RedirectToAction("ClinicalInfo", "DoctorHome", new { UserId = UserId });
            }
            else
            {
                return View(model);
            }
        }

        //获取就诊医院最新一个就诊号
        public JsonResult getLatestHUserIdByHCode(string UserId, string HCode)
        {
            var res = new JsonResult();
            /*对数据进行处理*/


            string HUserId = _ServicesSoapClient.getLatestHUserIdByHCode(UserId, HCode);


            res.Data = HUserId;
       
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //删除
        public JsonResult ClinicalInfoDelete(string UserId, string VisitId)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            string str = VisitId.Substring(0, 2);  //  str2="123";
            if (str == "In")
            {
                VisitId = VisitId.Split(' ')[0];

                flag = _ServicesSoapClient.DeleteInPatientInfo(UserId, VisitId);
            }
            else
            {
                flag = _ServicesSoapClient.DeleteOutPatientInfo(UserId, VisitId);

            }
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

        //编辑
        public JsonResult ClinicalInfoEdit(string UserId, string VisitId, int SortNo, string HospitalCode, string DepartmentCode, DateTime AdmissionDate, DateTime DischargeDate, string Doctor)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            bool flag = false;
            string str = VisitId.Substring(0, 2);  //  str2="123";
            if (str == "In")
            {
                flag = _ServicesSoapClient.SetInPatientInfo(UserId, VisitId, SortNo, AdmissionDate, DischargeDate, HospitalCode, DepartmentCode, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            }
            else
            {
                flag = _ServicesSoapClient.SetOutPatientInfo(UserId, VisitId, AdmissionDate, HospitalCode, DepartmentCode, Doctor, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            if (flag == true)
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

        #region 临床信息
        public ActionResult ClinicalProfile(string UserId, string Newer)     //传入患者Id
        {
            bool Set = true;
            Models.PatientBasicInfo ClinicalProfile = new Models.PatientBasicInfo();
            if (Newer == "new")
            {
                string VisitId = _ServicesSoapClient.GetNoByNumberingType(8);
                DateTime ClinicDate = Convert.ToDateTime(_ServicesSoapClient.GetServerTime());
                string HospitalCode = "HJZYY";
                string Department = "41";
                var user = Session["CurrentUser"] as UserAndRole;
                bool flag = _ServicesSoapClient.SetOutPatientInfo(UserId, VisitId, ClinicDate, HospitalCode, Department, user.UserName, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (flag == false)
                {
                    Set = false;
                }
            }
            //加载患者基本信息
            ClinicalProfile = GetPatientBasicInfo(UserId);
            //加载患者就诊信息列表，下拉框
            ClinicalProfile.ClinicalInfoList = GetClinicalInfoList(UserId);
            if (Newer == "new" && Set == true)
            {
                ClinicalProfile.LatestClinicalInfo = ClinicalProfile.ClinicalInfoList[ClinicalProfile.ClinicalInfoList.Count - 1].Value;
            }

            return View(ClinicalProfile);
        }

        public ActionResult ClinicalProfileLoadByVisitId(string UserId, string VisitId)     //传入患者Id
        {
            Models.PatientBasicInfo model = new Models.PatientBasicInfo();
            model.UserId = UserId;
            model.VisitId = VisitId;

            return View(model);
        }

        #region 症状信息
        //局部刷新
        public ActionResult SymptomInfo(string UserId, string VisitId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            SymptomsProfileViewModel SymptomInfoModel = new SymptomsProfileViewModel();
            SymptomInfoModel.UserId = UserId;
            SymptomInfoModel.VisitId = VisitId;

            //加载症状列表
            GetSymptomInfoList(ref SymptomInfoModel, DoctorId);

            ViewBag.MaxSynptomsNo = SymptomInfoModel.MaxSortNo;
            //SymptomInfoModel.ClinicalInfoList = GetClinicalInfoList(UserId);
            return PartialView("_SymptomInfo", SymptomInfoModel);
        }

        //根据SymptomsType动态加载GetSymptomsNameList下拉框
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

        //添加
        public JsonResult SymptomInfoAdd(string UserId, string VisitId, string SymptomsType, string SymptomsCode, string Description, string RecordDate, string RecordTime)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            var Record_Date = System.Text.RegularExpressions.Regex.Replace(RecordDate, @"[^0-9]+", "");
            var Record_Time = System.Text.RegularExpressions.Regex.Replace(RecordTime, @"[^0-9]+", "");
            if (Record_Date == "")
            {
                Record_Date = "0";
            }
            if (Record_Time == "")
            {
                Record_Time = "0";
            }
            bool flag = false;         //Convert.ToInt32(Admission)

            flag = _ServicesSoapClient.SetSymptomsInfo(UserId, VisitId, SymptomsType, SymptomsCode, Description, Convert.ToInt32(Record_Date), Convert.ToInt32(Record_Time), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult SymptomInfoEdit(string UserId, string VisitId, int SynptomsNo, string SymptomsType, string SymptomsCode, string Description, string RecordDate, string RecordTime)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            var Record_Date = System.Text.RegularExpressions.Regex.Replace(RecordDate, @"[^0-9]+", "");
            var Record_Time = System.Text.RegularExpressions.Regex.Replace(RecordTime, @"[^0-9]+", "");
            if (Record_Date == "")
            {
                Record_Date = "0";
            }
            if (Record_Time == "")
            {
                Record_Time = "0";
            }
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.UpdateSymptomsInfo(UserId, VisitId, SynptomsNo, SymptomsType, SymptomsCode, Description, Convert.ToInt32(Record_Date), Convert.ToInt32(Record_Time), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult SymptomInfoDelete(string UserId, string VisitId, int SynptomsNo)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteSymptomsInfo(UserId, VisitId, SynptomsNo);
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

        #region 诊断信息
        //局部刷新
        public ActionResult DiagnosisInfo(string UserId, string VisitId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            DiagnosisInfoProfileViewModel DiagnosisInfoModel = new DiagnosisInfoProfileViewModel();
            DiagnosisInfoModel.UserId = UserId;
            DiagnosisInfoModel.VisitId = VisitId;
            //加载诊断列表
            GetDiagnosisInfoList(ref DiagnosisInfoModel, DoctorId);

            ViewBag.MaxDiagnosisNo = DiagnosisInfoModel.MaxSortNo;
            //DiagnosisInfoModel.ClinicalInfoList = GetClinicalInfoList(UserId);

            return PartialView("_DiagnosisInfo", DiagnosisInfoModel);
        }

        //根据Type动态加载GetTypeNameList下拉框
        public JsonResult GetListbyType(string TypeSelected)
        {
            var res = new JsonResult();
            List<string> TypeNameList = new List<string>();
            if (TypeSelected != "0")
            {
                DataSet TypeNameDS = _ServicesSoapClient.GetDiagNameList(TypeSelected);
                DataTable TypeNameDT = TypeNameDS.Tables[0];

                foreach (DataRow DR in TypeNameDT.Rows)
                {
                    TypeNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = TypeNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //增加
        public JsonResult DiagnosisInfoAdd(string UserId, string VisitId, string DiagnosisNo, int DiagnosisType, string Type, string DiagnosisCode, string Description, string RecordDate)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Record_Date = System.Text.RegularExpressions.Regex.Replace(RecordDate, @"[^0-9]+", "");
            //var Record_Time = System.Text.RegularExpressions.Regex.Replace(RecordTime, @"[^0-9]+", "");
            //if (Record_Date == "")
            //{
            //    Record_Date = "0";
            //}
            //if (Record_Time == "")
            //{
            //    Record_Time = "0";
            //}
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetDiagnosisInfo(UserId, VisitId, DiagnosisType, DiagnosisNo, Type, DiagnosisCode, Description, RecordDate, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult DiagnosisInfoEdit(string UserId, string VisitId, string DiagnosisNo, int DiagnosisType, string Type, string DiagnosisCode, string Description, string RecordDate)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            var Record_Date = System.Text.RegularExpressions.Regex.Replace(RecordDate, @"[^0-9]+", "");
            //if (Record_Date == "")
            //{
            //    Record_Date = "0";
            //}
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetDiagnosisInfo(UserId, VisitId, DiagnosisType, DiagnosisNo, Type, DiagnosisCode, Description, RecordDate, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult DiagnosisInfoDelete(string UserId, string VisitId, string DiagnosisNo, int DiagnosisType)
        {
            var res = new JsonResult();
            /*对数据进行处理*/
            int flag = 2;
            flag = _ServicesSoapClient.DeleteDiagnosisInfo(UserId, VisitId, DiagnosisType, DiagnosisNo);
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

        #region 检查信息
        //局部刷新
        public ActionResult ExaminationInfo(string UserId, string VisitId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            ExaminationProfileViewModel ExaminationInfoModel = new ExaminationProfileViewModel();
            ExaminationInfoModel.UserId = UserId;
            ExaminationInfoModel.VisitId = VisitId;

            GetExaminationInfoList(ref ExaminationInfoModel, DoctorId);
            ViewBag.MaxSortNo = ExaminationInfoModel.MaxSortNo;
            //ExaminationInfoModel.ClinicalInfoList = GetClinicalInfoList(UserId);

            return PartialView("_ExaminationInfo", ExaminationInfoModel);
        }

        //局部刷新
        public ActionResult ExaminationInfoDetail(string UserId, string VisitId, string SortNo, string ItemCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            ExaminationProfileViewModel model = new ExaminationProfileViewModel();
            model.UserId = UserId;
            model.VisitId = VisitId;

            ExaminationInfo examInfo = new Models.ExaminationInfo();
            examInfo.SortNo = SortNo;
            examInfo.ItemCode = ItemCode;
            model.ExamInfo = examInfo;

            GetExamDetailInfoList(ref model, DoctorId);

            model.ExamSubItemList = GetExamSubItemNameList(ItemCode);
            return PartialView("_ExaminationInfoDetail", model);
        }

        //根据ExamType动态加载GetExamTypeNameList下拉框
        public JsonResult GetListbyExamType(string ExamTypeSelected)
        {
            var res = new JsonResult();
            List<string> ExamTypeNameList = new List<string>();
            if (ExamTypeSelected != "0")
            {
                DataSet TypeNameDS = _ServicesSoapClient.GetExamItemNameList(ExamTypeSelected);
                DataTable TypeNameDT = TypeNameDS.Tables[0];

                foreach (DataRow DR in TypeNameDT.Rows)
                {
                    ExamTypeNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = ExamTypeNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //添加
        public JsonResult ExaminationInfoAdd(string UserId, string VisitId, string ExamType, string ExamDate, string ItemCode, string ExamPara, string Description, string Impression, string Recommendation, int IsAbnormal, string Status, string ReportDate, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Exam_Date = System.Text.RegularExpressions.Regex.Replace(ExamDate, @"[^0-9]+", "");
            //var Report_Date = System.Text.RegularExpressions.Regex.Replace(ReportDate, @"[^0-9]+", "");
            //if (Exam_Date == "")
            //{
            //    Exam_Date = "0";
            //}
            //if (Report_Date == "")
            //{
            //    Report_Date = "0";
            //}

            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetExamination(UserId, VisitId, ExamType, ExamDate, ItemCode, ExamPara, Description, Impression, Recommendation, IsAbnormal, Status, ReportDate, "", DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult ExaminationInfoEdit(string UserId, string VisitId, int SortNo, string ExamType, string ItemCode, string ExamDate, string Status, int IsAbnormal, string ReportDate, string ExamPara, string Description, string Impression, string Recommendation, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Exam_Date = System.Text.RegularExpressions.Regex.Replace(ExamDate, @"[^0-9]+", "");
            //var Report_Date = System.Text.RegularExpressions.Regex.Replace(ReportDate, @"[^0-9]+", "");
            //if (Exam_Date == "")
            //{
            //    Exam_Date = "0";
            //}
            //if (Report_Date == "")
            //{
            //    Report_Date = "0";
            //}
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.UpdateExamination(UserId, VisitId, SortNo, ExamType, ExamDate, ItemCode, ExamPara, Description, Impression, Recommendation, IsAbnormal, Status, ReportDate, "", DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult ExaminationInfoDelete(string UserId, string VisitId, int SortNo)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteExamination(UserId, VisitId, SortNo);
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

        //添加
        public JsonResult ExaminationInfoDetailAdd(string UserId, string VisitId, int SortNo, string SubCode, string Value, string UnitCode, int IsAbnormalCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetExamDtl(UserId, VisitId, SortNo, SubCode, Value, IsAbnormalCode, UnitCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult ExaminationInfoDetailEdit(string UserId, string VisitId, int SortNo, string Code, string Value, int UnitCode, string IsAbnormalCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            bool flag = false;
            flag = _ServicesSoapClient.SetExamDtl(UserId, VisitId, SortNo, Code, Value, UnitCode, IsAbnormalCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult ExaminationInfoDetailDelete(string UserId, string VisitId, int SortNo, string Code)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteExamDtl(UserId, VisitId, SortNo, Code);
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

        #region 化验信息
        //局部刷新
        public ActionResult LabTestInfo(string UserId, string VisitId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            LabTestProfileViewModel LabTestInfoModel = new LabTestProfileViewModel();
            LabTestInfoModel.UserId = UserId;
            LabTestInfoModel.VisitId = VisitId;

            GetLabTestInfoList(ref LabTestInfoModel, DoctorId);
            ViewBag.MaxSortNo = LabTestInfoModel.MaxSortNo;
            //LabTestInfoModel.ClinicalInfoList = GetClinicalInfoList(UserId);

            return PartialView("_LabTestInfo", LabTestInfoModel);
        }

        //局部刷新
        public ActionResult LabTestInfoDetail(string UserId, string VisitId, string SortNo, string LabItemType, string LabItemCode, string operationFlag)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            LabTestProfileViewModel model = new LabTestProfileViewModel();
            model.UserId = UserId;
            model.VisitId = VisitId;
            LabTestInfo labTestInfo = new Models.LabTestInfo();
            //labTestInfo.SortNo = Convert.ToInt32(SortNo);
            labTestInfo.SortNo = SortNo;

            labTestInfo.LabItemType = LabItemType;
            labTestInfo.LabItemCode = LabItemCode;
            model.LabTestInfo = labTestInfo;

            if (operationFlag == "1")
            {
                ViewBag.operationFlag = "true";
            }
            if (operationFlag == "0")
            {
                ViewBag.operationFlag = "false";
            }

            GetLabTestInfoDetailList(ref model, DoctorId);

            //model.LabSubItemList = GetLabTestSubItemNameList(LabItemCode);
            model.LabSubItemList = GetLabTestSubItemNameList();
            return PartialView("_LabTestInfoDetail", model);
        }

        //根据LabTestType动态加载LabTestNameList下拉框
        public JsonResult GetListbyLabTestType(string LabTestTypeSelected)
        {
            var res = new JsonResult();
            List<string> LabTestNameList = new List<string>();
            if (LabTestTypeSelected != "0")
            {
                DataSet TypeNameDS = _ServicesSoapClient.GetLabTestItemsNameList(LabTestTypeSelected);
                DataTable TypeNameDT = TypeNameDS.Tables[0];

                foreach (DataRow DR in TypeNameDT.Rows)
                {
                    LabTestNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = LabTestNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //增加
        public JsonResult LabTestInfoAdd(string UserId, string VisitId, string LabItemType, string LabItemCode, string ExamDate, string Status, string ReportDate, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Exam_Date = System.Text.RegularExpressions.Regex.Replace(ExamDate, @"[^0-9]+", "");
            //var Report_Date = System.Text.RegularExpressions.Regex.Replace(ReportDate, @"[^0-9]+", "");
            //if (Exam_Date == "")
            //{
            //    Exam_Date = "0";
            //}
            //if (Report_Date == "")
            //{
            //    Report_Date = "0";
            //}

            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetLabTest(UserId, VisitId, LabItemType, LabItemCode, ExamDate, Status, ReportDate, DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult LabTestInfoEdit(string UserId, string VisitId, string SortNo, string LabItemType, string LabItemCode, string ExamDate, string Status, string ReportDate, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Exam_Date = System.Text.RegularExpressions.Regex.Replace(ExamDate, @"[^0-9]+", "");
            //var Report_Date = System.Text.RegularExpressions.Regex.Replace(ReportDate, @"[^0-9]+", "");
            //if (Exam_Date == "")
            //{
            //    Exam_Date = "0";
            //}
            //if (Report_Date == "")
            //{
            //    Report_Date = "0";
            //}
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.UpdateLabTest(UserId, VisitId, SortNo, LabItemType, LabItemCode, ExamDate, Status, ReportDate, "deptcode", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult LabTestInfoDelete(string UserId, string VisitId, string SortNo)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteLabTest(UserId, VisitId, SortNo);
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

        //编辑
        public JsonResult LabTestInfoDetailEdit(string UserId, string VisitId, string SortNo, string Code, string Value, int UnitCode, string IsAbnormalCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            bool flag = false;
            flag = _ServicesSoapClient.SetLabTestDtl(UserId, VisitId, SortNo, Code, Value, UnitCode, IsAbnormalCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //增加
        public JsonResult LabTestInfoDetailAdd(string UserId, string VisitId, string SortNo, string SubCode, string Value, string UnitCode, int IsAbnormalCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/

            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.SetLabTestDtl(UserId, VisitId, SortNo, SubCode, Value, IsAbnormalCode, UnitCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //增加
        public JsonResult LabTestInfoDetailDelete(string UserId, string VisitId, string SortNo, string Code)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeteleLabTestDtl(UserId, VisitId, SortNo, Code);
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

        #region 药物治疗信息
        //局部刷新
        public ActionResult DrugInfo(string UserId, string VisitId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string DoctorId = user.UserId;

            DrugInfoProfileViewModel DrugInfoModel = new DrugInfoProfileViewModel();
            DrugInfoModel.UserId = UserId;
            DrugInfoModel.VisitId = VisitId;

            GetDrugInfoList(ref DrugInfoModel, DoctorId);
            ViewBag.MaxOrderNo = DrugInfoModel.MaxSortNo;
            //DrugInfoModel.ClinicalInfoList = GetClinicalInfoList(UserId);

            return PartialView("_DrugInfo", DrugInfoModel);
        }

        //增加
        public JsonResult DrugInfoAdd(string UserId, string VisitId, int OrderSubNo, string OrderClass, int RepeatIndicator, string OrderCode, string OrderContent, double Dosage, string DosageUnits, string Administration, DateTime StartDateTime, DateTime StopDateTime, int FreqCounter, string Frequency, int FreqInteval, string FreqIntevalUnit, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Start_DateTime = System.Text.RegularExpressions.Regex.Replace(StartDateTime, @"[^0-9]+", "");
            //var Stop_DateTime = System.Text.RegularExpressions.Regex.Replace(StopDateTime, @"[^0-9]+", "");         
            //DateTime Stop_DateTime1 = DateTime.MinValue;
            //if (Start_DateTime == "")
            //{
            //if (Stop_DateTime == "")
            //{
            //    Stop_DateTime = "0";
            //}
            //if (Start_DateTime != "1900-01-01 0:00:00") 
            bool flag = false;         //Convert.ToInt32(Admission)
            //flag = _ServicesSoapClient.SetDrugRecord(UserId, VisitId, OrderSubNo, RepeatIndicator, OrderClass, OrderCode, OrderContent, Dosage, DosageUnits, Administration, StartDateTime, StopDateTime, 1, "", Frequency, FreqCounter, FreqInteval, FreqIntevalUnit, DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            //OrderSubNo = 1 修改 ZC 2015-05-19
            decimal test = (decimal)Dosage;
            flag = _ServicesSoapClient.SetDrugRecord(UserId, VisitId, 1, RepeatIndicator, OrderClass, OrderCode, OrderContent, test, DosageUnits, Administration, StartDateTime, StopDateTime, 1, "", Frequency, FreqCounter, FreqInteval, FreqIntevalUnit, DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //编辑
        public JsonResult DrugInfoEdit(string UserId, string VisitId, int OrderNo, int OrderSubNo, string OrderClass, int RepeatIndicator, string OrderCode, string OrderContent, double Dosage, string DosageUnitsCode, string Administration, DateTime StartDateTime, DateTime StopDateTime, int FreqCounter, string FreqCounterUnit, int FreqInteval, string FreqIntevalUnit, string DeptCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            /*对数据进行处理*/
            //var Start_DateTime = System.Text.RegularExpressions.Regex.Replace(StartDateTime, @"[^0-9]+", "");
            //var Stop_DateTime = System.Text.RegularExpressions.Regex.Replace(StopDateTime, @"[^0-9]+", "");
            //if (Start_DateTime == "")
            //{
            //    Start_DateTime = "0";
            //}
            //if (Stop_DateTime == "")
            //{
            //    Stop_DateTime = "0";
            //}
            decimal test = (decimal)Dosage;
            bool flag = false;         //Convert.ToInt32(Admission)
            flag = _ServicesSoapClient.UpdateDrugRecord(UserId, VisitId, OrderNo, OrderSubNo, RepeatIndicator, OrderClass, OrderCode, OrderContent, test, DosageUnitsCode, Administration, StartDateTime, StopDateTime, 1, "", FreqCounterUnit, FreqCounter, FreqInteval, FreqIntevalUnit, DeptCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag == true)
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

        //删除
        public JsonResult DrugInfoDelete(string UserId, string VisitId, int OrderNo, int OrderSubNo)
        {
            var res = new JsonResult();
            /*对数据进行处理*/

            int flag = 2;
            flag = _ServicesSoapClient.DeleteDrugRecord(UserId, VisitId, OrderNo, OrderSubNo);
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

        #endregion

        #region 集成
        //获取ip地址
        //public JsonResult GetIPAndPort(string Device)
        //{
        //    var res = new JsonResult();
        //    /*对数据进行处理*/

        //    string address = "";
        //    address = _ServicesSoapClient.GetIPAndPort(Device);
        //    res.Data = address;
        //    res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return res;
        //}

        //集成
        public JsonResult GetIntegrationData(string UserId, string PatientId, string StartDateTime, string HospitalCode)
        {
            //UserId: CDMISPatientId, PatientId: "10156471", StartDateTime: 0, HospitalCode: "HJZYY"
            HttpContext.Server.ScriptTimeout = 600;
            var res = new JsonResult();
            /*对数据进行处理*/
            if (StartDateTime == "0")
            {
                StartDateTime = "1900-01-01";
            }
            WebReferenceJC.resSetInfo resInfo = regionCenterWeb.GetPatient(UserId, PatientId, Convert.ToDateTime(StartDateTime), true, HospitalCode);

            res.Data = resInfo.Status.ToString();
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult GetIntegrationBasicData(string UserId, string PatientId, string HospitalCode)
        {
            //UserId: CDMISPatientId, PatientId: "10156471", StartDateTime: 0, HospitalCode: "HJZYY"
            HttpContext.Server.ScriptTimeout = 600;
            var res = new JsonResult();

            WebReferenceJC.resSetPatient resInfo = regionCenterWeb.GetBasicInfo(UserId, PatientId, HospitalCode);

            res.Data = resInfo.Status.ToString();
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region function CSQ

        //获取患者个人信息
        public Models.PatientBasicInfo GetPatientBasicInfo(string UserId)
        {
            Models.PatientBasicInfo Patient = new Models.PatientBasicInfo();
            ServiceReference.PatientBasicInfo basicInfo = new ServiceReference.PatientBasicInfo();
            basicInfo = _ServicesSoapClient.GetPatBasicInfo(UserId);
            Patient.UserId = basicInfo.UserId;
            Patient.UserName = basicInfo.UserName;
            Patient.Birthday = basicInfo.Birthday;
            Patient.Age = Convert.ToInt32(basicInfo.Age);
            Patient.Gender = basicInfo.Gender;
            Patient.GenderText = basicInfo.GenderText;
            Patient.BloodType = basicInfo.BloodTypeText;
            Patient.InsuranceType = basicInfo.InsuranceTypeText;
            Patient.Module = basicInfo.Module;
            return Patient;
        }

        //获取就诊信息列表，下拉框   门诊和住院
        public List<SelectListItem> GetClinicalInfoList(string PatientId)
        {
            DataSet ClinicalInfoListDs = _ServicesSoapClient.GetClinicalInfoList(PatientId);
            List<SelectListItem> ClinicalInfoList = new List<SelectListItem>();
            ClinicalInfoList.Add(new SelectListItem { Text = "", Value = "" });

            string DischargeDate = "";
            if (ClinicalInfoListDs.Tables.Count > 0)
            {

                foreach (DataRow dr in ClinicalInfoListDs.Tables[0].Rows)
                {
                    if (dr["DischargeDate"].ToString() != "9999/1/1 0:00:00")
                    {
                        DischargeDate = dr["DischargeDate"].ToString();
                    }
                    else
                    {
                        DischargeDate = "";
                    }
                    //ClinicalInfoList.Add(new SelectListItem { Text = dr["VisitId"].ToString() + "_" + dr["SortNo"].ToString() + "_" + dr["AdmissionDate"].ToString() + "_" + DischargeDate + "_" + dr["HospitalName"].ToString() + "_" + dr["DepartmentName"].ToString() + "_" + dr["Doctor"].ToString(), Value = dr["VisitId"].ToString() });
                    ClinicalInfoList.Add(new SelectListItem { Text = "住院_" + dr["AdmissionDate"].ToString() + "_" + DischargeDate + "_" + dr["HospitalName"].ToString() + "_" + dr["DepartmentName"].ToString() + "_" + dr["Doctor"].ToString(), Value = dr["VisitId"].ToString() });

                }

                foreach (DataRow dr in ClinicalInfoListDs.Tables[1].Rows)
                {
                    //ClinicalInfoList.Add(new SelectListItem { Text = dr["VisitId"].ToString() + "_" + dr["ClinicDate"].ToString() + "_" + dr["HospitalName"].ToString() + "_" + dr["DepartmentName"].ToString() + "_" + dr["Doctor"].ToString(), Value = dr["VisitId"].ToString() });
                    ClinicalInfoList.Add(new SelectListItem { Text = "门急诊_" + dr["ClinicDate"].ToString() + "_" + dr["HospitalName"].ToString() + "_" + dr["DepartmentName"].ToString() + "_" + dr["Doctor"].ToString(), Value = dr["VisitId"].ToString() });

                }
            }
            return ClinicalInfoList;
        }

        //获取住院信息列表，下拉框      转科
        public List<SelectListItem> GetInPatientInfoList(string PatientId)
        {
            DataSet ClinicalInfoListDs = _ServicesSoapClient.GetClinicalInfoList(PatientId);
            List<SelectListItem> ClinicalInfoList = new List<SelectListItem>();
            ClinicalInfoList.Add(new SelectListItem { Text = "请选择住院信息", Value = "" });

            //if (ClinicalInfoListDs.Tables.Count > 0)
            //{
            //    foreach (DataRow dr in ClinicalInfoListDs.Tables[0].Rows)
            //    {
            //        ClinicalInfoList.Add(new SelectListItem { Text = dr["AdmissionDate"].ToString() + "_" + dr["HospitalName"].ToString() + "_" + dr["DepartmentName"].ToString(), Value = dr["VisitId"].ToString() + "_" + dr["SortNo"].ToString() });
            //    }
            //}
            if (ClinicalInfoListDs.Tables.Count > 0)
            {
                DataRow[] rows = ClinicalInfoListDs.Tables[0].Select();
                if (rows.Length > 0)
                {
                    string VID = rows[0]["VisitId"].ToString();
                    //string VIDtemp;
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (VID == rows[i]["VisitId"].ToString())
                        {
                            continue;
                        }
                        else
                        {
                            ClinicalInfoList.Add(new SelectListItem { Text = rows[i - 1]["AdmissionDate"].ToString() + "_" + rows[i - 1]["HospitalName"].ToString() + "_" + rows[i - 1]["DepartmentName"].ToString(), Value = rows[i - 1]["VisitId"].ToString() + "_" + rows[i - 1]["SortNo"].ToString() + "_" + rows[i - 1]["AdmissionDate"].ToString() });
                            VID = rows[i]["VisitId"].ToString();
                        }
                    }
                    ClinicalInfoList.Add(new SelectListItem { Text = rows[rows.Length - 1]["AdmissionDate"].ToString() + "_" + rows[rows.Length - 1]["HospitalName"].ToString() + "_" + rows[rows.Length - 1]["DepartmentName"].ToString(), Value = rows[rows.Length - 1]["VisitId"].ToString() + "_" + rows[rows.Length - 1]["SortNo"].ToString() + "_" + rows[rows.Length - 1]["AdmissionDate"].ToString() });
                }
            }
            return ClinicalInfoList;
        }

        //获取住院信息列表，Table
        public List<ClinicalInfo> GetInPatientList(string UserId, string DoctorId)
        {
            DataSet ClinicalInfoListDs = _ServicesSoapClient.GetClinicalInfoList(UserId);
            List<ClinicalInfo> InPatientList = new List<Models.ClinicalInfo>();
            if (ClinicalInfoListDs.Tables.Count > 0)
            {
                foreach (DataRow dr in ClinicalInfoListDs.Tables[0].Rows)
                {
                    ClinicalInfo item = new ClinicalInfo();
                    item.VisitId = dr["VisitId"].ToString();

                    item.SortNo = Convert.ToInt32(dr["SortNo"]);
                    item.AdmissionDate = Convert.ToDateTime(dr["AdmissionDate"]);
                    item.DischargeDate = Convert.ToDateTime(dr["DischargeDate"]);
                    item.HospitalCode = dr["HospitalCode"].ToString();
                    item.HospitalName = dr["HospitalName"].ToString();
                    item.DepartmentCode = dr["Department"].ToString();
                    item.DepartmentName = dr["DepartmentName"].ToString();
                    item.Doctor = dr["Doctor"].ToString();
                    item.Creator = dr["Creator"].ToString();
                    if (item.Creator == DoctorId)
                    {
                        item.IsAllowed = true;
                    }
                    else
                    {
                        item.IsAllowed = false;
                    }
                    InPatientList.Add(item);
                }
            }
            return InPatientList;
        }

        //获取门诊信息列表，Table
        public List<ClinicalInfo> GetOutPatientList(string UserId, string DoctorId)
        {
            DataSet ClinicalInfoListDs = _ServicesSoapClient.GetClinicalInfoList(UserId);
            List<ClinicalInfo> OutPatientList = new List<Models.ClinicalInfo>();
            if (ClinicalInfoListDs.Tables.Count > 0)
            {
                foreach (DataRow dr in ClinicalInfoListDs.Tables[1].Rows)
                {
                    ClinicalInfo item = new ClinicalInfo();
                    item.VisitId = dr["VisitId"].ToString();
                    item.ClinicDate = Convert.ToDateTime(dr["ClinicDate"]);
                    item.HospitalCode = dr["HospitalCode"].ToString();
                    item.HospitalName = dr["HospitalName"].ToString();
                    item.DepartmentCode = dr["Department"].ToString();
                    item.DepartmentName = dr["DepartmentName"].ToString();
                    item.Doctor = dr["Doctor"].ToString();
                    item.Creator = dr["Creator"].ToString();
                    if (item.Creator == DoctorId)
                    {
                        item.IsAllowed = true;
                    }
                    else
                    {
                        item.IsAllowed = false;
                    }
                    OutPatientList.Add(item);
                }
            }
            return OutPatientList;
        }

        //转换日期
        private string ConvertDate(string iDate)
        {
            string sDate = "";
            if (iDate == "0")
            {
                return sDate;
            }
            else if (iDate.Length == 8)
            {
                sDate = iDate.Substring(0, 4) + "-" + iDate.Substring(4, 2) + "-" + iDate.Substring(6, 2);
                return sDate;
            }
            else
            {
                return sDate;
            }
        }

        //转换时间
        private string ConvertTime(string iTime)
        {
            string sTime = "";
            if (iTime == "0")
            {
                return sTime;
            }
            else if (iTime.Length == 6)
            {
                sTime = iTime.Substring(0, 2) + ":" + iTime.Substring(2, 2) + ":" + iTime.Substring(4, 2);
                return sTime;
            }
            else if (iTime.Length == 5)
            {
                sTime = "0" + iTime.Substring(0, 1) + ":" + iTime.Substring(1, 2) + ":" + iTime.Substring(3, 2);
                return sTime;
            }
            else
            {
                return sTime;
            }
        }

        //下拉框生成（改变选中值）
        public List<SelectListItem> GetTypeList(string Type, string Value)
        {
            DataSet typeset = _ServicesSoapClient.GetTypeList(Type);   //字典表

            List<SelectListItem> dropdownList = new List<SelectListItem>();
            dropdownList.Add(new SelectListItem { Text = "请选择", Value = "0" });
            foreach (System.Data.DataRow typerow in typeset.Tables[0].Rows)
            {
                dropdownList.Add(new SelectListItem { Text = typerow[1].ToString(), Value = typerow[0].ToString() });
            }

            if (Value != "")
            {
                string[] values = Value.Split(',');
                int vLength = values.Length;
                if (vLength > 1)
                {
                    for (int vnum = 0; vnum < vLength; vnum++)
                    {
                        foreach (var item in dropdownList)
                        {
                            if (values[vnum] == item.Value)
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in dropdownList)
                    {
                        if (Value == item.Value)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            else
            {

                foreach (var item in dropdownList)
                {
                    if (item.Value == "0")
                    {
                        item.Selected = true;
                    }
                }
            }
            return dropdownList;
        }

        //高血压药物类型下拉框
        public List<SelectListItem> GetHypertensionDrugTypeNameList(string selectedValue)
        {
            DataSet HypertensionDrugTypeNameDs = _ServicesSoapClient.GetHypertensionDrugTypeNameList();
            List<SelectListItem> HypertensionDrugTypeNameList = new List<SelectListItem>();
            HypertensionDrugTypeNameList.Add(new SelectListItem { Text = "请选择", Value = "0" });

            if (HypertensionDrugTypeNameDs != null)
            {
                DataTable HypertensionDrugTypeNameDt = HypertensionDrugTypeNameDs.Tables[0];
                foreach (DataRow DR in HypertensionDrugTypeNameDt.Rows)
                {
                    HypertensionDrugTypeNameList.Add(new SelectListItem { Text = DR["TypeName"].ToString(), Value = DR["Type"].ToString() });
                }
                if (selectedValue != "")
                {
                    foreach (var item in HypertensionDrugTypeNameList)
                    {
                        if (selectedValue == item.Value)
                        {
                            item.Selected = true;
                        }
                    }
                }
                else
                {
                    foreach (var item in HypertensionDrugTypeNameList)
                    {
                        if (item.Value == "0")
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            return HypertensionDrugTypeNameList;
        }

        //糖尿病药物类型下拉框
        public List<SelectListItem> GetDiabetesDrugTypeNameList(string selectedValue)
        {
            DataSet DiabetesDrugTypeNameDs = _ServicesSoapClient.GetDiabetesDrugTypeNameList();
            List<SelectListItem> DiabetesDrugTypeNameList = new List<SelectListItem>();
            DiabetesDrugTypeNameList.Add(new SelectListItem { Text = "请选择", Value = "0" });

            if (DiabetesDrugTypeNameDs != null)
            {
                DataTable DiabetesDrugTypeNameDt = DiabetesDrugTypeNameDs.Tables[0];
                foreach (DataRow DR in DiabetesDrugTypeNameDt.Rows)
                {
                    DiabetesDrugTypeNameList.Add(new SelectListItem { Text = DR["TypeName"].ToString(), Value = DR["Type"].ToString() });
                }
                if (selectedValue != "")
                {
                    foreach (var item in DiabetesDrugTypeNameList)
                    {
                        if (selectedValue == item.Value)
                        {
                            item.Selected = true;
                        }
                    }
                }
                else
                {
                    foreach (var item in DiabetesDrugTypeNameList)
                    {
                        if (item.Value == "0")
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            return DiabetesDrugTypeNameList;
        }

        //根据TypeName动态加载DrugNameList下拉框
        public JsonResult GetListbyTypeName(string TypeSelected, string Category)
        {
            var res = new JsonResult();
            List<string> DrugNameList = new List<string>();
            if (TypeSelected != "0")
            {
                DataSet DrugNameDS = new DataSet();
                if (Category == "Cm.MstHypertensionDrug")
                {
                    DrugNameDS = _ServicesSoapClient.GetHypertensionDrugNameList(TypeSelected);
                }
                if (Category == "Cm.MstDiabetesDrug")
                {
                    DrugNameDS = _ServicesSoapClient.GetDiabetesDrugNameList(TypeSelected);
                }
                if (Category == "Cm.MstLipidDrug")
                {
                    DrugNameDS = _ServicesSoapClient.GetLipidDrugNameList(TypeSelected);
                }
                if (Category == "Cm.MstUricAcidReductionDrug")
                {
                    DrugNameDS = _ServicesSoapClient.GetAcidDrugNameList(TypeSelected);
                }
                DataTable DrugNameDT = DrugNameDS.Tables[0];
                foreach (DataRow DR in DrugNameDT.Rows)
                {
                    DrugNameList.Add(DR["Name"].ToString() + "|" + DR["Code"].ToString());
                }
            }
            res.Data = DrugNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //动态加载药物类型下拉框
        public JsonResult GetTypeNameList(string Category)
        {
            var res = new JsonResult();
            List<string> TypeNameList = new List<string>();
            DataSet TypeNameDS = new DataSet();
            if (Category == "MstHypertensionDrug")
            {
                TypeNameDS = _ServicesSoapClient.GetHypertensionDrugTypeNameList();
            }
            if (Category == "MstDiabetesDrug")
            {
                TypeNameDS = _ServicesSoapClient.GetDiabetesDrugTypeNameList();
            }
            if (Category == "MstLipidDrug")
            {
                TypeNameDS = _ServicesSoapClient.GetLipidDrugTypeNameList();
            }
            if (Category == "MstUricAcidReductionDrug")
            {
                TypeNameDS = _ServicesSoapClient.GetUricAcidReductionDrugTypeNameList();
            }
            DataTable TypeNameDT = TypeNameDS.Tables[0];
            
            foreach (DataRow DR in TypeNameDT.Rows)
            {
                TypeNameList.Add(DR["TypeName"].ToString() + "|" + DR["Type"].ToString());
            }
            res.Data = TypeNameList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //添加口服药数据
        public JsonResult SetDrugData(string PatientId, string CategoryCode, string ItemCode, int ItemSeq, string Value, int SortNo)
        {
            var res = new JsonResult();
            var user = Session["CurrentUser"] as UserAndRole;
            bool Flag = true;
            Flag = _ServicesSoapClient.SetBasicInfoDetail(PatientId, CategoryCode, ItemCode, ItemSeq, Value, "", SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = Flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //删除口服药数据      
        public JsonResult DeleteDrugData(string PatientId, string CategoryCode, string ItemCode, int ItemSeq)
        {
            var res = new JsonResult();
            int flag = 0;
            flag = _ServicesSoapClient.DeleteModule(PatientId, CategoryCode, ItemCode, ItemSeq);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //同步部分模块信息(高血压)
        public JsonResult SynBasicInfoDetail(string PatientId)
        {
            var res = new JsonResult();
            List<string> SynDetailList = new List<string>();
            DataSet DS_SynDetail = new DataSet();
            DS_SynDetail = _ServicesSoapClient.SynBasicInfoDetail(PatientId);
            DataTable SynDetailDT = DS_SynDetail.Tables[0];
            if (SynDetailDT.Rows.Count != 0)
            {
                DataRow DR_1 = SynDetailDT.Rows[0];
                {
                    SynDetailList.Add(DR_1["Name1"].ToString() + "|" + DR_1["Value1"].ToString() + "|" + DR_1["Date"].ToString().Substring(0, 10));
                    SynDetailList.Add(DR_1["Name2"].ToString() + "|" + DR_1["Value2"].ToString() + "|" + DR_1["Date"].ToString().Substring(0, 10));
                    SynDetailList.Add(DR_1["Name3"].ToString() + "|" + DR_1["Value3"].ToString() + "|" + DR_1["Date"].ToString().Substring(0, 10));
                }
            }
            SynDetailDT = DS_SynDetail.Tables[1];
            foreach (DataRow DR_2 in SynDetailDT.Rows)
            {
                SynDetailList.Add(DR_2["Code"].ToString() + "|" + DR_2["Value"].ToString() + "|" + DR_2["Date"].ToString().Substring(0, 10));
            }
            res.Data = SynDetailList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //同步部分模块信息(糖尿病)
        public JsonResult SynDiabetesInfoDetail(string PatientId)
        {
            var res = new JsonResult();
            List<string> SynDetailList = new List<string>();
            DataSet DS_SynDetail = new DataSet();
            DS_SynDetail = _ServicesSoapClient.SynBasicInfoDetailForM2(PatientId);
            DataTable SynDetailDT = DS_SynDetail.Tables[0];
            foreach (DataRow DR_2 in SynDetailDT.Rows)
            {
                SynDetailList.Add(DR_2["Code"].ToString() + "|" + DR_2["Value"].ToString() + "|" + DR_2["Date"].ToString().Substring(0, 10));
            }
            res.Data = SynDetailList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //同步部分模块信息(心衰)
        public JsonResult SynBasicInfoDetailForM3(string PatientId)
        {
            var res = new JsonResult();
            List<string> SynDetailList = new List<string>();
            DataSet DS_SynDetail = new DataSet();
            DS_SynDetail = _ServicesSoapClient.SynBasicInfoDetailForM3(PatientId);
            DataTable SynDetailDT = DS_SynDetail.Tables[0];
            /*if (SynDetailDT.Rows.Count != 0)
            {
                DataRow DR_1 = SynDetailDT.Rows[0];
                {
                    SynDetailList.Add(DR_1["ItemCode"].ToString() + "|" + DR_1["Value"].ToString());
                }
            }*/
            SynDetailDT = DS_SynDetail.Tables[1];
            foreach (DataRow DR_2 in SynDetailDT.Rows)
            {
                SynDetailList.Add(DR_2["Code"].ToString() + "|" + DR_2["Value"].ToString() + "|" + DR_2["Date"].ToString().Substring(0, 10));
            }
            res.Data = SynDetailList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //加载操作医生未负责患者列表
        public List<Models.PatientBasicInfo> GetPotentialPatientList(string DoctorId)
        {
            DataSet patientListDs = _ServicesSoapClient.GetPatListByDoctorId(DoctorId);
            List<Models.PatientBasicInfo> patientList = new List<Models.PatientBasicInfo>();
            if (patientListDs.Tables.Count != 0)
            {
                DataTable patientListDt = patientListDs.Tables[0];

                foreach (DataRow dr in patientListDt.Rows)
                {
                    Models.PatientBasicInfo item = new Models.PatientBasicInfo();
                    item.UserId = dr["UserId"].ToString();
                    item.UserName = dr["UserName"].ToString();
                    item.Gender = dr["Gender"].ToString();
                    item.Age = Convert.ToInt32(dr["Age"]);
                    item.Module = dr["Module"].ToString();

                    patientList.Add(item);
                }
            }
            return patientList;
        }

        //加载患者详细信息
        public BasicProfileViewModel GetPatientInfoDetail(ref BasicProfileViewModel model)
        {
            string PatientId = model.Patient.UserId;
            var basicInfoDtl = _ServicesSoapClient.GetPatientDetailInfo(PatientId);


            model.Patient.IDNo = basicInfoDtl.IDNo;
            model.Occupation = basicInfoDtl.Occupation;
            model.Nationality = basicInfoDtl.Nationality;
            model.Phone = basicInfoDtl.PhoneNumber;
            model.Address = basicInfoDtl.HomeAddress;
            model.EmergencyContact = basicInfoDtl.EmergencyContact;
            model.EmergencyContactNumber = basicInfoDtl.EmergencyContactPhoneNumber;

            return model;
        }

        //加载症状列表
        public SymptomsProfileViewModel GetSymptomInfoList(ref SymptomsProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            DataSet SymptomsListds = _ServicesSoapClient.GetSymptomsList(UserId, VisitId);
            if (SymptomsListds.Tables.Count != 0)
            {
                DataTable SymptomsListdt = SymptomsListds.Tables[0];
                List<SymptomInfo> list = new List<Models.SymptomInfo>();
                int max = 0;
                foreach (DataRow dr in SymptomsListdt.Rows)
                {
                    SymptomInfo item = new SymptomInfo();
                    item.SymptomsNo = Convert.ToInt32(dr["SynptomsNo"]);
                    item.SymptomsType = dr["SymptomsType"].ToString();
                    item.SymptomsTypeName = dr["SymptomsTypeName"].ToString();
                    item.SymptomsCode = dr["SymptomsCode"].ToString();
                    item.SymptomsName = dr["SymptomsName"].ToString();
                    item.Description = dr["Description"].ToString();
                    item.RecordDate = ConvertDate(dr["RecordDate"].ToString());

                    item.RecordTime = ConvertTime(dr["RecordTime"].ToString());
                    item.Creator = dr["Creator"].ToString();
                    if (item.Creator == DoctorId)
                    {
                        item.IsAllowed = true;
                    }
                    else
                    {
                        item.IsAllowed = false;
                    }
                    list.Add(item);
                    max = Convert.ToInt32(dr["SynptomsNo"]);

                }
                model.MaxSortNo = max;
                model.SymptomsList = list;
            }
            return model;
        }

        //加载诊断列表
        public DiagnosisInfoProfileViewModel GetDiagnosisInfoList(ref DiagnosisInfoProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            DataSet DiagnosisListds = _ServicesSoapClient.GetDiagnosisInfoList(UserId, VisitId);
            //DataTable DiagnosisListdt = _ServicesSoapClient.GetDiagnosisInfoList(UserId, VisitId).Tables[0];
            if (DiagnosisListds.Tables.Count != 0)
            {
                DataTable DiagnosisListdt = DiagnosisListds.Tables[0];
                List<DiagnosisInfo> list = new List<Models.DiagnosisInfo>();
                int max = 0;
                foreach (DataRow dr in DiagnosisListdt.Rows)
                {
                    DiagnosisInfo item = new DiagnosisInfo();
                    item.DiagnosisType = dr["DiagnosisType"].ToString();
                    item.DiagnosisTypeName = dr["DiagnosisTypeName"].ToString();
                    item.DiagnosisNo = dr["DiagnosisNo"].ToString();
                    item.Type = dr["Type"].ToString();
                    item.TypeName = dr["TypeName"].ToString();
                    item.DiagnosisCode = dr["DiagnosisCode"].ToString();
                    item.DiagnosisName = dr["DiagnosisName"].ToString();
                    item.Description = dr["Description"].ToString();
                    item.RecordDate = dr["RecordDate"].ToString();
                    item.Creator = dr["Creator"].ToString();
                    if (item.Creator == DoctorId)
                    {
                        item.IsAllowed = true;
                    }
                    else
                    {
                        item.IsAllowed = false;
                    }
                    list.Add(item);
                    if (Convert.ToInt32(dr["DiagnosisNo"]) > max)
                    {
                        max = Convert.ToInt32(dr["DiagnosisNo"]);
                    }
                }
                model.MaxSortNo = max;
                model.DiagnosisList = list;
            }
            return model;
        }

        //加载检查列表
        public ExaminationProfileViewModel GetExaminationInfoList(ref ExaminationProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            DataSet ExaminationListds = _ServicesSoapClient.GetExaminationList(UserId, VisitId);
            if (ExaminationListds.Tables.Count != 0)
            {
                DataTable ExaminationListdt = ExaminationListds.Tables[0];
                List<ExaminationInfo> list = new List<ExaminationInfo>();
                int max = 0;
                foreach (DataRow dr in ExaminationListdt.Rows)
                {
                    ExaminationInfo item = new ExaminationInfo();
                    item.SortNo = dr["SortNo"].ToString();
                    item.ExamType = dr["ExamType"].ToString();
                    item.ExamTypeName = dr["ExamTypeName"].ToString();
                    item.ExamDate = dr["ExamDate"].ToString();
                    item.ItemCode = dr["ItemCode"].ToString();
                    item.ExamName = dr["ItemName"].ToString();
                    item.ExamPara = dr["ExamPara"].ToString();
                    item.Description = dr["Description"].ToString();
                    item.Impression = dr["Impression"].ToString();
                    item.Recommendation = dr["Recommendation"].ToString();
                    //ZC 2015-06-15
                    if (dr["IsAbnormalCode"].ToString() == "")
                    {
                        item.IsAbnormalCode = 0;
                    }
                    else
                    {
                        item.IsAbnormalCode = Convert.ToInt32(dr["IsAbnormalCode"]);
                    } 
                    item.IsAbnormal = dr["IsAbnormal"].ToString();
                    item.StatusCode = dr["StatusCode"].ToString();
                    item.Status = dr["Status"].ToString();
                    item.ReportDate = dr["ReportDate"].ToString();
                    item.ImageURL = dr["ImageURL"].ToString();
                    item.DeptCode = dr["DeptCode"].ToString();
                    item.Creator = dr["Creator"].ToString();
                    if (item.Creator == DoctorId)
                    {
                        item.IsAllowed = true;
                    }
                    else
                    {
                        item.IsAllowed = false;
                    }
                    list.Add(item);
                    max = Convert.ToInt32(dr["SortNo"]);
                }
                model.ExaminationList = list;
                model.MaxSortNo = max;
            }
            return model;
        }

        //加载检查参数列表
        public ExaminationProfileViewModel GetExamDetailInfoList(ref ExaminationProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            string SortNo = model.ExamInfo.SortNo;
            string ItemCode = model.ExamInfo.ItemCode;
            DataSet ExamDtlDs = _ServicesSoapClient.GetExamDtlList(UserId, VisitId, SortNo, ItemCode);
            if (ExamDtlDs.Tables.Count != 0)
            {
                DataTable ExamDtlDt = ExamDtlDs.Tables[0];
                List<DetailInfo> list = new List<DetailInfo>();
                //int max = 0;
                foreach (DataRow dr in ExamDtlDt.Rows)
                {
                    DetailInfo detailInfo = new DetailInfo();

                    detailInfo.Code = dr["Code"].ToString();
                    detailInfo.ItemName = dr["Name"].ToString();
                    detailInfo.Value = dr["Value"].ToString();
                    detailInfo.IsAbnormalCode = Convert.ToInt32(dr["IsAbnormalCode"]);
                    detailInfo.IsAbnormal = dr["IsAbnormal"].ToString();
                    detailInfo.UnitCode = dr["UnitCode"].ToString();
                    detailInfo.Unit = dr["Unit"].ToString();
                    //detailInfo.Code=dr["Code"].ToString();
                    detailInfo.Creator = dr["Creator"].ToString();
                    if (detailInfo.Creator == DoctorId)
                    {
                        detailInfo.IsAllowed = true;
                    }
                    else
                    {
                        detailInfo.IsAllowed = false;
                    }
                    list.Add(detailInfo);
                }
                model.ExamInfo.Detail = list;
            }
            return model;
        }

        //加载检查参数项目下拉框
        public List<SelectListItem> GetExamSubItemNameList(string ItemCode)
        {
            DataTable ExamSubItemDt = _ServicesSoapClient.GetExamSubItemNameList(ItemCode).Tables[0];
            List<SelectListItem> ExamSubItemNameList = new List<SelectListItem>();
            foreach (DataRow DR in ExamSubItemDt.Rows)
            {
                ExamSubItemNameList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["SubCode"].ToString() });
            }
            return ExamSubItemNameList;
        }

        //加载化验信息
        public LabTestProfileViewModel GetLabTestInfoList(ref LabTestProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            DataSet LabTestListds = _ServicesSoapClient.GetLabTestList(UserId, VisitId);
            if (LabTestListds != null)
            {
                if (LabTestListds.Tables.Count != 0)
                {
                    DataTable LabTestListdt = LabTestListds.Tables[0];
                    List<LabTestInfo> list = new List<LabTestInfo>();
                    string max = "0";
                    foreach (DataRow dr in LabTestListdt.Rows)
                    {
                        LabTestInfo item = new LabTestInfo();
                        item.SortNo = dr["SortNo"].ToString();
                        item.LabItemType = dr["LabItemType"].ToString();
                        item.LabItemTypeName = dr["LabItemTypeName"].ToString();
                        item.LabItemCode = dr["LabItemCode"].ToString();
                        item.LabItemName = dr["LabItemName"].ToString();
                        item.ExamDate = dr["LabTestDate"].ToString();
                        item.StatusCode = dr["StatusCode"].ToString();
                        item.Status = dr["Status"].ToString();
                        item.ReportDate = dr["ReportDate"].ToString();
                        item.Creator = dr["Creator"].ToString();
                        if (item.Creator == DoctorId)
                        {
                            item.IsAllowed = true;
                        }
                        else
                        {
                            item.IsAllowed = false;
                        }
                        list.Add(item);
                        max = dr["SortNo"].ToString();
                    }
                    model.LabTestList = list;
                    model.MaxSortNo = max;
                }
            }
            return model;
        }

        //加载化验参数列表
        public LabTestProfileViewModel GetLabTestInfoDetailList(ref LabTestProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            string SortNo = model.LabTestInfo.SortNo;
            string LabItemType = model.LabTestInfo.LabItemType;
            string LabItemCode = model.LabTestInfo.LabItemCode;
            //string Code = LabItemType + "**" + LabItemCode;

            DataSet labTestDtlDs = _ServicesSoapClient.GetLabTestDtlList(UserId, VisitId, SortNo);
            if (labTestDtlDs != null)
            {
                if (labTestDtlDs.Tables.Count != 0)
                {
                    DataTable labTestDtlDt = labTestDtlDs.Tables[0];
                    List<DetailInfo> list = new List<DetailInfo>();
                    //int max = 0;
                    foreach (DataRow dr in labTestDtlDt.Rows)
                    {
                        DetailInfo detailInfo = new DetailInfo();
                        detailInfo.Code = dr["Code"].ToString();
                        detailInfo.ItemName = dr["Name"].ToString();
                        detailInfo.Value = dr["Value"].ToString();
                        detailInfo.IsAbnormalCode = Convert.ToInt32(dr["IsAbnormalCode"]);
                        detailInfo.IsAbnormal = dr["IsAbnormal"].ToString();
                        detailInfo.UnitCode = dr["UnitCode"].ToString();
                        detailInfo.Unit = dr["Unit"].ToString();
                        detailInfo.Creator = dr["Creator"].ToString();
                        if (detailInfo.Creator == DoctorId)
                        {
                            detailInfo.IsAllowed = true;
                        }
                        else
                        {
                            detailInfo.IsAllowed = false;
                        }
                        list.Add(detailInfo);
                    }
                    model.LabTestInfo.Detail = list;
                }
            }
            return model;
        }

        //加载药物治疗信息
        public DrugInfoProfileViewModel GetDrugInfoList(ref DrugInfoProfileViewModel model, string DoctorId)
        {
            string UserId = model.UserId;
            string VisitId = model.VisitId;
            DataSet DrugRecordListds = _ServicesSoapClient.GetDrugRecordList(UserId, VisitId);
            if (DrugRecordListds != null)
            {
                if (DrugRecordListds.Tables.Count != 0)
                {
                    DataTable DrugRecordListdt = DrugRecordListds.Tables[0];
                    List<DrugInfo> list = new List<DrugInfo>();
                    int max = 0;
                    foreach (DataRow dr in DrugRecordListdt.Rows)
                    {
                        DrugInfo item = new DrugInfo();
                        item.OrderNo = Convert.ToInt32(dr["OrderNo"]);
                        item.OrderSubNo = Convert.ToInt32(dr["OrderSubNo"]);
                        item.RepeatIndicatorCode = Convert.ToInt32(dr["RepeatIndicatorCode"]);
                        item.RepeatIndicator = dr["RepeatIndicator"].ToString();
                        item.OrderClassCode = dr["OrderClassCode"].ToString();
                        item.OrderClass = dr["OrderClass"].ToString();
                        item.OrderCode = dr["OrderCode"].ToString();
                        item.OrderContent = dr["OrderContent"].ToString();
                        item.Dosage = dr["Dosage"].ToString();
                        item.DosageUnitsCode = dr["DosageUnitsCode"].ToString();
                        item.DosageUnits = dr["DosageUnits"].ToString();
                        item.AdministrationCode = dr["AdministrationCode"].ToString();
                        item.Administration = dr["Administration"].ToString();
                        item.StartDateTime = dr["StartDateTime"].ToString();
                        item.StopDateTime = dr["StopDateTime"].ToString();
                        item.Frequency = dr["Frequency"].ToString();
                        item.FreqCounter = Convert.ToInt32(dr["FreqCounter"]);
                        item.FreqInteval = Convert.ToInt32(dr["FreqInteval"]);
                        item.FreqIntevalUnitCode = dr["FreqIntevalUnitCode"].ToString();
                        item.FreqIntevalUnit = dr["FreqIntevalUnit"].ToString();
                        item.DeptCode = dr["DeptCode"].ToString();
                        item.Creator = dr["Creator"].ToString();
                        if (item.Creator == DoctorId)
                        {
                            item.IsAllowed = true;
                        }
                        else
                        {
                            item.IsAllowed = false;
                        }
                        list.Add(item);
                        max = Convert.ToInt32(dr["OrderNo"]);
                    }
                    model.DrugRecordList = list;
                    model.MaxSortNo = max;
                }
            }
            return model;
        }

        //加载化验参数项目下拉框
        public List<SelectListItem> GetLabTestSubItemNameList()
        {
            DataTable LabTestSubItemNameDt = _ServicesSoapClient.GetLabTestSubItemNameList().Tables[0];
            List<SelectListItem> LabTestSubItemNameList = new List<SelectListItem>();
            foreach (DataRow DR in LabTestSubItemNameDt.Rows)
            {
                LabTestSubItemNameList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["CodeSubCode"].ToString() });
            }
            return LabTestSubItemNameList;
        }
        #endregion

        #region 分配健康专员
        public ActionResult HealthCoachManagement(string PatientId)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                ModuleManagementViewModel MMVM = new ModuleManagementViewModel();
                if (PatientId != null && PatientId != "")
                {
                    MMVM.PatientId = PatientId;
                    if (user.Role == "Doctor")
                    {
                        MMVM.HealthCoachInfoList = GetHealthCareList(MMVM.PatientId);
                        MMVM.HealthCoachList = GetHealthCoachInfoList("HealthCoach");
                    }
                    else
                    {
                        MMVM.HealthCoachInfoList = GetDoctorCareInfo(MMVM.PatientId);
                        MMVM.HealthCoachList = GetHealthCoachInfoList("Doctor");
                    }
                }
                return View(MMVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //UpdateModuleInfo 修改某患者某模块的负责医生
        public JsonResult UpdateHCInfo(string PatientId, string DoctorId, string Seq, string PreDocId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int Ret = 0;
            if (DoctorId != "0")
            {
                Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, "HC", "Doctor", Convert.ToInt32(Seq), DoctorId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                Ret = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, "HM1", PatientId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
            }
            if (PreDocId != "0")
            {
                Ret = _ServicesSoapClient.DeletePatient(PreDocId, "HM1", PatientId);
            }
            if (Ret == 1)
            {
                string planNo = _ServicesSoapClient.GetExecutingPlanByModule(PatientId, "M1");
                if (planNo != null && planNo != "")
                {
                    _ServicesSoapClient.UpdatePlanStatus(planNo, 4, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                }
            }
            res.Data = Ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region<" function ">
        public List<HealthCoach> GetHealthCareList(string PatientId)
        {
            List<HealthCoach> hclist = new List<HealthCoach>();
            DataTable hcOfPat = _ServicesSoapClient.GetConForPatient(PatientId, "HC").Tables[0];
            foreach (DataRow row in hcOfPat.Rows)
            {
                HealthCoach hc = new HealthCoach()
                {
                    ItemSeq = row[2].ToString(),
                    HealthCoachId = row[0].ToString(),
                    HealthCoachName = _ServicesSoapClient.GetUserName(row[0].ToString()),
                    HCDivName = "HC" + row[2].ToString() + "Div",
                    DataTableName = "HC" + row[2].ToString() + "DataTable",
                    HealthCoachList = GetHealthCoachInfoList("HealthCoach")
                };
                hclist.Add(hc);
            }
            if (hclist.Count == 0)
            {
                hclist.Add(new HealthCoach { ItemSeq = "1", HealthCoachId = "0", HealthCoachName = "", HCDivName = "HC1Div", DataTableName = "HC1DataTable", HealthCoachList = GetHealthCoachInfoList("HealthCoach") });
            }
            return hclist;
        }

        public List<HealthCoach> GetDoctorCareInfo(string PatientId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            List<HealthCoach> dclist = new List<HealthCoach>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://121.43.107.106:9000/");
            HttpResponseMessage response = client.GetAsync("Api/v1/Users/HModulesByID?PatientId=" + PatientId + "&DoctorId=" + user.UserId).Result;
            if (response.IsSuccessStatusCode)
            {
                if (response.Content.ReadAsStringAsync().Result != "[]")
                {
                    string[] Modules = response.Content.ReadAsStringAsync().Result.Split(new string[] { "},{", "[{\"", "\"}]" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Module in Modules)
                    {
                        string[] Detail = Module.Split(new string[] { "\",\"", "\":\"" }, StringSplitOptions.RemoveEmptyEntries);
                        DataTable dcOfPat = _ServicesSoapClient.GetConForPatient(PatientId, Detail[1].Substring(1)).Tables[0];
                        foreach (DataRow line in dcOfPat.Rows)
                        {
                            HealthCoach dc = new HealthCoach()
                            {
                                ItemSeq = line[2].ToString(),
                                HealthCoachId = line[0].ToString(),
                                HealthCoachName = _ServicesSoapClient.GetUserName(line[0].ToString()),
                                HCDivName = Detail[1],
                                DataTableName = Detail[3],
                                HealthCoachList = GetHealthCoachInfoList("Doctor")
                            };
                            dclist.Add(dc);
                        }
                        if (dcOfPat.Rows.Count == 0)
                        {
                            HealthCoach dc = new HealthCoach()
                            {
                                ItemSeq = "",
                                HealthCoachId = "0",
                                HealthCoachName = "",
                                HCDivName = Detail[1],
                                DataTableName = Detail[3],
                                HealthCoachList = GetHealthCoachInfoList("Doctor")
                            };
                            dclist.Add(dc);
                        }
                    }
                }
            }
            return dclist;
        }

        //获取健康专员列表
        public List<DoctorAndHCInfo> GetHealthCoachInfoList(string Type)
        {
            DataTable docList = _ServicesSoapClient.GetActiveUserByRole(Type).Tables[0];
            List<DoctorAndHCInfo> DoctorList = new List<DoctorAndHCInfo>();
            foreach (DataRow DR in docList.Rows)
            {
                DoctorList.Add(new DoctorAndHCInfo { DoctorId = DR["UserId"].ToString(), DoctorName = DR["UserName"].ToString(), Hospital = DR["Hospital"].ToString(), Dept = DR["Dept"].ToString() });
            }
            return DoctorList;
        }
        #endregion
        
    }
}
