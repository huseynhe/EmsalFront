using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    public class CreatedUsersController : Controller
    {
        //
        // GET: /CreatedUsers/
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private User modelUser;
        public ActionResult Index(int? page, long?UserId)
        {
            CreatedUser created;

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            long? userId = null;
            modelUser = new User();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelUser.FutureUser);
            modelUser.UserName = modelUser.FutureUser.Username;
                
            BaseOutput users = srv.WS_GetUsers(baseInput, out modelUser.UserArray);
            modelUser.UserList = modelUser.UserArray.Where(x => (x.createdUser == modelUser.FutureUser.Username)).ToList();

            modelUser.CreatedUserList = new List<CreatedUser>();
            foreach (var item in modelUser.UserList)
            {
                created = new CreatedUser();
                baseInput = new BaseInput();
                
                BaseOutput usertypeOut = srv.WS_GetEnumValueById(baseInput, (long)item.userType_eV_ID, true, out modelUser.EnumValue);

                if(modelUser.EnumValue.name == "fizikişexs")
                {
                    BaseOutput personOut = srv.WS_GetPersonByUserId(baseInput, item.Id, true, out modelUser.FuturePerson);
                    if(modelUser.FuturePerson != null)
                    {
                        created.Name = modelUser.FuturePerson.Name;
                        created.Surname = modelUser.FuturePerson.Surname;
                        created.FatherName = modelUser.FuturePerson.FatherName;
                        created.UserType = "Fiziki Şəxs";
                    }

                    //get the full address
                    if(modelUser.FuturePerson.address_Id != null)
                    {
                        BaseOutput addressout = srv.WS_GetAddressById(baseInput, (long)modelUser.FuturePerson.address_Id, true, out modelUser.FutureAddress);
                        BaseOutput fulladdressListOut = srv.WS_GetAdminUnitListForID(baseInput, (long)modelUser.FutureAddress.adminUnit_Id, true, out modelUser.PRMAdminUnitArray);

                        foreach (var adminunit in modelUser.PRMAdminUnitArray)
                        {
                            created.FullAddress += adminunit.Name + ",";
                        }
                        created.FullAddress = created.FullAddress.Remove(created.FullAddress.Length - 1);
                    }

                }
                if(modelUser.EnumValue.name == "legalPerson")
                {
                    BaseOutput orgOut = srv.WS_GetForeign_OrganizationByUserId(baseInput, item.Id, true, out modelUser.ForeignOrganisation);
                    if(modelUser.ForeignOrganisation != null)
                    {
                        created.Name = modelUser.ForeignOrganisation.name;
                        created.UserType = "Hüquqi Şəxs";
                    }

                    //get the full address
                    if(modelUser.ForeignOrganisation.address_Id != null)
                    {
                        BaseOutput addressout = srv.WS_GetAddressById(baseInput, (long)modelUser.ForeignOrganisation.address_Id, true, out modelUser.FutureAddress);
                        BaseOutput fulladdressListOut = srv.WS_GetAdminUnitListForID(baseInput, (long)modelUser.FutureAddress.adminUnit_Id, true, out modelUser.PRMAdminUnitArray);

                        foreach (var adminunit in modelUser.PRMAdminUnitArray)
                        {
                            created.FullAddress += adminunit.Name + ",";
                        }
                        created.FullAddress = created.FullAddress.Remove(created.FullAddress.Length - 1);
                    }
                   
                }

                created.Id = item.Id;
                BaseOutput rolesOut = srv.WS_GetUserRolesByUserId(baseInput, (long)item.Id, true, out modelUser.UserRolesArray);

                if(modelUser.UserRolesArray.Length == 0)
                {
                    created.Role = "İstifadəçiyə rol verilməyib";
                }
                else
                {
                    foreach (var items in modelUser.UserRolesArray)
                    {
                        BaseOutput roleOut = srv.WS_GetRoleById(baseInput, items.RoleId, true, out modelUser.Role);
                        if (modelUser.Role.Name == "producerPerson")
                        {
                            created.Role = "İstehsalçı";
                        }
                        if (modelUser.Role.Name == "sellerPerson")
                        {
                            created.Role = "Satıcı";
                        }
                    }
                }
               
                

                modelUser.CreatedUserList.Add(created);

            }
            modelUser.PagingCreatedUsers = modelUser.CreatedUserList.ToPagedList(pageNumber, pageSize);



            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelUser)
                : View(modelUser);
        }

    }
}
