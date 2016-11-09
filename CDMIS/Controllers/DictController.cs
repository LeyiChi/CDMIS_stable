using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using CDMIS.ViewModels;
using CDMIS.Models;
using CDMIS.ServiceReference;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class DictController : Controller
    {
        //
        // GET: /Dict/
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();//WebService

        //字典选择
        public ActionResult Index()
        {
            DictViewModel dict = new DictViewModel();
            return View(dict);
        }

        #region < "YDS" >
        //治疗字典
        public ActionResult Treatment()
        {
            List<AbsType> treatment = new List<AbsType>();
            DataSet info = _ServicesSoapClient.GetTreatment();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                AbsType absinfo = new AbsType();

                absinfo.Type = row[0].ToString();
                absinfo.Code = row[1].ToString();
                absinfo.TypeName = row[2].ToString();
                absinfo.Name = row[3].ToString();
                absinfo.InputCode = row[4].ToString();
                absinfo.SortNo = Convert.ToInt32(row[5]);

                treatment.Add(absinfo);
            }
            return View(treatment);
        }

        //症状字典
        public ActionResult Symptoms()
        {
            List<AbsType> symptoms = new List<AbsType>();
            DataSet info = _ServicesSoapClient.GetSymptoms();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                AbsType absinfo = new AbsType();

                absinfo.Type = row[0].ToString();
                absinfo.Code = row[1].ToString();
                absinfo.TypeName = row[2].ToString();
                absinfo.Name = row[3].ToString();
                absinfo.InputCode = row[4].ToString();
                absinfo.SortNo = Convert.ToInt32(row[5]);

                symptoms.Add(absinfo);
            }
            return View(symptoms);
        }



        //体征字典
        //public ActionResult VitalSigns()
        //{
        //    List<AbsType> vitalsigns = new List<AbsType>();
        //    DataSet info = _ServicesSoapClient.GetVitalSigns();
        //    foreach (DataRow row in info.Tables[0].Rows)
        //    {
        //        AbsType absinfo = new AbsType();

        //        absinfo.Type = row[0].ToString();
        //        absinfo.Code = row[1].ToString();
        //        absinfo.TypeName = row[2].ToString();
        //        absinfo.Name = row[3].ToString();
        //        absinfo.InputCode = row[4].ToString();
        //        absinfo.SortNo = Convert.ToInt32(row[5]);

        //        vitalsigns.Add(absinfo);
        //    }
        //    return View(vitalsigns);
        //}

        //检查字典
        public ActionResult ExaminationItem()
        {
            List<AbsType> examinationitem = new List<AbsType>();
            DataSet info = _ServicesSoapClient.GetExaminationItem();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                AbsType absinfo = new AbsType();

                absinfo.Type = row[0].ToString();
                absinfo.Code = row[1].ToString();
                absinfo.TypeName = row[2].ToString();
                absinfo.Name = row[3].ToString();
                absinfo.InputCode = row[4].ToString();
                absinfo.SortNo = Convert.ToInt32(row[5]);

                examinationitem.Add(absinfo);
            }
            return View(examinationitem);
        }

        //检验字典
        public ActionResult LabTestItems()
        {
            List<AbsType> labtestitems = new List<AbsType>();
            DataSet info = _ServicesSoapClient.GetLabTestItems();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                AbsType absinfo = new AbsType();

                absinfo.Type = row[0].ToString();
                absinfo.Code = row[1].ToString();
                absinfo.TypeName = row[2].ToString();
                absinfo.Name = row[3].ToString();
                absinfo.InputCode = row[4].ToString();
                absinfo.SortNo = Convert.ToInt32(row[5]);

                labtestitems.Add(absinfo);
            }
            return View(labtestitems);
        }

        //类型字典表
        public ActionResult BasicType()
        {
            List<BasicType> basictype = new List<BasicType>();
            DataSet info = _ServicesSoapClient.GetType();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                BasicType basicinfo = new BasicType();

                basicinfo.Category = row[0].ToString();
                basicinfo.Type = Convert.ToInt32(row[1]);
                basicinfo.Name = row[2].ToString();
                basicinfo.SortNo = Convert.ToInt32(row[4]);

                basictype.Add(basicinfo);
            }
            return View(basictype);
        }

        //医保类别字典表
        public ActionResult Insurance()
        {
            List<Insurance> insurance = new List<Insurance>();
            DataSet info = _ServicesSoapClient.GetInsurance();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                Insurance insuranceinfo = new Insurance();

                insuranceinfo.Code = row[0].ToString();
                insuranceinfo.Name = row[1].ToString();
                insuranceinfo.InputCode = row[2].ToString();
                insuranceinfo.Redundance = row[3].ToString();
                insuranceinfo.InvalidFlag = Convert.ToInt32(row[4]);

                insurance.Add(insuranceinfo);
            }
            return View(insurance);
        }

        //任务字典表
        public ActionResult Task()
        {
            List<MstTask> tasks = new List<MstTask>();
            DataSet info = _ServicesSoapClient.GetTasks();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MstTask task = new MstTask();

                task.CategoryCode = row[0].ToString();
                task.Code = row[1].ToString();
                task.Name = row[2].ToString();
                task.ParentCode = row[3].ToString();
                task.Description = row[4].ToString();
                task.GroupHeaderFlag = Convert.ToInt32(row[5]);
                task.ControlType = Convert.ToInt32(row[6]);
                task.OptionCategory = row[7].ToString();

                tasks.Add(task);
            }
            return View(tasks);
        }
        //警戒字典表
        public ActionResult BasicAlert()
        {
            BasicAlertViewModel basicalertview = new BasicAlertViewModel();
            DataSet info = _ServicesSoapClient.GetBasicAlert();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                BasicAlert basicinfo = new BasicAlert();

                basicinfo.AlertItemCode = row[0].ToString();
                basicinfo.AlertItemName = row[1].ToString();
                basicinfo.Min = Convert.ToDecimal(row[2]);
                basicinfo.Max = Convert.ToDecimal(row[3]);
                basicinfo.Units = row[4].ToString();

                basicalertview.BasicAlertList.Add(basicinfo);
            }
            return View(basicalertview);
        }
        

        #region<"科室字典表">
        //未匹配科室字典表
        public ActionResult UnCompDivision()
        {
            UnCompDivisionViewModel UnCompDivision = new UnCompDivisionViewModel();
            List<TmpDivisionDict> TmpDivi = UnCompDivision.TmpDivision;
            DataSet info = _ServicesSoapClient.GetTmpDivisionByStatus(1);        
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpDivisionDict UnCompDivisioninfo = new TmpDivisionDict();
                UnCompDivisioninfo.HospitalCode = row[0].ToString();
                UnCompDivisioninfo.HospitalName = row[1].ToString();
                UnCompDivisioninfo.Type = row[2].ToString();
                UnCompDivisioninfo.Code = row[3].ToString();
                UnCompDivisioninfo.TypeName = row[4].ToString();
                UnCompDivisioninfo.Name = row[5].ToString();
                UnCompDivisioninfo.InputCode = row[6].ToString();
                UnCompDivisioninfo.Description = row[7].ToString();
                UnCompDivisioninfo.Status = Convert.ToInt32(row[8]);

                TmpDivi.Add(UnCompDivisioninfo);
            }
            return View(UnCompDivision);
        }
        //已匹配科室字典表
        public ActionResult CompDivision()
        {
            List<MpDivisionCmp> division = new List<MpDivisionCmp>();
            DataSet info = _ServicesSoapClient.GetMpDivisionCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpDivisionCmp CompDivisioninfo = new MpDivisionCmp();
                CompDivisioninfo.HospitalCode = row[0].ToString();
                CompDivisioninfo.HospitalName = row[1].ToString();
                CompDivisioninfo.Type = Convert.ToInt32(row[2]);
                CompDivisioninfo.TypeName = row[3].ToString();
                CompDivisioninfo.Code = row[4].ToString();
                CompDivisioninfo.Name = row[5].ToString();
                CompDivisioninfo.HZCode = row[6].ToString();
                CompDivisioninfo.HZName = row[7].ToString();
                CompDivisioninfo.Redundance = row[8].ToString();

                division.Add(CompDivisioninfo);
            }
            return View(division);
        }
        //科室字典表
        public ActionResult Division()
        {
            List<Hospital> division = new List<Hospital>();
            DataSet info = _ServicesSoapClient.GetDivision();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                Hospital divisioninfo = new Hospital();

                divisioninfo.Type = row[0].ToString();
                divisioninfo.Code = row[1].ToString();
                divisioninfo.TypeName = row[2].ToString();
                divisioninfo.Name = row[3].ToString();
                divisioninfo.InputCode = row[4].ToString();
                divisioninfo.Description = row[5].ToString();

                division.Add(divisioninfo);
            }
            return View(division);
        }

        //科室字典表数据插入
        public JsonResult DivisionEdit(int Type, string Code, string TypeName, string Name, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetDivision(Type, Code, TypeName, Name, InputCode, Description, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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
        //由未匹配科室表插入匹配科室
        public JsonResult AddToCompDivision(string HospitalCode, string HZType, string HZCode, string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpDivisionCmp(HospitalCode, TypeInt, Code, HZCode, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpDivision(HospitalCode, HZType, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //获取科室类别名称
        public JsonResult GetDivisionTypeNamebyType(string Type)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int TypeInt = Convert.ToInt32(Type);
            string TypeName = _ServicesSoapClient.GetDivisionTypeNamebyType(TypeInt);

            res.Data = TypeName;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //获取科室名称
        public JsonResult GetDivisionNamebyCode(string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetDivisionNamebyCode(Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion
        
        #region<"诊断字典表">
        //未匹配诊断字典表
        public ActionResult UnCompDiagnosis()
        {
            UnCompDiagnosisViewModel UnCompDivision = new UnCompDiagnosisViewModel();
            List<TmpDivisionDict> TmpDivi = UnCompDivision.TmpDiagnosis;
            DataSet info = _ServicesSoapClient.GetTmpDiagnosisByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpDivisionDict UnCompDivisioninfo = new TmpDivisionDict();
                UnCompDivisioninfo.HospitalCode = row[0].ToString();
                UnCompDivisioninfo.HospitalName = row[1].ToString();
                //UnCompDivisioninfo.Type = row[2].ToString();
                UnCompDivisioninfo.Code = row[2].ToString();
                //UnCompDivisioninfo.TypeName = row[4].ToString();
                UnCompDivisioninfo.Name = row[3].ToString();
                UnCompDivisioninfo.InputCode = row[4].ToString();
                UnCompDivisioninfo.Description = row[5].ToString();
                UnCompDivisioninfo.Status = Convert.ToInt32(row[6]);

                TmpDivi.Add(UnCompDivisioninfo);
            }
            return View(UnCompDivision);
        }
        //已匹配诊断字典表
        public ActionResult CompDiagnosis()
        {
            List<MpDiagnosisCmp> division = new List<MpDiagnosisCmp>();
            DataSet info = _ServicesSoapClient.GetMpDiagnosisCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpDiagnosisCmp CompDivisioninfo = new MpDiagnosisCmp();
                CompDivisioninfo.HospitalCode = row[0].ToString();
                CompDivisioninfo.HospitalName = row[1].ToString();
                CompDivisioninfo.Type = row[2].ToString();
                CompDivisioninfo.TypeName = row[3].ToString();
                CompDivisioninfo.Code = row[4].ToString();
                CompDivisioninfo.Name = row[5].ToString();
                CompDivisioninfo.HZCode = row[6].ToString();
                CompDivisioninfo.HZName = row[7].ToString();
                CompDivisioninfo.Redundance = row[8].ToString();

                division.Add(CompDivisioninfo);
            }
            return View(division);
        }
        //科室字典表
        public ActionResult Diagnosis()
        {
            List<Hospital> division = new List<Hospital>();
            DataSet info = _ServicesSoapClient.GetDiagnosis();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                Hospital divisioninfo = new Hospital();

                divisioninfo.Type = row[0].ToString();
                divisioninfo.Code = row[1].ToString();
                divisioninfo.TypeName = row[2].ToString();
                divisioninfo.Name = row[3].ToString();
                divisioninfo.InputCode = row[4].ToString();
                divisioninfo.Description = row[6].ToString();

                division.Add(divisioninfo);
            }
            return View(division);
        }
        ////诊断字典
        //public ActionResult Diagnosis()
        //{
        //    List<AbsType> diagnosis = new List<AbsType>();
        //    DataSet info = _ServicesSoapClient.GetDiagnosis();
        //    foreach (DataRow row in info.Tables[0].Rows)
        //    {
        //        AbsType absinfo = new AbsType();

        //        absinfo.Type = row[0].ToString();
        //        absinfo.Code = row[1].ToString();
        //        absinfo.TypeName = row[2].ToString();
        //        absinfo.Name = row[3].ToString();
        //        absinfo.InputCode = row[4].ToString();
        //        absinfo.SortNo = Convert.ToInt32(row[5]);

        //        diagnosis.Add(absinfo);
        //    }
        //    return View(diagnosis);
        //}
        //诊断字典表数据插入
        public JsonResult DiagnosisEdit(string Type, string Code, string TypeName, string Name, string InputCode, string Description)
        {
            var res = new JsonResult();

            bool flag = _ServicesSoapClient.SetDiagnosis(Type, Code, TypeName, Name, InputCode, Description, 0);
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
        //由未匹配科室表插入匹配科室
        public JsonResult AddToCompDiagnosis(string HospitalCode, string HZType, string HZCode, string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpDiagnosisCmp(HospitalCode, HZCode, Type, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpDiagnosis(HospitalCode,HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //获取科室类别名称
        public JsonResult GetDiagnosisTypeNamebyType(string Type)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            string TypeName = _ServicesSoapClient.GetDiagnosisTypeNamebyType(Type);

            res.Data = TypeName;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //获取科室名称
        public JsonResult GetDiagnosisNamebyCode(string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetDiagnosisNamebyCode(Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        //医院字典表
        public ActionResult Hospital()
        {
            List<Hospital> hospital = new List<Hospital>();
            DataSet info = _ServicesSoapClient.GetHospital();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                Hospital hospitalinfo = new Hospital();

                hospitalinfo.Type = row[1].ToString();    
                hospitalinfo.Code = row[0].ToString();       //数据库调用方法写反了，故此处也交换次序
                hospitalinfo.TypeName = row[2].ToString();
                hospitalinfo.Name = row[3].ToString();
                hospitalinfo.InputCode = row[4].ToString();
                hospitalinfo.Description = row[5].ToString();

                hospital.Add(hospitalinfo);
            }
            return View(hospital);
        }

        //用户信息表
        public ActionResult InfoItemCategory()
        {
            List<InfoItemCategory> infoitemcategory = new List<InfoItemCategory>();
            DataSet info = _ServicesSoapClient.GetInfoItemCategory();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                InfoItemCategory infoitemcategoryinfo = new InfoItemCategory();

                infoitemcategoryinfo.Code = row[0].ToString();
                infoitemcategoryinfo.Name = row[1].ToString();
                infoitemcategoryinfo.SortNo = Convert.ToInt32(row[2]);
                infoitemcategoryinfo.StartDate = row[3].ToString();
                infoitemcategoryinfo.EndDate = row[4].ToString();

                infoitemcategory.Add(infoitemcategoryinfo);
            }
            return View(infoitemcategory);
        }

        //详细信息表
        public ActionResult InfoItem()
        {
            InfoItemViewModel infoitemview = new InfoItemViewModel();
            DataSet info = _ServicesSoapClient.GetInfoItem();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                InfoItem infoiteminfo = new InfoItem();

                infoiteminfo.CategoryCode = row[0].ToString();
                infoiteminfo.Code = row[1].ToString();
                infoiteminfo.Name = row[2].ToString();
                infoiteminfo.ParentCode = row[3].ToString();
                infoiteminfo.SortNo = Convert.ToInt32(row[4]);
                infoiteminfo.ControlCode = row[8].ToString();
                if (infoiteminfo.ControlCode != "")
                {
                    infoiteminfo.ControlType = _ServicesSoapClient.GetMstTypeName("ControlType", Convert.ToInt32(infoiteminfo.ControlCode));
                }
                else
                {
                    infoiteminfo.ControlType = "";
                }
                infoiteminfo.OptionCategory = row[9].ToString();

                infoitemview.InfoItemList.Add(infoiteminfo);
            }
            return View(infoitemview);
        }



        //治疗字典表数据插入
        public JsonResult TreatmentEdit(string Type, string Code, string TypeName, string Name, string InputCode)
        {
            var res = new JsonResult();
            if (Code == "") Code = _ServicesSoapClient.GetTreatmentMaxCode(Type);
            int SortNo = Convert.ToInt32(Code);
            bool flag = _ServicesSoapClient.SetTreatment(Type, Code, TypeName, Name, InputCode, SortNo, "", 0);
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

        //治疗字典表数据删除
        public JsonResult TreatmentDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteTreatment(Type, Code);
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

        //类型字典表数据插入
        public JsonResult BasicTypeEdit(string Category, string Type, string Name)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            if (Type == "")
            {
                Type = _ServicesSoapClient.GetTypeMaxCode(Category);
            }
            int SortNo = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetType(Category, Convert.ToInt32(Type), Name, 0, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //类型字典表数据删除
        public JsonResult BasicTypeDelete(string Category, int Type)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteType(Category, Type);
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

        //医保类别字典表数据插入
        public JsonResult InsuranceEdit(string Code, string Name, string InputCode, string Redundance, string InvalidFlag)
        {
            //var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int InvalidFlag1 = Convert.ToInt32(InvalidFlag);
            bool flag = _ServicesSoapClient.SetInsurance(Code, Name, InputCode, Redundance, InvalidFlag1);
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

        //医保类别字典表数据删除
        public JsonResult InsuranceDelete(string Code)
        {
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.DeleteInsurance(Code);
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

        //任务字典表数据插入
        public JsonResult TaskEdit(string CategoryCode, string Code, string Name, string ParentCode, string Description, string StartDate, string EndDate, int GroupHeaderFlag, int ControlType, string OptionCategory)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int flag = 0;
            if (StartDate != "")
            {
                flag = _ServicesSoapClient.SetMstTask(CategoryCode, Code, Name, ParentCode, Description, -1, 99999999, GroupHeaderFlag, ControlType, OptionCategory, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            else 
            {
                flag = _ServicesSoapClient.SetMstTask(CategoryCode, Code, Name, ParentCode, Description,-2,99999999, GroupHeaderFlag, ControlType, OptionCategory, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            
            if (flag==1)
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

        //任务字典表数据删除
        public JsonResult TaskDelete(string CategoryCode, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteMstTask(CategoryCode,Code);
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


        //诊断字典表数据删除
        public JsonResult DiagnosisDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteDiagnosis(Type, Code);
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

        //检验字典表数据插入
        public JsonResult LabTestItemsEdit(string Type, string Code, string TypeName, string Name, string InputCode)
        {
            var res = new JsonResult();
            if (Code == "") Code = _ServicesSoapClient.GetLabTestItemsMaxCode(Type);
            int SortNo = Convert.ToInt32(Code.Split(new char[] { '_' })[1]);
            bool flag = _ServicesSoapClient.SetLabTestItems(Type, Code, TypeName, Name, InputCode, SortNo, "", 0);
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

        //检验字典表数据删除
        public JsonResult LabTestItemsDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteLabTestItems(Type, Code);
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

        //检查字典表数据插入
        public JsonResult ExaminationItemEdit(string Type, string Code, string TypeName, string Name, string InputCode)
        {
            var res = new JsonResult();
            if (Code == "") Code = _ServicesSoapClient.GetExaminationItemMaxCode(Type);
            int SortNo = Convert.ToInt32(Code.Split(new char[] { '_' })[1]);
            bool flag = _ServicesSoapClient.SetExaminationItem(Type, Code, TypeName, Name, InputCode, SortNo, "", 0);
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

        //检查字典表数据删除
        public JsonResult ExaminationItemDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteExaminationItem(Type, Code);
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

        //体征字典表数据插入
        //public JsonResult VitalSignsEdit(string Type, string Code, string TypeName, string Name, string InputCode)
        //{
        //    var res = new JsonResult();
        //    if (Code == "") Code = _ServicesSoapClient.GetVitalSignsMaxCode(Type);
        //    int SortNo = Convert.ToInt32(Code.Split(new char[] { '_' })[1]);
        //    bool flag = _ServicesSoapClient.SetVitalSigns(Type, Code, TypeName, Name, InputCode, SortNo, "", 0);
        //    if (flag)
        //    {
        //        res.Data = true;
        //    }
        //    else
        //    {
        //        res.Data = false;
        //    }
        //    res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return res;
        //}

        //体征字典表数据删除
        public JsonResult VitalSignsDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteVitalSigns(Type, Code);
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

        //症状字典表数据插入
        public JsonResult SymptomsEdit(string Type, string Code, string TypeName, string Name, string InputCode)
        {
            var res = new JsonResult();
            if (Code == "")
            {
                Code = _ServicesSoapClient.GetSymptomsMaxCode(Type);
            }
            int SortNo = Convert.ToInt32(Code);
            bool flag = _ServicesSoapClient.SetSymptoms(Type, Code, TypeName, Name, InputCode, SortNo, "", 0);
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

        //症状字典表数据删除
        public JsonResult SymptomsDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteSymptoms(Type, Code);
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

        //医院字典表数据插入
        public JsonResult HospitalEdit(string Type, string Code, string Name, string SortNo, string StartDate, string EndDate)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            int k;
            if (SortNo == "")
            {
                k = _ServicesSoapClient.GetDivisionMaxCode() + 1;
            }
            else
            {
                k = Convert.ToInt32(SortNo);
            }
            bool flag = _ServicesSoapClient.SetHospital(Code, Convert.ToInt32(Type), Name, k, Convert.ToInt32(StartDate), Convert.ToInt32(EndDate), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //医院字典表数据删除
        public JsonResult HospitalDelete(string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteHospital(Code);
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



        //科室字典表数据删除
        public JsonResult DivisionDelete(string Type, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteDivision(Convert.ToInt32(Type), Code);
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

        //用户信息表数据插入
        public JsonResult InfoItemCategoryEdit(string Code, string Name, string SortNo, string StarDate, string EndDate)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetInfoItemCategory(Code, Name, Convert.ToInt32(SortNo), Convert.ToInt32(StarDate), Convert.ToInt32(EndDate), user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //用户信息表数据删除
        public JsonResult InfoItemCategoryDelete(string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteInfoItemCategory(Code);
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

        //警戒字典表数据插入
        public JsonResult BasicAlertEdit(string AlertItemCode, string AlertItemName, string Min, string Max, string Units)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            if (Units != "")
            {
                int type = Convert.ToInt32(Units);
                Units = _ServicesSoapClient.GetMstTypeName("CommonUnit", type);
            }
            bool flag = _ServicesSoapClient.SetBasicAlert(AlertItemCode, AlertItemName, Convert.ToDecimal(Min), Convert.ToDecimal(Max), Units, "", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //警戒字典表数据删除
        public JsonResult BasicAlertDelete(string AlertItemCode)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteBasicAlert(AlertItemCode);
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

        //详细信息表数据插入
        public JsonResult InfoItemEdit(string CategoryCode, string Code, string Name, string SortNo, string ControlType, string OptionCategory)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = false;
            if (ControlType == "0")
            {
                string ParentCode = "";
                flag = _ServicesSoapClient.SetInfoItem(CategoryCode, Code, Name, ParentCode, Convert.ToInt32(SortNo), 0, 99999999, 1, "", "", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            else
            {
                string ParentCode = Code.Split(new char[] { '_' })[0];
                if (SortNo == "")
                {
                    SortNo = Code.Split(new char[] { '_' })[1];
                }
                flag = _ServicesSoapClient.SetInfoItem(CategoryCode, Code, Name, ParentCode, Convert.ToInt32(SortNo), 0, 99999999, 0, ControlType, OptionCategory, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //详细信息表数据删除
        public JsonResult InfoItemDelete(string CategoryCode, string Code)
        {
            var res = new JsonResult();
            int flag = _ServicesSoapClient.DeleteInfoItem(CategoryCode, Code);
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

        //详细信息表根据CategoryCode动态加载Code下拉框
        public JsonResult GetListbyCategory(string CategoryCode)
        {
            var res = new JsonResult();
            DataSet info = _ServicesSoapClient.GetNextCode(CategoryCode);
            List<string> codelist = new List<string>();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                string rowinfo = row[0].ToString();
                codelist.Add(rowinfo);
            }
            res.Data = codelist;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region < "GL" >
        //血压字典获取信息
        public ActionResult MstBloodPressure()
        {
            List<BloodPressure> items = new List<BloodPressure>();
            DataSet ds = _ServicesSoapClient.GetBloodPressureList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                BloodPressure item = new BloodPressure();
                item.Code = row[0].ToString();
                item.Name = row[1].ToString();
                item.Description = row[2].ToString();
                item.SBP = Convert.ToInt32(row[3]);
                item.DBP = Convert.ToInt32(row[4]);
                item.PatientClass = row[5].ToString();
                item.Redundance = row[6].ToString();

                items.Add(item);
            }
            return View(items);
        }

        //血压字典数据插入
        public JsonResult BloodPressureEdit(string Code, string Name, string Description, int SBP, int DBP, string PatientClass, string Redundance)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.SetBloodPressure(Code, Name, Description, SBP, DBP, PatientClass, "");
            var ret = false;
            if (flag == 1)
            {
                ret = true;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //血压字典数据删除

        public JsonResult BloodPressureDelete(string Code)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.DeleteBloodPressure(Code);
            var ret = false;
            if (flag == 1)
            {
                ret = true;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //健康教育字典
        public ActionResult MstHealthEducation()
        {
            List<MstHealthEducation> items = new List<MstHealthEducation>();
            DataSet ds = _ServicesSoapClient.GetHealthEducationList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                MstHealthEducation item = new MstHealthEducation();
                item.Module = row[0].ToString();
                item.HealthId = row[1].ToString();
                item.Type = Convert.ToInt32(row[2]);
                item.FileName = row[3].ToString();
                item.Path = row[4].ToString();
                item.Introduction = row[5].ToString();
                item.Redundance = row[6].ToString();

                items.Add(item);
            }
            return View(items);
        }

        //健康教育字典数据插入
        public JsonResult HealthEducationEdit(string Module, string HealthId, int Type, string FileName, string Path, string Introduction)
        {
            var res = new JsonResult();
            var user = Session["CurrentUser"] as UserAndRole;
            var flag = false;
            flag = _ServicesSoapClient.SetHealthEducation(Module, HealthId, Type, FileName, Path, Introduction, "", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        //健康教育字典数据删除
        public JsonResult HealthEducationDelete(string Module, string HealthId)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.DeleteHealthEducationInfo(Module, HealthId);
            var ret = false;
            if (flag == 1)
            {
                ret = true;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //生活方式字典
        public ActionResult MstLifeStyle()
        {
            List<LifeStyle> items = new List<LifeStyle>();
            DataSet ds = _ServicesSoapClient.GetLifeStyleList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                LifeStyle item = new LifeStyle();
                item.StyleId = row[0].ToString();
                item.Name = row[1].ToString();
                item.Redundance = row[2].ToString();

                items.Add(item);
            }
            return View(items);
        }

        //生活方式字典数据插入
        public JsonResult LifeStyleEdit(string StyleId, string Name)
        {
            var res = new JsonResult();
            var flag = false;
            flag = _ServicesSoapClient.SetLifeStyle(StyleId, Name, "");
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //生活方式字典数据删除
        public JsonResult LifeStyleDelete(string StyleId)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.DeleteLifeStyle(StyleId);
            var ret = false;
            if (flag == 1)
            {
                ret = true;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //生活方式详细字典
        // //StyleId, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, Redundance
        public ActionResult MstLifeStyleDetail()
        {
            List<LifeStyleDetail> items = new List<LifeStyleDetail>();
            DataSet ds = _ServicesSoapClient.GetLifeStyleDetailList();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                LifeStyleDetail item = new LifeStyleDetail();
                item.StyleId = row[0].ToString();
                item.Module = row[1].ToString();
                item.CurativeEffect = row[2].ToString();
                item.SideEffect = row[3].ToString();
                item.Instruction = row[4].ToString();
                item.HealthEffect = row[5].ToString();
                item.Unit = row[6].ToString();
                item.Redundance = row[7].ToString();

                items.Add(item);
            }
            return View(items);
        }

        //生活方式详细字典数据插入
        public JsonResult LifeStyleDetailEdit(string StyleId, string Module, string CurativeEffect, string SideEffect, string Instruction, string HealthEffect, string Unit)
        {
            var res = new JsonResult();
            var flag = false;
            flag = _ServicesSoapClient.SetLifeStyleDetail(StyleId, Module, CurativeEffect, SideEffect, Instruction, HealthEffect, Unit, "");
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //生活方式详细字典数据删除
        public JsonResult LifeStyleDetailDelete(string StyleId, string Module)
        {
            var res = new JsonResult();
            var flag = 0;
            flag = _ServicesSoapClient.DeleteLifeStyleDetail(StyleId, Module);
            var ret = false;
            if (flag == 1)
            {
                ret = true;
            }
            res.Data = ret;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region<"lpf">
        //已匹配体征字典表 lpf 20150709
        public ActionResult CompVitalSigns()
        {
            UnCompVitalSignsViewModel CompVitalSigns = new UnCompVitalSignsViewModel();
            List<MpVitalSignsCmp> TmpVS = CompVitalSigns.VitalSignsCmp;
            DataSet info = _ServicesSoapClient.GetMpVitalSignsCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpVitalSignsCmp CompVitalinfo = new MpVitalSignsCmp();
                CompVitalinfo.HospitalCode = row[0].ToString();
                CompVitalinfo.HospitalName = row[1].ToString();
                CompVitalinfo.Type = row[2].ToString();
                CompVitalinfo.TypeName = row[3].ToString();
                CompVitalinfo.Code = row[4].ToString();
                CompVitalinfo.Name = row[5].ToString();
                CompVitalinfo.HZCode = row[6].ToString();
                CompVitalinfo.HZName = row[7].ToString();
                CompVitalinfo.Redundance = row[8].ToString();

                TmpVS.Add(CompVitalinfo);
            }
            return View(CompVitalSigns);
        }
        //体征字典表 lpf 20150709
        public ActionResult VitalSigns()
        {
            List<AbsType> vitalsigns = new List<AbsType>();
            DataSet info = _ServicesSoapClient.GetVitalSigns();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                AbsType absinfo = new AbsType();

                absinfo.Type = row[0].ToString();
                absinfo.Code = row[1].ToString();
                absinfo.TypeName = row[2].ToString();
                absinfo.Name = row[3].ToString();
                absinfo.InputCode = row[4].ToString();
                absinfo.SortNo = Convert.ToInt32(row[5]);
                absinfo.Redundance = row[6].ToString();

                vitalsigns.Add(absinfo);
            }
            return View(vitalsigns);
        }

        //体征字典表数据插入 lpf 20150710
        public JsonResult VitalSignsEdit(string Type, string Code, string TypeName, string Name, string InputCode, string Description)
        {
            var res = new JsonResult();
            if (Code == "") Code = _ServicesSoapClient.GetVitalSignsMaxCode(Type);
            int SortNo = Convert.ToInt32(Code.Split(new char[] { '_' })[1]);
            bool flag = _ServicesSoapClient.SetVitalSigns(Type, Code, TypeName, Name, InputCode, SortNo, Description, 0);
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

        //获取体征类别名称 lpf 20150710
        public JsonResult GetVitalSignsTypeNamebyType(string Type)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string TypeName = _ServicesSoapClient.GetVitalSignsTypeNamebyType(Type);

            res.Data = TypeName;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取体征名称 lpf 20150710
        public JsonResult GetVitalSignsName(string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetVitalSignsName(Type, Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //未匹配手术字典表 lpf 20150710
        public ActionResult UnCompOperation()
        {
            UnCompOperationViewModel UnCompOperation = new UnCompOperationViewModel();
            List<TmpOperationDict> TmpOper = UnCompOperation.TmpOperation;
            DataSet info = _ServicesSoapClient.GetTmpOperationByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpOperationDict UnCompOperationinfo = new TmpOperationDict();
                UnCompOperationinfo.HospitalCode = row[0].ToString();
                UnCompOperationinfo.HospitalName = row[1].ToString();
                UnCompOperationinfo.Code = row[2].ToString();
                UnCompOperationinfo.Name = row[3].ToString();
                UnCompOperationinfo.InputCode = row[4].ToString();
                UnCompOperationinfo.Description = row[5].ToString();
                UnCompOperationinfo.Status = Convert.ToInt32(row[6]);

                TmpOper.Add(UnCompOperationinfo);
            }
            return View(UnCompOperation);
        }

        //已匹配手术字典表 lpf 20150709
        public ActionResult CompOperation()
        {
            List<MpOperationCmp> operation = new List<MpOperationCmp>();
            DataSet info = _ServicesSoapClient.GetMpOperationCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpOperationCmp CompOperationinfo = new MpOperationCmp();
                CompOperationinfo.HospitalCode = row[0].ToString();
                CompOperationinfo.HospitalName = row[1].ToString();
                CompOperationinfo.Code = row[2].ToString();
                CompOperationinfo.Name = row[3].ToString();
                CompOperationinfo.HZCode = row[4].ToString();
                CompOperationinfo.HZName = row[5].ToString();
                CompOperationinfo.Redundance = row[6].ToString();

                operation.Add(CompOperationinfo);
            }
            return View(operation);
        }
        //手术字典表 lpf 20150709
        public ActionResult Operation()
        {
            List<CmMstOperation> operation = new List<CmMstOperation>();
            DataSet info = _ServicesSoapClient.GetOperation();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                CmMstOperation operationinfo = new CmMstOperation();

                operationinfo.Code = row[0].ToString();
                operationinfo.Name = row[1].ToString();
                operationinfo.InputCode = row[2].ToString();
                operationinfo.Redundance = row[3].ToString();

                operation.Add(operationinfo);
            }
            return View(operation);
        }

        //由未匹配手术表插入匹配手术 lpf 20150713
        public JsonResult AddToCompOperation(string HospitalCode, string HZCode, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetMpOperationCmp(HospitalCode, HZCode, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpOperation(HospitalCode, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //手术字典表数据插入 lpf 20150713
        public JsonResult OperationEdit(string Code, string Name, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetOperation(Code, Name, InputCode, Description, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
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

        //获取手术名称 lpf 20150713
        public JsonResult GetOperationName(string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetOperationName(Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //编辑体征匹配表 lpf 20150714
        public JsonResult AddToCompVitalSigns(string HospitalCode, string HZCode, string Type, string Code, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetMpVitalSignsCmp(HospitalCode, HZCode, Type, Code, Description, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

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


        #region "WY"
        //未匹配药物字典表
        public ActionResult UnCompDrug()
        {
            UnCompDrugViewModel UnCompDrug = new UnCompDrugViewModel();
            List<TmpDrugDict> TmpDrug = UnCompDrug.TmpDrug;
            DataSet info = _ServicesSoapClient.GetTmpDrugByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpDrugDict UnCompDruginfo = new TmpDrugDict();
                UnCompDruginfo.HospitalCode = row[0].ToString();
                UnCompDruginfo.HospitalName = row[1].ToString();
                UnCompDruginfo.DrugCode = row[2].ToString();
                UnCompDruginfo.DrugName = row[3].ToString();
                UnCompDruginfo.DrugSpec = row[4].ToString();
                UnCompDruginfo.Units = row[5].ToString();
                UnCompDruginfo.InputCode = row[6].ToString();
                UnCompDruginfo.Description = row[7].ToString();
                UnCompDruginfo.Status = Convert.ToInt32(row[8]);

                TmpDrug.Add(UnCompDruginfo);
            }
            return View(UnCompDrug);
        }

        //已匹配药物字典表
        public ActionResult CompDrug()
        {
            List<MpDrugCmp> drug = new List<MpDrugCmp>();
            DataSet info = _ServicesSoapClient.GetMpDrugCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpDrugCmp CompDrugninfo = new MpDrugCmp();
                CompDrugninfo.HospitalCode = row[0].ToString();
                CompDrugninfo.HospitalName = row[1].ToString();
                CompDrugninfo.DrugCode = row[2].ToString();
                CompDrugninfo.DrugName = row[3].ToString();
                CompDrugninfo.DrugSpec = row[4].ToString();
                CompDrugninfo.HZCode = row[5].ToString().Split(new char[2] { '*', '*' })[0].ToString();
                CompDrugninfo.HZName = row[6].ToString();
                CompDrugninfo.HZSpec = row[5].ToString().Split(new char[2] { '*', '*' })[2].ToString();
                CompDrugninfo.Redundance = row[7].ToString();

                drug.Add(CompDrugninfo);
            }
            return View(drug);
        }

        //药物字典表
        public ActionResult Drug()
        {
            DrugViewModel drugvm = new DrugViewModel();
            List<Drug> drug = new List<Drug>();
            DataSet info = _ServicesSoapClient.GetDrugNameList();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                Drug druginfo = new Drug();
                druginfo.DrugCode = row[0].ToString().Split(new char[2] { '*', '*' })[0];
                druginfo.DrugName = row[1].ToString();
                druginfo.DrugSpec = row[2].ToString();
                druginfo.Units = row[3].ToString();
                druginfo.InputCode = row[4].ToString();

                drug.Add(druginfo);
            }
            drugvm.Drug = drug;
            return View(drugvm);
        }

        //药物字典表数据插入
        public JsonResult DrugEdit(string DrugCode, string DrugName, string DrugSpec, string Units, string InputCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetDrug(DrugCode, DrugName, DrugSpec, Units, "1", InputCode);
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

        //获取药物名称
        public JsonResult GetDrugName(string DrugCode, string DrugSpec)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetDrugName(DrugCode, DrugSpec);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //由未匹配药物表插入匹配药物
        public JsonResult AddToCompDrug(string HospitalCode, string HZCode, string HZSpec, string DrugCode, string DrugSpec)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpDrugCmp(HospitalCode, HZCode, HZSpec, DrugCode, DrugSpec, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpDrug(HospitalCode, HZCode, HZSpec, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        
        //未匹配检验项目字典表
        public ActionResult UnCompLabTestItem()
        {
            UnCompLabViewModel UnCompLI = new UnCompLabViewModel();
            List<TmpLabItemDict> TmpLI = UnCompLI.TmpLabItem;
            DataSet info = _ServicesSoapClient.GetTmpLabItemByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpLabItemDict UnCompLIinfo = new TmpLabItemDict();
                UnCompLIinfo.HospitalCode = row[0].ToString();
                UnCompLIinfo.HospitalName = row[1].ToString();
                UnCompLIinfo.Code = row[2].ToString();
                UnCompLIinfo.Name = row[3].ToString();
                UnCompLIinfo.InputCode = row[4].ToString();
                UnCompLIinfo.Description = row[5].ToString();
                UnCompLIinfo.Status = Convert.ToInt32(row[6]);

                TmpLI.Add(UnCompLIinfo);
            }
            return View(UnCompLI);
        }

        //已匹配检验项目字典表
        public ActionResult CompLabTestItem()
        {
            List<MpLabItemCmp> labitems = new List<MpLabItemCmp>();
            DataSet info = _ServicesSoapClient.GetMpLabItemsCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpLabItemCmp CompLabItemsinfo = new MpLabItemCmp();
                CompLabItemsinfo.HospitalCode = row[0].ToString();
                CompLabItemsinfo.HospitalName = row[1].ToString();
                CompLabItemsinfo.Type = row[2].ToString();
                CompLabItemsinfo.TypeName = row[3].ToString();
                CompLabItemsinfo.Code = row[4].ToString();
                CompLabItemsinfo.Name = row[5].ToString();
                CompLabItemsinfo.HZCode = row[6].ToString();
                CompLabItemsinfo.HZName = row[7].ToString();
                CompLabItemsinfo.Redundance = row[8].ToString();

                labitems.Add(CompLabItemsinfo);
            }
            return View(labitems);
        }

        //检验项目字典表
        public ActionResult LabTestItem()
        {
            List<LabItem> LabItem = new List<LabItem>();
            DataSet info = _ServicesSoapClient.GetLabTestItems();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                LabItem labiteminfo = new LabItem();
                labiteminfo.Type = row[0].ToString();
                labiteminfo.Code = row[1].ToString();
                labiteminfo.TypeName = row[2].ToString();
                labiteminfo.Name = row[3].ToString();
                labiteminfo.InputCode = row[4].ToString();
                labiteminfo.SortNo = Convert.ToInt32(row[5]);
                labiteminfo.Description = row[6].ToString();

                LabItem.Add(labiteminfo);
            }

            return View(LabItem);
        }

        //由未匹配检验项目表插入匹配检验项目
        public JsonResult AddToCompLabItem(string HospitalCode, string HZCode, string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpLabItemCmp(HospitalCode, HZCode, Type, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpLabItem(HospitalCode, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检验项目字典表数据插入
        public JsonResult LabItemEdit(string Type, string Code, string TypeName, string Name, int SortNo, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetLabTestItem(Type, Code, TypeName, Name, SortNo, InputCode, Description, 0);
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

        //获取检验项目类别名称
        public JsonResult GetLabItemTypeNamebyType(string Type)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            string TypeName = _ServicesSoapClient.GetLabItemTypeNameByType(Type);

            res.Data = TypeName;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取检验项目名称
        public JsonResult GetLabItemName(string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetLabItemName(Type, Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //未匹配检验子项目字典表
        public ActionResult UnCompLabSubItem()
        {
            UnCompLabSubViewModel UnCompLSI = new UnCompLabSubViewModel();
            List<TmpLabSubItemDict> TmpLSI = UnCompLSI.TmpLabSubItem;
            DataSet info = _ServicesSoapClient.GetTmpLabSubItemByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpLabSubItemDict UnCompLIinfo = new TmpLabSubItemDict();
                UnCompLIinfo.HospitalCode = row[0].ToString();
                UnCompLIinfo.HospitalName = row[1].ToString();
                UnCompLIinfo.Code = row[2].ToString();
                UnCompLIinfo.Name = row[3].ToString();
                UnCompLIinfo.InputCode = row[4].ToString();
                UnCompLIinfo.Description = row[5].ToString();
                UnCompLIinfo.Status = Convert.ToInt32(row[6]);

                TmpLSI.Add(UnCompLIinfo);
            }
            return View(UnCompLSI);
        }

        //已匹配检验子项目字典表
        public ActionResult CompLabSubItem()
        {
            List<MpLabSubItemCmp> labsubitems = new List<MpLabSubItemCmp>();
            DataSet info = _ServicesSoapClient.GetMpLabSubItemCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpLabSubItemCmp CompLabSubItemsinfo = new MpLabSubItemCmp();
                CompLabSubItemsinfo.HospitalCode = row[0].ToString();
                CompLabSubItemsinfo.HospitalName = row[1].ToString();
                CompLabSubItemsinfo.Code = row[2].ToString();
                CompLabSubItemsinfo.Name = row[3].ToString();
                CompLabSubItemsinfo.HZCode = row[4].ToString();
                CompLabSubItemsinfo.HZName = row[5].ToString();
                CompLabSubItemsinfo.Redundance = row[6].ToString();

                labsubitems.Add(CompLabSubItemsinfo);
            }
            return View(labsubitems);
        }

        //检验项目子字典表
        public ActionResult LabSubItem()
        {
            List<LabSubItem> LabSubItems = new List<LabSubItem>();
            DataSet info = _ServicesSoapClient.GetLabTestSubItems();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                LabSubItem labsubiteminfo = new LabSubItem();
                labsubiteminfo.Code = row[0].ToString();
                labsubiteminfo.Name = row[1].ToString();
                labsubiteminfo.SortNo = Convert.ToInt32(row[2]);
                labsubiteminfo.InputCode = row[3].ToString();
                labsubiteminfo.Description = row[4].ToString();

                LabSubItems.Add(labsubiteminfo);
            }

            return View(LabSubItems);
        }

        //由未匹配检验子项目表插入匹配检验子项目
        public JsonResult AddToCompLabSubItem(string HospitalCode, string HZCode, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpLabSubItemCmp(HospitalCode, HZCode, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpLabSubItem(HospitalCode, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检验子项目字典表数据插入
        public JsonResult LabSubItemEdit(string Code, string Name, int SortNo, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetLabSubItem(Code, Name, SortNo, InputCode, Description, 0);
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

        //获取检验子项目名称
        public JsonResult GetLabSubItemName(string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetLabSubItemName(Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //未匹配检查子项目字典表
        public ActionResult UnCompExamSubItem()
        {
            UnCompExamSubViewModel UnCompESI = new UnCompExamSubViewModel();
            List<TmpExamSubItemDict> TmpESI = UnCompESI.TmpExamSubItem;
            DataSet info = _ServicesSoapClient.GetTmpExamItemByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpExamSubItemDict UnCompEIinfo = new TmpExamSubItemDict();
                UnCompEIinfo.HospitalCode = row[0].ToString();
                UnCompEIinfo.HospitalName = row[1].ToString();
                UnCompEIinfo.Code = row[2].ToString();
                UnCompEIinfo.Name = row[3].ToString();
                UnCompEIinfo.InputCode = row[4].ToString();
                UnCompEIinfo.Description = row[5].ToString();
                UnCompEIinfo.Status = Convert.ToInt32(row[6]);

                TmpESI.Add(UnCompEIinfo);
            }
            return View(UnCompESI);
        }

        //已匹配检查子项目字典表
        public ActionResult CompExamSubItem()
        {
            List<MpExamSubItemCmp> examsubitems = new List<MpExamSubItemCmp>();
            DataSet info = _ServicesSoapClient.GetMpExamItemCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpExamSubItemCmp CompExamSubItemsinfo = new MpExamSubItemCmp();
                CompExamSubItemsinfo.HospitalCode = row[0].ToString();
                CompExamSubItemsinfo.HospitalName = row[1].ToString();
                CompExamSubItemsinfo.Code = row[2].ToString();
                CompExamSubItemsinfo.Name = row[3].ToString();
                CompExamSubItemsinfo.HZCode = row[4].ToString();
                CompExamSubItemsinfo.HZName = row[5].ToString();
                CompExamSubItemsinfo.Redundance = row[6].ToString();

                examsubitems.Add(CompExamSubItemsinfo);
            }
            return View(examsubitems);
        }

        //检验项目子字典表
        public ActionResult ExamSubItem()
        {
            List<ExamSubItem> ExamSubItems = new List<ExamSubItem>();
            DataSet info = _ServicesSoapClient.GetExaminationSubItem();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                ExamSubItem examsubiteminfo = new ExamSubItem();
                examsubiteminfo.Code = row[0].ToString();
                examsubiteminfo.Name = row[1].ToString();
                examsubiteminfo.SortNo = Convert.ToInt32(row[2]);
                examsubiteminfo.InputCode = row[3].ToString();
                examsubiteminfo.Description = row[4].ToString();

                ExamSubItems.Add(examsubiteminfo);
            }

            return View(ExamSubItems);
        }

        //由未匹配检查子项目表插入匹配检查子项目
        public JsonResult AddToCompExamSubItem(string HospitalCode, string HZCode, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpExamItemCmp(HospitalCode, HZCode, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpExamItem(HospitalCode, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检查子项目字典表数据插入
        public JsonResult ExamSubItemEdit(string Code, string Name, int SortNo, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetExaminationSubItemS(Code, Name, SortNo, InputCode, Description, 0);
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

        //获取检查子项目名称
        public JsonResult GetExamSubItemName(string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetExamSubItemName(Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        //未匹配检查项目字典表
        public ActionResult UnCompExaminationItem()
        {
            UnCompExamViewModel UnCompEI = new UnCompExamViewModel();
            List<TmpExamDict> TmpEI = UnCompEI.TmpExamItem;
            DataSet info = _ServicesSoapClient.GetTmpExaminationByStatus(1);
            foreach (DataRow row in info.Tables[0].Rows)
            {
                TmpExamDict UnCompEIinfo = new TmpExamDict();
                UnCompEIinfo.HospitalCode = row[0].ToString();
                UnCompEIinfo.HospitalName = row[1].ToString();
                UnCompEIinfo.Code = row[2].ToString();
                UnCompEIinfo.TypeName = row[3].ToString().Split(new char[2] { '*', '*' })[0];
                UnCompEIinfo.Name = row[3].ToString().Split(new char[2] { '*', '*' })[2];
                UnCompEIinfo.InputCode = row[4].ToString();
                UnCompEIinfo.Description = row[5].ToString();
                UnCompEIinfo.Status = Convert.ToInt32(row[6]);

                TmpEI.Add(UnCompEIinfo);
            }
            return View(UnCompEI);
        }

        //已匹配检查项目字典表
        public ActionResult CompExaminationItem()
        {
            List<MpExaminationCmp> examitems = new List<MpExaminationCmp>();
            DataSet info = _ServicesSoapClient.GetMpExaminationCmp();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                MpExaminationCmp CompExamItemsinfo = new MpExaminationCmp();
                CompExamItemsinfo.HospitalCode = row[0].ToString();
                CompExamItemsinfo.HospitalName = row[1].ToString();
                CompExamItemsinfo.Type = row[2].ToString();
                CompExamItemsinfo.TypeName = row[3].ToString();
                CompExamItemsinfo.Code = row[4].ToString();
                CompExamItemsinfo.Name = row[5].ToString();
                CompExamItemsinfo.HZCode = row[6].ToString();
                CompExamItemsinfo.HZName = row[7].ToString();
                CompExamItemsinfo.Redundance = row[8].ToString();

                examitems.Add(CompExamItemsinfo);
            }
            return View(examitems);
        }

        //检验项目字典表
        public ActionResult ExaminationItems()
        {
            List<ExaminationItem> ExamItems = new List<ExaminationItem>();
            DataSet info = _ServicesSoapClient.GetExaminationItem();
            foreach (DataRow row in info.Tables[0].Rows)
            {
                ExaminationItem examiteminfo = new ExaminationItem();
                examiteminfo.Type = row[0].ToString();
                examiteminfo.Code = row[1].ToString();
                examiteminfo.TypeName = row[2].ToString();
                examiteminfo.Name = row[3].ToString();
                examiteminfo.InputCode = row[4].ToString();
                examiteminfo.SortNo = Convert.ToInt32(row[5]);
                examiteminfo.Description = row[6].ToString();

                ExamItems.Add(examiteminfo);
            }

            return View(ExamItems);
        }

        //由未匹配检查项目表插入匹配检查项目
        public JsonResult AddToCompExamItem(string HospitalCode, string HZCode, string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            bool flag = _ServicesSoapClient.SetMpExaminationCmp(HospitalCode, HZCode, Type, Code, "手动匹配", user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);

            if (flag)
            {
                flag = _ServicesSoapClient.ChangeStatusForTmpExamination(HospitalCode, HZCode, 2);
                if (flag)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //检查项目字典表数据插入
        public JsonResult ExaminationEdit(string Type, string Code, string TypeName, string Name, int SortNo, string InputCode, string Description)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = _ServicesSoapClient.SetExamItem(Type, Code, TypeName, Name, SortNo, InputCode, Description, 0);
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

        //获取检查项目类别名称
        public JsonResult GetExaminationTypeNamebyType(string Type)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //int TypeInt = Convert.ToInt32(Type);
            string TypeName = _ServicesSoapClient.GetExamItemTypeNameByType(Type);

            res.Data = TypeName;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //获取检查项目名称
        public JsonResult GetExaminationName(string Type, string Code)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string Name = _ServicesSoapClient.GetExamItemName(Type, Code);

            res.Data = Name;

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion
    }
}

