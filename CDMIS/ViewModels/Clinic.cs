using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CDMIS.ViewModels
{
    public class Clinic
    {
    }

    //参数图画图     新增
    public class Picture
    {
        public string lineColor { get; set; }
        public string date { get; set; }
        public decimal duration { get; set; }  //实际值
        public string unit { get; set; }
        public decimal min { get; set; }
        public decimal max { get; set; }
    }

    //时间轴-基础-基础     新增
    public class ClinicBasicInfoandTime
    {
        public string Time { get; set; }

        public List<SomeDayEvent> ItemGroup { get; set; }     //时间轴 

        public string Location { get; set; }

        public ClinicBasicInfoandTime()
        {
            ItemGroup = new List<SomeDayEvent>();
        }
    }

    //时间轴-详细 新增
    public class ClinicInfoDetailViewModel
    {
       
        public string UserId { get; set; }

        public string VisitId { get; set; }

        public string NowType { get; set; }  //目前类型

        public string UserName { get; set; }

        public string NowDate { get; set; }

        public DataTable DiagnosisInfoTable { get; set; }  //某天诊断信息

        public DataTable ExaminationInfoTable { get; set; }  //某天检查信息

        public DataTable LabTestInfoTable { get; set; }  //某天化验信息

        public DataTable DrugRecordTable { get; set; }  //某天化验信息

    }


    //时间轴-基础-总     新增   
    public class ClinicBasicInfoViewModel
    {
        public string UserId { get; set; }

        public DataTable datasource { get; set; }

        public List<ClinicBasicInfoandTime> ClinicBasicInfoandTime { get; set; }     //时间轴

        public ClinicBasicInfoViewModel()
        {
            ClinicBasicInfoandTime = new List<ClinicBasicInfoandTime>();
        }
    }


    //患者信息-临床信息-检查/化验子表      新增
    public class ClinicInfoDetailByTypeViewModel
    {
        public string NowDetailType { get; set; }

        public DataTable ExamDetailsTable { get; set; }  //某天诊断信息

        public DataTable LabTestDetailsTable { get; set; }  //某天检查信息
    }


    //患者信息-健康参数界面   修改
    public class HealthParametersViewModel
    {
        //上半部为用到的
        public string UserId { get; set; }

        public string VitalSignSelectItem { get; set; }

        public List<SelectListItem> VitalSignList { get; set; }  //生理参数列表


        public List<SelectListItem> AssessmentTypeList()    //选择评估类型
        {
            return CommonVariables.GetAssessmentTypeList();
        }
        public string AssessmentTypeSelected { get; set; }  //选中的评估类型
        public List<SelectListItem> AssessmentNameList()   //选择评估名称
        {
            return CommonVariables.GetAssessmentNameList();
        }
        public string AssessmentNameSelected { get; set; }

        //ParametersArea
        public string AssessmentResult { get; set; }
        List<TreatmentIndicators> AssessmentResultList { get; set; }
        public HealthParametersViewModel()
        {

            AssessmentResultList = new List<TreatmentIndicators>();
        }

    }

    //患者信息-症状管理界面 WF 2014-12-08
    public class SymptomsViewModel
    {
        public string UserId { get; set; }		  //用户Id
        public string PId { get; set; }		  //PId
        public string VisitId { get; set; }		  //用户VisitId
        public List<SymptomInfo> SymptomsList { get; set; } //字段：症状类型、症状名称、症状描述、添加时间 
        public List<SelectListItem> SymptomsTypeList()
        {
            return CommonVariables.GetSymptomsTypeList();	  //症状类型下拉框
        }
        public List<SelectListItem> SymptomsNameList(string Type)
        {
            return CommonVariables.GetSymptomsNameList(Type); 		  //症状名称下拉框
        }
        public string SymptomsTypeSelected { get; set; }	//选中的症状类型
        public string SymptomsNameSelected { get; set; }	//选中的症状名称
        public string Description { get; set; }		        //症状描述
        public DateTime? RecordTime { get; set; }		        //症状描述

        public SymptomsViewModel()
        {
            SymptomsList = new List<SymptomInfo>();
        }

    }

    //患者信息-治疗方案界面 WF 2014-12-08
    public class TreatmentViewModel
    {
        public string UserId { get; set; }		  //用户Id
        public string PId { get; set; }		  //PId
        public List<TreatmentInfo> TreatmentList { get; set; } //字段:治疗目标、特殊人群、治疗方案、添加时间 
        public List<SelectListItem> TreatmentGoalList()
        {
            return CommonVariables.GetTreatmentGoalList();      //治疗目标下拉框
        }
        public List<SelectListItem> TreatmentAction()
        {
            return CommonVariables.GetTreatmentAction();      //治疗措施下拉框
        }
        public List<SelectListItem> SpecialGroupList()
        {
            return CommonVariables.GetSpecialGroupList();      //特殊人群下拉框
        }
        public List<SelectListItem> DurationNameList()
        {
            return CommonVariables.GetDurationNameList();      //特殊人群下拉框
        }
        public TreatmentInfo TreatmentInfo { get; set; }	//一个治疗方案的录入信息（包含TreatmentGoalSelected、ActionSelected、SpecialGroupSelected、TreatmentPlan）
        public TreatmentViewModel()
        {
            TreatmentList = new List<TreatmentInfo>();
        }
    }

    //患者信息-警报记录界面 WF 2014-12-08
    public class PatientAlertViewModel
    {
        public string UserId { get; set; }		           //用户Id
        public string DoctorId { get; set; }
        public string AlertStatusSelected { get; set; }  //选中的警报处置状态

        public List<SelectListItem> AlertStatusList()
        {
            return CommonVariables.GetAlertStatusList();
        }
        public List<AlertInfo> AlertList { get; set; }	   //字段：警报类型、警报项目、警报时间根据处置状态和警报时间倒序排列，有操作列，可直接给患者发信，发信后推送状态置位
        public PatientAlertViewModel()
        {
            AlertList = new List<AlertInfo>();
        }
    }

    //患者信息-健康参数界面 LS 2014-12-09
    public class EverydayTaskViewModel
    {
        public string PatientId { get; set; }

        public List<Reminder> TodayTask { get; set; }

        [DisplayName("内容")] 
        public string Content { get; set; }
        public string ReminderNo { get; set; }
        public List<SelectListItem> ReminderTypeList()    //提醒类型
        {
            return CommonVariables.GetReminderTypeList();
        }
        public string ReminderTypeSelected { get; set; }

        public List<SelectListItem> AlertModeList()       //提醒方式
        {
            return CommonVariables.GetAlertModeList();
        }
        public string AlertModeSelected { get; set; }

        public string StartDateTime { get; set; }
        public string CreatedBy { get; set; }

        public int EveryDayNumber { get; set; }
        public int WeeklyNumber { get; set; }
        public int MonthlyNumber { get; set; }
        public int AnnualNumber { get; set; }

        public DateTime OnceDateTime { get; set; }

        public List<string> EveryDay { get; set; }
        
        public List<SelectListItem> WeeklyList()     //每周：周  
        {
            return CommonVariables.GetWeeklyList();
        }
        public List<Weekly> WeeklySelected { get; set; }

        public List<SelectListItem> MonthlyList()   //每月：日期
        {
            return CommonVariables.GetMonthlyList();
        }
        public List<Monthly> MonthlySelected { get; set; }

        public List<SelectListItem> AnnualMothList()   //每年：月份
        {
            return CommonVariables.GetAnnualMothList();
        }
        public List<SelectListItem> AnnualDayList()   //每年：日
        {
            return CommonVariables.GetAnnualDayList();
        }
        public List<Annual> AnnualSelected { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]\d*|0$", ErrorMessage = "请输入非负整数")]
        public int FreqYear { get; set; }      //间隔：年
        [RegularExpression(@"^[1-9]\d*|0$", ErrorMessage = "请输入非负整数")]
        public int FreqMonth { get; set; }
        [RegularExpression(@"^[1-9]\d*|0$", ErrorMessage = "请输入非负整数")]
        public int FreqDay { get; set; }
        [RegularExpression(@"^[1-9]\d*|0$", ErrorMessage = "请输入非负整数")]
        public int FreqHour { get; set; }
        [RegularExpression(@"^[1-9]\d*|0$", ErrorMessage = "请输入非负整数")]
        public int FreqMunite { get; set; }   //间隔：分

        public EverydayTaskViewModel()
        {
            TodayTask = new List<Reminder>();
            EveryDay = new List<string>();
            WeeklySelected = new List<Weekly>();
            MonthlySelected = new List<Monthly>();
            AnnualSelected = new List<Annual>();
            OnceDateTime = DateTime.Now;
        }
    }

    public class TaskListViewModel
    {
        public string PatientId { get; set; }
        public int Type { get; set; }
        public List<TaskList> UndoneList { get; set; }
        public List<TaskList> ToDoList { get; set; }
        public TaskListViewModel()
        {
            UndoneList = new List<TaskList>();
            ToDoList = new List<TaskList>();
        }
    }

    //患者信息-治疗方案界面 WF 2014-12-08
    public class TreatmentListViewModel
    {
        public string UserId { get; set; }		  //用户Id
        public List<TreatmentInfo> TreatmentList { get; set; }		  //字段:治疗目标、特殊人群、治疗方案、添加时间 
        public TreatmentListViewModel()
        {
            TreatmentList = new List<TreatmentInfo>();
        }

    }

    //建立档案-就诊 CSQ 2014-12-17
    public class ClinicalInfoProfileViewModel
    {
        public string UserId { get; set; }
        public PatientBasicInfo PatientBasicInfo { get; set; }
        public ClinicalInfo ClinicalInfo { get; set; }
        public List<SelectListItem> HospitalList()
        {
            return CommonVariables.GetHospitalList();
        }

        public List<SelectListItem> VisitTypeList()
        {
            return CommonVariables.GetVisitTypeList();
        }
        public List<SelectListItem> DeptList()
        {
            return CommonVariables.GetDeptList();
        }
        public string HospitalSelected { get; set; }
        public string VisitTypeSelected { get; set; }
        
        public string DepartmentNameSelected { get; set; }

        //入院时间
        public string AdmissionDate { get; set; }
        //出院时间
        public string DischargeDate { get; set; }
        //就诊信息列表 
        public List<ClinicalInfo> InPatientList { get; set; }
        public List<ClinicalInfo> OutPatientList { get; set; }
        public List<SelectListItem> ClinicalInfoList { get; set; }

        public ClinicalInfoProfileViewModel()
        {
            InPatientList = new List<ClinicalInfo>();
            OutPatientList = new List<ClinicalInfo>();
            ClinicalInfoList = new List<SelectListItem>();
            //PatientBasicInfo = new PatientBasicInfo();
        }
    }

    //建立档案-症状 CSQ 2014-12-17
    public class SymptomsProfileViewModel
    {
        public string UserId { get; set; }		  //用户Id
        public string VisitId { get; set; }
        public int MaxSortNo { get; set; }

        public PatientBasicInfo PatientBasicInfo { get; set; }
        public SymptomInfo Symptoms { get; set; } //一条症状信息
        public List<SymptomInfo> SymptomsList { get; set; } //字段：症状类型、症状名称、症状描述、添加时间 
        public List<SelectListItem> SymptomsTypeList()
        {
            return CommonVariables.GetSymptomsTypeList();	  //症状类型下拉框
        }
        //public List<SelectListItem> SymptomsNameList()
        //{
        //    return CommonVariables.GetSymptomsNameList(); 		  //症状名称下拉框
        //}
        public string SymptomsTypeSelected { get; set; }	//选中的症状类型
        public string SymptomsNameSelected { get; set; }	//选中的症状名称
        public string Description { get; set; }		        //症状描述

        public SymptomsProfileViewModel()
        {
            SymptomsList = new List<SymptomInfo>();
        }
    }

    //建立档案-诊断 CSQ 2014-12-17
    public class DiagnosisInfoProfileViewModel
    {

        public string UserId { get; set; }
        public string VisitId { get; set; }
        public int MaxSortNo { get; set; }

        public PatientBasicInfo PatientBasicInfo { get; set; }
        public DiagnosisInfo DiagnosisInfo { get; set; }
        public List<SelectListItem> DiagnosisTypeList()
        {
            return CommonVariables.GetDiagnosisTypeList();
        }
        public List<SelectListItem> TypeList()
        {
            return CommonVariables.GetTypeList();
        }

        //public List<SelectListItem> DiagnosisNameList()
        //{
        //    return CommonVariables.GetDiagnosisNameList();
        //}

        public string DiagnosisTypeSelected { get; set; }
        public string TypeSelected { get; set; }
        public string DiagnosisNameSelected { get; set; }
        public string DiagnosisDate { get; set; }
        //已有诊断列表 
        public List<DiagnosisInfo> DiagnosisList { get; set; }

        public DiagnosisInfoProfileViewModel()
        {
            DiagnosisList = new List<DiagnosisInfo>();

        }
    }

    //建立档案-检查 CSQ 2014-12-17
    public class ExaminationProfileViewModel
    {
        public string UserId { get; set; }
        public string VisitId { get; set; }
        public int MaxSortNo { get; set; }

        public PatientBasicInfo PatientBasicInfo { get; set; }
        //已同步检查列表 
        public List<ExaminationInfo> ExaminationList { get; set; }
        //可供同步的检查列表 
        public List<ExaminationInfo> ExaminationSelection { get; set; }
        public List<SelectListItem> ExamTypeList()
        {
            return CommonVariables.GetExamTypeList();
        }
        public List<SelectListItem> DeptList()
        {
            return CommonVariables.GetDeptList();
        }
        public string ExamDate { get; set; }
        //检查信息参数项目
        public List<SelectListItem> ExamSubItemList { get; set; }
        public List<SelectListItem> Unit()
        {
            return CommonVariables.GetUnit();
        }
        public List<SelectListItem> LabResultUnit()
        {
            return CommonVariables.GetLabResultUnit();
        }
        public List<SelectListItem> StatusList()
        {
            return CommonVariables.GetStatus();
        }

        public List<SelectListItem> IsAbnormal()
        {
            return CommonVariables.GetIsAbnormal();
        }

        //一次检查信息
        public ExaminationInfo ExamInfo { get; set; }
        public List<SelectListItem> HospitalList()
        {
            return CommonVariables.GetHospitalList();
        }
        public string HospitalSelected { get; set; }

        

        public ExaminationProfileViewModel()
        {
            ExaminationList = new List<ExaminationInfo>();
        }
    }

    //建立档案-化验 CSQ 2014-12-17
    public class LabTestProfileViewModel
    {
        public string UserId { get; set; }
        public string VisitId { get; set; }
        public string MaxSortNo { get; set; }

        public PatientBasicInfo PatientBasicInfo { get; set; }
        //已同步化验列表 
        public List<LabTestInfo> LabTestList { get; set; }
        //可供同步的化验列表 
        public List<LabTestInfo> LabTestSelection { get; set; }
        public List<SelectListItem> Unit()
        {
            return CommonVariables.GetUnit();
        }
        public List<SelectListItem> DeptList()
        {
            return CommonVariables.GetDeptList();
        }
        public List<SelectListItem> IsAbnormal()
        {
            return CommonVariables.GetIsAbnormal2();
        }
        public List<SelectListItem> LabItemTypeNameList()
        {
            return CommonVariables.GetLabItemTypeNameList();
        }
        //化验信息参数项目
        public List<SelectListItem> LabSubItemList { get; set; }

        //一次化验信息
        public LabTestInfo LabTestInfo { get; set; }
        public List<SelectListItem> HospitalList()
        {
            return CommonVariables.GetHospitalList();
        }
        public string HospitalSelected { get; set; }
        
        public List<SelectListItem> StatusList()
        {
            return CommonVariables.GetStatus();
        }
        public List<SelectListItem> LabResultUnit()
        {
            return CommonVariables.GetLabResultUnit();
        }
        public LabTestProfileViewModel()
        {
            LabTestList = new List<LabTestInfo>();
        }
    }

    //建立档案-药物 CSQ 2014-12-17
    public class DrugInfoProfileViewModel
    {
        public string UserId { get; set; }
        public string VisitId { get; set; }
        public int MaxSortNo { get; set; }

        public PatientBasicInfo PatientBasicInfo { get; set; }
        //已同步药物治疗列表 
        public List<DrugInfo> DrugRecordList { get; set; }
        //可供同步的药物治疗列表 
        public List<DrugInfo> DrugRecordSelection { get; set; }
        public List<SelectListItem> Unit()
        {
            return CommonVariables.GetUnit();
        }

        public List<SelectListItem> IsAbnormal()
        {
            return CommonVariables.GetIsAbnormal();
        }

        public List<SelectListItem> DrugTypeList()
        {
            return CommonVariables.GetDrugTypeList();
        }

        public List<SelectListItem> RouteList()
        {
            return CommonVariables.GetRouteList();
        }

        public List<SelectListItem> FreqUnit()
        {
            return CommonVariables.GetFreqUnit();
        }

        public List<SelectListItem> FreqUnit2()
        {
            return CommonVariables.GetFreqUnit2();
        }
        public List<SelectListItem> DeptList()
        {
            return CommonVariables.GetDeptList();
        }

        public List<SelectListItem> DrugNameList()
        {
            return CommonVariables.GetDrugNameList();
        }
        
        //一条药物记录
        public DrugInfo DrugInfo { get; set; }

        public DrugInfoProfileViewModel()
        {
            DrugRecordList = new List<DrugInfo>();
            DrugInfo = new DrugInfo();
        }

    }

    //医生首页-警报信息 ZAM 2015-01-04
    public class AlertListViewModel
    {
        public string PatientId { get; set; }                   //检索患者ID
        public string PatientName { get; set; }                 //检索患者姓名
        public string DoctorId { get; set; }

        public string AlertStatusSelected { get; set; }  //选中的警报处置状态

        public List<SelectListItem> AlertStatusList()
        {
            return CommonVariables.GetAlertStatusList();
        }

        public List<AlertInfo> AlertRecordList { get; set; }    //警报列表
        public AlertListViewModel(string UserId)
        {
            AlertRecordList = new List<AlertInfo>();
            DoctorId = UserId;
        }

        public AlertListViewModel()
        {
            AlertRecordList = new List<AlertInfo>();
        }

    }

    //患者信息-健康计划界面   新增
    public class HealthPlanViewModel
    {

        public string UserId { get; set; }

    }
}