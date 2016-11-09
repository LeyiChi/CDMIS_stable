using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CDMIS.ServiceReference;
using System.Web.UI.WebControls;

namespace CDMIS.ViewModels
{
    public class CommonVariables
    {
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();

        #region <ZC>
        public static List<SelectListItem> GetGenderList()
        {
            List<SelectListItem> GenderList = new List<SelectListItem>();
            DataSet ds = _ServicesSoapClient.GetTypeList("SexType");
            if (ds == null)
            {
                return GenderList;
            }
            GenderList.Add(new SelectListItem { Text = "---请选择---", Value = "" });
            foreach (DataTable itemtable in ds.Tables)
            {
                foreach (DataRow item in itemtable.Rows)
                {
                    GenderList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                }
            }
            return GenderList;
        }

        public static List<SelectListItem> GetBloodTypeList()
        {
            List<SelectListItem> BloodTypeList = new List<SelectListItem>();
            DataSet ds = _ServicesSoapClient.GetTypeList("AboBloodType");
            if (ds == null)
            {
                return BloodTypeList;
            }
            BloodTypeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataTable itemtable in ds.Tables)
            {
                foreach (DataRow item in itemtable.Rows)
                {
                    BloodTypeList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                }
            }
            return BloodTypeList;
        }

        //public static List<SelectListItem> GetInsuranceTypeList()
        //{
        //    List<SelectListItem> InsuranceTypeList = new List<SelectListItem>();
        //    DataSet ds = _ServicesSoapClient.GetTypeList("InsuranceType");
        //    if (ds == null)
        //    {
        //        return InsuranceTypeList;
        //    }
        //    InsuranceTypeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
        //    foreach (DataTable itemtable in ds.Tables)
        //    {
        //        foreach (DataRow item in itemtable.Rows)
        //        {
        //            InsuranceTypeList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

        //        }
        //    }
        //    return InsuranceTypeList;
        //}

        public static List<SelectListItem> GetInsuranceTypeList()
        {
            List<SelectListItem> InsuranceTypeList = new List<SelectListItem>();
            DataSet ds = _ServicesSoapClient.GetInsuranceType();
            if (ds == null)
            {
                return InsuranceTypeList;
            }
            InsuranceTypeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataTable itemtable in ds.Tables)
            {
                foreach (DataRow item in itemtable.Rows)
                {
                    InsuranceTypeList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Code"].ToString() });

                }
            }
            return InsuranceTypeList;
        }

        public static List<CheckBox> GetBasicModuleList()
        {
            List<CheckBox> ModuleList = new List<CheckBox>();
            DataTable dt = _ServicesSoapClient.GetModuleList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ModuleList.Add(new CheckBox { Text = dr["Name"].ToString() });
            }
            return ModuleList;
        }

        public static List<SelectListItem> GetModuleList()
        {
            List<SelectListItem> ModuleList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetModuleList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ModuleList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Code"].ToString() });
            }
            return ModuleList;
        }

        public static IEnumerable<SelectListItem> GetAllModuleList()
        {
            List<SelectListItem> ModuleList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetModuleList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ModuleList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Code"].ToString() });
            }
            return ModuleList.AsEnumerable();
        }
    

        public static List<SelectListItem> GetVitalSignsTypeNameList()
        {
            List<SelectListItem> VitalSignsTypeNameList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetVitalSignsTypeNameList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                VitalSignsTypeNameList.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["Type"].ToString() });
            }
            return VitalSignsTypeNameList;
        }

        public static List<SelectListItem> GetDoctorList()
        {
            List<SelectListItem> DoctorList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetDoctorList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DoctorList.Add(new SelectListItem { Text = dr["UserName"].ToString(), Value = dr["UserId"].ToString() });
            }
            return DoctorList;
        }
        #endregion

        #region <ZAM 2015-01-04>

        public static List<SelectListItem> GetCareLevelList()
        {
            List<SelectListItem> CareLevelList = new List<SelectListItem>();
            try
            {
                DataSet ds = _ServicesSoapClient.GetTypeList("SpecialLevel");
                if (ds == null)
                {
                    return CareLevelList;
                }
                CareLevelList.Add(new SelectListItem { Text = "全部", Value = "0" });
                foreach (DataTable itemtable in ds.Tables)
                {
                    foreach (DataRow item in itemtable.Rows)
                    {
                        CareLevelList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                    }
                }
                return CareLevelList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            //CareLevelList.Add(new SelectListItem { Text = "", Value = "0" });
            //CareLevelList.Add(new SelectListItem { Text = "一般关注", Value = "1" });
            //CareLevelList.Add(new SelectListItem { Text = "特殊关注", Value = "2" });
        }

        public static List<SelectListItem> GetAlertStatusList()
        {
            List<SelectListItem> AlertStatusList = new List<SelectListItem>();
            try
            {
                DataSet ds = _ServicesSoapClient.GetTypeList("ProcessStatus");
                if (ds == null)
                {
                    return AlertStatusList;
                }
                AlertStatusList.Add(new SelectListItem { Text = "全部", Value = "0" });

                foreach (DataTable itemtable in ds.Tables)
                {
                    foreach (DataRow item in itemtable.Rows)
                    {
                        AlertStatusList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                    }
                }
                return AlertStatusList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<SelectListItem> GetModuleListByDoctorId(string doctorId)
        {
            List<SelectListItem> ModuleList = new List<SelectListItem>();
            try
            {
                DataSet ds = _ServicesSoapClient.GetDoctorModuleList(doctorId);
                if (ds == null)
                {
                    return ModuleList;
                }
                ModuleList.Add(new SelectListItem { Text = "全部", Value = "" });

                foreach (DataTable itemtable in ds.Tables)
                {
                    foreach (DataRow item in itemtable.Rows)
                    {
                        //value ? index
                        ModuleList.Add(new SelectListItem { Text = item["CategoryName"].ToString(), Value = item["CategoryCode"].ToString() });

                    }
                }

                ////Test Data
                //ModuleList.Add(new SelectListItem { Text = "高血压模块", Value = "M1" });
                //ModuleList.Add(new SelectListItem { Text = "糖尿病模块", Value = "M2" });
                //ModuleList.Add(new SelectListItem { Text = "心律失常模块", Value = "M3" });
                //ModuleList.Add(new SelectListItem { Text = "心力衰竭模块", Value = "M4" });
                //ModuleList.Add(new SelectListItem { Text = "健康管理模块", Value = "M5" });

                return ModuleList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region <ZYF>
        //public static List<SelectListItem> GetUserClassList()
        //{
        //    List<SelectListItem> UserClassList = new List<SelectListItem>();
        //    UserClassList.Add(new SelectListItem { Text = "管理员", Value = "Administrator" });
        //    UserClassList.Add(new SelectListItem { Text = "医生", Value = "Doctor" });
        //    UserClassList.Add(new SelectListItem { Text = "患者", Value = "Patient" });

        //    return UserClassList;
        //}
        public static List<SelectListItem> GetUserClassList()
        {
            List<SelectListItem> UserClassList = new List<SelectListItem>();
            try
            {
                DataSet ds = _ServicesSoapClient.GetRoleList();
                if (ds == null)
                {
                    return UserClassList;
                }
                UserClassList.Add(new SelectListItem { Text = "全部", Value = "" });
                foreach (DataTable itemtable in ds.Tables)
                {
                    foreach (DataRow item in itemtable.Rows)
                    {
                        
                        UserClassList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Code"].ToString() });

                    }
                }
                return UserClassList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        public static List<SelectListItem> GetRoleClassList()
        {
            List<SelectListItem> RoleClassList = new List<SelectListItem>();
            RoleClassList.Add(new SelectListItem { Text = "——请选择——", Value = "0" });
            RoleClassList.Add(new SelectListItem { Text = "管理员", Value = "Administrator" });
            RoleClassList.Add(new SelectListItem { Text = "医生", Value = "Doctor" });
            RoleClassList.Add(new SelectListItem { Text = "健康专员", Value = "HealthCoach" });

            return RoleClassList;
        }

        public static List<SelectListItem> GetRoleNameList()
        {
            List<SelectListItem> RoleNameList = new List<SelectListItem>();
            RoleNameList.Add(new SelectListItem { Text = "管理员", Value = "Administrator" });
            RoleNameList.Add(new SelectListItem { Text = "医生", Value = "Doctor" });
            RoleNameList.Add(new SelectListItem { Text = "患者", Value = "Patient" });

            return RoleNameList;
        }

        #endregion

        #region <YDS>
        public static List<SelectListItem> GetDictList()
        {
            List<SelectListItem> DictList = new List<SelectListItem>();
            DictList.Add(new SelectListItem { Text = "用户信息表", Value = "1" });
            DictList.Add(new SelectListItem { Text = "详细信息表", Value = "2" });
            DictList.Add(new SelectListItem { Text = "类型字典表", Value = "3" });//父子表
            DictList.Add(new SelectListItem { Text = "科室字典表", Value = "4" });
            DictList.Add(new SelectListItem { Text = "医院字典表", Value = "5" });
            DictList.Add(new SelectListItem { Text = "症状字典表", Value = "6" });
            DictList.Add(new SelectListItem { Text = "诊断字典表", Value = "7" });
            DictList.Add(new SelectListItem { Text = "体征字典表", Value = "8" });
            DictList.Add(new SelectListItem { Text = "检查字典表", Value = "9" });
            DictList.Add(new SelectListItem { Text = "检查子项目字典表", Value = "10" });
            DictList.Add(new SelectListItem { Text = "检验字典表", Value = "11" });
            DictList.Add(new SelectListItem { Text = "检验子项目字典表", Value = "12" });
            DictList.Add(new SelectListItem { Text = "治疗字典表", Value = "13" });
            DictList.Add(new SelectListItem { Text = "警戒字典表", Value = "14" });
            DictList.Add(new SelectListItem { Text = "血压字典表", Value = "15" });
            DictList.Add(new SelectListItem { Text = "生活方式字典表", Value = "16" });//父子表
            DictList.Add(new SelectListItem { Text = "生活方式详细字典表", Value = "17" });
            DictList.Add(new SelectListItem { Text = "手术字典表", Value = "18" });
            DictList.Add(new SelectListItem { Text = "药物字典表", Value = "19" });
            DictList.Add(new SelectListItem { Text = "医保类别字典表", Value = "20" });
            DictList.Add(new SelectListItem { Text = "任务字典表", Value = "21" });
            //DictList.Add(new SelectListItem { Text = "任务子项目字典表", Value = "22" });
            return DictList;
        }

        public static List<SelectListItem> GetOptionCategoryList()
        {
            List<SelectListItem> OptionCategoryList = new List<SelectListItem>();
            OptionCategoryList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            OptionCategoryList.Add(new SelectListItem { Text = "是否类型", Value = "YesNoType" });
            OptionCategoryList.Add(new SelectListItem { Text = "血型类型", Value = "AboBloodType" });
            OptionCategoryList.Add(new SelectListItem { Text = "性别类型", Value = "SexType" });
            OptionCategoryList.Add(new SelectListItem { Text = "无", Value = "" });
            return OptionCategoryList;
        }

        public static List<SelectListItem> GetControlTypeList()
        {
            DataSet typeinfo = _ServicesSoapClient.GetTypeList("ControlType");
            List<SelectListItem> ControlTypeList = new List<SelectListItem>();
            ControlTypeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in typeinfo.Tables[0].Rows)
            {
                ControlTypeList.Add(new SelectListItem { Text = row[1].ToString().Trim(), Value = row[0].ToString().Trim() });
            }
            return ControlTypeList;
        }

        public static List<SelectListItem> GetCategoryCodeList()
        {
            DataSet categoryinfo = _ServicesSoapClient.GetInfoItemCategory();
            List<SelectListItem> CategoryCodeList = new List<SelectListItem>();
            CategoryCodeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in categoryinfo.Tables[0].Rows)
            {
                CategoryCodeList.Add(new SelectListItem { Text = row[1].ToString().Trim(), Value = row[0].ToString().Trim() });
            }
            return CategoryCodeList;
        }

        //警戒字典表获取单位下拉框 2015-01-13
        public static List<SelectListItem> GetAllUnit()
        {
            List<SelectListItem> Unit = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("CommonUnit").Tables[0];
            Unit.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow dr in dt.Rows)
            {
                Unit.Add(new SelectListItem { Text = dr["Name"].ToString().Trim(), Value = dr["Type"].ToString().Trim() });
            }
            return Unit;
        }
        #endregion

        #region <CSQ>
        //public static List<SelectListItem> GetInfoItemSelectList()
        //{
        //    List<SelectListItem> InfoItemSelectList = new List<SelectListItem>();
        //    InfoItemSelectList.Add(new SelectListItem { Text = "是否遗传", Value = "1" });
        //    InfoItemSelectList.Add(new SelectListItem { Text = "是否有家族史", Value = "2" });
        //    return InfoItemSelectList;
        //}

        public static List<SelectListItem> GetDiagnosisTypeList()
        {
            List<SelectListItem> DiagnosisTypeList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("DiagnosisType").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DiagnosisTypeList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }

            return DiagnosisTypeList;
        }

        public static List<SelectListItem> GetTypeList()
        {
            List<SelectListItem> DiagnosisTypeList = new List<SelectListItem>();
            DiagnosisTypeList.Add(new SelectListItem { Text = "请选择", Value = "" });

            DataTable dt = _ServicesSoapClient.GetDiagTypeNameList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DiagnosisTypeList.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["Type"].ToString() });
            }
            return DiagnosisTypeList;
        }

        //public static List<SelectListItem> GetDiagnosisNameList()
        //{
        //    List<SelectListItem> DiagnosisNameList = new List<SelectListItem>();
        //    DiagnosisNameList.Add(new SelectListItem { Text = "神经病", Value = "1" });
        //    DiagnosisNameList.Add(new SelectListItem { Text = "精神病", Value = "2" });
        //    DiagnosisNameList.Add(new SelectListItem { Text = "糖尿病", Value = "3" });
        //    DiagnosisNameList.Add(new SelectListItem { Text = "高血压", Value = "4" });
        //    DiagnosisNameList.Add(new SelectListItem { Text = "乳腺癌", Value = "5" });
        //    return DiagnosisNameList;
        //}

        public static List<SelectListItem> GetHospitalList()
        {
            List<SelectListItem> HospitalList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetHospitalList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                HospitalList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Code"].ToString() });
            }
            return HospitalList;
        }

        public static List<SelectListItem> GetVisitTypeList()
        {
            List<SelectListItem> VisitTypeList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("VisitType").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                VisitTypeList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return VisitTypeList;
        }

        public static List<SelectListItem> GetExamTypeList()
        {
            List<SelectListItem> ExamTypeList = new List<SelectListItem>();
            ExamTypeList.Add(new SelectListItem { Text = "请选择", Value = "" });

            DataTable dt = _ServicesSoapClient.GetExamItemTypeNameList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ExamTypeList.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["Type"].ToString() });
            }
            return ExamTypeList;
        }

        //public static List<SelectListItem> GetExamSubItemList()
        //{
        //    List<SelectListItem> ExamSubItemList = new List<SelectListItem>();
        //    DataTable dt = _ServicesSoapClient.GetExamSubItemNameList("").Tables[0];
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        ExamSubItemList.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["Type"].ToString() });
        //    }
        //    return ExamSubItemList;
        //}

        public static List<SelectListItem> GetUnit()   //单次剂量单位
        {
            List<SelectListItem> Unit = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("OrderingUsageUnit").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                Unit.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return Unit;
        }

        public static List<SelectListItem> GetStatus()
        {
            List<SelectListItem> StatusList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("ExamStatus").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                StatusList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return StatusList;
        }

        public static List<SelectListItem> GetIsAbnormal()
        {
            List<SelectListItem> IsAbnormal = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("NegativePositiveType").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                IsAbnormal.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return IsAbnormal;
        }

        public static List<SelectListItem> GetIsAbnormal2()
        {
            List<SelectListItem> IsAbnormal = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("HighLowType").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                IsAbnormal.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return IsAbnormal;
        }


        public static List<SelectListItem> GetLabItemTypeNameList()
        {
            List<SelectListItem> LabItemTypeNameList = new List<SelectListItem>();
            LabItemTypeNameList.Add(new SelectListItem { Text = "请选择", Value = "" });

            DataTable dt = _ServicesSoapClient.GetLabTestItemsTypeNameList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LabItemTypeNameList.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["Type"].ToString() });
            }
            return LabItemTypeNameList;
        }

        //public static List<SelectListItem> GetLabSubItemList()
        //{
        //    List<SelectListItem> LabSubItemList = new List<SelectListItem>();
        //    LabSubItemList.Add(new SelectListItem { Text = "红细胞", Value = "1" });
        //    LabSubItemList.Add(new SelectListItem { Text = "血红蛋白", Value = "2" });
        //    LabSubItemList.Add(new SelectListItem { Text = "白细胞", Value = "3" });
        //    return LabSubItemList;
        //}

        public static List<SelectListItem> GetDrugTypeList()
        {
            List<SelectListItem> DrugTypeList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("OrderClass").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DrugTypeList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return DrugTypeList;
        }

        //GetDrugNameList 20150513 CSQ
        public static List<SelectListItem> GetDrugNameList()
        {
            List<SelectListItem> DrugNameList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetDrugNameList().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DrugNameList.Add(new SelectListItem { Text = dr["DrugName"].ToString(), Value = dr["DrugCode"].ToString() });
            }
            return DrugNameList;
        }

        public static List<SelectListItem> GetRouteList()
        {
            List<SelectListItem> RouteList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("DrugAction").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                RouteList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return RouteList;
        }

        public static List<SelectListItem> GetFreqUnit()
        {
            List<SelectListItem> FreqUnit = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("CommonUnit").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                FreqUnit.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return FreqUnit;
        }

        public static List<SelectListItem> GetFreqUnit2()
        {
            List<SelectListItem> FreqUnit2 = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("TimeUnits").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                FreqUnit2.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return FreqUnit2;
        }

        public static List<SelectListItem> GetYesNoTypeList()
        {
            List<SelectListItem> YesNoTypeList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("YesNoType").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                YesNoTypeList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return YesNoTypeList;
        }

        public static List<SelectListItem> GetDeptList()
        {
            List<SelectListItem> DeptList = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetDivision().Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                DeptList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Code"].ToString() });
            }
            return DeptList;
        }

        public static List<SelectListItem> GetLabResultUnit()   //单次剂量单位
        {
            List<SelectListItem> Unit = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("LabResultUnit").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                Unit.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Type"].ToString() });
            }
            return Unit;
        }
        #endregion

        #region <LS>
        //患者信息-健康参数界面-选择评估类型  LS 2014-12-09
        public static List<SelectListItem> GetAssessmentTypeList()
        {
            List<SelectListItem> AssessmentTypeList = new List<SelectListItem>();
            AssessmentTypeList.Add(new SelectListItem { Text = "评估类型1", Value = "1" });
            AssessmentTypeList.Add(new SelectListItem { Text = "评估类型2", Value = "2" });
            return AssessmentTypeList;
        }

        //患者信息-健康参数界面-选择评估名称  LS 2014-12-09
        public static List<SelectListItem> GetAssessmentNameList()
        {
            List<SelectListItem> AssessmentNameList = new List<SelectListItem>();
            AssessmentNameList.Add(new SelectListItem { Text = "评估名称1", Value = "1" });
            AssessmentNameList.Add(new SelectListItem { Text = "评估名称2", Value = "2" });
            return AssessmentNameList;
        }

        //患者信息-每日任务-提醒类型选择下拉框  LS 2014-12-09
        public static List<SelectListItem> GetReminderTypeList()
        {
            List<SelectListItem> ReminderTypeList = new List<SelectListItem>();
            DataSet ds = _ServicesSoapClient.GetTypeList("ReminderType");
            if (ds == null)
            {
                return ReminderTypeList;
            }
            ReminderTypeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataTable itemtable in ds.Tables)
            {
                foreach (DataRow item in itemtable.Rows)
                {
                    ReminderTypeList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                }
            }
            return ReminderTypeList;
        }

        //患者信息-每日任务-提醒方式选择下拉框  LS 2014-12-09
        public static List<SelectListItem> GetAlertModeList()
        {
            List<SelectListItem> AlertModeList = new List<SelectListItem>();
            DataSet ds = _ServicesSoapClient.GetTypeList("AlertMode");
            if (ds == null)
            {
                return AlertModeList;
            }
            AlertModeList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataTable itemtable in ds.Tables)
            {
                foreach (DataRow item in itemtable.Rows)
                {
                    AlertModeList.Add(new SelectListItem { Text = item["Name"].ToString(), Value = item["Type"].ToString() });

                }
            }
            return AlertModeList;
        }

        //患者信息-每日任务-每周：周  LS 2014-12-09
        public static List<SelectListItem> GetWeeklyList()
        {
            List<SelectListItem> WeeklyList = new List<SelectListItem>();
            WeeklyList.Add(new SelectListItem { Text = "周一", Value = "1" });
            WeeklyList.Add(new SelectListItem { Text = "周二", Value = "2" });
            WeeklyList.Add(new SelectListItem { Text = "周三", Value = "3" });
            WeeklyList.Add(new SelectListItem { Text = "周四", Value = "4" });
            WeeklyList.Add(new SelectListItem { Text = "周五", Value = "5" });
            WeeklyList.Add(new SelectListItem { Text = "周六", Value = "6" });
            WeeklyList.Add(new SelectListItem { Text = "周日", Value = "7" });
            return WeeklyList;
        }

        //患者信息-每日任务-每月：日期  LS 2014-12-09
        public static List<SelectListItem> GetMonthlyList()
        {
            List<SelectListItem> MonthlyList = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                MonthlyList.Add(new SelectListItem { Text = i + "日", Value = i.ToString() });
                //MonthlyList.Add(new SelectListItem { Text = i.ToString(), Value = i + "日" });
            }
            return MonthlyList;
        }

        //患者信息-每日任务-每年：月份  LS 2014-12-09
        public static List<SelectListItem> GetAnnualMothList()
        {
            List<SelectListItem> AnnualMothList = new List<SelectListItem>();
            for (int i = 1; i < 13; i++)
            {
                if (i < 10)
                {
                    AnnualMothList.Add(new SelectListItem { Text = i + "月", Value = "0" + i.ToString() });
                }
                else
                {
                    AnnualMothList.Add(new SelectListItem { Text = i + "月", Value = i.ToString() });
                }

            }
            return AnnualMothList;
        }

        //患者信息-每日任务-每年：日  LS 2014-12-09
        public static List<SelectListItem> GetAnnualDayList()
        {
            List<SelectListItem> AnnualDayList = new List<SelectListItem>();
            for (int i = 1; i < 32; i++)
            {
                if (i < 10)
                {
                    AnnualDayList.Add(new SelectListItem { Text = i + "日", Value = "0" + i.ToString() });
                }
                else
                {
                    AnnualDayList.Add(new SelectListItem { Text = i + "日", Value = i.ToString() });
                }
            }
            return AnnualDayList;
        }

        #endregion

        #region <WF>
        public static List<SelectListItem> GetSymptomsTypeList()
        {
            //症状类型下拉框
            List<SelectListItem> SymptomsTypeList = new List<SelectListItem>();
            DataSet SymptomsTypeDS = _ServicesSoapClient.GetSymptomsTypeList();
            DataTable SymptomsTypeDT = SymptomsTypeDS.Tables[0];
            SymptomsTypeList.Add(new SelectListItem { Text = "---------请选择---------", Value = "0" });
            foreach (DataRow DR in SymptomsTypeDT.Rows)
            {
                SymptomsTypeList.Add(new SelectListItem { Text = DR["TypeName"].ToString(), Value = DR["Type"].ToString() });
            }
            return SymptomsTypeList;
        }

        public static List<SelectListItem> GetSymptomsNameList(string Type)
        {
            List<SelectListItem> SymptomsNameList = new List<SelectListItem>();
            SymptomsNameList.Add(new SelectListItem { Text = "---请选择症状类型---", Value = "0" });
            if (Type != "0")
            {
                DataSet SymptomsNameDS = _ServicesSoapClient.GetSymptomsNameList(Type);
                DataTable SymptomsNameDT = SymptomsNameDS.Tables[0];
                foreach (DataRow DR in SymptomsNameDT.Rows)
                {
                    SymptomsNameList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["Code"].ToString() });
                }
            }
            return SymptomsNameList;
        }

        public static List<SelectListItem> GetTreatmentGoalList()
        {
            List<SelectListItem> TreatmentGoalList = new List<SelectListItem>();
            DataSet TreatmentGoalDS = _ServicesSoapClient.GetTreatmentGoalsList();
            DataTable TreatmentGoalDT = TreatmentGoalDS.Tables[0];
            TreatmentGoalList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow DR in TreatmentGoalDT.Rows)
            {
                TreatmentGoalList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["Code"].ToString() });
            }
            return TreatmentGoalList;
        }

        public static List<SelectListItem> GetTreatmentAction()
        {
            List<SelectListItem> TreatmentActionList = new List<SelectListItem>();
            DataSet TreatmentActionDS = _ServicesSoapClient.GetTreatmentActionList();
            DataTable TreatmentActionDT = TreatmentActionDS.Tables[0];
            TreatmentActionList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow DR in TreatmentActionDT.Rows)
            {
                TreatmentActionList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["Code"].ToString() });
            }
            return TreatmentActionList;
        }

        public static List<SelectListItem> GetSpecialGroupList()
        {
            List<SelectListItem> SpecialGroupList = new List<SelectListItem>();
            DataSet SpecialGroupDS = _ServicesSoapClient.GetGroupList();
            DataTable SpecialGroupDT = SpecialGroupDS.Tables[0];
            SpecialGroupList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow DR in SpecialGroupDT.Rows)
            {
                SpecialGroupList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["Code"].ToString() });
            }
            return SpecialGroupList;
        }

        public static List<SelectListItem> GetDurationNameList()
        {
            List<SelectListItem> DurationNameList = new List<SelectListItem>();
            DataSet DurationNameDS = _ServicesSoapClient.GetDurationNameList();
            DataTable DurationNameDT = DurationNameDS.Tables[0];
            DurationNameList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow DR in DurationNameDT.Rows)
            {
                DurationNameList.Add(new SelectListItem { Text = DR["Name"].ToString(), Value = DR["Type"].ToString() });
            }
            return DurationNameList;
        }

        public static List<SelectListItem> GetDivisionList()
        {
            DataSet Divisioninfo = _ServicesSoapClient.GetDivision();
            List<SelectListItem> DivisionList = new List<SelectListItem>();
            DivisionList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in Divisioninfo.Tables[0].Rows)
            {
                DivisionList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() + "#" + row["Code"].ToString().Trim() });
            }
            return DivisionList;
        }

        public static List<SelectListItem> GetDiagnosisList()
        {
            DataSet Divisioninfo = _ServicesSoapClient.GetDiagnosis();
            List<SelectListItem> DivisionList = new List<SelectListItem>();
            DivisionList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in Divisioninfo.Tables[0].Rows)
            {
                DivisionList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() + "#" + row["Code"].ToString().Trim() });
            }
            return DivisionList;
        }

        //获得手术列表
        public static List<SelectListItem> GetOperationList()
        {
            DataSet Operationinfo = _ServicesSoapClient.GetOperation();
            List<SelectListItem> OperationList = new List<SelectListItem>();
            OperationList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in Operationinfo.Tables[0].Rows)
            {
                OperationList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Code"].ToString().Trim() });
            }
            return OperationList;
        }

        //获得体征列表
        public static List<SelectListItem> GetVitalSignsList()
        {
            DataSet VitalSignsinfo = _ServicesSoapClient.GetVitalSigns();
            List<SelectListItem> VitalSignsList = new List<SelectListItem>();
            VitalSignsList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in VitalSignsinfo.Tables[0].Rows)
            {
                VitalSignsList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() + "#" + row["Code"].ToString().Trim() });
            }
            return VitalSignsList;
        }

        #endregion

        #region "WY"

        public static List<SelectListItem> GetDrugList()
        {
            DataSet Druginfo = _ServicesSoapClient.GetDrugNameList();
            List<SelectListItem> DrugList = new List<SelectListItem>();
            DrugList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in Druginfo.Tables[0].Rows)
            {
                DrugList.Add(new SelectListItem { Text = row["DrugName"].ToString().Trim(), Value = row["DrugCode"].ToString().Trim() + "#" + row["DrugSpec"].ToString().Trim() });
            }
            return DrugList;
        }

        public static List<SelectListItem> GetOrderingMaterialUnitList() //获取药物单位字典
        {
            List<SelectListItem> Unit = new List<SelectListItem>();
            DataTable dt = _ServicesSoapClient.GetTypeList("OrderingMaterialUnit").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                Unit.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Name"].ToString() });
            }
            return Unit;
        }

        public static List<SelectListItem> GetLabItemList()
        {
            DataSet LabTestIteminfo = _ServicesSoapClient.GetLabTestItems();
            List<SelectListItem> LabTestItemList = new List<SelectListItem>();
            LabTestItemList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in LabTestIteminfo.Tables[0].Rows)
            {
                LabTestItemList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() + "#" + row["Code"].ToString().Trim() });
            }
            return LabTestItemList;
        }

        public static List<SelectListItem> GetLabSubItemList()
        {
            DataSet LabSubIteminfo = _ServicesSoapClient.GetLabTestSubItems();
            List<SelectListItem> LabSubItemList = new List<SelectListItem>();
            LabSubItemList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in LabSubIteminfo.Tables[0].Rows)
            {
                LabSubItemList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Code"].ToString().Trim() });
            }
            return LabSubItemList;
        }

        public static List<SelectListItem> GetExamSubItemList()
        {
            DataSet ExamSubIteminfo = _ServicesSoapClient.GetExaminationSubItem();
            List<SelectListItem> ExamSubItemList = new List<SelectListItem>();
            ExamSubItemList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow dr in ExamSubIteminfo.Tables[0].Rows)
            {
                ExamSubItemList.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["Code"].ToString() });
            }
            return ExamSubItemList;
        }

        public static List<SelectListItem> GetExaminationItemList()
        {
            DataSet ExamIteminfo = _ServicesSoapClient.GetExaminationItem();
            List<SelectListItem> ExamItemList = new List<SelectListItem>();
            ExamItemList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in ExamIteminfo.Tables[0].Rows)
            {
                ExamItemList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() + "#" + row["Code"].ToString().Trim() });
            }
            return ExamItemList;
        }
        #endregion

        #region "LY"

        public static List<SelectListItem> GetJobTitleList()
        {
            DataSet JobTitleInfo = _ServicesSoapClient.GetTypeList("JobTitle");
            List<SelectListItem> JobTitleList = new List<SelectListItem>();
            JobTitleList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in JobTitleInfo.Tables[0].Rows)
            {
                JobTitleList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() });
            }
            return JobTitleList;
        }

        public static List<SelectListItem> GetLevelList()
        {
            DataSet LevelInfo = _ServicesSoapClient.GetTypeList("TitleLevel");
            List<SelectListItem> LevelList = new List<SelectListItem>();
            LevelList.Add(new SelectListItem { Text = "---请选择---", Value = "0" });
            foreach (DataRow row in LevelInfo.Tables[0].Rows)
            {
                LevelList.Add(new SelectListItem { Text = row["Name"].ToString().Trim(), Value = row["Type"].ToString().Trim() });
            }
            return LevelList;
        }

        #endregion
    }
}