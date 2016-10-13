using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class AnnouncementViewModel : UserInfoViewModel
    {
        public tblPRM_AdminUnit AdminUnit;
        public tblPRM_AdminUnit[] AdminUnitArray;
        public List<tblPRM_AdminUnit> AdminUnitList { get; set; }

        public tblDemand_Production DemandProduction;
        public tblDemand_Production[] DemandProductionArray;
        public List<tblDemand_Production> DemandProductionList { get; set; }

        public tblAnnouncement Announcement;
        public tblAnnouncement AnnouncementOUT;
        public tblAnnouncement[] AnnouncementArray;
        public List<tblAnnouncement> AnnouncementList { get; set; }

        public AnnouncementDetail AnnouncementDetail;
        public AnnouncementDetail[] AnnouncementDetailArray;
        public List<AnnouncementDetail> AnnouncementDetailList { get; set; }

        public tblEnumValue EnumValue;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;

        public tblEnumCategory EnumCategory;


        public DemanProductionGroup DemanProductionGroup;
        public IList<DemanProductionGroup> DemanProductionGroupList { get; set; }
        public DemanProductionGroup[] DemanProductionGroupArray;


        public PagedList.IPagedList<AnnouncementDetail> Paging { get; set; }

        [Display(Name = "Məhsulun adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string productName { get; set; }

        [Display(Name = "Həcmi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string totalQuantity { get; set; }

        [Display(Name = "Ölçü vahidi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string quantityName { get; set; }

        [Display(Name = "Qiyməti")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string unitPrice { get; set; }

        [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime? startDate { get; set; }


        [Display(Name = "Bitmə tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime? endDate { get; set; }


        [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime[] arrayStartDate { get; set; }


        [Display(Name = "Bitmə tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime[] arrayEndDate { get; set; }

        public int approv { get; set; }
        public string pname { get; set; }
    }
}