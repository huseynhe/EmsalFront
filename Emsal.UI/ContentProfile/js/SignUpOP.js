function supplierType() {
    var sup = $("#supplierTypyId").val();
    $("#suplierType").val(sup);

    if (sup > 0) {
        document.getElementById("personTypeDiv").style.display = "block"
        document.getElementById("legalFizikiDiv").style.display = "block"
    }
    else {
        document.getElementById("personTypeDiv").style.display = "none"
        document.getElementById("legalFizikiDiv").style.display = "none"
        document.getElementById("buttonDiv").style.display = "none"

        $("#personType").val("").attr("selected",true).change();
    }
}

function PersonType() {
    var id = document.getElementById("personType").value;
    if (id == 1) {
        $("#voen").val('');

        $('#formBody').hide();

        document.getElementById("picture").style.display = "none";
        document.getElementById("fizikiShexs").style.display = "block";
        document.getElementById("fizikiShexs").style.display = "block";
        document.getElementById("emailDiv").style.display = "block";
        document.getElementById("userNameDiv").style.display = "block";
        document.getElementById("mobilDiv").style.display = "block";
        document.getElementById("genderDiv").style.display = "block";
        document.getElementById("birtdayDiv").style.display = "block";
        document.getElementById("educDiv").style.display = "block";
        document.getElementById("jobDiv").style.display = "block";
        document.getElementById("huquqiShexs").style.display = "none";
        document.getElementById("huquqiShexsName").style.display = "none";
        document.getElementById("buttonDiv").style.display = "block"
        $("#cechkButton").attr("disabled", false);

    }

    else if (id == 2) {
        $("#fin").val('');

        $('#formBody').hide();

        document.getElementById("picture").style.display = "none";
        document.getElementById("fizikiShexs").style.display = "none";
        document.getElementById("emailDiv").style.display = "block";
        document.getElementById("userNameDiv").style.display = "block";
        document.getElementById("mobilDiv").style.display = "block";
        document.getElementById("genderDiv").style.display = "block";
        document.getElementById("birtdayDiv").style.display = "block";
        document.getElementById("educDiv").style.display = "block";
        document.getElementById("jobDiv").style.display = "block";
        document.getElementById("huquqiShexs").style.display = "block";
        document.getElementById("buttonDiv").style.display = "block"
        document.getElementById("huquqiShexsName").style.display = "block";
        $("#cechkButton").attr("disabled", false);
    }

    if (id == "") {
        document.getElementById("fizikiShexs").style.display = "none";
        document.getElementById("huquqiShexs").style.display = "none";
        document.getElementById("picture").style.display = "none";
        document.getElementById("buttonDiv").style.display = "none"
        $("#cechkButton").attr("disabled", true);
        $('#formBody').hide();
    }
};

var str = "";

var ai = 0;
var res = "";

function check() {

    $('#formBody').find('input:text').val(null);
    $('#formBody').find('input:password').val(null);
    $('#eMail').val("");
    $('#formBody').find('select').val("").change();
    document.getElementById("picture").src = "";

    var sup = $("#supplierTypyId").val();

    $("addressId").val(null);
    ai = 0;
    var id = $("#personType").val();
    var chekId;
    if (id == 1) {
        chekId = $("#fin").val();
    }
    else if (id == 2) {
        chekId = $("#voen").val();
    }

    //if (chekId != "") {
    $.ajax({
        url: '/SignUpOP/Check?pId=' + chekId + "&type=" + id + "&supType=" + sup,
        type: 'POST',
        datatype: 'json',
        success: function (result) {
            $('#formBody').hide();
            if (result.message != null) {
                alert(result.message);
                $('#formBody').hide();

                window.setTimeout(function () {
                    window.location.href = '/Login';
                }, 2000);
                $('#formBody').hide();
            } else
            if (result.data == null) {
                alert('FİN və ya VÖEN doğru deyil');
                if (window.location.search.indexOf('uid') > -1 && window.location.search.indexOf('type')) {
                    $('#formBody').hide();
                    window.setTimeout(function () {
                        window.location.href = '/Login';
                    }, 1000);
                    $('#formBody').hide();
                }
            } else {

                var regCombo;
                $('#regionContainer').append(regCombo);
                //if (result.data.length > 0) {
                if (result.data.Person.Name != null) {
                    $("#Name").val(result.data.Person.Name);
                    $("#Surname").val(result.data.Person.Surname);
                    $("#FatherName").val(result.data.Person.FatherName);
                    $('#gender').val(result.data.Person.gender).change();
                    $("#descAddress").val(result.data.descAddress);
                    $("#birtday").val(result.data.birtday);

                    if (result.data.profilePicture != null) {
                        document.getElementById("picture").style.display = "block";
                        elem = $('#createdUser');
                        $(elem).val(result.data.createdUser);
                        if (result.data.profilePicture != null) {
                            document.getElementById("picture").src = "data:image/png;base64," + result.data.profilePicture;
                            $('#picture').show();
                        }
                    }
                    str = result.data.FullAddress;
                    res = str.split(",");
                    if (res != '') {
                        for (var i = 0; i < res.length; i++) {
                            if (i == 0) {
                                $("#mainRegion").val(res[i]).change();
                                $("#mainRegion").attr('disabled', true);
                            }

                        }
                    }

                    if (id == 2) {
                        $("#legalPersonName").val(result.data.legalPersonName);
                        if ($("#legalPersonName").val() != "") {
                            $("#legalPersonName").attr('readonly', true);
                        }
                    }

                    if ($("#Name").val() != "") {
                        $("#Name").attr('readonly', true);
                    }

                    if ($("#Surname").val() != "") {
                        $("#Surname").attr('readonly', true);
                    }

                    if ($("#FatherName").val() != "") {
                    $("#FatherName").attr('readonly', true);
                    }
                    //$('#gender').attr('disabled', true);
                    //$("#birtday").attr('disabled', true);

                    $('#formBody').show();
                }
                else {
                    $("#birtday").val(result.data.birtday);

                    $("#Name").attr('readonly', false);
                    $("#Surname").attr('readonly', false);
                    $("#FatherName").attr('readonly', false);

                    $('#formBody').hide();
                    //$('#gender').attr('disabled', false);
                    //$("#birtday").attr('disabled', false);
                }

                //}

                
            }
            
        },
        error: function () {

        }
    });
    //}
}

$(document).ready(function () {

    var url = window.location.href;
    var uid = ""; type = "";

    if (window.location.search.indexOf('uid') > -1 && window.location.search.indexOf('type')) {
        uid = /uid=([^&]+)/.exec(url)[1];
        type = /type=([^&]+)/.exec(url)[1];
    }

    if (type > 0 && uid > 0) {
        $("#supplierTypyId").attr("disabled", true);
        document.getElementById("personTypeDiv").style.display = "block"
        document.getElementById("legalFizikiDiv").style.display = "block"
    }
    else {
        $("#supplierTypyId").attr("disabled", false);
        document.getElementById("personTypeDiv").style.display = "none"
        document.getElementById("legalFizikiDiv").style.display = "none"
        document.getElementById("buttonDiv").style.display = "none"
    }

    $(".numbersOnly").on("keyup", function () {
        this.value = this.value.replace(/[^0-9\.]/g, '');

        if (this.value.length > 7) {
            this.value = this.value.slice(0, 7);
        }
    })


    $('.datepicker, input[type=datetime]').datepicker({
        format: 'dd.mm.yyyy',
        autoclose: true,
        "setDate": new Date()
    })
    .attr('readonly', 'readonly')
    .on('changeDate', function (ev) {
        (ev.viewMode == 'days') ? $(this).datepicker('hide') : '';
    });
    //.on('changeDate', function (e) {
    //    $(this).datepicker('hide');
    //});

    document.getElementById("picture").style.display = "none";
    $("#cechkButton").attr("disabled", true);

    $('#formBody').hide();

    if ($('#pType').val() != "") {
        $('#personType').val($('#pType').val());
        $('#personType').change();
    }
});

function GetAdminUnit(elem) {
    aId = $(elem).val();
    if (aId != "") {
        $('#addressId').val(aId);
    }

    $(elem).parent().nextAll().remove();

    //var valu = 0;
    //if (aId == '') {
    //    var name = $(elem).attr('name');
    //    var i = name.substring(5, name.length - 1);
    //    var val = 0;

    //    for (var d = 0; d < i; d++) {
    //        val = $('select[name="adId[' + d + ']"]').val();

    //        if (val != undefined) {
    //            valu = $('select[name="adId[' + d + ']"]').val();
    //        }
    //    }

    //    //$('#addressId').val(valu);
    //    aId = valu;
    //}

    if (aId > 0) {
        $.ajax({
            url: '/SignUpOP/FillRelations?pId=' + aId,
            type: 'POST',
            datatype: 'json',
            data: 'data',
            success: function (result) {
                var regCombo = "";
                $('#regionContainer').append(regCombo);
                if (result.data.length > 0) {
                    regCombo += "<div class='col-md-3'> <select id='" + "adId" + result.data[0].parentId + "' onchange='GetAdminUnit(this)' class='form-control select2'>"
                    regCombo += " <option value=''>Seçim edin</option>"
                    for (var i = 0; i < result.data.length; i++) {
                        regCombo += " <option value='" + result.data[i].id + "'> " + result.data[i].name + " </option>"
                    }
                    regCombo += "</div> </select> "
                    //regCombo += "<span data-valmsg-replace='true' data-valmsg-for=adId[" + result.data[0].parentId + "]  class='field-validation-valid text-danger'></span>"
                    $('#regionContainer').append(regCombo);
                }




                //if (ai > 0) {

                $("#adId" + res[ai] + "").val(res[ai + 1]).change();

                if ($("#adId" + res[ai] + "").val() != '')
                {                    
                    $("#adId" + res[ai] + "").attr('disabled', true);
                }


                //if (result.data != null)
                //{
                //    $("#mainRegion").attr('disabled', true);
                //}

                //for ( y = 2; y < s; y++) {
                //    y++;
                //    alert(y);
                //    alert(res[ai]);
                //    $("#adId" + res[ai] + "").attr('disabled', true);
                //}
   
                //}

                ai = ai + 1;

                $('.select2').select2();
            },
            error: function () {

            }
        });
    }
};

var ri = 0;

function SaveChanges() {
    var form = document.getElementById('signUpDiv');

    var user = $("#userName").val();
    var perefix = $("#mPerefix").val();
    var mobile = $("#mNumber").val();

    if ($('#signUpDiv').validate().form()) {

        if (ri == 0) {

            ri = 1;
            $.ajax({
                url: '/SignUpOP/CheckForExistence?userName=' + user + "&perefix=" + perefix + "&mobile=" + mobile,
                type: 'POST',
                datatype: 'json',
                data: 'data',
                success: function (result) {
                    if (result.err == null && result.suc == null) {
                        alert('FİN və ya VÖEN doğru deyil');
                        window.setTimeout(function () {
                            window.location.href = '/Login';
                        }, 5000);
                    } else {
                        var regCombo;
                        $('#regionContainer').append(regCombo);
                        //if (result.data.length > 0) {
                        if (result.err != "") {
                            document.getElementById("warningDiv").style.display = "block";
                        }

                        if (result.suc != "") {
                            document.getElementById("warningDiv").style.display = "none";

                            form.submit();
                        }
                    }
                },
                error: function () {

                }
            });
        }
        //alert("Submit");
    }

}


//function check(){
//    var id = document.getElementById("personType").value;
//    var chekId;
//    if (id == 0)
//    {
//        chekId = $("#Person_PinNumber").val();
//    }
//    else if (id == 1) {
//        chekId = $("#Person_PinNumber").val();
//    }

//    if (chekId.length > 0) {
//        alert(chekId);
//        $.ajax({
//            url: '/SignUpOP/Check?pId=' + chekId,
//            type: 'POST',
//            datatype: 'json',
//            data: 'data',
//            success: function (result) {
//                var regCombo;
//                $('#regionContainer').append(regCombo);
//                if (result.data.length > 0) {

//                }

//            },
//            error: function () {

//            }
//        });
//    }
//}

