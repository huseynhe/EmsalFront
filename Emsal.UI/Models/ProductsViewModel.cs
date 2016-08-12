using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emsal.WebInt.EmsalSrv;

namespace Emsal.UI.Models
{
    public class ProductsViewModel : UserRoles
    {
        public tblOffer_Production OfferProduction;
        public tblPotential_Production PotentialProduction;
        public tblProduction_Document ProductionDocument;
        public tblDemand_Production DemandProduction;
        public tblProductCatalog ProductCatalog;
        public tblProductionControl ProductionControl;

        public string quantity;


        public tblOffer_Production[] OfferProductionArray;
        public tblPotential_Production[] PotentialProductionArray;
        public tblDemand_Production[] DemandProductionArray;
        public tblProductCatalog[] ProductCatalogArray;
        public tblProduction_Document[] ProductionDocumentArray;
        public tblEnumValue[] EnumValueArray;
        public tblProductionControl[] ProductionControlArray;

        public List<tblOffer_Production> OfferProductionList { get; set; }
        public List<tblPotential_Production> PotentialProductionList { get; set; }
        public List<tblDemand_Production> DemandProductionList { get; set; }
        public List<tblProductCatalog> ProductCatalogList { get; set; }
        public List<tblProductionControl> ProductionControlList { get; set; }
        public List<tblProduction_Document> ProductionDocumentList { get; set; }
        public List<tblEnumValue> EnumValueList { get; set; }

        public tblEnumValue EnumVal;
        public tblEnumCategory EnumCat;

    }
}