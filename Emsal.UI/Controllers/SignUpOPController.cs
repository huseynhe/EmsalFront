using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.Utility.UtilityObjects;
using Emsal.WebInt.EmsalSrv;
using Emsal.WebInt.IAMAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    public class SignUpOPController : Controller
    {
        //
        // GET: /SignUpOP/

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private static string fullAddressId = "";
        private static string addressDesc = "";
        private static string fin = "";
        private static string voen = "";
        private BaseInput baseInput;
        UserViewModel modelUser;

        public ActionResult Index(string fin = null)
        {
            try
            {
                baseInput = new BaseInput();
                modelUser = new UserViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelUser.User);
                baseInput.userName = modelUser.User.Username;

                modelUser.Person = new tblPerson();
                modelUser.Person=GetPhysicalPerson(fin);
                modelUser.birtday=(modelUser.Person.birtday).toShortDate().Date.ToString();

                return View(modelUser);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }


        public tblPerson GetPhysicalPerson(string fin)
        {
            try
            {
                baseInput = new BaseInput();
                modelUser = new UserViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelUser.User);
                baseInput.userName = modelUser.User.Username;
                

                SingleServiceControl srvcontrol = new SingleServiceControl();
                getPersonalInfoByPinNewResponseResponse iamasPerson;
                tblPerson person;

                int control = srvcontrol.getPersonInfoByPin(fin, out person, out iamasPerson);

                long auid = 0;

                modelUser.Person = new tblPerson();

                if (person != null)
                {
                    modelUser.Person.Name = person.Name;
                    modelUser.Person.Surname = person.Surname;
                    modelUser.Person.FatherName = person.FatherName;
                    modelUser.Person.createdUser = person.profilePicture;
                    modelUser.Person.gender = person.gender;
                    modelUser.Person.birtday = person.birtday;

                    modelUser.Person.profilePicture = Convert.ToBase64String(StringExtension.StringToByteArray(person.profilePicture));

                    BaseOutput gabui = srv.WS_GetAddressesByUserId(baseInput, (long)person.UserId, true, out modelUser.AddressArray);

                    modelUser.Address = modelUser.AddressArray.ToList().FirstOrDefault();
                    auid = (long)modelUser.Address.adminUnit_Id;
                    addressDesc = modelUser.Address.addressDesc;


                }
                else if (iamasPerson.Name != null)
                {
                    if (iamasPerson.Adress.cityId != null)
                    {
                        if (iamasPerson.Adress.cityId != "0")
                        {
                            BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.cityId), true, true, true, out modelUser.PRMAdminUnit);
                        }
                    }

                    if (iamasPerson.Adress.districtId != null)
                    {
                        if (iamasPerson.Adress.districtId != "0")
                        {
                            BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.districtId), true, false, true, out modelUser.PRMAdminUnit);
                        }
                    }

                    auid = modelUser.PRMAdminUnit.Id;


                    addressDesc = "";

                    if (!String.IsNullOrEmpty(iamasPerson.Adress.place))
                        addressDesc += iamasPerson.Adress.place;

                    if (!String.IsNullOrEmpty(iamasPerson.Adress.street))
                        addressDesc += ", " + iamasPerson.Adress.street;

                    if (!String.IsNullOrEmpty(iamasPerson.Adress.building))
                        addressDesc += ", " + iamasPerson.Adress.building;

                    if (!String.IsNullOrEmpty(iamasPerson.Adress.block))
                        addressDesc += ", " + iamasPerson.Adress.block;

                    if (!String.IsNullOrEmpty(iamasPerson.Adress.apartment))
                        addressDesc += ", " + iamasPerson.Adress.apartment;

                    //addressDesc = iamasPerson.Adress.place + ", " + iamasPerson.Adress.street + ", " + iamasPerson.Adress.apartment + ", " + iamasPerson.Adress.block + ", " + iamasPerson.Adress.building;

                    modelUser.Person.Name = iamasPerson.Name;
                    modelUser.Person.Surname = iamasPerson.Surname;
                    string[] pa = iamasPerson.Patronymic.Split(' ').ToArray();
                    modelUser.Person.FatherName = pa[0];
                    modelUser.Person.gender = iamasPerson.gender;
                    modelUser.Person.birtday = (DateTime.Parse(iamasPerson.birthDate)).getInt64ShortDate();
                    modelUser.Person.createdUser = string.Join(",", iamasPerson.photo);
                    modelUser.Person.profilePicture = Convert.ToBase64String(iamasPerson.photo);
                }


                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, auid, true, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();

                fullAddressId = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Id));

                return modelUser.Person;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
