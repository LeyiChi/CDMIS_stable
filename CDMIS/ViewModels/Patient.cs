using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CDMIS.ViewModels
{
    public class Patient
    {
    }
    //患者信息-基本信息 LS 2014-12-14
    public class PatientBasicInfoViewModel
    {
        public PatientBasicInfo PatientBasicInfo { get; set; }

        public PatientBasicInfoViewModel()
        {
            PatientBasicInfo = new PatientBasicInfo();
        }
    }

    //建立档案-基本信息 ZC 2014-12-08
    public class BasicProfileViewModel
    {

        public List<SelectListItem> GenderList()
        {
            return CommonVariables.GetGenderList();
        }

        public List<SelectListItem> BloodTypeList()
        {
            return CommonVariables.GetBloodTypeList();
        }

        public List<SelectListItem> InsuranceTypeList()
        {
            return CommonVariables.GetInsuranceTypeList();
        }

        public List<CheckBox> BasicModuleList()
        {
            return CommonVariables.GetBasicModuleList();
        }

        public List<SelectListItem> ModuleList()
        {
            return CommonVariables.GetModuleList();
        }

        public List<SelectListItem> HospitalList()
        {
            return CommonVariables.GetHospitalList();
        }

        public string[] DoctorIdSelected { get; set; }      //CSQ 20141213  选中的医生
        public List<SelectListItem> DoctorList { get; set; }
        

        public PatientBasicInfo Patient { get; set; }          //CSQ 20141215
        public List<List<PatientDetailInfo>> PatientDetailInfo { get; set; }          //CSQ 20141215
        public ArrayList moduleUnBoughtCode = new ArrayList();
        public ArrayList moduleUnBoughtName = new ArrayList();
        public List<ModuleInfo> ModuleBoughtInfo = new List<ModuleInfo>();
        public List<ModuleInfo> ModuleUnBoughtInfo = new List<ModuleInfo>();
        public List<PatientDetailInfo> ModuleDetailList = new List<PatientDetailInfo>();
        public bool InvalidFlag { get; set; }

         [RegularExpression(@"(^((\+?86)|(\(\+86\)))?(13[012356789][0-9]{8}|15[012356789][0-9]{8}|18[02356789][0-9]{8}|147[0-9]{8}|1349[0-9]{7})$)|(^([0-9]{3,4}-)?[0-9]{7,8}$)", ErrorMessage = "联系方式输入不正确")]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }              //职业
        public string Nationality { get; set; }             //国籍
        public string EmergencyContact { get; set; }        //紧急联系人

         [RegularExpression(@"(^((\+?86)|(\(\+86\)))?(13[012356789][0-9]{8}|15[012356789][0-9]{8}|18[02356789][0-9]{8}|147[0-9]{8}|1349[0-9]{7})$)|(^([0-9]{3,4}-)?[0-9]{7,8}$)", ErrorMessage = "联系方式输入不正确")]
        public string EmergencyContactNumber { get; set; }  //紧急联系电话

        public List<List<InfoItem>> InfoItemList { get; set; }    //模块信息、模块关注详细信息  从字典表获取   //20141215修改 
        //public List<List<PatientDetailInfo>> InfoItemListBought { get; set; }   //模块信息、模块关注详细信息  从病人详细信息表获取

        public List<List<string>> InfoItemSelected { get; set; }      //CSQ 20141213  选中的模块详细信息   //待定

        public List<SelectListItem> YesNoTypeList()        //CSQ 20141215  
        {
            return CommonVariables.GetYesNoTypeList();
        }


        //public List<SelectListItem> InfoItemSelectList()        //CSQ 20141213  供选择的模块详细信息
        //{ 
        //    return CommonVariables.GetInfoItemSelectList();
        //}     

        public List<SelectListItem> VitalSignsTypeNameList()
        {
            return CommonVariables.GetVitalSignsTypeNameList();
        }
        public List<string> VitalSignsTypeNameSelected { get; set; }         //需要关注的生命体征
        public List<string> VitalSignsNameSelected { get; set; }         //需要关注的生命体征
        public List<string> VitalSignsFocused { get; set; }
        //public List<VitalSignInfo> VitalSignsFocused { get; set; }
        public List<PatientBasicInfo> PatientList { get; set; }

        public BasicProfileViewModel()
        {
            InfoItemList = new List<List<InfoItem>>();
            InfoItemSelected = new List<List<string>>();
            PatientDetailInfo = new List<List<PatientDetailInfo>>();
            Patient = new PatientBasicInfo();
            VitalSignsFocused = new List<string>();
            DoctorList = new List<SelectListItem>();
            PatientList = new List<PatientBasicInfo>();

        }
    }

    //患者信息-个人信息 LS 2014-12-09
    public class PatientDetailInfoViewModel
    {
        public string UserId { get; set; }
        public List<List<PatientDetailInfo>> PatientDetailInfo { get; set; }  //模块信息、模块关注详细信息
        public List<ModuleInfo> ModuleBoughtInfo = new List<ModuleInfo>();
        public List<PatientDetailInfo> ModuleDetailList = new List<PatientDetailInfo>();
        public PatientDetailInfoViewModel()
        {
            PatientDetailInfo = new List<List<PatientDetailInfo>>();
        }
    }

    //医生首页-患者列表界面 ZAM 2015-01-04
    public class PatientListViewModel
    {

        public PatientListViewModel(string UserId)
        {
            PatientList = new List<PatientBasicInfo>();
            DoctorId = UserId;
            List<SelectListItem> moduleList = this.ModuleList();
            if (moduleList.Count > 1)
            {
                this.ModuleSelected = moduleList[1].Value;
            }
            if (moduleList.Count == 1)
            {
                this.ModuleSelected = moduleList[0].Value;
            }
            else
            {
                this.ModuleSelected = "";
            }
        }

        public PatientListViewModel()
        {
            PatientList = new List<PatientBasicInfo>();
        }
        public List<SelectListItem> GenderList()
        {
            return CommonVariables.GetGenderList();
        }

        public List<SelectListItem> CareLevelList()
        {
            return CommonVariables.GetCareLevelList();
        }

        public List<SelectListItem> ModuleList()
        {
            return CommonVariables.GetModuleListByDoctorId(this.DoctorId);
            //return CommonVariables.GetModuleList();
        }

        public List<SelectListItem> AlertStatusList()
        {
            return CommonVariables.GetAlertStatusList();
        }

        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string GenderSelected { get; set; }              //选中的性别
        public string CareLevelSelected { get; set; }             //选中的关注等级
        public string ModuleSelected { get; set; }        //选中的管理模块
        public string AlertStatusSelected { get; set; }  //选中的警报处置状态
        public string AdvancedSearchEnable { get; set; }   //高级搜索状态标志
        public string DoctorId { get; set; }
        public List<PatientBasicInfo> PatientList { get; set; }    //模块信息、模块关注详细信息
    }
}