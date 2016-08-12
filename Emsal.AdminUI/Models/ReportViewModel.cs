using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class ReportViewModel : UserInfoViewModel
    {
        public tblUser User;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public tblProductCatalog[] ProductCatalogArray;


        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblProduct_Document ProductDocument;
        public tblProduct_Document ProductDocumentFile;
        public IList<tblProduct_Document> ProductDocumentList { get; set; }
        public tblProduct_Document[] ProductDocumentArray;
        public tblProduct_Document[] ProductDocumentArrayFile;

        public tblEnumValue EnumValue;
        public tblEnumValue EnumValueFP;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;

        public tblProductCatalogControl ProductCatalogControl;
        public IList<tblProductCatalogControl> ProductCatalogControlList { get; set; }
        public tblProductCatalogControl[] ProductCatalogControlArray;


        public DemandOfferDetail DemandOfferDetail;
        public IList<DemandOfferDetail> DemandOfferDetailList { get; set; }
        public IList<ReportDonut> ReportDonutList { get; set; }
        public ReportDonut ReportDonut;
        public DemandOfferDetail[] DemandOfferDetailArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;

        public PotentialClientDetail PotentialClientDetail;
        public IList<PotentialClientDetail> PotentialClientDetailList { get; set; }
        public PotentialClientDetail[] PotentialClientDetailArray;

        public string fullAddress { get; set; }


        public long[] demands { get; set; }
        public string strDemand { get; set; }
        public long[] offers { get; set; }
        public string strOffer { get; set; }
        public string[] products { get; set; }
        public string strProducts { get; set; }
    }


    public class ReportDonut
    {
        public string label { get; set; }
        public long value { get; set; }
    }
}