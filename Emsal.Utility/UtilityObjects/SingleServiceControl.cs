using Emsal.WebInt.EmsalSrv;
using Emsal.WebInt.IAMAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emsal.Utility.UtilityObjects
{
    public class SingleServiceControl
    {

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;


        public int getPersonInfoByPin(string pin, out tblPerson person, out getPersonalInfoByPinNewResponseResponse imasPerson)
        {

            BaseInput input = new BaseInput();
            person = null;
            imasPerson = null;
            BaseOutput output = srv.WS_GetPersonByPinNumber(input, pin + "1", out person);
            if (person != null)
            {
                return 1;
            }

            else if (person == null)
            {
                imasPerson = iamasSrv.getPersonalInfoByPinNew(pin, "1");
                return 2;
            }
            else
            {
                return 0;
            }

        }


    }
}
