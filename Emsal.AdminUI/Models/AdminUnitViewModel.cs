using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class AdminUnitViewModel : UserInfoViewModel
    {
        public tblPRM_AdminUnit AdminUnit;
        public tblPRM_AdminUnit[] AdminUnitArray;
        public List<tblPRM_AdminUnit> AdminUnitList { get; set; }

        public tblEnumValue EnumValue;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;

        public tblEnumCategory EnumCategory;
    }
}