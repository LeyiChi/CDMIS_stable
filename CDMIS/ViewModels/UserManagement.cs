using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;

namespace CDMIS.ViewModels
{
    public class UserManagement
    {
    }
    //用户管理-用户管理  ZYF 2014-12-08
    public class UserViewModel
    {
        public List<User> UserList { get; set; }                       //用户列表
        public User Patient { get; set; }

        public string Role { get; set; }                                //检索用户角色

        public string SearchUserId { get; set; }                       //检索用户Id

        public string SearchUserName { get; set; }                     //检索用户用户名

        public string SearchPhoneNo { get; set; }                     //检索用户手机号

        public string UnitName { get; set; }
        public string JobTitle { get; set; }
        public string Dept { get; set; }

        public List<String> models { get; set; }

        public IEnumerable<SelectListItem> ModelsTypeList { get; set; }


        public List<SelectListItem> UserClassList()                    //用户类别下拉框
        {
            return CommonVariables.GetUserClassList();
        }
        public List<SelectListItem> RoleClassList()                    //用户类别下拉框
        {
            return CommonVariables.GetRoleClassList();
        }
         
        public List<SelectListItem> GetHospitalList()                    //GetHospitalList
        {
            return CommonVariables.GetHospitalList();
        }
        public List<SelectListItem> GetDeptList()                    //GetDeptList
        {
            return CommonVariables.GetDeptList();
        }
        public List<SelectListItem> JobTitleList()                    //JobTitle
        {
            return CommonVariables.GetJobTitleList();
        }
        public int RowCount { get; set; }
        public UserViewModel()
        {
            RowCount = 0;
            UserList = new List<User>();
            ModelsTypeList = CommonVariables.GetAllModuleList();
        }
    }

    //用户管理-权限分配  ZYF 2014-12-08
    public class Role2AuthorityViewModel
    {
        [Display(Name = "角色编码：")]
        public string RoleCode { get; set; }
        [Display(Name = "角色名称：")]
        public List<SelectListItem> RoleNameList()                                  //角色名称下拉框
        {
            return CommonVariables.GetRoleNameList();
        }
        public string RoleNameSelected { get; set; }                                //设定的角色名称

        public List<Authority> AuthorityList { get; set; }                    //数据库中所有权限列表
        public List<AuthorityDetail> RoleAuthorityList { get; set; }                //该角色已有权限列表
        public List<AuthorityDetail> SubAuthorityListSelected { get; set; }         //选中需要新增的大类及子类列表
        public int AuthorityRowCount { get; set; }
        public int RoleAuthorityRowCount { get; set; }
        public int SubAuthorityRowCount { get; set; }
        public Role2AuthorityViewModel()
        {
            AuthorityRowCount = 0;
            RoleAuthorityRowCount = 0;
            SubAuthorityRowCount = 0;
            AuthorityList = new List<Authority>();
            RoleAuthorityList = new List<AuthorityDetail>();
            SubAuthorityListSelected = new List<AuthorityDetail>();
        }

    }

    //用户管理-医生管理  ZYF 2014-12-08
    public class DoctorViewModel
    {
        public List<Doctor> DoctorList { get; set; }                   //医生列表
        public Doctor DoctorInfo { get; set; }                         //医生信息

        [Display(Name = "用户ID：")]
        public string SearchDoctorId { get; set; }                     //检索医生Id

        [Display(Name = "姓名：")]
        public string SearchDoctorName { get; set; }                   //检索医生姓名

        public List<SelectListItem> ModuleList()                       //医生类别下拉框
        {
            return CommonVariables.GetModuleList();
        }

        [Display(Name = "负责模块：")]
        public List<string> DoctorModuleSelected { get; set; }         //下拉框选中的医生类别Code列表

        public List<SelectListItem> GenderList()                       //医生性别下拉框
        {
            return CommonVariables.GetGenderList();
        }

        public int RowCount { get; set; }
        public DoctorViewModel()
        {
            RowCount = 0;
            DoctorList = new List<Doctor>();
        }


    }


    //资质审核界面 GL 2015-05-27
    public class QualiCheckViewModel
    {
        public List<List<QualiCheck>> QualiCheckList { get; set; }  //列表
        public QualiCheckViewModel()
        {
            QualiCheckList = new List<List<QualiCheck>>();
        }
    }

    //患者模块管理界面 ZC 2015-06-02
    public class ModuleManagementViewModel
    {
        public string PatientId { get; set; }
        public List<ModuleAndDoctor> ModuleInfoList { get; set; }
        public List<HealthCoach> HealthCoachInfoList { get; set; }
        public List<DoctorAndHCInfo> HealthCoachList { get; set; }
        public ModuleManagementViewModel()
        {
            ModuleInfoList = new List<ModuleAndDoctor>();
            HealthCoachInfoList = new List<HealthCoach>();
            HealthCoachList = new List<DoctorAndHCInfo>();
        }
        
    }
}