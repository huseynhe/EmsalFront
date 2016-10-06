using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class ProductionDocumentViewModel : UserInfoViewModel
    {
        public int fileSize = 2097152;
        public string fileDirectory = @"C:\inetpub\emsalfiles";
        public string tempFileDirectoryFV = @"/Content/tempFile/";
        public string tempFileDirectory = HttpContext.Current.Server.MapPath("~/Content/tempFile/");
        //public string tempFileDirectory = @"D:\workspace\vs\PYAS\PYAS.UI\Content\tempFile";
        public List<string> cotentTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };
        public long totalSize = 0;
        public string fileTypes = ".pdf, .jpeg, .jpg, .png";

        public tblProduction_Document ProductionDocument;
        public tblProduction_Document[] ProductionDocumentArray;
        public List<tblProduction_Document> ProductionDocumentList { get; set; }

        public tblAnnouncement Announcement;
        public tblAnnouncement AnnouncementOUT;
        public tblAnnouncement[] AnnouncementArray;
        public List<tblAnnouncement> AnnouncementList { get; set; }

        public string FCType { get; set; }

    }
}