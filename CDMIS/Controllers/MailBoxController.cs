using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.ViewModels;
using CDMIS.ServiceReference;
using System.Data;
using CDMIS.Models;
using System.IO;
using CDMIS.OtherCs;
using CDMIS.CommonLibrary;

namespace CDMIS.Controllers
{
    [StatisticsTracker]
    [UserAuthorize]
    public class MailBoxController : Controller
    {
        public static ServicesSoapClient _ServicesSoapClient = new ServicesSoapClient();
        //
        // GET: /MailBox/

        # region <"公用部分视图">
        //信箱公共部分
        [ChildActionOnly]
        public ActionResult MailBoxShare()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MailboxViewModel ShareView = new MailboxViewModel();
            ShareView.UserId = user.UserId;
            ShareView.UnreadNum = _ServicesSoapClient.GetUnreadCount(ShareView.UserId);
            ShareView.UnsentNum = _ServicesSoapClient.GetDraftCount(ShareView.UserId);
            return PartialView(ShareView);
        }
        # endregion

        # region <"写信">
        //写信
        public ActionResult Write(MessageInfo Rei)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                MessageListViewModel WriteView = new MessageListViewModel();
                if (Rei.SendByName != null)
                {
                    WriteView.Receiver = Rei.SendByName + ";" + "　";
                }
                WriteView.UserId = user.UserId;
                WriteView.ModuleInfoList = GetModuleInfo();
                WriteView.ContactsList = GetContactsById(WriteView.UserId, WriteView.ModuleInfoList);
                return View(WriteView);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult WtrieJson(string PiTitle, string PiReceiver, string PiContent, string PiSelection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            //上传文件
            HttpPostedFileBase file = Request.Files["file"];
            //if (file != null)
            //{
            //    string filePath = Path.Combine(HttpContext.Server.MapPath("../UpLoad"), Path.GetFileName(file.FileName));
            //    file.SaveAs(filePath);
            //    return RedirectToAction("Index", "MailBox", new { });
            //}

            var res = new JsonResult();
            string[] ReceiverList1 = PiReceiver.Split(';');
            int len = ReceiverList1.Length;
            bool flag = false;
            int Selection = int.Parse(PiSelection);
            for (int i = 0; i < len - 1; i++)
            {
                int IndexofA = ReceiverList1[i].IndexOf("＜");
                int IndexofB = ReceiverList1[i].IndexOf("＞");
                string ReceiverTemp = ReceiverList1[i].Substring(IndexofA + 1, IndexofB - IndexofA - 1);
                if ((PiTitle == "") || (PiTitle == null))
                {
                    PiTitle = "无主题";
                }
                flag = false;
                flag = _ServicesSoapClient.SetMessage("", 1, user.UserId, PiTitle, ReceiverTemp, PiContent, Selection, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }


        public JsonResult GetLatestMeNo(string PiReceiver)
        {
            var res = new JsonResult();
            var user = Session["CurrentUser"] as UserAndRole;
            var MeNo = _ServicesSoapClient.GetLatestSMS(PiReceiver, user.UserId);
            res.Data = MeNo.MessageNo;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        # endregion

        #region <"收信">
        //收信
        public ActionResult Receive()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MessageListViewModel ReceiveView = new MessageListViewModel();
            ReceiveView.MessageList = GetReceiveList(user.UserId);
            return View(ReceiveView);
        }

        //已收消息详细
        public ActionResult ReceiveDetail(string ID)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            //点击后消息变为已读
            bool flag = false;
            flag = _ServicesSoapClient.ChangeReadStatus(ID, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            return View(GetMessageDetail(ID));
        }
        # endregion

        # region <"已发送">
        //已发送
        public ActionResult HaveSent()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MessageListViewModel HaveSentView = new MessageListViewModel();
            HaveSentView.MessageList = GetHaveSentList(user.UserId, "HaveSent");
            return View(HaveSentView);
        }

        //已发送消息详细
        public ActionResult SendDetail(string ID)
        {
            return View(GetMessageDetail(ID));
        }
        #endregion

        # region <"草稿箱">
        //草稿箱
        public ActionResult Draft()
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MessageListViewModel DraftView = new MessageListViewModel();
            DraftView.MessageList = GetHaveSentList(user.UserId, "Draft");
            return View(DraftView);
        }

        //删除
        public JsonResult DeleteJson(string PiMessageNo)
        {
            var res = new JsonResult();
            bool flag = false;
            flag = _ServicesSoapClient.DeleteDraft(PiMessageNo);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        //草稿箱消息详细
        public ActionResult DraftDetail(string ID)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MessageListViewModel DraftDetailView = new MessageListViewModel();
            DraftDetailView.MessageNo = ID;
            var MessageDetail = _ServicesSoapClient.GetMessageDetail(ID);
            if (MessageDetail != null)
            {
                DraftDetailView.Receiver = MessageDetail.RecieverName + "＜" + MessageDetail.Reciever + "＞;" + "　";
                DraftDetailView.Title = MessageDetail.Title;
                DraftDetailView.Content = MessageDetail.Content;
            }
            DraftDetailView.UserId = user.UserId;
            DraftDetailView.ModuleInfoList = GetModuleInfo();
            DraftDetailView.ContactsList = GetContactsById(DraftDetailView.UserId, DraftDetailView.ModuleInfoList);
            return View(DraftDetailView);
        }

        //发送或保存
        public JsonResult DraftJson(string PiMessageNo, string PiTitle, string PiReceiver, string PiContent, string PiSelection)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            bool flag = false;
            int Selection = int.Parse(PiSelection);
            int IndexofA = PiReceiver.IndexOf("＜");
            int IndexofB = PiReceiver.IndexOf("＞");
            string ReceiverTemp = PiReceiver.Substring(IndexofA + 1, IndexofB - IndexofA - 1);
            if ((PiTitle == "") || (PiTitle == null))
            {
                PiTitle = "无主题";
            }
            flag = false;
            flag = _ServicesSoapClient.SetMessage(PiMessageNo, 1, user.UserId, PiTitle, ReceiverTemp, PiContent, Selection, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            res.Data = flag;
            res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return res;
        }

        #endregion

        #region <"快捷写信 ZAM 2015-1-6">
        //快捷写信
        [HttpGet]
        public ActionResult FastWrite(string SendBy, string Receiver)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            MessageInfo Meg = new MessageInfo();
            Meg.Receiver = Receiver;
            Meg.SendBy = SendBy;
            Meg.SendByName = user.UserName;
           // Meg.UserId = user.UserId;
            Meg.ServerIP = _ServicesSoapClient.getLocalmachineIPAddress();//ServerIP
            return View(Meg);
        }

        public JsonResult FastWrite(string SendBy, string Title, string Receiver, string Content)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            //if (Code == "") Code = _ServicesSoapClient.GetTreatmentMaxCode(Type);
            if ((Title == "") || (Title == null))
            {
                Title = "无主题";
            }
            bool flag = false;
            flag = _ServicesSoapClient.SetMessage("", 1, SendBy, Title, Receiver, Content, 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            if (flag)
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
        #endregion

        #region <"快捷写信 WF 2015-1-14">
        //快捷写信
        [HttpGet]
        public ActionResult FastWriteFromPat(string SendBy)
        {
            try
            {
                var user = Session["CurrentUser"] as UserAndRole;
                MessageListViewModel WriteView = new MessageListViewModel();
                WriteView.UserId = user.UserId;
                WriteView.UserName = user.UserName;
                WriteView.ServerIP = _ServicesSoapClient.getLocalmachineIPAddress();//ServerIP
                WriteView.ModuleInfoList = GetModuleInfo();
                WriteView.ContactsList = GetContactsById(WriteView.UserId, WriteView.ModuleInfoList);
                return View(WriteView);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult FastWriteFromPat(string SendBy, string Title, string Receiver, string Content)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            var res = new JsonResult();
            string[] ReceiverList1 = Receiver.Split(';');
            int len = ReceiverList1.Length;
            bool flag = false;
            for (int i = 0; i < len - 1; i++)
            {
                int IndexofA = ReceiverList1[i].IndexOf("＜");
                int IndexofB = ReceiverList1[i].IndexOf("＞");
                string ReceiverTemp = ReceiverList1[i].Substring(IndexofA + 1, IndexofB - IndexofA - 1);
                if ((Title == "") || (Title == null))
                {
                    Title = "无主题";
                }
                flag = _ServicesSoapClient.SetMessage("", 1, SendBy, Title, ReceiverTemp, Content, 1, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            }

            if (flag)
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
        #endregion

        #region <"Function">
        //Patient获取联系人列表
        public List<ContractsInfo> GetConForPatient(string UserId, string CategoryCode)
        {
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetConForPatient(UserId, CategoryCode);
            List<ContractsInfo> items = new List<ContractsInfo>();
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ContractsInfo item = new ContractsInfo();
                        if ((dr["DoctorName"].ToString() != null) && (dr["DoctorName"].ToString() != ""))
                        {
                            item.ContractId = dr["DoctorId"].ToString();
                            item.ContractName = dr["DoctorName"].ToString();
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        //Doctor获取联系人列表
        public List<ContractsInfo> GetConForDoctor(string UserId, string CategoryCode)
        {
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetConForDoctor(UserId, CategoryCode);
            List<ContractsInfo> items = new List<ContractsInfo>();
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ContractsInfo item = new ContractsInfo();
                        if ((dr["PatientName"].ToString() != null) && (dr["PatientName"].ToString() != ""))
                        {
                            item.ContractId = dr["PatientId"].ToString();
                            item.ContractName = dr["PatientName"].ToString();
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        //Administrator获取联系人列表
        public List<ContractsInfo> GetConForAdmin()
        {
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetConForAdmin();
            List<ContractsInfo> items = new List<ContractsInfo>();
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ContractsInfo item = new ContractsInfo();
                        if ((dr["piUserName"].ToString() != null) && (dr["piUserName"].ToString() != ""))
                        {
                            item.ContractId = dr["piUserId"].ToString();
                            item.ContractName = dr["piUserName"].ToString();
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        //获取模块编码与名称
        public List<ModuleInfo> GetModuleInfo()
        {
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetModuleList();
            List<ModuleInfo> items = new List<ModuleInfo>();
            ModuleInfo item0 = new ModuleInfo();
            item0.Category = "";
            item0.ModuleName = "联系人";
            items.Add(item0);
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ModuleInfo item = new ModuleInfo();
                        item.Category = dr["Code"].ToString();
                        item.ModuleName = dr["Name"].ToString();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        //根据ID获取联系人列表
        public List<List<ContractsInfo>> GetContactsById(string UserId, List<ModuleInfo> MList)
        {
            var user = Session["CurrentUser"] as UserAndRole;
            string IdenClass = user.Role;
            //string IdenClass = _ServicesSoapClient.GetClass(UserId);
            int ModuleCount = MList.Count;
            List<List<ContractsInfo>> items = new List<List<ContractsInfo>>();

            if (IdenClass == "Patient")
            {
                items.Add(null);
                for (int i = 1; i < ModuleCount; i++)
                {
                    List<ContractsInfo> item = new List<ContractsInfo>();
                    item = GetConForPatient(UserId, MList[i].Category);
                    items.Add(item);
                }
            }
            else if (IdenClass == "Doctor")
            {
                items.Add(null);
                for (int i = 1; i < ModuleCount; i++)
                {
                    List<ContractsInfo> item = new List<ContractsInfo>();
                    item = GetConForDoctor(UserId, MList[i].Category);
                    items.Add(item);
                }

            }
            else
            {
                items.Add(GetConForAdmin());
                for (int i = 1; i < ModuleCount; i++)
                {
                    items.Add(null);
                }
            }
            return items;
        }

        //获取已收消息列表
        public List<MessageInfo> GetReceiveList(string UserId)
        {
            DataSet DS = new DataSet();
            DS = _ServicesSoapClient.GetReceiveList(UserId);
            List<MessageInfo> items = new List<MessageInfo>();
            if (DS != null)
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        MessageInfo item = new MessageInfo();
                        //MessageNo, Receiver, ReceiverName, Title, SendDateTime, Content, ReadStatus
                        item.MessageNo = dr["MessageNo"].ToString();
                        item.SendBy = dr["SendBy"].ToString();
                        item.SendByName = dr["SendByName"].ToString();
                        item.Title = dr["Title"].ToString();
                        item.SendDateTime = dr["SendDateTime"].ToString();
                        item.Content = dr["Content"].ToString();
                        item.ReadStatus = Convert.ToInt32(dr["ReadStatus"]);
                        item.OutDateFlag = Convert.ToInt32(dr["Flag"]);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        //获取已发送消息或草稿箱消息列表
        public List<MessageInfo> GetHaveSentList(string UserId, string Type)
        {
            DataSet DS = new DataSet();
            if (Type == "Draft")
            {
                DS = _ServicesSoapClient.GetDraftList(UserId);
            }
            else
            {
                DS = _ServicesSoapClient.GetHaveSentList(UserId);
            }
            List<MessageInfo> items = new List<MessageInfo>();
            if (!(DS == null))
            {
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        MessageInfo item = new MessageInfo();
                        //MessageNo, Receiver, ReceiverName, Title, piSendDateTime, Content
                        item.MessageNo = dr["MessageNo"].ToString();
                        item.Receiver = dr["Reciever"].ToString();
                        item.ReceiverName = dr["RecieverName"].ToString();
                        item.Title = dr["Title"].ToString();
                        item.SendDateTime = dr["SendDateTime"].ToString();
                        item.Content = dr["Content"].ToString();
                        item.OutDateFlag = Convert.ToInt32(dr["Flag"]);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        //获取消息详细信息
        public MessageInfo GetMessageDetail(string MessageNo)
        {
            MessageInfo item = new MessageInfo();
            var MessageDetail = _ServicesSoapClient.GetMessageDetail(MessageNo);
            if (MessageDetail != null)
            {
                item.SendBy = MessageDetail.SendBy;
                item.SendByName = MessageDetail.SendByName;
                item.Receiver = MessageDetail.Reciever;
                item.ReceiverName = MessageDetail.RecieverName;
                item.Title = MessageDetail.Title;
                item.SendDateTime = MessageDetail.SendDateTime;
                item.Content = MessageDetail.Content;
            }
            return item;
        }
        #endregion


    }
}
