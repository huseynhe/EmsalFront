using Emsal.Utility.CustomObjects;
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
        //public int fileSize = 2097152;
        //public string fileDirectory = @"C:\inetpub\emsalfiles";
        //public string tempFileDirectoryFV = @"/Content/tempFile/";
        //public string tempFileDirectory = HttpContext.Current.Server.MapPath("~/Content/tempFile/");
        //public List<string> cotentTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };
        //public long totalSize = 0;
        //public string fileTypes = ".pdf, .jpeg, .jpg, .png";

        public int fileSize = FileExtension.fileSize;
        public string fileDirectory = FileExtension.fileDirectoryExternal;
        public string tempFileDirectoryFV = FileExtension.tempFileDirectoryFV;
        public string tempFileDirectory = HttpContext.Current.Server.MapPath(FileExtension.fileDirectoryTempFile);
        public List<string> cotentTypes = FileExtension.fileMimeTypes;
        public long totalSize = 0;
        public string fileTypes = FileExtension.fileTypes;

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