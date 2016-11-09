using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDMIS.Models
{
    //时间轴 同一天下：类型、事件
    public class SomeDayEvent
    {
        public string Type { get; set; }         //类型
        public string Event { get; set; }        //事件
        public string KeyCode { get; set; }         //关键主键（用于查看详细）
    }

    //就诊信息 CSQ 2014-12-08
    public class ClinicalInfo
    {
        public string UserId { get; set; }
        public string VisitId { get; set; }
        public int SortNo { get; set; }
        public string VisitIdSelected { get; set; }

        public DateTime ClinicDate { get; set; }

        public DateTime AdmissionDate { get; set; }
        public DateTime DischargeDate { get; set; }
        public string HospitalCode { get; set; }
        public string HospitalName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Doctor { get; set; }
        public string Creator { get; set; }
        public bool IsAllowed { get; set; }
    }

    //诊断信息 CSQ 2014-12-08
    public class DiagnosisInfo
    {
        public string UserId { get; set; }
        public string DiagnosisNo { get; set; }    //CSQ 20141219
        public string DiagnosisType { get; set; }
        public string DiagnosisTypeName { get; set; }
        public string Type { get; set; }
        public string TypeName { get; set; }
        public string DiagnosisCode { get; set; }
        public string DiagnosisName { get; set; }
        public string Description { get; set; }
        public string RecordDate { get; set; }
        

        public bool IsAllowed { get; set; }
        public string Creator { get; set; }
    }

    //检查信息 CSQ 2014-12-08
    public class ExaminationInfo
    {
        public string UserId { get; set; }
        public string SortNo { get; set; }
        public string ExamType { get; set; }
        public string ExamTypeName { get; set; }
        public string ExamDate { get; set; }
        public string ItemCode { get; set; }
        public string ExamName { get; set; }
        public string ExamPara { get; set; }
        public string Description { get; set; }
        public string Impression { get; set; }
        public string Recommendation { get; set; }
        public int IsAbnormalCode { get; set; }
        public string IsAbnormal { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string ReportDate { get; set; }
        public string ImageURL { get; set; }
        public string DeptCode { get; set; }

        public bool IsAllowed { get; set; }
        public string Creator { get; set; }
        //public List<SelectListItem> ExamSubItemList { get; set; }
        public DetailInfo detail { get; set; }
        public List<DetailInfo> Detail { get; set; }
        public ExaminationInfo()
        {
            detail = new DetailInfo();
            Detail = new List<DetailInfo>();
        }
    }

    //化验信息 CSQ 2014-12-08
    public class LabTestInfo
    {
        public string UserId { get; set; }
        public string SortNo { get; set; }
        //CSQ 2014/12/11
        public string LabItemType { get; set; }
        //CSQ 2014/12/11
        public string LabItemTypeName { get; set; }
        public string LabItemCode { get; set; }
        public string LabItemName { get; set; }
        public string ExamDate { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string ReportDate { get; set; }
        public string DeptCode { get; set; }

        public bool IsAllowed { get; set; }
        public string Creator { get; set; }
        public List<DetailInfo> Detail { get; set; }
        public DetailInfo detail { get; set; }
        public LabTestInfo()
        {
            Detail = new List<DetailInfo>();
            detail = new DetailInfo();
        }
    }

    //检查/化验详情 CSQ 2014-12-08
    public class DetailInfo
    {
        public string Code { get; set; }
        public string ItemName { get; set; }
        public string Value { get; set; }
        public int IsAbnormalCode { get; set; }
        public string IsAbnormal { get; set; }
        public string UnitCode { get; set; }
        public string Unit { get; set; }
        public bool IsAllowed { get; set; }
        public string Creator { get; set; }
    }

    //用药信息 CSQ 2014-12-08
    public class DrugInfo
    {
        public string UserId { get; set; }
        public int OrderNo { get; set; }
        public int OrderSubNo { get; set; }
        public int RepeatIndicatorCode { get; set; }
        public string RepeatIndicator { get; set; }
        public string OrderCode { get; set; }
        public string OrderClassCode { get; set; }
        public string OrderClass { get; set; }
        public string OrderContent { get; set; }
        public string Dosage { get; set; }
        public string DosageUnitsCode { get; set; }
        public string DosageUnits { get; set; }
        public string AdministrationCode { get; set; }
        public string Administration { get; set; }
        public string StartDateTime { get; set; }
        public string StopDateTime { get; set; }
        public int Duration { get; set; }
        public string DurationUnits { get; set; }
        public string Frequency { get; set; }
        public int FreqCounter { get; set; }
        public int FreqInteval { get; set; }
        public string FreqIntevalUnitCode { get; set; }
        public string FreqIntevalUnit { get; set; }
        public string DeptCode { get; set; }

        public bool IsAllowed { get; set; }
        public string Creator { get; set; }
        public DrugInfo()
        {
            FreqIntevalUnit = "4";
        }
    }

    //基础列表 LS 2014-12-09
    public class ClinicBasicInfo
    {
        public int VisitId { get; set; }           //就诊序号
        public int TypeOrSortNo { get; set; }     //类型或序号
        public string Time { get; set; }
        public string Hospital { get; set; }
        public string Department { get; set; }
        public string Event { get; set; }        //事件
        public string Result { get; set; }
        public string Detail { get; set; }
    }

    //体征记录 LS 2014-12-09
    public class VitalSignInfo
    {
        public string UserId { get; set; }
        public string RecordDate { get; set; }
        public string RecordTime { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public decimal ThreholdMin { get; set; }  //阈值下限
        public decimal ThreholdMax { get; set; }
    }

    //评估参数结构 LS 2014-12-09
    public class Parameters
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    //评估指标及结果 LS 2014-12-09
    public class TreatmentIndicators
    {
        public string UserId { get; set; }
        public int SortNo { get; set; }
        public string AssessmentType { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentTime { get; set; }
        public string Result { get; set; }
        public List<Parameters> Parameters { get; set; }
    }
 
    //症状信息 WF 2014-12-08
    public class SymptomInfo
    {
        public string UserId { get; set; }		  //用户Id
        public string VisitId { get; set; }		  //用户VisitId
        public int SymptomsNo { get; set; }		  //症状序号
        public string SymptomsType { get; set; }		  //症状类型编码
        public string SymptomsTypeName { get; set; }		  //症状类型
        public string SymptomsCode { get; set; }		  //症状名称编码
        public string SymptomsName { get; set; }		  //症状名称
        public string Description { get; set; }		  //症状描述
        public string RecordDate { get; set; }		  //症状记录日期
        public string RecordTime { get; set; }		  //症状记录时间
        public string ReInUserId { get; set; }       //RevisionInfo的UserId
        public bool IsAllowed { get; set; }
        public string Creator { get; set; }		  //症状记录时间
    }

    //治疗方案 WF 2014-12-08
    public class TreatmentInfo
    {
        public string UserId { get; set; }		  //用户Id
        //public int Type { get; set; }		  //记录类型
        public int SortNo { get; set; }		  //序号
        public string TreatmentGoal { get; set; }		  //治疗目标
        public string TreatmentAction { get; set; }		  //治疗措施
        public string Group { get; set; }		  //特殊人群
        public string TreatmentPlan { get; set; }		  //治疗方案
        public string Description { get; set; }		  //说明
        public string TreatTime { get; set; }		  //治疗时间
        public string Duration { get; set; }	//疗程
        public string ReInUserId { get; set; }	//RevisonInfo.UserId
        public bool IsAllowed { get; set; }
    }

    //警报记录 WF 2014-12-08
    public class AlertInfo
    {
        public string UserId { get; set; }		  //用户Id
        public string UserName { get; set; }
        public int SortNo { get; set; }		  //序号
        public string AlertItemCode { get; set; }		  //警戒项目编码
        public string AlertItemName { get; set; }		  //警戒项目名称
        public string AlertDateTime { get; set; }		  //警报时间
        public string AlertType { get; set; }		  //警报类型
        public string AlertTypeName { get; set; }		  //警报类型名称
        public int PushFlag { get; set; }		  //推送状态
        public int ProcessFlag { get; set; }		  //处置状态

    }

    //提醒记录 LS 2014-12-09
    public class Reminder
    {
        public string UserId { get; set; }
        public string ReminderType { get; set; }
        public string ReminderTypeName { get; set; }
        public string ReminderNo { get; set; }
        public string Content { get; set; }
        public string AlertMode { get; set; }
        public string AlertModeName { get; set; }
        public string StartDateTime { get; set; }
        public string NextDate { get; set; }   //下次提醒日期
        public string NextTime { get; set; }   //下次提醒时间
        public string Interval { get; set; }   //间隔时间
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public bool IsAllowed { get; set; }
    }

    //提醒记录 ZC 2015-01-21
    public class TaskList
    {
        public string PatientId { get; set; }
        public string ReminderNo { get; set; }
        public string TaskDate { get; set; }
        public string TaskTime { get; set; }
        public string TaskDateTime { get; set; }
        public int IsDone { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
    }

    //提醒记录 ZC 2015-01-21
    public class ToDoList
    {
        public string Num { get; set; }
        public string PatientId { get; set; }
        public string ReminderNo { get; set; }
        public double ReminderTime { get; set; }
        public string Content { get; set; }
    }

    //每周：周几及时间 LS 2014-12-09
    public class Weekly
    {
        public string Week { get; set; }
        public string Time { get; set; }
    }

    //每月：日期及时间 LS 2014-12-09
    public class Monthly
    {
        public string Day { get; set; }
        public string Time { get; set; }
    }

    //每年：月、日、时间 LS 2014-12-09
    public class Annual
    {
        public string Month { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
    }
}