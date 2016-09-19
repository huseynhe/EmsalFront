using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Emsal.WebInt
{
    public class EmsalService
    {

        private static Emsal.WebInt.EmsalSrv.EmsalService _emsalService;


        public static Emsal.WebInt.EmsalSrv.EmsalService emsalService
        {
            get
            {
                if (_emsalService == null)
                {
                    _emsalService = new Emsal.WebInt.EmsalSrv.EmsalService();
                    // _emsalService.SWSessionInfo = new SWSessionInfo();

                    if (ConfigurationManager.AppSettings["Emsal_WebInt_EmsalSrv_EmsalService"] != null)
                        _emsalService.Url = ConfigurationManager.AppSettings["Emsal_WebInt_EmsalSrv_EmsalService"];

                }
                return _emsalService;
            }
        }

        //private static Emsal.WebInt.IAMAS.Service1 _iamasService;


        //public static Emsal.WebInt.IAMAS.Service1 iamasService
        //{
        //    get
        //    {
        //        if (_iamasService == null)
        //        {
        //            _iamasService = new Emsal.WebInt.IAMAS.Service1();
        //            // _emsalService.SWSessionInfo = new SWSessionInfo();

        //            if (ConfigurationManager.AppSettings["Emsal_WebInt_IAMAS_Service1"] != null)
        //                _iamasService.Url = ConfigurationManager.AppSettings["Emsal_WebInt_IAMAS_Service1"];

        //        }
        //        return _iamasService;
        //    }
        //}


    }
}
