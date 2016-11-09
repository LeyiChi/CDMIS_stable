using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Data;
using CDMIS.Models;
using System.IO;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class ManagementController : Controller
    {
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
        //
        // GET: /UserManagement/
        //
        // GET: /UserManagement/

        static string _PatientId { get; set; }
        static string _ErrorMSG { get; set; }

        #region 用户管理
        //用户管理
        public ActionResult Index()
        {
            UserViewModel userVM = new UserViewModel();
            GetUserInfoList("", "", ref userVM);
            return View(userVM);
        }

        [HttpPost]
        public ActionResult UserPartialView(UserViewModel userVM)
        {
            try
            {
                if (userVM.SearchUserId == null)
                {
                    userVM.SearchUserId = "";
                }
                if (userVM.SearchUserName == null)
                {
                    userVM.SearchUserName = "";
                }
                GetUserInfoList(userVM.SearchUserId, userVM.SearchUserName, ref userVM);
                return View(userVM);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult Index(UserViewModel piUserViewData)
        {
            if (ModelState.IsValid)
            {
                var user = Session["CurrentUser"] as UserAndRole;
                string UserId = piUserViewData.Patient.UserId;
                string UserName = piUserViewData.Patient.UserName;
                if (UserName == null)
                {
                    UserName = "";
                }
                string Password = piUserViewData.Patient.Password;
                if (Password == null)
                {
                    Password = "";
                }
                string Class = piUserViewData.Patient.Class;
                string strEndDate = piUserViewData.Patient.EndDate;
                int EndDate = 0;
                EndDate = Convert.ToInt32(strEndDate.Replace("-", ""));

                //往数据库插数据
                //

                bool SetUserInfoFlag = _ServicesSoapClient.SetMstUserUM(UserId, UserName, Password, Class, EndDate, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (SetUserInfoFlag)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(piUserViewData);
        }

        public ActionResult DoctorManagement()
        {
            DoctorViewModel doctorVM = new DoctorViewModel();
            GetDoctorInfoList("", "", ref doctorVM);
            return View(doctorVM);
        }
        //权限分配
        public ActionResult RoleToAuthority()
        {
            Role2AuthorityViewModel role2AuthorityVM = new Role2AuthorityViewModel();
            GetAllAuthorityList(ref role2AuthorityVM);
            return View(role2AuthorityVM);
        }

        [HttpPost]
        public ActionResult r2aPartialView(Role2AuthorityViewModel r2aVM)
        {
            try
            {
                GetRoleAuthorityList(r2aVM.RoleNameSelected, ref r2aVM);
                return View(r2aVM);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        #endregion

        #region 患者模块管理
        public ActionResult ModuleManagement(string PatientId)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                ModuleManagementViewModel MMVM = new ModuleManagementViewModel();
                if (PatientId != null && PatientId != "")
                {
                    MMVM.PatientId = PatientId;
                    MMVM.ModuleInfoList = GetModuleAndDoctorInfo(MMVM.PatientId);
                    MMVM.HealthCoachInfoList = GetHealthCareList(MMVM.PatientId);
                }
                //MMVM.ModuleInfoList = GetModuleAndDoctorInfo();
                return View(MMVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ModuleManagement(ModuleManagementViewModel MMVM)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                MMVM.ModuleInfoList = GetModuleAndDoctorInfo(MMVM.PatientId);
                MMVM.HealthCoachInfoList = GetHealthCareList(MMVM.PatientId);
                return View(MMVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //UpdateModuleInfo 修改某患者某模块的负责医生
        public JsonResult UpdateModuleInfo(string PatientId, string Module, string DoctorId, string Seq, string PreDocId)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int Ret = 0;
            Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, Module, "Doctor", Convert.ToInt32(Seq), DoctorId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
            Ret = _ServicesSoapClient.DeletePatient(PreDocId, Module, PatientId);
            Ret = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, Module, PatientId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
            res.Data = Ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //UpdateModuleInfo 修改某患者某模块的负责医生
        public JsonResult UpdateHCInfo(string PatientId, string DoctorId, string Seq, string PreDocId, string Module)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int Ret = 0;
            var ModuleTable = _ServicesSoapClient.GetModulesBoughtByPId(PatientId);
            if (DoctorId != "0")
            {
                if (user.Role == "Doctor")
                {
                    Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, "HC", "Doctor", Convert.ToInt32(Seq), DoctorId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                    foreach (DataRow Row in ModuleTable.Tables[0].Rows)
                    {
                        Ret = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, "H" + Row[0].ToString(), PatientId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                        Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, "H" + Row[0].ToString(), "InvalidFlag", 1, "0", "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                        Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, "H" + Row[0].ToString(), "Doctor", 1, DoctorId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                    }
                }
                else
                {
                    Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, Module.Substring(1), "Doctor", 1, DoctorId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                    Ret = _ServicesSoapClient.SetBasicInfoDetail(PatientId, Module.Substring(1), "InvalidFlag", 1, "0", "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                    Ret = _ServicesSoapClient.SetPsDoctorDetailOnPat(DoctorId, Module.Substring(1), PatientId, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) ? 1 : 0;
                }
            }
            if (PreDocId != "0")
            {
                if (user.Role == "Doctor")
                {
                    foreach (DataRow Row in ModuleTable.Tables[0].Rows)
                    {
                        Ret = _ServicesSoapClient.DeletePatient(PreDocId, "H" + Row[0].ToString(), PatientId);
                    }
                }
                else
                {
                    Ret = _ServicesSoapClient.DeletePatient(PreDocId, Module.Substring(1), PatientId);
                }
            }
            if (Ret == 1)
            {
                if (user.Role == "HealthCoach")
                {
                    foreach (DataRow Row in ModuleTable.Tables[0].Rows)
                    {
                        string planNo = _ServicesSoapClient.GetExecutingPlanByModule(PatientId, Row[0].ToString());
                        if (planNo != null && planNo != "")
                        {
                            _ServicesSoapClient.UpdatePlanStatus(planNo, 4, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        }
                    }
                }
            }
            res.Data = Ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        public ActionResult QualificationCheck()
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                QualiCheckViewModel CheckView = new QualiCheckViewModel();
                CheckView.QualiCheckList = SetQualiCheckList();
                return View(CheckView);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region<" function ">
        #region 用户管理
        //查询输出所有用户信息列表
        public void GetUserInfoList(string id, string name, ref UserViewModel userInfoVM)
        {
            List<User> userList = new List<User>();
            int Row = 0;
            String UId = "";
            String PhoneNo = "";
            System.Data.DataTable userListDT = _ServicesSoapClient.GetUserInfoList(id, name).Tables[0];
            foreach (DataRow row in userListDT.Rows)
            {
                UId = row["UserId"].ToString();
                PhoneNo = _ServicesSoapClient.GetPhoneNoByUserId(UId);
                userList.Add(new User { UserId = row["UserId"].ToString(), UserName = row["UserName"].ToString(), Password = row["Password"].ToString(), Class = row["Class"].ToString(), ClassName = row["ClassName"].ToString(), StartDate = row["StartDate"].ToString(), EndDate = row["EndDate"].ToString().Substring(0, 4) + "-" + row["EndDate"].ToString().Substring(4, 2) + "-" + row["EndDate"].ToString().Substring(6, 2), PhoneNo = PhoneNo });
                Row++;
            }

            userInfoVM.UserList = userList;
            userInfoVM.RowCount = Row;
        }

        //获取用户基本信息
        public JsonResult GetUserInfoById(string UId)
        {
            var res = new JsonResult();

            string ret = "";
            string Class = "";
            System.Data.DataTable userListDT = _ServicesSoapClient.GetUserInfoList(UId, "").Tables[0];
            string UserName = userListDT.Rows[0]["UserName"].ToString();
            string Password = userListDT.Rows[0]["Password"].ToString();
            string EndDate = userListDT.Rows[0]["EndDate"].ToString();
            ret = UserName + "_" + Password + "_" + EndDate;
            foreach (DataRow row in userListDT.Rows)
            {
                Class = row["Class"].ToString();
                ret = ret + "_" + Class;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取用户基本信息
        public JsonResult DeleteRoleData(string UId, string userClassCode)
        {
            var res = new JsonResult();
            int ret = 2;
            ret = _ServicesSoapClient.DeleteRoleData(UId, userClassCode);
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取默认密码有效期
        public JsonResult GetDefaultEndTime()
        {
            var res = new JsonResult();
            string curDateTime = _ServicesSoapClient.GetServerTime();
            string curYear = curDateTime.Substring(0, 4);
            int endYear = Convert.ToInt32(curYear) + 1;
            string strEndYear = endYear.ToString();
            string endMonth = curDateTime.Substring(5, 2);
            string endDay = curDateTime.Substring(8, 2);

            res.Data = strEndYear + '-' + endMonth + '-' + endDay;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取手机号
        public JsonResult GetPhoneNo(string UserId)
        {
            var res = new JsonResult();
            string PhoneNo = _ServicesSoapClient.GetPhoneNoByUserId(UserId);

            res.Data = PhoneNo;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取手机号对应用户ID
        public JsonResult GetUIdByPhoneNo(string PhoneNo)
        {
            var res = new JsonResult();
            string UserId = _ServicesSoapClient.GetIDByInput("PhoneNo", PhoneNo);

            res.Data = UserId;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取医生详细信息_工作单位、科室、职务
        public JsonResult GetDoctorDetailInfo(string UserId, string userClassCode)
        {
            var res = new JsonResult();
            DoctorDetailInfo1 DoctorDetailInfo = _ServicesSoapClient.GetDoctorDetailInfo(UserId);
            string UnitName = DoctorDetailInfo.UnitName.ToString();
            string Dept = DoctorDetailInfo.Dept.ToString();
            string JobTitle = DoctorDetailInfo.JobTitle.ToString();
            string ret = UnitName + "_" + Dept + "_" + JobTitle;
            if (userClassCode == "Doctor")
            {
                string Module = _ServicesSoapClient.GetModuleByDoctorId(UserId);
                ret = ret + "_" + Module;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //保存基本信息
        public JsonResult setCmMstUser(string UserId, string UserName, string Password, string EndDate, string PhoneNo, string userClassCode, int NewFlag)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            EndDate = EndDate.Replace(" ", "");
            EndDate = EndDate.Replace("-", "");
            int intEndDate = Convert.ToInt32(EndDate);
            var res = new JsonResult();
            string Class = ""; //该字段已作废
            bool IsSaved = false;
            int flag = 0;
            IsSaved = _ServicesSoapClient.SetMstUserUM(UserId, UserName, Password, Class, intEndDate, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (userClassCode == "Patient")
            {
                if (NewFlag == 0)
                {
                    IsSaved = _ServicesSoapClient.SetPatName(UserId, UserName);
                }
                else
                {
                    //此界面不涉及病人新建
                }
            }
            else
            {
                string Code = "";
                IsSaved = _ServicesSoapClient.SetDocName(UserId, UserName);
                if (NewFlag != 0)
                {
                    if (userClassCode == "Administrator")
                    {
                        flag = _ServicesSoapClient.SetPsRoleMatch(UserId, "Administrator", "", "0", "");
                    }
                    else if (userClassCode == "Doctor")
                    {
                        Code = _ServicesSoapClient.GetNoByNumberingType(13);
                        flag = _ServicesSoapClient.SetPsRoleMatch(UserId, "Doctor", Code, "1", "");
                    }
                    else if (userClassCode == "HealthCoach")
                    {
                        Code = _ServicesSoapClient.GetNoByNumberingType(12);
                        flag = _ServicesSoapClient.SetPsRoleMatch(UserId, "HealthCoach", Code, "1", "");
                    }
                }
            }
            if (IsSaved == true)
            {
                flag = _ServicesSoapClient.SetPhoneNo(UserId, "PhoneNo", PhoneNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            if ((flag == 1) && (userClassCode != "Patient"))
            {
                IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, "Contact", "Contact002_1", 1, PhoneNo, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (IsSaved)
                {
                    flag = 1;
                }
                else
                {
                    flag = 0;
                }
            }
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //保存医生工作单位科室职务模块信息
        public JsonResult setDocDetailInfo(string UserId, string userClassCode, string UnitName, string Dept, string JobTitle, string Modules)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();

            bool IsSaved = false;
            IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, "Contact", "Contact001_5", 1, UnitName, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, "Contact", "Contact001_6", 1, JobTitle, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, "Contact", "Contact001_8", 1, Dept, "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);



            if (userClassCode == "Doctor")
            {
                string Module = _ServicesSoapClient.GetModuleByDoctorId(UserId);
                if (Module != null)
                {
                    int length = Module.Split('_').Length;
                    int i = 0;
                    for (; i < length; i++)
                    {
                        IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, Module.Split('_')[i], "InvalidFlag", 1, "1", "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        if (IsSaved == false)
                        {
                            break;
                        }
                    }
                }
                if ((IsSaved) || (Module == null))
                {
                    int len = Modules.Split(',').Length;
                    int j = 0;
                    for (; j < len; j++)
                    {

                        IsSaved = _ServicesSoapClient.SetDoctorInfoDetail(UserId, Modules.Split(',')[j], "InvalidFlag", 1, "0", "", 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                        if (IsSaved == false)
                        {
                            break;
                        }
                    }
                }

            }
            res.Data = IsSaved;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //保存医生工作单位科室职务模块信息
        public JsonResult GetRoleNo(string userClass)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();

            string No = "";

            if ((userClass == "Doctor") || (userClass == "HealthCoach"))
            {
                //No = _ServicesSoapClient.GetNoByNumberingType(4);
                No = _ServicesSoapClient.GetNoByNumberingType(17);
            }
            else if (userClass == "Administrator")
            {
                No = _ServicesSoapClient.GetNoByNumberingType(16);
            }
            else
            {
                No = "";
            }
            res.Data = No;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检查手机号是否存在
        public JsonResult checkPhoneNoExist(string PhoneNo)
        {
            var res = new JsonResult();
            int isExist = _ServicesSoapClient.CheckRepeat(PhoneNo, "PhoneNo");

            res.Data = isExist;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检查该用户是否存在
        public JsonResult checkUserExist(string userId)
        {
            var res = new JsonResult();
            bool isExist = _ServicesSoapClient.CheckUserExist(userId);

            res.Data = isExist;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //查询输出数据库中所有权限列表
        public void GetAllAuthorityList(ref Role2AuthorityViewModel r2aInfoVM)
        {
            List<Authority> authorityList = new List<Authority>();
            int Row = 0;

            System.Data.DataTable authorityListDT = _ServicesSoapClient.GetAuthorityList().Tables[0];
            foreach (DataRow row in authorityListDT.Rows)
            {
                authorityList.Add(new Authority { Code = row["AuthorityCode"].ToString(), Name = row["AuthorityName"].ToString() });
                Row++;
            }

            r2aInfoVM.AuthorityList = authorityList;
            r2aInfoVM.AuthorityRowCount = Row;
        }

        //查询输出该角色（roleCode）已有权限列表
        public void GetRoleAuthorityList(string roleCode, ref Role2AuthorityViewModel r2aVM)
        {
            List<AuthorityDetail> roleAuthorityList = new List<AuthorityDetail>();
            int Row = 0;

            System.Data.DataTable roleAuthorityListDT = _ServicesSoapClient.GetRoleAuthorityList(roleCode).Tables[0];
            foreach (DataRow row in roleAuthorityListDT.Rows)
            {
                roleAuthorityList.Add(new AuthorityDetail { AuthorityCode = row["AuthorityCode"].ToString(), AuthorityName = row["AuthorityName"].ToString(), SubAuthorityCode = row["SubAuthorityCode"].ToString(), SubAuthorityName = row["SubAuthorityName"].ToString() });
                Row++;
            }

            r2aVM.RoleAuthorityList = roleAuthorityList;
            r2aVM.RoleAuthorityRowCount = Row;
        }

        //合并新增权限（authorityCode大分类下所有子项目）和角色原有权限的 List<AuthorityDetail>
        public JsonResult GetNewAuthorityList(string authorityCode, string roleCode)
        {
            var res = new JsonResult();
            List<AuthorityDetail> newRoleAuthorityList = new List<AuthorityDetail>();

            //新增权限（authorityCode大分类下所有子项目）         
            System.Data.DataTable subAuthorityListDT = _ServicesSoapClient.GetSubAuthorityList(authorityCode).Tables[0];
            //角色原有权限的 List<AuthorityDetail>
            System.Data.DataTable roleAuthorityListDT = _ServicesSoapClient.GetRoleAuthorityList(roleCode).Tables[0];


            for (int i = 0; i < roleAuthorityListDT.Rows.Count; i++)
            {
                if (roleAuthorityListDT.Rows[i]["AuthorityCode"].ToString() == authorityCode)
                {
                    for (int j = 0; j < subAuthorityListDT.Rows.Count; j++)
                    {
                        if (subAuthorityListDT.Rows[j]["SubAuthorityCode"].ToString() != roleAuthorityListDT.Rows[i]["SubAuthorityCode"].ToString())
                        {
                            DataRow dr = roleAuthorityListDT.NewRow();
                            dr["AuthorityCode"] = roleAuthorityListDT.Rows[j]["AuthorityCode"].ToString();
                            dr["AuthorityName"] = roleAuthorityListDT.Rows[j]["AuthorityName"].ToString();
                            dr["SubAuthorityCode"] = roleAuthorityListDT.Rows[j]["SubAuthorityCode"].ToString();
                            dr["SubAuthorityName"] = roleAuthorityListDT.Rows[j]["SubAuthorityName"].ToString();
                            roleAuthorityListDT.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    DataRow dr = roleAuthorityListDT.NewRow();
                    dr["AuthorityCode"] = roleAuthorityListDT.Rows[i]["AuthorityCode"].ToString();
                    dr["AuthorityName"] = roleAuthorityListDT.Rows[i]["AuthorityName"].ToString();
                    dr["SubAuthorityCode"] = roleAuthorityListDT.Rows[i]["SubAuthorityCode"].ToString();
                    dr["SubAuthorityName"] = roleAuthorityListDT.Rows[i]["SubAuthorityName"].ToString();

                }
            }

            List<AuthorityDetail> roleAuthorityList = new List<AuthorityDetail>();
            foreach (DataRow row in roleAuthorityListDT.Rows)
            {
                roleAuthorityList.Add(new AuthorityDetail { AuthorityCode = row["AuthorityCode"].ToString(), AuthorityName = row["AuthorityName"].ToString(), SubAuthorityCode = row["SubAuthorityCode"].ToString(), SubAuthorityName = row["SubAuthorityName"].ToString() });
            }
            res.Data = roleAuthorityList;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public void GetDoctorInfoList(string id, string name, ref DoctorViewModel doctorInfoVM)
        {
            List<Doctor> doctorList = new List<Doctor>();
            int Row = 0;
            //  doctorList.Add(new Doctor { UserId = "1", UserName = "zyf", Birthday = "19910214", Gender = "0", InvalidFlag = "0", IDNo = "0", ModuleNameString = "高血压，糖尿病" });

            //    System.Data.DataTable userListDT = _ServicesSoapClient.GetUserInfoList("", "").Tables[0];
            //  foreach (DataRow row in userListDT.Rows)
            //   {
            //       doctorList.Add(new Doctor { UserId = row["DoctorId"].ToString(), UserName = row["DoctorName"].ToString(), Birthday = row["Birthday"].ToString(), Gender = row["Gender"].ToString(),InvalidFlag = row["InvalidFlag"].ToString(), IDNo = row["IDNo"].ToString(), ModuleNameString = row["ModuleString"].ToString() });
            //        Row++;
            //    }

            Row++;
            doctorInfoVM.DoctorList = doctorList;
            doctorInfoVM.RowCount = Row;
        }

        #endregion

        #region 患者模块管理
        public List<ModuleAndDoctor> GetModuleAndDoctorInfo(string PatientId)
        {
            List<ModuleAndDoctor> mdlist = new List<ModuleAndDoctor>();
            DataSet ItemInfoBoughtds = _ServicesSoapClient.GetPatBasicInfoDtlList(PatientId);
            foreach (DataTable datatable in ItemInfoBoughtds.Tables)
            {
                foreach (DataRow row in datatable.Rows)
                {
                    if (row[3].ToString() == "Doctor")
                    {
                        ModuleAndDoctor md = new ModuleAndDoctor()
                        {
                            Module = row[1].ToString(),
                            ModuleName = row[2].ToString(), 
                            ItemSeq = row[6].ToString(),
                            DoctorId = row[7].ToString(),
                            ModalName = row[1].ToString() + "Modal",
                            DataTableName = row[1].ToString() + "DataTable",
                            DoctorName = _ServicesSoapClient.GetUserName(row[7].ToString()),
                            DoctorList = GetDoctorList(row[1].ToString())
                        };
                        mdlist.Add(md);
                        break;
                    }
                }
            }
            return mdlist;
        }

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
                    HealthCoachList = GetHealthCoachInfoList()
                };
                hclist.Add(hc);
            }
            if (hclist.Count == 0)
            {
                hclist.Add(new HealthCoach { ItemSeq = "1", HealthCoachId = "0", HealthCoachName = "", HCDivName = "HC1Div", DataTableName = "HC1DataTable", HealthCoachList = GetHealthCoachInfoList() });
            }
            return hclist;
        }

        //获取某模块的医生列表
        public List<DoctorAndHCInfo> GetDoctorList(string Module)
        {
            DataTable docList = new DataTable();
            docList = _ServicesSoapClient.GetDoctorListByModule(Module).Tables[0];
            List<DoctorAndHCInfo> DoctorList = new List<DoctorAndHCInfo>();
            foreach (DataRow DR in docList.Rows)
            {
                //string description = "";
                //if (DR["Hospital"].ToString() != null && DR["Hospital"].ToString() != "")
                //{
                //    description = DR["Hospital"].ToString();
                //}
                //if (DR["Dept"].ToString() != null && DR["Dept"].ToString() != "" && description != "")
                //{
                //    description += "-" + DR["Dept"].ToString();
                //}
                //else if (DR["Dept"].ToString() != null && DR["Dept"].ToString() != "" && description == "")
                //{
                //    description += DR["Dept"].ToString();
                //}
                //if (description != "")
                //{
                //    description = "（" + description + "）";
                //}
                DoctorList.Add(new DoctorAndHCInfo { DoctorId = DR["DoctorId"].ToString(), DoctorName = DR["DoctorName"].ToString(), Hospital = DR["Hospital"].ToString(), Dept = DR["Dept"].ToString() });
            }
            return DoctorList;
        }

        //获取健康专员列表
        public List<DoctorAndHCInfo> GetHealthCoachInfoList()
        {
            DataTable docList = _ServicesSoapClient.GetActiveUserByRole("HealthCoach").Tables[0];
            List<DoctorAndHCInfo> DoctorList = new List<DoctorAndHCInfo>();
            foreach (DataRow DR in docList.Rows)
            {
                //string description = "";
                //if (DR["Hospital"].ToString() != null && DR["Hospital"].ToString() != "")
                //{
                //    description = DR["Hospital"].ToString();
                //}
                //if (DR["Dept"].ToString() != null && DR["Dept"].ToString() != "" && description != "")
                //{
                //    description += "-" + DR["Dept"].ToString();
                //}
                //else if (DR["Dept"].ToString() != null && DR["Dept"].ToString() != "" && description == "")
                //{
                //    description += DR["Dept"].ToString();
                //}
                //if (description != "")
                //{
                //    description = "（" + description + "）";
                //}
                DoctorList.Add(new DoctorAndHCInfo { DoctorId = DR["UserId"].ToString(), DoctorName = DR["UserName"].ToString(), Hospital = DR["Hospital"].ToString(), Dept = DR["Dept"].ToString() });
            }
            return DoctorList;
        }
        #endregion

        //GetInactiveUserByRole
        public List<QualiCheck> GetInactiveUserByRole(string RoleClass)
        {
            //UserName, UserId, ActivationCode, PhoneNo
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetInactiveUserByRole(RoleClass);
            List<QualiCheck> items = new List<QualiCheck>();
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        QualiCheck item = new QualiCheck();
                        if ((dr["UserId"].ToString() != null) && (dr["UserId"].ToString() != ""))
                        {
                            item.UserId = dr["UserId"].ToString();
                            item.UserName = dr["UserName"].ToString();
                            item.ActivationCode = dr["ActivationCode"].ToString();
                            item.PhoneNo = dr["PhoneNo"].ToString();
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        //SetQualiCheckList
        public List<List<QualiCheck>> SetQualiCheckList()
        {
            List<List<QualiCheck>> items = new List<List<QualiCheck>>();
            List<QualiCheck> item = new List<QualiCheck>();
            item = GetInactiveUserByRole("Doctor");
            items.Add(item);
            item = GetInactiveUserByRole("HealthCoach");
            items.Add(item);
            return items;
        }

        //读取照片
        public JsonResult GetPhotoAddress(string UserId)
        {
            var res = new JsonResult();
            var DetailInfo = _ServicesSoapClient.GetDoctorDetailInfo(UserId);
            string hostAddress = System.Configuration.ConfigurationManager.AppSettings["WebServe"];
            string PhotoAddress = "";
            if (DetailInfo.PhotoAddress == null || DetailInfo.PhotoAddress == "")
            {
                PhotoAddress = "http://" + hostAddress + "/PersonalPhotoCheck/non2.jpg";
            }
            else
            {
                PhotoAddress = "http://" + hostAddress + "/PersonalPhotoCheck/" + DetailInfo.PhotoAddress;
            }
            res.Data = PhotoAddress;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //GetActivationCodeJson 生成邀请码
        public JsonResult GetActivationCodeJson(int Role)
        {
            var res = new JsonResult();
            string Ret = "";
            Ret = _ServicesSoapClient.GetNoByNumberingType(Role);
            res.Data = Ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //将邀请码写入数据库
        public JsonResult SetActivationCodeJson(string UserId, string RoleClass, string ActivationCode, string InvalidFlag, string PhoneNo)
        {
            string ActivateState = "";
            if (InvalidFlag == "已激活")
            {
                ActivateState = "0";
            }
            else
            {
                ActivateState = "1";
            }
            var res = new JsonResult();
            int Ret = 0;
            Ret = _ServicesSoapClient.SetPsRoleMatch(UserId, RoleClass, ActivationCode, ActivateState, "");
            res.Data = Ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            string RoleName = "";
            if (RoleClass == "Doctor")
            {
                RoleName = "医生";
            }
            else
            {
                RoleName = "健康专员";
            }
            string Message = _ServicesSoapClient.sendSMS(PhoneNo, "Activision", "您的" + RoleName + "权限" + InvalidFlag);
            return res;
        }
        #endregion

    }
}
