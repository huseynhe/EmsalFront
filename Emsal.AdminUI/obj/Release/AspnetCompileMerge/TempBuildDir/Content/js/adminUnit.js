$(function(){
    var parId = 0;
    var isSelectedParentId = 0;

    $('#adminUnitCheckEdit').click(function () {
        if (this.checked) {
            $('#adminUnitName').val(name);
            $('#adminUnitDescription').val(description);
            $('#AdminUnitHeaderGroup').hide();
        }
        else {
            $('#adminUnitName').val('');
            $('#adminUnitDescription').val('');
            $('#AdminUnitHeaderGroup').show();
        }
    });

    $("#footerAdminUnit").click(function () {
        AddAdminUnit();
    })

    function setAdminUnit(elem, AdminUnittId, AdminUnitParentId, AdminUnitName, AdminUnitDescription) {
        isSelectedParentId = AdminUnitParentId;
        parId = AdminUnittId;
        name = AdminUnitName;
        description = AdminUnitDescription;
        $('#AdminUnitHeader').html($(elem).html());
        $("#AdminUnitTree").find('.btn-info').removeClass("btn-info btn-xs");
        $(elem).addClass("btn-info btn-xs");
    }



    function AddAdminUnit() {
        adminUnitName = $('#adminUnitName').val();
        adminUnitDescription = $('#adminUnitDescription').val();
        enumValueID = $("#enumSelect").val();
        if ($.trim(adminUnitName).length < 3) {
            $('#dadminUnitName').html('Admin unit daxil edilməyib');
            return false;
        }

        if (document.getElementById('adminUnitCheckEdit').checked) {
            $.ajax({
                url: '/AdminUnit/UpdateAdminUnit',
                type: 'GET',
                data: { Id: parId, ParentID: isSelectedParentId, Name: adminUnitName, Description: adminUnitDescription, EnumValueId: enumValueID },
                success: function (result) {
                    console.log("enumeee",enumValueID);
                    $('#productName').val('');
                    $('#productDescription').val('');
                    location.reload();
                },
                error: function () {

                }
            });
        }
        else {
            $.ajax({
                url: '/AdminUnit/AddAdminUnit',
                type: 'GET',
                data: { adminUnitName: adminUnitName, adminUnitDescription: adminUnitDescription, adminUnitParentID: parId, enumValueID: enumValueID },
                success: function (result) {

                    $('#adminUnitName').val('');
                    $('#adminUnitDescription').val('');
                    location.reload();
                },
                error: function () {

                }
            });
        }
    }

    //function getAdminUnitsByparentId(parentId) {
    //    $.ajax({
    //        type: "GET",
    //        url: "/AdminUnit/GetAdminUnitsByParentId?parentId=" + parentId,
    //        success: function (result) {
    //            result.map(function (item) {
    //                if (item.ParentID == 0) {
    //                    $("#treeAdminUnit").append("<li id =ad" + item.Id + "><i class='indicator glyphicon glyphicon-chevron-down' id = adi" + item.Id + "></i><span id =ads" + item.Id + ">" + item.Name + "</span></li>");
    //                    getAdminUnitsByparentId(item.Id);
    //                }
    //                else if (item.ParentID == parentId) {
    //                    $("#ad" + parentId).append("<ul id=adp" + parentId + " style='display:none'></ul>");
    //                    $("#adp" + parentId).append("<li id =ad" + item.Id + ">" + "<i class='indicator glyphicon glyphicon-chevron-down' id = adi" + item.Id + "></i><span id =ads" + item.Id + ">" + item.Name + "</span>" + "</li>");
    //                    getAdminUnitsByparentId(item.Id);
    //                }
    //            });

    //            result.map(function (item) {
    //                $("#ads" + item.Id).click(function () {
    //                    setAdminUnit("#ads" + item.Id, item.Id, item.ParentID, item.Name, item.Description);
    //                })
    //                $("#adi" + item.Id).click(function () {
    //                    if ($("#adp" + item.Id).css("display") == "none") {
    //                        $("#adi" + item.Id).removeClass("glyphicon glyphicon-chevron-down");
    //                        $("#adi" + item.Id).addClass("glyphicon glyphicon-chevron-right");
    //                        $("#adp" + item.Id).css("display", "block");
    //                    }
    //                    else {
    //                        $("#adp" + item.Id).css("display", "none");
    //                        $("#adi" + item.Id).removeClass("glyphicon glyphicon-chevron-right");
    //                        $("#adi" + item.Id).addClass("glyphicon glyphicon-chevron-down");
    //                    }
    //                })
    //            })
    //        },
    //        error: function (e) {
    //            console.log(e);
    //        }
    //    })
    //}

    $("#first").click(function () {
        setAdminUnit("#first", 0, '', '', '')
    })
    //getAdminUnitsByparentId(0);
})
