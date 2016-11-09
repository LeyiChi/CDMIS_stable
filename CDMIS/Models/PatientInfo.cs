using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CDMIS.Models
{
    public class PatientInfo
    {
    }

    //患者基本信息 ZC 2014-12-08
    public class PatientBasicInfo
    {
        public string UserId { get; set; }
        public string VisitId { get; set; }

        // 就诊医院Id/就诊号
        public string HospitalId { get; set; }
        public string HospitalCode { get; set; }



        [Required(ErrorMessage = "姓名必填")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "性别必填")]
        public string Gender { get; set; }
        public string GenderText { get; set; }

        public int Age { get; set; }
        public string BloodType { get; set; }
        public string InsuranceType { get; set; }
        public string Diagnosis { get; set; }
        [Required(ErrorMessage = "出生日期必填")]
        public string Birthday { get; set; }                //患者出生年月   2014/12/26  CSQ
        [RegularExpression(@"(^\d{18}$)|(^\d{15}$)|(^\d{17}X$)", ErrorMessage = "身份证号码格式输入不正确")]
        public string IDNo { get; set; }                 //患者身份证号码   2014/12/11 CSQ

        public string Module { get; set; }
        public string singleModule { get; set; }    //单个模块
        public int AlertNumber { get; set; }
        public int CareLevel { get; set; }
        public List<SelectListItem> ClinicalInfoList { get; set; }
        public string LatestClinicalInfo { get; set; }
        public PatientBasicInfo()
        {

            ClinicalInfoList = new List<SelectListItem>();
        }
    }

    //患者详细信息 WF 2014-12-08
    public class PatientDetailInfo
    {
        public string PatientId { get; set; }		          //患者Id
        public string CategoryCode { get; set; }		      //患者详细信息大分类
        public string CategoryName { get; set; }	     	  //患者详细信息大分类名称
        public string ItemCode { get; set; }		          //患者详细信息项目编码
        public string ItemName { get; set; }		          //患者详细信息项目名称
        public int GroupHeaderFlag { get; set; }           //组标题标志
        public string ParentCode { get; set; }		          //父级项目
        public string ControlType { get; set; }		          //控件形式
        public string OptionCategory { get; set; }		      //选项类别
        public List<SelectListItem> OptionList { get; set; }  //选项下拉框
        public string OptionSelected { get; set; }		      //选中项
        public int ItemSeq { get; set; }		              //项目SEQ
        public string Value { get; set; }		              //值
        public string Content { get; set; }		              //值
        public string Description { get; set; }		          //说明
        public string EditDeleteFlag { get; set; }       //20150113 CSQ

    }

    //患者警戒值记录 LS 2014-12-09
    public class PersonalAlertInfo
    {
        public string UserId { get; set; }
        public string AlertItemCode { get; set; }
        public string AlertItemName { get; set; }
        public int SortNo { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string Units { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Remarks { get; set; }
    }
}