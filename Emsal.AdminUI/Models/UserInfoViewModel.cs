using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public abstract class UserInfoViewModel
    {
        public tblUser Admin;
        public tblUserRole[] userRole;
    }
}