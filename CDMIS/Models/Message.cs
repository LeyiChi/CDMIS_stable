using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
namespace CDMIS.Models
{
    public class Message
    {
    }

    //消息记录 GL 2014-12-09
    public class MessageInfo
    {
        public string UserId { get; set; }
        public string MessageNo { get; set; }
        public int MessageType { get; set; }
        public int SendStatus { get; set; }
        public int ReadStatus { get; set; }
        public string SendBy { get; set; }
        public string SendByName { get; set; }
        public string SendDateTime { get; set; }
        [StringLength(512, ErrorMessage = "消息主题不能超过512字")]
        public string Title { get; set; }
        [Required(ErrorMessage = "消息内容不能为空")]
        [StringLength(1024, ErrorMessage = "消息内容不能超过1024字")]
        public string Content { get; set; }
        [Required(ErrorMessage = "收件人不能为空")]
        public string Receiver { get; set; }
        public string ReceiverName { get; set; }
        public int SMSFlag { get; set; }
        public int OutDateFlag { get; set; } //身份过期标志
        public string ServerIP { get; set; } //服务器IP，用于消息推送

    }

    //联系人信息 GL 2014-12-09
    public class ContractsInfo
    {
        public string ContractId { get; set; }
        public string ContractName { get; set; }
    }

    //模块信息 GL 20150124
    public class ModuleInfo
    {
        public string ModuleName { get; set; }
        public string Category { get; set; }
    }
}