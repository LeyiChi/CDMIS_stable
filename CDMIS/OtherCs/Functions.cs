using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDMIS.ViewModels;
using CDMIS.Models;
using System.Data;
using CDMIS.ServiceReference;
using System.Web.Mvc;

namespace CDMIS.OtherCs
{
    public class Functions
    {
        public static string ConvertDate(string iDate)
        {
            string sDate = "";
            if (iDate == "0")
            {
                return sDate;
            }
            else if (iDate.Length == 8)
            {
                sDate = iDate.Substring(0, 4) + "-" + iDate.Substring(4, 2) + "-" + iDate.Substring(6, 2);
                return sDate;
            }
            else
            {
                return sDate;
            }
        }

        public static string ConvertTime(string iTime)
        {
            string sTime = "";
            if (iTime == "0")
            {
                return sTime;
            }
            switch(iTime.Length)
            {
                case 6: sTime = iTime.Substring(0, 2) + ":" + iTime.Substring(2, 2) + ":" + iTime.Substring(4, 2);
                    break;
                case 5: sTime = "0" + iTime.Substring(0, 1) + ":" + iTime.Substring(1, 2) + ":" + iTime.Substring(3, 2);
                    break;
                case 4: sTime = "00" + ":" + iTime.Substring(0, 2) + ":" + iTime.Substring(2, 2);
                    break;
                case 3: sTime = "00:0" + iTime.Substring(0, 1) + ":" + iTime.Substring(1, 2);
                    break;
                case 2: sTime = "00:00:" + iTime;
                    break;
                case 1: sTime = "00:00:0" + iTime;
                    break;
                default: break;
            }
            return sTime;
        }
    }

    public class ETFunctions
    {
        //获取每日任务列表
        public static void GetReminderList(ServicesSoapClient _ServicesSoapClient, ref EverydayTaskViewModel et, string UserId)
        {
            List<Reminder> taskList = new List<Reminder>();
            DataSet EverydayTaskDS = _ServicesSoapClient.GetReminder(et.PatientId);
            DataTable EverydayTaskDT = EverydayTaskDS.Tables[0];
            foreach (DataRow dr in EverydayTaskDT.Rows)
            {
                Reminder reminder = new Reminder();
                reminder.ReminderType = dr["ReminderType"].ToString();
                reminder.ReminderTypeName = dr["ReminderTypeName"].ToString();
                reminder.ReminderNo = dr["ReminderNo"].ToString();
                reminder.Content = dr["Content"].ToString();
                reminder.AlertMode = dr["AlertMode"].ToString();
                reminder.AlertModeName = dr["AlertModeName"].ToString();
                reminder.StartDateTime = Convert.ToDateTime(dr["StartDateTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                reminder.NextDate = Functions.ConvertDate(dr["NextDate"].ToString());
                reminder.NextTime = Functions.ConvertTime(dr["NextTime"].ToString());
                reminder.Description = dr["Description"].ToString();
                reminder.CreatedBy = dr["CreatedBy"].ToString();
                if (UserId == reminder.CreatedBy)
                {
                    reminder.IsAllowed = true;
                }
                else 
                {
                    reminder.IsAllowed = false;
                }
                taskList.Add(reminder);
            }
            et.TodayTask = taskList;
        }
    }

    public class TLFunctions
    {
        //获取每日任务列表
        public static void GetTaskList(ServicesSoapClient _ServicesSoapClient, ref TaskListViewModel et)
        {
            List<TaskList> undoneList = new List<TaskList>();
            DataSet UndoneDS = _ServicesSoapClient.GetUndoneList(et.PatientId);
            if (UndoneDS.Tables.Count != 0) 
            {
                DataTable UndoneDT = UndoneDS.Tables[0];
                foreach (DataRow dr in UndoneDT.Rows)
                {
                    TaskList task = new TaskList();
                    task.ReminderNo = dr["ReminderNo"].ToString();
                    task.TaskDate = dr["TaskDate"].ToString();
                    task.TaskTime = dr["TaskTime"].ToString();
                    task.TaskDateTime = Functions.ConvertDate(task.TaskDate) + " " + Functions.ConvertTime(task.TaskTime);
                    task.IsDone = Convert.ToInt16(dr["IsDone"].ToString());
                    task.Description = dr["Description"].ToString();
                    task.Content = dr["Content"].ToString();
                    undoneList.Add(task);
                }
                et.UndoneList = undoneList;
            }

            List<TaskList> toDoList = new List<TaskList>();
            DataSet ToDoDS = _ServicesSoapClient.GetToDoList(et.PatientId);
            if (ToDoDS.Tables.Count != 0)
            {
                DataTable ToDoDT = ToDoDS.Tables[0];
                foreach (DataRow dr in ToDoDT.Rows)
                {
                    TaskList task = new TaskList();
                    task.ReminderNo = dr["ReminderNo"].ToString();
                    task.TaskDate = dr["TaskDate"].ToString();
                    task.TaskTime = dr["TaskTime"].ToString();
                    task.TaskDateTime = Functions.ConvertDate(task.TaskDate) + " " + Functions.ConvertTime(task.TaskTime);
                    task.IsDone = Convert.ToInt16(dr["IsDone"].ToString());
                    task.Description = dr["Description"].ToString();
                    task.Content = dr["Content"].ToString();

                    toDoList.Add(task);
                }
                et.ToDoList = toDoList;
            }
        }

        //public static void GetTaskTime(ServicesSoapClient _ServicesSoapClient, string PatientId, ref double[] reminder, ref string[] content)
        //{
        //    List<TaskList> toDoList = new List<TaskList>();
        //    DataSet ToDoDS = _ServicesSoapClient.GetToDoList(PatientId);
        //    if (ToDoDS.Tables.Count != 0)
        //    {
        //        DataTable ToDoDT = ToDoDS.Tables[0];
        //        for (int i = 0; i < 3 && i < ToDoDT.Rows.Count; i++)
        //        {
        //            DateTime taskTime = DateTime.Parse(Functions.ConvertTime(ToDoDT.Rows[i]["TaskTime"].ToString()));
        //            DateTime nowTime = DateTime.Now;
        //            TimeSpan span = taskTime.Subtract(nowTime);
        //            reminder[i] = span.TotalMilliseconds;
        //            content[i] = ToDoDT.Rows[i]["Content"].ToString();
        //        }
        //    }
        //}

        public static void GetTaskTime(ServicesSoapClient _ServicesSoapClient, string PatientId, ref List<ToDoList> TodoList)
        {
            List<TaskList> toDoList = new List<TaskList>();
            DataSet ToDoDS = _ServicesSoapClient.GetToDoList(PatientId);
            if (ToDoDS.Tables.Count != 0)
            {
                DataTable ToDoDT = ToDoDS.Tables[0];
                for (int i = 0; i < 3 && i < ToDoDT.Rows.Count; i++)
                {
                    DateTime taskTime = DateTime.Parse(Functions.ConvertTime(ToDoDT.Rows[i]["TaskTime"].ToString()));
                    DateTime nowTime = DateTime.Now;
                    TimeSpan span = taskTime.Subtract(nowTime);
                    ToDoList todo = new ToDoList();
                    todo.Num = (i + 1).ToString();
                    todo.PatientId = PatientId;
                    todo.ReminderNo = ToDoDT.Rows[i]["ReminderNo"].ToString();
                    todo.ReminderTime = span.TotalMilliseconds;
                    todo.Content = ToDoDT.Rows[i]["Content"].ToString();
                    TodoList.Add(todo);
                }
            }
        }
    }
    //症状管理-WF
    public class SYFunctions
    {
        public static void GetSymptomsList(ServicesSoapClient _ServicesSoapClient, ref SymptomsViewModel sy)
        {
            DataSet SymptomsDS = new DataSet();
            DataTable SymptomsInfoList = new DataTable();
            SymptomsDS = _ServicesSoapClient.GetSymptomsListByPId(sy.PId);
            SymptomsInfoList = SymptomsDS.Tables[0];
            foreach (DataRow SymptomsDR in SymptomsInfoList.Rows)
            {
                SymptomInfo SymptomInfoItem = new SymptomInfo();
                String Date = string.Empty;
                String Time = string.Empty;
                SymptomInfoItem.UserId = sy.PId;
                SymptomInfoItem.VisitId = SymptomsDR["VisitId"].ToString();
                SymptomInfoItem.SymptomsNo = Convert.ToInt32(SymptomsDR["SynptomsNo"]);
                SymptomInfoItem.SymptomsTypeName = SymptomsDR["SymptomsTypeName"].ToString();
                SymptomInfoItem.SymptomsName = SymptomsDR["SymptomsName"].ToString();
                SymptomInfoItem.Description = SymptomsDR["Description"].ToString();
                SymptomInfoItem.RecordDate = Functions.ConvertDate(SymptomsDR["RecordDate"].ToString());
                SymptomInfoItem.RecordTime = Functions.ConvertTime(SymptomsDR["RecordTime"].ToString());
                SymptomInfoItem.ReInUserId = SymptomsDR["ReInUserId"].ToString();
                if (SymptomInfoItem.ReInUserId == sy.UserId)
                {
                    SymptomInfoItem.IsAllowed = true;
                }
                else
                {
                    SymptomInfoItem.IsAllowed = false;
                }
                sy.SymptomsList.Add(SymptomInfoItem);
            }
        }
        public static void SetRecordDateTime(ServicesSoapClient _ServicesSoapClient, DateTime? Time, ref int RecordDate, ref int RecordTime)
        {
            if (Time != null)
            {
                string head = Time.ToString().Split(' ')[0];
                string tail = Time.ToString().Split(' ')[1];
                string year = head.ToString().Split('/')[0];
                string month = head.ToString().Split('/')[1];
                if (Convert.ToInt32(month) < 10)
                {
                    month = "0" + month;
                }
                string day = head.ToString().Split('/')[2];
                if (Convert.ToInt32(day) < 10)
                {
                    day = "0" + day;
                }
                string hour = tail.ToString().Split(':')[0];
                string minute = tail.ToString().Split(':')[1];
                string second = tail.ToString().Split(':')[2];
                RecordDate = Convert.ToInt32(year + month + day);
                RecordTime = Convert.ToInt32(hour + minute + second);
            }
        }
        public static bool SetSymptomsInfo(ServicesSoapClient _ServicesSoapClient, SymptomsViewModel sy, int RecordDate, int RecordTime, UserAndRole user)
        {
            string symptomtype = sy.SymptomsTypeSelected;
            //string symptom = Request.Form["SymptomsNameSelected"];
            string symptom = sy.SymptomsNameSelected;
            string Description = sy.Description;
            string UserId = sy.PId;
            string VisitId = _ServicesSoapClient.GetNoByNumberingType(6);
            bool SetSymptomsInfoFlag = _ServicesSoapClient.SetSymptomsInfo(UserId, VisitId, symptomtype, symptom, Description, RecordDate, RecordTime, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            return SetSymptomsInfoFlag;
        }
    }
    //治疗方案-WF
    public class TreatFunctions
    {
        public static void GetTreatmentList(ServicesSoapClient _ServicesSoapClient, string DoctorId, string PId, List<TreatmentInfo> Tr)
        {
            DataSet TreatmentInfoDS = _ServicesSoapClient.GetTreatmentList(PId);
            DataTable TreatmentInfoListCache = TreatmentInfoDS.Tables[0];
            foreach (DataRow TreatmentInfoItem in TreatmentInfoListCache.Rows)
            {
                TreatmentInfo TreatmentInfomation = new TreatmentInfo();
                TreatmentInfomation.UserId = PId;
                TreatmentInfomation.SortNo = Convert.ToInt32(TreatmentInfoItem["SortNo"]);
                TreatmentInfomation.TreatmentGoal = TreatmentInfoItem["TreatmentGoalName"].ToString();
                TreatmentInfomation.TreatmentAction = TreatmentInfoItem["TreatmentActionName"].ToString();
                TreatmentInfomation.Group = TreatmentInfoItem["GroupName"].ToString();
                TreatmentInfomation.TreatmentPlan = TreatmentInfoItem["TreatmentPlan"].ToString();
                TreatmentInfomation.TreatTime = TreatmentInfoItem["TreatTime"].ToString();
                TreatmentInfomation.Duration = TreatmentInfoItem["DurationName"].ToString();
                TreatmentInfomation.ReInUserId = TreatmentInfoItem["ReInUserId"].ToString();
                if (DoctorId != string.Empty)
                {
                    if (TreatmentInfomation.ReInUserId == DoctorId)
                    {
                        TreatmentInfomation.IsAllowed = true;
                    }
                    else
                    {
                        TreatmentInfomation.IsAllowed = false;
                    }
                }
                Tr.Add(TreatmentInfomation);
            }
        }
        public static bool SetTreatmentInfo(ServicesSoapClient _ServicesSoapClient, TreatmentViewModel Tr, UserAndRole user)
        {
            string UserId = Tr.PId;
            string TreatmentGoal = Tr.TreatmentInfo.TreatmentGoal;
            string TreatmentAction = Tr.TreatmentInfo.TreatmentAction;
            string Group = Tr.TreatmentInfo.Group;
            string TreatmentPlan = Tr.TreatmentInfo.TreatmentPlan;
            string Description = string.Empty;
            //取数据库时间
            DateTime TreatTime = DateTime.Now; //输入无效，数据库取当前时间自动存入
            string Duration = Tr.TreatmentInfo.Duration;
            bool SetTreatmentInfoFlag = _ServicesSoapClient.SetTreatmentInfo(UserId, Convert.ToInt32(TreatmentGoal), Convert.ToInt32(TreatmentAction), Convert.ToInt32(Group), TreatmentPlan, Description, TreatTime, Duration, user.UserId, user.TerminalName, user.TerminalIP, user.DeviceType);
            return SetTreatmentInfoFlag;
        }
    }
    //患者警报-WF
    public class TrnFunctions
    {
        public static void GetTrnList(ServicesSoapClient _ServicesSoapClient, ref DataTable PatientAlertInfoList, ref PatientAlertViewModel Trn, bool flag)
        {
            Trn.AlertList.Clear();
            Trn.AlertList = new List<AlertInfo>();
            if (flag == false)
            {
                DataSet PatientAlertDS = new DataSet();
                //DataTable PatientAlertInfoList = new DataTable();
                PatientAlertDS = _ServicesSoapClient.GetTrnAlertRecordList(Trn.UserId);
                PatientAlertInfoList = PatientAlertDS.Tables[0];
            }
            int processFlag = Convert.ToInt32(Trn.AlertStatusSelected);
            DataTable dt = new DataTable();
            dt = OtherCs.TrnFunctions.SelectAlerts(PatientAlertInfoList, processFlag);
            foreach (DataRow PatientAlertInfoListRow in dt.Rows)
            {
                AlertInfo AlertInfoItem = new AlertInfo();
                AlertInfoItem.UserId = Trn.UserId;
                AlertInfoItem.SortNo = Convert.ToInt32(PatientAlertInfoListRow["SortNo"]);
                AlertInfoItem.AlertTypeName = PatientAlertInfoListRow["AlertTypeName"].ToString();
                AlertInfoItem.AlertItemName = PatientAlertInfoListRow["AlertItem"].ToString();
                AlertInfoItem.AlertDateTime = PatientAlertInfoListRow["AlertDateTime"].ToString();
                AlertInfoItem.ProcessFlag = Convert.ToInt32(PatientAlertInfoListRow["ProcessFlag"]);

                Trn.AlertList.Add(AlertInfoItem);
            }
        }
        public static void SetRecordDateTime(ServicesSoapClient _ServicesSoapClient, DateTime? Time, ref int RecordDate, ref int RecordTime)
        {
            if (Time != null)
            {
                string head = Time.ToString().Split(' ')[0];
                string tail = Time.ToString().Split(' ')[1];
                string year = head.ToString().Split('/')[0];
                string month = head.ToString().Split('/')[1];
                if (Convert.ToInt32(month) < 10)
                {
                    month = "0" + month;
                }
                string day = head.ToString().Split('/')[2];
                if (Convert.ToInt32(day) < 10)
                {
                    day = "0" + day;
                }
                string hour = tail.ToString().Split(':')[0];
                string minute = tail.ToString().Split(':')[1];
                string second = tail.ToString().Split(':')[2];
                RecordDate = Convert.ToInt32(year + month + day);
                RecordTime = Convert.ToInt32(hour + minute + second);
            }
        }
        //警报列表的模糊检索 PID Name alertstatus
        public static DataTable SelectAlerts(DataTable dt, int alertStatus)
        {
            DataTable dt_SelectedPatients = dt.Clone();
            //DataRow[] drArr = dt.Select("PatientId LIKE 'PID20150107%'");
            string filterExpression = "";
            if (alertStatus == 1) //1: 未处理
            {
                filterExpression += "processFlag = 1";
            }
            else if (alertStatus == 2)
            {
                filterExpression += "processFlag = 2";
            }


            DataRow[] drArr = dt.Select(filterExpression);
            //DataRow[] drArr = dt.Select("PatientId LIKE \'" + patientId + "%' AND PatientName LIKE \'%" + patientName + "%'");
            for (int i = 0; i < drArr.Length; i++)
            {
                dt_SelectedPatients.ImportRow(drArr[i]);
            }
            return dt_SelectedPatients;
        }

    }

    #region 李山
    public class PDCHPFunctions
    {

        //个人信息（不可编辑）
        public static List<PatientDetailInfo> GetPatientDetailInfo(ServicesSoapClient _ServicesSoapClient, string UserId, string Category)
        {
            List<PatientDetailInfo> ItemInfo = new List<PatientDetailInfo>();
            DataSet ItemInfoSet = _ServicesSoapClient.GetItemInfoByPIdAndModule(UserId, Category);
            foreach (DataTable item in ItemInfoSet.Tables)
            {
                foreach (DataRow row in item.Rows)
                {
                    if (row[3].ToString() != "InvalidFlag" && row[3].ToString() != "Patient")
                    {
                        if (row[3].ToString() == "Doctor")
                        {
                            PatientDetailInfo NewLine = new PatientDetailInfo()
                            {
                                CategoryCode = row[1].ToString(),
                                CategoryName = row[2].ToString(),
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = _ServicesSoapClient.GetUserName(row[7].ToString())
                            };
                            ItemInfo.Add(NewLine);
                        }
                        else
                        {
                            PatientDetailInfo NewLine = new PatientDetailInfo()
                            {
                                ItemCode = row[3].ToString(),
                                ItemName = row[4].ToString(),
                                ParentCode = row[5].ToString(),
                                ControlType = row[11].ToString(),
                                OptionCategory = row[12].ToString(),
                                ItemSeq = Convert.ToInt32(row[6]),
                                Value = row[7].ToString(),
                                Content = row[8].ToString(),
                                GroupHeaderFlag = Convert.ToInt32(row[13])
                            };
                            if (NewLine.ControlType != "7")
                                NewLine.OptionList = GetTypeList(_ServicesSoapClient, NewLine.OptionCategory, NewLine.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
                            ItemInfo.Add(NewLine);
                        }
                    }
                }
            }
            return ItemInfo;
        }

        //个人信息（可编辑）
        public static List<List<PatientDetailInfo>> GetPatientDetailInfoEdit(ServicesSoapClient _ServicesSoapClient, string UserId, string DoctorId)
        {

            DataSet set = _ServicesSoapClient.GetPatBasicInfoDtlList(UserId);  //获取关注的详细信息

            List<List<PatientDetailInfo>> zong1 = new List<List<PatientDetailInfo>>();

            foreach (DataTable ta in set.Tables)
            {
                List<PatientDetailInfo> items1 = new List<PatientDetailInfo>();
                foreach (System.Data.DataRow row in ta.Rows)
                {
                    if (row[3].ToString() != "InvalidFlag")
                    {
                        if (row[3].ToString() == "Doctor")
                        {


                            PatientDetailInfo one = new PatientDetailInfo
                            {
                                //PatientId = row[0].ToString(),
                                //CategoryCode = row[1].ToString(),               //主键  之后来自数据库(因为改变的只有value）
                                CategoryName = row[2].ToString(),         //界面
                                ItemCode = row[3].ToString(),             //界面 //主键    
                                ItemName = row[4].ToString(),             //界面
                                ParentCode = row[5].ToString(),           //界面
                                ControlType = row[11].ToString(),         //界面    //控制是下拉框还是自由文本（放在value里，description？）
                                OptionCategory = row[12].ToString(),               //yesorno
                                //OptionList = row[],?                             //下拉框（需选中）
                                ItemSeq = Convert.ToInt32(row[6]),              //主键  
                                Value = row[7].ToString(),
                                Content = _ServicesSoapClient.GetUserName(row[7].ToString()),           //界面
                                Description = row[9].ToString()
                                //int SortNo = Convert.ToInt32(row[10]); 
                            };

                            one.OptionList = GetTypeList(_ServicesSoapClient, one.OptionCategory, one.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
                            if (one.Value == DoctorId)
                            {
                                one.EditDeleteFlag = "true";
                            }
                            else
                            {
                                one.EditDeleteFlag = "false";
                            }
                            items1.Add(one);
                        }

                        else
                        {
                            PatientDetailInfo one = new PatientDetailInfo
                            {
                                //PatientId = row[0].ToString(),
                                //CategoryCode = row[1].ToString(),               //主键  之后来自数据库(因为改变的只有value）
                                CategoryName = row[2].ToString(),         //界面
                                ItemCode = row[3].ToString(),             //界面 //主键    
                                ItemName = row[4].ToString(),             //界面
                                ParentCode = row[5].ToString(),           //界面
                                ControlType = row[11].ToString(),         //界面    //控制是下拉框还是自由文本（放在value里，description？）
                                OptionCategory = row[12].ToString(),               //yesorno
                                //OptionList = row[],?                             //下拉框（需选中）
                                ItemSeq = Convert.ToInt32(row[6]),              //主键  
                                Value = row[7].ToString(),
                                Content = row[8].ToString(),             //界面
                                Description = row[9].ToString()
                                //int SortNo = Convert.ToInt32(row[10]); 
                            };
                            one.OptionList = GetTypeList(_ServicesSoapClient, one.OptionCategory, one.Value);  //通过yesornoh和value，结合字典表，生成有值的下拉框
                            items1.Add(one);
                        }
                    }
                }
                zong1.Add(items1);
            }
            return zong1;
        }

        //下拉框生成（确定选中值）
        public static List<SelectListItem> GetTypeList(ServicesSoapClient _ServicesSoapClient, string Type, string Value)
        {
            DataSet typeset = _ServicesSoapClient.GetTypeList(Type);   //字典表

            List<SelectListItem> dropdownList = new List<SelectListItem>();
            dropdownList.Add(new SelectListItem { Text = "请选择", Value = "0" });
            foreach (System.Data.DataRow typerow in typeset.Tables[0].Rows)
            {
                dropdownList.Add(new SelectListItem { Text = typerow[1].ToString(), Value = typerow[0].ToString() });
            }

            if (Value != "")
            {
                string[] values = Value.Split(',');
                int vLength = values.Length;
                if (vLength > 1)
                {
                    for (int vnum = 0; vnum < vLength; vnum++)
                    {
                        foreach (var item in dropdownList)
                        {
                            if (values[vnum] == item.Value)
                            {
                                item.Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in dropdownList)
                    {
                        if (Value == item.Value)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            else
            {

                foreach (var item in dropdownList)
                {
                    if (item.Value == "0")
                    {
                        item.Selected = true;
                    }
                }
            }
            return dropdownList;
        }

        public static void GetClinicInfoDetail(ServicesSoapClient _ServicesSoapClient, ref ClinicInfoDetailViewModel ei, string PatientId, string keycode)
        {
            string[] s = keycode.Split(new char[] { '|' });
            string type = s[0];
            string vid = s[1];
            string date = s[2];
            string date1 = date.Substring(0, 10) + " " + date.Substring(10, 8);
            ei.UserId = PatientId;
            ei.NowType = type;
            ei.VisitId = vid;
            ServiceReference.PatientBasicInfo zn = _ServicesSoapClient.GetPatBasicInfo(PatientId);  //获取基本信息
            ei.UserName = zn.UserName;

            string DateShow = Convert.ToDateTime(date1).ToString("yyyy年MM月dd日");
            ei.NowDate = DateShow;

            ei.DiagnosisInfoTable = new DataTable();
            ei.ExaminationInfoTable = new DataTable();
            ei.LabTestInfoTable = new DataTable();
            ei.DrugRecordTable = new DataTable();

            DataSet set = new DataSet();
            set = _ServicesSoapClient.GetClinicInfoDetail(PatientId, type, vid, date);

            switch (type)
            {
                //case "ClinicalInfo": ; //就诊表 
                //break;
                case "DiagnosisInfo": ei.DiagnosisInfoTable = set.Tables[0]; //诊断表                                      
                    break;
                case "ExaminationInfo": ei.ExaminationInfoTable = set.Tables[0]; //检查表（有子表）
                    break;
                case "LabTestInfo": ei.LabTestInfoTable = set.Tables[0]; //化验表（有子表）
                    break;
                case "DrugRecord": ei.DrugRecordTable = set.Tables[0]; //用药
                    break;
                default: break;
            }
        }

        public static void GetClinicInfoDetailByType(ServicesSoapClient _ServicesSoapClient, ref ClinicInfoDetailByTypeViewModel ei, string PatientId, string vid, string type, string sortno, string itemcode)
        {
            ei.NowDetailType = type;
            ei.ExamDetailsTable = new DataTable();
            ei.LabTestDetailsTable = new DataTable();

            #region table初始化

            #endregion

            if (type == "ExamDetails")
            {
                //DataSet ExamDetailsset = _ServicesSoapClient.GetExamDtlList(PatientId, vid, sortno, itemcode); //检查子表 
                //ei.ExamDetailsTable = ExamDetailsset.Tables[0];
            }
            else
            {
                DataSet LabTestDetailsset = _ServicesSoapClient.GetLabTestDtlList(PatientId, vid, sortno); //化验子表 
                ei.LabTestDetailsTable = LabTestDetailsset.Tables[0];
            }
        }

        public static List<Picture> GetPictureList(ServicesSoapClient _ServicesSoapClient, string PatientId, string Itemcode)
        {
            string[] s = Itemcode.Split(new char[] { '_' });
            string ItemType = s[0];

            DataSet set = new DataSet();
            set = _ServicesSoapClient.GetPatientVitalSignsAndThreshold(PatientId, ItemType, Itemcode);

            //set包含：RecordDate、RecordTime、Value、Unit、ThreholdMin、ThreholdMax

            List<Picture> PictureList = new List<Picture>();
            if (set != null)
            {
                foreach (System.Data.DataRow row in set.Tables[0].Rows)
                {
                    Picture Picture = new Picture();
                    Picture.date = row[0].ToString() + " " + row[1].ToString();
                    Picture.duration = Convert.ToDecimal(row[2]);
                    Picture.unit = row[3].ToString();
                    Picture.min = Convert.ToDecimal(row[4]);
                    Picture.max = Convert.ToDecimal(row[5]);

                    if ((Picture.duration >= Picture.min) && (Picture.duration <= Picture.max))
                    {
                        Picture.lineColor = "#b7e021";  //正常黄色
                    }
                    else
                    {
                        Picture.lineColor = "#FF2D2D";  //不正常红色
                    }

                    PictureList.Add(Picture);
                    //Picture.date = "2014-12-12 08:00";
                    //Picture.duration=101m;
                    //Picture.min =80m;
                }
            }
            return PictureList;
        }

        //public static bool SetPatientDetailInfoEdit(ServicesSoapClient _ServicesSoapClient, PatientDetailInfoViewModel ei, FormCollection formCollection)
        //{

        //    return true;
        //}

    }
    #endregion

}