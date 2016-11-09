using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDMIS.Models;

namespace CDMIS.ViewModels
{
    public class HealthEducationList
    {
        public string selectedModuleId { get; set; }
        public List<SelectListItem> ModuleList { get; set; }
        //{
        //    return CommonVariables.GetModuleList();
        //}

        public List<HealthEducation> HEList { get; set; }

        public HealthEducationList()
        {
            selectedModuleId = "";
            HEList = new List<HealthEducation>();
        }
    }

    public class NewHealthEducationFile
    {
        public string selectedModuleId { get; set; }
        public List<SelectListItem> ModuleList { get; set; }

        public HealthEducation news { get; set; }
        public NewHealthEducationFile()
        {
            selectedModuleId = "";
            news = new HealthEducation();
        }
    }
}