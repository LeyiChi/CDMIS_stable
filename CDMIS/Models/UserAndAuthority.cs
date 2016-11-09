using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMIS.Models
{
    public class UserAndAuthority
    {
    }

    //用户信息 ZYF 2014-12-08
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Class { get; set; }
        public string ClassName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PatientClass { get; set; }
        public string PhoneNo { get; set; }
    }

    //角色信息 ZYF 2014-12-08
    public class Role
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int SortNo { get; set; }
        public string Redundance { get; set; }
        public int InvalidFlag { get; set; }
    }

    //权限分类 ZYF 2014-12-08
    public class Authority
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int SortNo { get; set; }
        public string Redundance { get; set; }
        public int InvalidFlag { get; set; }

    }

    //详细权限 ZYF 2014-12-08
    public class AuthorityDetail
    {
        public string AuthorityCode { get; set; }
        public string AuthorityName { get; set; }
        public string SubAuthorityCode { get; set; }
        public string SubAuthorityName { get; set; }
        public int SortNo { get; set; }
        public string Redundance { get; set; }
        public int InvalidFlag { get; set; }
    }

    //角色权限对应 ZYF 2014-12-08
    public class Role2Authority
    {
        public string RoleCode { get; set; }
        public string AuthorityCode { get; set; }
        public string SubAuthorityCode { get; set; }
        public string Redundance { get; set; }
        public int InvalidFlag { get; set; }
    }

    //医生信息 ZYF 2014-12-08
    public class Doctor
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public int InvalidFlag { get; set; }
        public string IDNo { get; set; }
        public string ModuleNameString { get; set; }
    }

    public class ModuleAndDoctor
    {
        public string Module { get; set; }
        public string ModuleName { get; set; }
        public string ItemSeq { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string ModalName { get; set; }
        public string DataTableName { get; set; }
        public List<DoctorAndHCInfo> DoctorList { get; set; }
    }

    public class HealthCoach
    {
        public string ItemSeq { get; set; }
        public string HealthCoachId { get; set; }
        public string HealthCoachName { get; set; }
        public string HCDivName { get; set; }
        public string DataTableName { get; set; }
        public List<DoctorAndHCInfo> HealthCoachList { get; set; }
    }

    public class DoctorAndHCInfo
    {
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Hospital { get; set; }
        public string Dept { get; set; }
    }

    //public class DoctorAndHC
    //{
    //    public string Module { get; set; }
    //    public List<DoctorAndHCInfo> InfoList { get; set; }
    //    public DoctorAndHC() {
    //        InfoList = new List<DoctorAndHCInfo>();
    //    }
    //}
}