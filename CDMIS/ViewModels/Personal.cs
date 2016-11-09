using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDMIS.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CDMIS.ViewModels
{
    public class Personal
    {
    }

    //母版 ZC 2014-12-08
    public class UserOverviewViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }  //新信息条数
        public string UndoneCount { get; set; } //未完成任务数
        public string ServerIP { get; set; } //服务器IP
        public string InvalidFlag { get; set; } //权限
        public List<ToDoList> TodoList { get; set; }
        public UserOverviewViewModel()
        {
            TodoList = new List<ToDoList>();
        }
    }

    //个人主页 TDY-20141209
    public class PersonalHomepageViewModel
    {
        public PatientBasicInfo Patient { get; set; }
        public string Role { get; set; }
        public List<SelectListItem> GenderList()
        {
            return CommonVariables.GetGenderList();
        }
        [RegularExpression(@"(^\d{4}-\d{2}-\d{2}$)", ErrorMessage = "请填写正确格式的出生日期")]
        public string Birthday { get; set; }                  //出生日期

        [RegularExpression(@"(^\d{18}$)|(^\d{15}$)|(^\d{17}X$)", ErrorMessage = "请填写正确格式的身份证号")]
        public string IDNO { get; set; }                        //身份证号码

        public string PhoneNumber { get; set; }                 //手机号码
        public string Address { get; set; }                     //家庭住址
        public string Occupation { get; set; }                  //职业
        public string Nationality { get; set; }                 //国籍
        public string EmergencyContact { get; set; }            //紧急联系人
        public string EmergencyContactPhoneNumber { get; set; } //紧急联系人手机号码
        public string PhotoAddress { get; set; }                //头像存放地址
        public string UnitName { get; set; }                    //医生单位
        public string JobTitle { get; set; }                    //医生职称
        public string Level { get; set; }                       //医生级别
        public string Dept { get; set; }                        //医生科室
        public List<SelectListItem> GetHospitalList()                    //GetHospitalList
        {
            return CommonVariables.GetHospitalList();
        }
        public List<SelectListItem> GetDeptList()                    //GetDeptList
        {
            return CommonVariables.GetDeptList();
        }
        public List<SelectListItem> GetJobTitleList()                    //JobTitle
        {
            return CommonVariables.GetJobTitleList();
        }
        public List<SelectListItem> GetLevelList()                    //Level
        {
            return CommonVariables.GetLevelList();
        }
    }

    //信箱母版 GL 2014-12-09
    public class MailboxViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int UnreadNum { get; set; } //收件箱未读条数
        public int UnsentNum { get; set; } //草稿箱未发送条数

    }

    //联系人列表 GL 2014-12-09
    public class ContractsListViewModel
    {
        public string UserId { get; set; } //用户ID
        public List<ContractsInfo> ContractsList { get; set; } //联系人列表

        public ContractsListViewModel()
        {
            ContractsList = new List<ContractsInfo>();
        }
    }

    //收信\已发送\草稿箱列表 GL 2014-12-09
    public class MessageListViewModel
    {
        public string UserId { get; set; } //用户ID
        public string UserName { get; set; } //用户姓名

        public string MessageNo { get; set; }
        public int ReadStatus { get; set; }
        public string SendBy { get; set; }
        [StringLength(512, ErrorMessage = "消息主题不能超过512字")]
        public string Title { get; set; }
        [Required(ErrorMessage = "消息内容不能为空")]
        [StringLength(1024, ErrorMessage = "消息内容不能超过1024字")]
        public string Content { get; set; }
        [Required(ErrorMessage = "收件人不能为空")]
        public string Receiver { get; set; }
        public int Selection { get; set; }
        public string ServerIP { get; set; } //用于消息推送获取服务器地址

        public bool Flag { get; set; } //保存标志位

        public List<MessageInfo> MessageList { get; set; }//收信\已发送\草稿箱列表    
        public List<List<ContractsInfo>> ContactsList { get; set; }//联系人列表
        public List<ModuleInfo> ModuleInfoList { get; set; } //模块信息
        public MessageListViewModel()
        {
            ContactsList = new List<List<ContractsInfo>>();
            ModuleInfoList = new List<ModuleInfo>();
            MessageList = new List<MessageInfo>();

        }
    }
}