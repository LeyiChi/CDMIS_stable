using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Web.Security;
using CDMIS.OtherCs;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
   [StatisticsTracker]
    public class PersonalController : Controller
    {
        //
        // GET: /Personal/
        static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult UserOverview()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            UserOverviewViewModel overview = new UserOverviewViewModel();
            if (user != null)
            {
                overview.UserId = user.UserId;
                overview.UserName = user.UserName;
                overview.Role = user.Role;
                overview.Message = _ServicesSoapClient.GetUnreadCount(user.UserId).ToString();
                overview.ServerIP = _ServicesSoapClient.getLocalmachineIPAddress();
                overview.InvalidFlag = _ServicesSoapClient.GetActivatedState(user.UserId, user.Role);
                if (user.Role == "Patient")
                {
                    List<ToDoList> list = new List<ToDoList>();
                    overview.UndoneCount = _ServicesSoapClient.GetUndoneNum(user.UserId).ToString();
                    double[] reminder = { 0, 0, 0 };
                    string[] content = { "", "", "" };
                    TLFunctions.GetTaskTime(_ServicesSoapClient, user.UserId, ref list);
                    overview.TodoList = list;
                    
                    //ViewData["reminder1"] = reminder[0];
                    //ViewData["reminder2"] = reminder[1];
                    //ViewData["reminder3"] = reminder[2];
                    //ViewData["content1"] = content[0];
                    //ViewData["content2"] = content[1];
                    //ViewData["content3"] = content[2];
                    //ViewData["reminder1"] = 2000;
                    //ViewData["reminder2"] = 3000;
                    //ViewData["reminder3"] = 6000;
                }
            }
            else 
            {
                overview.UserId = "";
                overview.UserName = "";
                overview.Role = "";
                overview.Message = "";
                overview.UndoneCount = "";
                FormsAuthentication.SignOut();
            }
            
            return PartialView(overview);
        }

        #region<"个人主页">
        [UserAuthorize]
        public ActionResult PersonalHomepage()
        {
            try
            {
                PersonalHomepageViewModel PersonalHomepageModel = new PersonalHomepageViewModel();
                CDMIS.Models.PatientBasicInfo patient = new CDMIS.Models.PatientBasicInfo();
                GetPersonalInfo(PersonalHomepageModel, patient);
                return View(PersonalHomepageModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "数据库连接失败");
                return View();
            }
        }

        [HttpPost]
        [UserAuthorize]
        public ActionResult PersonalHomepage(PersonalHomepageViewModel PersonalHomepageModel, CDMIS.Models.PatientBasicInfo patient)
        {

            try
            {
                var Flag = EditPersonalInfo(PersonalHomepageModel, patient);
                if (Flag == 1)
                {
                    return RedirectToAction("PersonalHomepage", "Personal");
                }
                else
                {
                    ModelState.AddModelError("", "数据库操作失败");
                    return View();
                }


            }
            catch (Exception)
            {
                ModelState.AddModelError("", "数据库连接失败");
                return View();
            }

        }      
        #endregion

        #region<"Function">
        private void GetPersonalInfo(PersonalHomepageViewModel PersonalHomepageModel, CDMIS.Models.PatientBasicInfo patient)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var UserId = user.UserId;
            string hostAddress = System.Configuration.ConfigurationManager.AppSettings["WebServe"];
            PersonalHomepageModel.Role = user.Role;
            if (user.Role == "Administrator" || user.Role == "Doctor" || user.Role == "HealthCoach")
            {
                var BasicInfo = _ServicesSoapClient.GetDoctorInfo(UserId);
                var DetailInfo = _ServicesSoapClient.GetDoctorInfoDetail(UserId);
                if (BasicInfo.Tables[0].Rows.Count > 0)
                {
                    patient.UserId = UserId;
                    var UserName = _ServicesSoapClient.GetUserName(UserId);//修改：从MstUser获取UserName ZC
                    //var UserName = BasicInfo.Tables[0].Rows[0]["DoctorName"].ToString();
                    if (UserName == null)
                    {
                        UserName = "";
                    }
                    patient.UserName = UserName;
                    var Gender = BasicInfo.Tables[0].Rows[0]["Gender"].ToString();
                    if (Gender == null)
                    {
                        Gender = "0";
                    }
                    patient.Gender = Gender;
                    PersonalHomepageModel.Patient = patient;
                    var birthday = BasicInfo.Tables[0].Rows[0]["Birthday"].ToString();
                    if (birthday.Length == 8)
                    {
                        PersonalHomepageModel.Birthday = (birthday.Substring(0, 4) + "-" + birthday.Substring(4, 2) + "-" + birthday.Substring(6, 2)).ToString();
                    }
                    else
                    {
                        PersonalHomepageModel.Birthday = "";
                    }
                    PersonalHomepageModel.IDNO = DetailInfo.IDNo;
                    PersonalHomepageModel.PhoneNumber = DetailInfo.PhoneNumber;
                    PersonalHomepageModel.Address = DetailInfo.HomeAddress;
                    PersonalHomepageModel.Occupation = DetailInfo.Occupation;
                    PersonalHomepageModel.Nationality = DetailInfo.Nationality;
                    PersonalHomepageModel.EmergencyContact = DetailInfo.EmergencyContact;
                    PersonalHomepageModel.EmergencyContactPhoneNumber = DetailInfo.EmergencyContactPhoneNumber;
                    if (DetailInfo.PhotoAddress == null || DetailInfo.PhotoAddress == "")
                    {
                        PersonalHomepageModel.PhotoAddress = "http://" + hostAddress + "/PersonalPhoto/non.jpg"; 
                    }
                    else 
                    {
                        PersonalHomepageModel.PhotoAddress = "http://" + hostAddress + "/PersonalPhoto/" + DetailInfo.PhotoAddress;
                        //PersonalHomepageModel.PhotoAddress = "CDFiles\\PersonalPhoto\\Doctor\\" + DetailInfo.PhotoAddress; 
                    }
                    if (PersonalHomepageModel.Role == "Doctor" || PersonalHomepageModel.Role == "HealthCoach")
                    {
                        var DoctorDetail = _ServicesSoapClient.GetDoctorDetailInfo(UserId);
                        PersonalHomepageModel.UnitName = DoctorDetail.UnitName;
                        PersonalHomepageModel.JobTitle = DoctorDetail.JobTitle;
                        PersonalHomepageModel.Level = DoctorDetail.Level;
                        PersonalHomepageModel.Dept = DoctorDetail.Dept;
                    }
                    
                }
            }
            else
            {
                var BasicInfo = _ServicesSoapClient.GetBasicInfo(UserId);
                var DetailInfo = _ServicesSoapClient.GetDetailInfo(UserId);
                patient.UserId = UserId;
                //patient.UserName = BasicInfo.UserName;
                patient.UserName = _ServicesSoapClient.GetUserName(UserId);//修改：从MstUser获取UserName ZC
                patient.Gender = BasicInfo.Gender;
                PersonalHomepageModel.Patient = patient;
                var birthday = BasicInfo.Birthday.ToString();
                if (birthday.Length == 8)
                {
                    PersonalHomepageModel.Birthday = (birthday.Substring(0, 4) + "-" + birthday.Substring(4, 2) + "-" + birthday.Substring(6, 2)).ToString();
                }
                else
                {
                    PersonalHomepageModel.Birthday = "";
                }
                PersonalHomepageModel.IDNO = DetailInfo.IDNo;
                PersonalHomepageModel.PhoneNumber = DetailInfo.PhoneNumber;
                PersonalHomepageModel.Address = DetailInfo.HomeAddress;
                PersonalHomepageModel.Occupation = DetailInfo.Occupation;
                PersonalHomepageModel.Nationality = DetailInfo.Nationality;
                PersonalHomepageModel.EmergencyContact = DetailInfo.EmergencyContact;
                PersonalHomepageModel.EmergencyContactPhoneNumber = DetailInfo.EmergencyContactPhoneNumber;
                if (DetailInfo.PhotoAddress == null || DetailInfo.PhotoAddress == "")
                {
                    //PersonalHomepageModel.PhotoAddress = "CDFiles\\PersonalPhoto\\Patient\\non.jpg";
                    PersonalHomepageModel.PhotoAddress = "http://" + hostAddress + "/PersonalPhoto/non.jpg"; 
                }
                else
                {
                    PersonalHomepageModel.PhotoAddress = "http://" + hostAddress + "/PersonalPhoto/" + DetailInfo.PhotoAddress;
                    //PersonalHomepageModel.PhotoAddress = "CDFiles\\PersonalPhoto\\Patient\\" + DetailInfo.PhotoAddress;
                }
            }
        }

        private int EditPersonalInfo(PersonalHomepageViewModel PersonalHomepageModel, CDMIS.Models.PatientBasicInfo patient)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var UserId = user.UserId;
            //var SetBasicInfo = service.SetBasicInfo(UserId, UserName, Birthday, Gender, BloodType, IDNo, DoctorId, InsuranceType, InvalidFlag, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            //var SetBasicInfoDetail = service.SetBasicInfoDetail(Patient, CategoryCode, ItemCode, ItemSeq, Value, Description, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            string CategoryCode = "Contact";
            int ItemSeq = 1;
            int SortNo = 1;
            int Birthday = 0;
            string avatarPath = "";
            //System.Data.DataSet GetPatientBasicInfoDetailList = _ServicesSoapClient.GetPatientBasicInfoDetail(UserId, CategoryCode);

            var UserName = patient.UserName;
            var Gender = Convert.ToInt32(patient.Gender);

            if (PersonalHomepageModel.Birthday != null)
            {
                var birthday = PersonalHomepageModel.Birthday;
                Birthday = Convert.ToInt32((birthday.Substring(0, 4) + birthday.Substring(5, 2) + birthday.Substring(8, 2)).ToString());
            }
            else
            {
                Birthday = 0;
            }
            var IDNo = PersonalHomepageModel.IDNO;
            var PhoneNumber = PersonalHomepageModel.PhoneNumber;
            var HomeAddress = PersonalHomepageModel.Address;
            var Occupation = PersonalHomepageModel.Occupation;
            var Nationality = PersonalHomepageModel.Nationality;
            var EmergencyContact = PersonalHomepageModel.EmergencyContact;
            var EmergencyContactPhoneNumber = PersonalHomepageModel.EmergencyContactPhoneNumber;
            //var PhotoAddress = PersonalHomepageModel.PhotoAddress;
            HttpPostedFileBase image = Request.Files["fileUpload"];
            if (image != null && image.ContentLength > 0)
            {
                string fileName = UserId + ".jpg";
                string filePath = "";
                string hostAddress = System.Configuration.ConfigurationManager.AppSettings["WebServe"];
                filePath = "PersonalPhoto/";
                avatarPath = fileName;
                ResizeAndSaveImage(image, 168, 168, filePath, fileName);
            }
            else
            {
                string[] s = PersonalHomepageModel.PhotoAddress.Split('/');
                avatarPath = s[s.Length - 1];
            }

            int setSuccessFlag = 0;
            if (user.Role == "Administrator" || user.Role == "Doctor" || user.Role == "HealthCoach")
            {
                var DoctorBasicInfo = _ServicesSoapClient.GetDoctorInfo(UserId);
                var DoctorInvalidFlag = Convert.ToInt32(DoctorBasicInfo.Tables[0].Rows[0]["InvalidFlag"].ToString());
                var SetDoctorBasicFlag = _ServicesSoapClient.SetPsDoctor(UserId, UserName, Birthday, Gender, null, DoctorInvalidFlag, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorPhoneNumberFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact002_1", ItemSeq, PhoneNumber, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorHomeAddressFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact002_2", ItemSeq, HomeAddress, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorOccupationFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_2", ItemSeq, Occupation, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorNationalityFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_3", ItemSeq, Nationality, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorECFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact002_3", ItemSeq, EmergencyContact, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorECPNFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact002_4", ItemSeq, EmergencyContactPhoneNumber, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorPhotoFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_4", ItemSeq, avatarPath, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetDoctorIDNoFlag = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_1", ItemSeq, IDNo, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (user.Role == "Doctor" || user.Role == "HealthCoach")
                {
                    var SetDoctorUnitName = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_5", ItemSeq, PersonalHomepageModel.UnitName, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    var SetDoctorJobTitle = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_6", ItemSeq, PersonalHomepageModel.JobTitle, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    var SetDoctorLevel = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_7", ItemSeq, PersonalHomepageModel.Level, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    var SetDoctorDept = _ServicesSoapClient.SetDoctorInfoDetail(UserId, CategoryCode, "Contact001_8", ItemSeq, PersonalHomepageModel.Dept, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                    if (SetDoctorUnitName == true && SetDoctorJobTitle == true && SetDoctorLevel == true && SetDoctorDept == true)
                    {
                        setSuccessFlag = 1;
                    }
                    else
                    {
                        ModelState.AddModelError("", "数据库连接失败");
                        return 0;
                    }
                }
                SetDoctorPhoneNumberFlag = _ServicesSoapClient.SetPhoneNo(UserId, "PhoneNo", PhoneNumber, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType) == 1? true: false;
                if (SetDoctorBasicFlag == true && SetDoctorPhoneNumberFlag == true && SetDoctorHomeAddressFlag == true && SetDoctorOccupationFlag == true && SetDoctorNationalityFlag == true && SetDoctorECFlag == true && SetDoctorECPNFlag == true && SetDoctorPhotoFlag == true && SetDoctorIDNoFlag == true)
                {
                    setSuccessFlag = 1;
                    //return 1;
                }
                else
                {
                    ModelState.AddModelError("", "数据库连接失败");
                    return 0;
                }
            }
            //判断该用户是否为患者，同步Ps.BasicInfo表
            int isPatientFlag = 0;
            if (user.Role == "Doctor" || user.Role == "HealthCoach")
            {
                DataSet roleDs = _ServicesSoapClient.GetAllRoleMatch(UserId);
                if (roleDs.Tables.Count != 0)
                {
                    DataTable roleDt = roleDs.Tables[0];
                    foreach (DataRow dr in roleDt.Rows)
                    {
                        if (dr["RoleClass"].ToString() == "Patient")
                        {
                            isPatientFlag = 1;
                            break;
                        }
                    }
                }
            }
            if ((user.Role == "Doctor" || user.Role == "HealthCoach") && isPatientFlag == 1)
            {
                var GetBasicInfoList = _ServicesSoapClient.GetUserBasicInfo(UserId);
                var BloodType = Convert.ToInt32(GetBasicInfoList.BloodType);
                var DoctorId = GetBasicInfoList.DoctorId;
                var InsuranceType = GetBasicInfoList.InsuranceType;
                var InvalidFlag = GetBasicInfoList.InvalidFlag;
                var SetPatientBasicFlag = _ServicesSoapClient.SetBasicInfo(UserId, UserName, Birthday, Gender, BloodType, null, DoctorId, InsuranceType, InvalidFlag, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                //var SetPatientPhoneNumberFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact002_1", ItemSeq, PhoneNumber, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientHomeAddressFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact002_2", ItemSeq, HomeAddress, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientOccupationFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact001_2", ItemSeq, Occupation, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientNationalityFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact001_3", ItemSeq, Nationality, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientECFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact002_3", ItemSeq, EmergencyContact, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientECPNFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact002_4", ItemSeq, EmergencyContactPhoneNumber, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientPhotoFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact001_4", ItemSeq, avatarPath, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                var SetPatientIDNoFlag = _ServicesSoapClient.SetBasicInfoDetail(UserId, CategoryCode, "Contact001_1", ItemSeq, IDNo, null, SortNo, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
                if (SetPatientBasicFlag == true && SetPatientHomeAddressFlag == true && SetPatientOccupationFlag == true && SetPatientNationalityFlag == true && SetPatientECFlag == true && SetPatientECPNFlag == true && SetPatientPhotoFlag == true && SetPatientIDNoFlag == true)
                {
                    setSuccessFlag = 1;
                    //return 1;
                }
                else
                {
                    ModelState.AddModelError("", "数据库连接失败");
                    return 0;
                }
            }
            if (setSuccessFlag == 1)
            {
                return 1;
            }
            else 
            {
                return 0;
            }
        }

        //压缩图片并保存
        public static void ResizeAndSaveImage(HttpPostedFileBase file, int width, int height, string imagePath, string fileName)
        {
            // 缩放上传的文件
            Image OrigImage = Image.FromStream(file.InputStream);
            // 创建缩略图对象
            Bitmap TempBitmap = new Bitmap(width, height);
            // 创建缩略图画质
            Graphics NewImage = Graphics.FromImage(TempBitmap);
            NewImage.CompositingQuality = CompositingQuality.HighQuality;
            NewImage.SmoothingMode = SmoothingMode.HighQuality;
            NewImage.InterpolationMode =InterpolationMode.HighQualityBicubic;
            // 创建Rectangle对象进行绘制
            Rectangle imageRectangle = new Rectangle(0, 0,width, height);
            NewImage.DrawImage(OrigImage, imageRectangle);
            // 上传缩略图
            //TempBitmap.Save(stream, OrigImage.RawFormat);
            MemoryStream ms = new MemoryStream();
            TempBitmap.Save(ms, OrigImage.RawFormat);
            byte[] byteImage = new Byte[ms.Length];
            byteImage = ms.ToArray();
            string requesturl = _ServicesSoapClient.UpLoadImage(byteImage, fileName, imagePath);   

            // 释放资源
            NewImage.Dispose();
            TempBitmap.Dispose();
            OrigImage.Dispose();
            ms.Close();
        }
        #endregion

    }
}
