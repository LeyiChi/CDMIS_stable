using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDMIS.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CDMIS.ViewModels
{
    public class Dictionary
    {
    }

    //字典维护界面 YDS 2014-12-09
    public class DictViewModel
    {
        public List<SelectListItem> DictList()
        {
            return CommonVariables.GetDictList();
        }
        public string DictSelected { get; set; }
    }

    //患者详细信息项目表 YDS 2014-12-09
    public class InfoItemViewModel
    {
        public List<InfoItem> InfoItemList { get; set; }  //列表

        public List<SelectListItem> ControlTypeList()
        {
            return CommonVariables.GetControlTypeList();
        }
        public string ControlTypeSelected { get; set; }

        public List<SelectListItem> OptionCategoryList()
        {
            return CommonVariables.GetOptionCategoryList();
        }
        public string OptionCategorySelected { get; set; }

        public List<SelectListItem> CategoryCodeList()
        {
            return CommonVariables.GetCategoryCodeList();
        }
        public string CategoryCodeSelected { get; set; }

        public InfoItemViewModel()
        {
            InfoItemList = new List<InfoItem>();
        }
    }

    //警戒字典表 YDS 2014-01-13
    public class BasicAlertViewModel
    {
        public List<BasicAlert> BasicAlertList { get; set; }  //列表

        public List<SelectListItem> UnitList()
        {
            return CommonVariables.GetAllUnit();
        }
        public string UnitSelected { get; set; }

        public BasicAlertViewModel()
        {
            BasicAlertList = new List<BasicAlert>();
        }
    }

    //未匹配科室字典表 WF 2015-07-07
    public class UnCompDivisionViewModel
    {
        public List<TmpDivisionDict> TmpDivision{ get; set; } 
        public List<SelectListItem> DivisionList()
        {
            return CommonVariables.GetDivisionList();
        }
        public string DivisionSelected { get; set; }

        public UnCompDivisionViewModel()
        {
            TmpDivision = new List<TmpDivisionDict>();
        }
    }

    //未匹配科室字典表 WF 2015-07-07
    public class UnCompDiagnosisViewModel
    {
        public List<TmpDivisionDict> TmpDiagnosis { get; set; }
        public List<SelectListItem> DiagnosisList()
        {
            return CommonVariables.GetDiagnosisList();
        }
        public string DiagnosisSelected { get; set; }

        public UnCompDiagnosisViewModel()
        {
            TmpDiagnosis = new List<TmpDivisionDict>();
        }
    }

    //未匹配手术字典表 lpf 2015-07-10
    public class UnCompOperationViewModel
    {
        public List<TmpOperationDict> TmpOperation { get; set; }
        public List<SelectListItem> OperationList()
        {
            return CommonVariables.GetOperationList();
        }
        public string OperationSelected { get; set; }

        public UnCompOperationViewModel()
        {
            TmpOperation = new List<TmpOperationDict>();
        }
    }

    //未匹配体征字典表 lpf 2015-07-14
    public class UnCompVitalSignsViewModel
    {
        public List<MpVitalSignsCmp> VitalSignsCmp { get; set; }
        public List<SelectListItem> VitalSignsList()
        {
            return CommonVariables.GetVitalSignsList();
        }
        public string VitalSignsSelected { get; set; }

        public UnCompVitalSignsViewModel()
        {
            VitalSignsCmp = new List<MpVitalSignsCmp>();
        }
    }

    #region "WY"
    //未匹配药物字典表 WY 2015-07-09
    public class UnCompDrugViewModel
    {
        public List<TmpDrugDict> TmpDrug { get; set; }
        public List<SelectListItem> DrugList()
        {
            return CommonVariables.GetDrugList();
        }
        public string DrugSelected { get; set; }

        public UnCompDrugViewModel()
        {
            TmpDrug = new List<TmpDrugDict>();
        }
    }

    //药物字典ViewModel
    public class DrugViewModel
    {
        public List<Drug> Drug { get; set; }
        public List<SelectListItem> UnitsList()
        {
            return CommonVariables.GetOrderingMaterialUnitList();
        }
        public string UnitsSelectd { get; set; }

        public DrugViewModel()
        {
            Drug = new List<Drug>();
        }
    }

    //未匹配检验项目字典表 WY 2015-07-09
    public class UnCompLabViewModel
    {
        public List<TmpLabItemDict> TmpLabItem { get; set; }
        public List<SelectListItem> LabItemList()
        {
            return CommonVariables.GetLabItemList();
        }
        public string LabItemSelected { get; set; }

        public UnCompLabViewModel()
        {
            TmpLabItem = new List<TmpLabItemDict>();
        }
    }

    //未匹配检验子项目字典表 WY 2015-07-13
    public class UnCompLabSubViewModel
    {
        public List<TmpLabSubItemDict> TmpLabSubItem { get; set; }
        public List<SelectListItem> LabSubItemList()
        {
            return CommonVariables.GetLabSubItemList();
        }
        public string LabSubItemSelected { get; set; }

        public UnCompLabSubViewModel()
        {
            TmpLabSubItem = new List<TmpLabSubItemDict>();
        }
    }

    //未匹配检查子项目字典表 WY 2015-07-13
    public class UnCompExamSubViewModel
    {
        public List<TmpExamSubItemDict> TmpExamSubItem { get; set; }
        public List<SelectListItem> ExamSubItemList()
        {
            return CommonVariables.GetExamSubItemList();
        }
        public string ExamSubItemSelected { get; set; }

        public UnCompExamSubViewModel()
        {
            TmpExamSubItem = new List<TmpExamSubItemDict>();
        }
    }

    //未匹配检验项目字典表 WY 2015-07-13
    public class UnCompExamViewModel
    {
        public List<TmpExamDict> TmpExamItem { get; set; }
        public List<SelectListItem> ExamItemList()
        {
            return CommonVariables.GetExaminationItemList();
        }
        public string ExamItemSelected { get; set; }

        public UnCompExamViewModel()
        {
            TmpExamItem = new List<TmpExamDict>();
        }
    }
    #endregion
}