using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDMIS.Models
{
    public class HealthEducation
    {
        public string Module { get; set; }
        public string ModuleName { get; set; }
        public string Id { get; set; }
        public int Type { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string CreateDateTime { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        [AllowHtml]
        public string htmlContent { get; set; }
    }
}