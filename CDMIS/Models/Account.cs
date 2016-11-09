using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CDMIS.Models
{
    public class Account 
    {
    }

    // GET: /Account/
    //登录 TDY-20141209
    public class LogOnModel
    {
        [Required(ErrorMessage = "请输入用户名或手机号")]
        public string UserId { get; set; }          //用户ID
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }        //用户名
        [Required(ErrorMessage = "请输入验证码")]
        public string ValidateCode { get; set; }    //登录时的验证码
        public bool RememberMe { get; set; }        //是否记住密码
        public string UserRole { get; set; }        //登录类型
    }

    //忘记密码-验证 TDY-20141209
    public class VerificationModel
    {
        public string UserId { get; set; }          //用户ID
        public string PhoneNumber { get; set; }     //手机号码
        public string ValidateCode { get; set; }    //发送给手机的验证码
    }

    //密码（忘记密码中复用） TDY-20141209
    public class PasswordModel
    {
        public string UserId { get; set; }          //用户ID
        public string OldPassword { get; set; }     //旧密码，在忘记密码页面中不显示旧密码输入
        public string NewPassword { get; set; }     //新密码
        [Compare("NewPassword", ErrorMessage = "与新密码不同，请重新输入")]
        public string ConfirmPassword { get; set; } //确认新密码
        public string ValidateCode { get; set; }    //修改密码时的验证码，忘记密码页面中不显示
    }

    public class UserAndRole
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string TerminalName { get; set; }
        public string TerminalIP { get; set; }
        public int DeviceType { get; set; }
    }

    //激活 TDY-20150512
    public class ActivitionModel
    {
        public string UserId { get; set; }
        public string InviteCode { get; set; }
    }

    public class QualiCheck //资质审核 GL 2015-05-27
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string ActivationCode { get; set; }
        public string PhoneNo { get; set; }
    }
}
