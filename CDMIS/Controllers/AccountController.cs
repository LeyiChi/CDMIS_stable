using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using CDMIS.ViewModels;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using CDMIS.ServiceReference;
using System.Net;
using System.Web.Security;
using System.Text.RegularExpressions;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
     [StatisticsTracker]
    public class AccountController : Controller
    {
        #region <" 私有变量 ">
        ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
        static string _validateCode1 = "";
        static string _validateCode2 = "";
        //static string PasswordFlag = "";
        static int ChangePasswordFlag = 0;
        static int ResetPasswordFlag = 0;
        #endregion 

        #region <"登录及注销">
        public ActionResult LogOn(string control, string page)
        {
            Session["CurrentUser"] = null;
            LogOnModel LogOnModel = new LogOnModel();
            ViewData["controller"] = control;
            ViewData["actionResult"] = page;
            return View(LogOnModel);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel LogOnModel, string control, string page)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var UserId = LogOnModel.UserId;
                    var Password = LogOnModel.Password;
                    var Type = "";
                    var EmailFlag = Regex.IsMatch(UserId, @"(^[-_A-Za-z0-9]+@([_A-Za-z0-9]+\.)+[A-Za-z0-9]{2,3}$)");
                    var PhoneFlag = Regex.IsMatch(UserId, @"(^1[3-8]\d{9}$)");
                    if (EmailFlag == true)
                    {
                        Type = "EmailAdd";
                    }
                    if (PhoneFlag == true)
                    {
                        Type = "PhoneNo";
                    }
                    if (Type != "")
                    {
                        UserId = _ServicesSoapClient.GetIDByInput(Type, UserId);
                    }
                    if (_ServicesSoapClient.CheckUserExist(UserId) == true)
                    {
                        if (_ServicesSoapClient.CheckPassword(UserId, Password) == 1)
                        {
                            var CurrentUser = new UserAndRole();
                            CurrentUser.UserId = UserId;
                            CurrentUser.UserName = _ServicesSoapClient.GetUserName(UserId);
                            //CurrentUser.Role = _ServicesSoapClient.GetClassByUserId(UserId);
                            var RoleList = _ServicesSoapClient.GetAllRoleMatch(UserId);
                            //var Role = RoleList.Tables[0].Rows[0]["RoleClass"];
                            var length = RoleList.Tables[0].Rows.Count;
                            string[] RoleClass = new string[length];
                            for (int i = 0; i < length; i++)
                            {
                                RoleClass[i] = RoleList.Tables[0].Rows[i]["RoleClass"].ToString();
                                CurrentUser.Role = RoleClass[i];
                            }
                            string hostAddress = Request.ServerVariables.Get("Remote_Addr").ToString();
                            if (hostAddress == "::1")
                            {
                                hostAddress = "127.0.0.1";
                            }
                            CurrentUser.TerminalIP = hostAddress;

                            //CurrentUser.TerminalName = Dns.GetHostName();
                            //CurrentUser.TerminalName = Request.ServerVariables.Get("Remote_Host").ToString();
                            string hostName = "";
                            try
                            {
                                System.Net.IPHostEntry host = new System.Net.IPHostEntry();
                                host = System.Net.Dns.GetHostEntry(hostAddress);
                                hostName = host.HostName;
                            }
                            catch
                            { 
                            }
                            finally 
                            {
                                if (hostName == "")
                                {
                                    hostName = Request.ServerVariables.Get("Remote_Host").ToString();
                                }
                            }
                            CurrentUser.TerminalName = hostName;
                            
                            CurrentUser.DeviceType = 1;

                            var ChangeLastLogOnTimeFlag = _ServicesSoapClient.UpdateLastLoginDateTime(CurrentUser.UserId, CurrentUser.UserName, CurrentUser.TerminalIP, CurrentUser.TerminalName, CurrentUser.DeviceType);
                            Session["CurrentUser"] = CurrentUser;
                            FormsAuthentication.SetAuthCookie(UserId, true);
                            if (control == null && page == null)
                            {
                                if (CurrentUser.Role == "Administrator" && LogOnModel.UserRole == "Administrator")
                                {
                                    return RedirectToAction("Index", "Management");
                                }
                                else
                                {
                                    var ActivitionFlag = _ServicesSoapClient.GetActivatedState(UserId, LogOnModel.UserRole);
                                    if (ActivitionFlag == "0")
                                    {
                                        CurrentUser.Role = LogOnModel.UserRole;
                                        if (CurrentUser.Role == "Doctor")
                                        {
                                            return RedirectToAction("PatientList", "DoctorHome");
                                        }
                                        else
                                        {
                                            return RedirectToAction("HealthCoachPatientList", "DoctorHome");
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("errorConnection", "该用户没有权限登录本系统");
                                        return View();
                                    }
                                }
                                //switch (CurrentUser.Role)
                                //{
                                //    case "Administrator": return RedirectToAction("Index", "Dict"); 
                                //    case "Doctor": return RedirectToAction("PatientList", "DoctorHome"); 
                                //    //case "Patient": return RedirectToAction("HealthParameters", "PatientHome");
                                //    default: ModelState.AddModelError("", "该用户没有权限登录本系统");
                                //        return View();
                                //}
                            }
                            else
                            {
                                return RedirectToAction(page, control);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("errorPassword", "密码错误，请重新输入密码");
                            return View(LogOnModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("errorUserId", "用户不存在，请重新输入用户ID");
                        return View(LogOnModel);
                    }
                }
                else
                {
                    return View(LogOnModel);
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("errorConnection", "数据库连接失败");
                return View(LogOnModel);
            }

        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["CurrentUser"] = null;
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult GotoLogin(string control, string page)
        {
            ViewData["controller"] = control;
            ViewData["actionResult"] = page;
            return View();
        }
        #endregion

        #region <"忘记密码">
        public ActionResult Verification()
        {
            VerificationModel VerificationModel = new VerificationModel();
            return View();
        }

        [HttpPost]
        public ActionResult Verification(VerificationModel VerificationModel)
        {
            try
            {
                var piPhoneNumber = VerificationModel.PhoneNumber;
                if (piPhoneNumber == null)
                {
                    ModelState.AddModelError("errorPhoneNo", "手机号码为空,请输入手机号码");
                    return View();
                }
                else
                {
                    string UserId = _ServicesSoapClient.GetIDByInput("PhoneNo", piPhoneNumber);
                    return RedirectToAction("ResetPassword", "Account", new { UserId = UserId });
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("errorConnection", "数据库连接失败");
                return View();
            }

        }

        public ActionResult ResetPassword(string UserId)
        {
            PasswordModel ResetPasswordModel = new PasswordModel();
            ResetPasswordModel.UserId = UserId;
            return View(ResetPasswordModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordModel ResetPasswordModel)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                var UserId = user.UserId;
                var newPassword = ResetPasswordModel.NewPassword;
                var ConfirmPassword = ResetPasswordModel.ConfirmPassword;

                if (newPassword == null)
                {
                    ModelState.AddModelError("errorNewPassword", "新密码为空,请输入新密码");
                    return View();
                }
                else if (ConfirmPassword == null)
                {
                    ModelState.AddModelError("errorConfirmPassword", "请再次输入新密码");
                    return View();
                }
                else
                {
                    //var ResetPasswordFlag = _ServicesSoapClient.ResetPassword(UserId, "#*bme319*#", newPassword, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    if (ResetPasswordFlag == 1)
                    {
                        //PasswordFlag = "1";
                        return RedirectToAction("LogOn", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("errorConnection", "新密码重置失败，请重新输入");
                        return View();
                    }
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("errorConnection", "数据库连接失败");
                return View();
            }


        }

        public JsonResult CheckResetPassword(string newPassword2, string UserId)
        {
            var CurrentUser = new UserAndRole();
            CurrentUser.UserId = UserId;
            CurrentUser.UserName = _ServicesSoapClient.GetUserName(UserId);
            CurrentUser.Role = _ServicesSoapClient.GetClassByUserId(UserId);
            //CurrentUser.UserName = UserName;
            CurrentUser.TerminalName = Dns.GetHostName();
            string hostAddress = Request.ServerVariables.Get("Local_Addr").ToString();
            if (hostAddress == "::1")
            {
                hostAddress = "127.0.0.1";
            }
            CurrentUser.TerminalIP = hostAddress;
            CurrentUser.DeviceType = 1;
            Session["CurrentUser"] = CurrentUser;
            var res = new JsonResult();
            //
            if (newPassword2 != null)
            {
                ResetPasswordFlag = _ServicesSoapClient.ResetPassword(UserId, "#*bme319*#", newPassword2, CurrentUser.UserId, CurrentUser.TerminalName, CurrentUser.TerminalIP, CurrentUser.DeviceType);
                if (ResetPasswordFlag == 1)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region<"修改密码">
        public ActionResult ChangePassword()
        {
            PasswordModel ChangePasswordModel = new PasswordModel();
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordModel ChangePasswordModel)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                var UserId = user.UserId;
                var OldPassword = ChangePasswordModel.OldPassword;
                var newPassword = ChangePasswordModel.NewPassword;
                var ConfirmPassword = ChangePasswordModel.ConfirmPassword;

                if (OldPassword == null)
                {
                    ModelState.AddModelError("errorPassword", "旧密码为空，请输入旧密码");
                    return View();
                }
                else if (newPassword == null)
                {
                    ModelState.AddModelError("errorNewPassword", "新密码为空,请输入新密码");
                    return View();
                }
                //else if (OldPassword == newPassword)
                //{
                //    ModelState.AddModelError("errorNewPassword", "新密码与旧密码相同，请重新输入新密码");
                //    return View();
                //}
                else if (ConfirmPassword == null)
                {
                    ModelState.AddModelError("errorConfirmPassword", "请再次输入新密码");
                    return View();
                }
                else
                {

                    //var ChangePasswordFlag = _ServicesSoapClient.ChangePassword(UserId, OldPassword, newPassword, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    if (ChangePasswordFlag == 1)
                    {
                        //PasswordFlag = "1";
                        //return Content("<script >alert('提交留言成功，谢谢对我们支持，我们会根据您提供联系方式尽快与您取的联系！');</script >", "text/html");


                        //return View();
                        return RedirectToAction("PersonalHomepage", "Personal");
                    }
                    else if (ChangePasswordFlag == 3)
                    {
                        ModelState.AddModelError("errorPassword", "旧密码错误");
                        return View();
                    }
                    else if (ChangePasswordFlag == 4)
                    {
                        ModelState.AddModelError("errorPassword", "密码已过期");
                        return View();
                    }
                    //else if (newPassword != ConfirmPassword)
                    //{
                    //    ModelState.AddModelError("", "两次输入的新密码不同，请确认后重新输入");
                    //    return View();
                    //}
                    else
                    {
                        return View();
                    }
                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("errorConnection", "数据库连接失败");
                return View();
            }

        }

        public JsonResult CheckChangePassword(string OldPassword, string newPassword1, string ValidateCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            var UserId = user.UserId;
            if (OldPassword != null && newPassword1 != null)
            {
                ChangePasswordFlag = _ServicesSoapClient.ChangePassword(UserId, OldPassword, newPassword1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (ValidateCode != null)
                {
                    if (ValidateCode == _validateCode1)
                    {
                        if (ChangePasswordFlag == 1)
                        {
                            res.Data = true;
                        }
                        else
                        {
                            res.Data = false;
                        }
                    }
                    else
                    {
                        res.Data = false;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("errorPassword", "旧密码为空，请输入旧密码");

            }

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region <"激活">
        public ActionResult Activition()
        {
            ActivitionModel ActivitionModel = new ActivitionModel();
            return View();
        }

        //[HttpPost]
        //public ActionResult Activition(ActivitionModel ActivitionModel)
        //{
        //    try
        //    {
        //        var user = Session["CurrentUser"] as UserAndRole;
        //        var UserId = user.UserId;
        //        var InviteCode = ActivitionModel.InviteCode;
        //        if (InviteCode != "")
        //        {
        //            var ActivitionFlag = _ServicesSoapClient.Activition(UserId, InviteCode, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
        //            if (ActivitionFlag == 1)
        //            {
        //                return RedirectToAction("PatientList", "DoctorHome");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("errorInviteCode", "邀请码错误，请输入正确邀请码");
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("errorInviteCode", "邀请码为空，请输入邀请码");
        //            return View();
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        ModelState.AddModelError("errorConnection", "数据库连接失败");
        //        return View();
        //    }


        //}

        public JsonResult CheckInviteCode(string InviteCode)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            var UserId = user.UserId;
            var Flag = _ServicesSoapClient.SetActivition(user.UserId, user.Role, InviteCode);
            if (Flag == 1)
            {
                res.Data = true;
            }
            else
            {
                res.Data = false;
                ModelState.AddModelError("errorActivition", "邀请码错误，请输入正确的邀请码");
            }

            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }
        #endregion

        #region<"验证码">
        public ActionResult GetValidateCode1()
        {
            string ValidateCode = CreateValidateCode(5);
            if (ValidateCode != null)
            {
                byte[] bytes = CreateValidateGraphic(ValidateCode);
                _validateCode1 = ValidateCode;
                return File(bytes, @"image/jpeg");
            }

            else
            {
                return View();
            }
        }

        public ActionResult GetValidateCode2()
        {
            string ValidateCode = CreateValidateCode(6);
            if (ValidateCode != null)
            {
                byte[] bytes = CreateValidateGraphic(ValidateCode);
                _validateCode2 = ValidateCode;
                return File(bytes, @"image/jpeg");
            }

            else
            {
                return View();
            }
        }

        public JsonResult CheckValidateCode1(string ValidateCode)
        {
            var res = new JsonResult();
            if (ValidateCode != null)
            {
                if (ValidateCode == _validateCode1)
                {
                    res.Data = true;
                }
                else
                {
                    res.Data = false;
                }
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult CheckValidateCode2(string PhoneNo, string ValidateCode)
        {
            var res = new JsonResult();
            int ret = _ServicesSoapClient.checkverification(PhoneNo, "verification", ValidateCode);
            if (ret == 1)
            {
                res.Data = true;
            }
            else
            {
                res.Data = false;
            }
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        public JsonResult SendSMS(string PhoneNo)
        {
            var res = new JsonResult();
            res.Data = _ServicesSoapClient.sendSMS(PhoneNo, "verification","");
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public string CreateValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        #endregion

    }
}
