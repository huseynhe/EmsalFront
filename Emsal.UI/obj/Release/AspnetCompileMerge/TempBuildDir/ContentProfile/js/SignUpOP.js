﻿function PersonType() {
    var id = document.getElementById("personType").value;
    if (id == 1) {
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
    }

    else if (id == 2) {
        document.getElementById("fizikiShexs").style.display = "none";
        document.getElementById("emailDiv").style.display = "block";
        document.getElementById("userNameDiv").style.display = "block"; 
        document.getElementById("mobilDiv").style.display = "block";
        document.getElementById("genderDiv").style.display = "block"; 
        document.getElementById("birtdayDiv").style.display = "block";
        document.getElementById("educDiv").style.display = "block";
        document.getElementById("jobDiv").style.display = "block";
        document.getElementById("huquqiShexs").style.display = "block"; 
        document.getElementById("huquqiShexsName").style.display = "block";
    }

    if (id == "") {
        document.getElementById("fizikiShexs").style.display = "none";
        document.getElementById("huquqiShexs").style.display = "none";
        $('#formBody').hide();
    }
};

var str = "";

var ai=0;
var res="";

function check() {
    ai=0;
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
            url: '/SignUpOP/Check?pId=' + chekId + "&type=" + id,
            type: 'POST',
            datatype: 'json',
            data: 'data',
            success: function (result) {

                if (result.data == null) {
                    alert('FİN və ya VÖEN doğru deyil');
                    window.setTimeout(function () {
                        window.location.href = '/Login';
                    }, 5000);
                } else {

                    var regCombo;
                    $('#regionContainer').append(regCombo);
                    //if (result.data.length > 0) {
                    if (result.data.Person != null) {
                        $("#Name").val(result.data.Person.Name);
                        $("#Surname").val(result.data.Person.Surname);
                        $("#FatherName").val(result.data.Person.FatherName);
                        $('#gender').val(result.data.Person.gender);
                        $("#descAddress").val(result.data.descAddress);
                        $("#birtday").val(result.data.birtday);
                        str = result.data.FullAddress;
                        res = str.split(",");
                        for (var i = 0; i < res.length; i++) {
                            if (i == 0) {
                                $("#mainRegion").val(res[i]).change();
                            }

                        }
                    }
                    else {
                        $("#birtday").val(result.data.birtday);
                    }

                    //}

                    $('#formBody').show();
                }
            },
            error: function () {

            }
        });
    //}
}

$(document).ready(function () {

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
                    regCombo += "<div class='col-md-3'> <select id='" + "id" + result.data[0].parentId + "' onchange='GetAdminUnit(this)' class='form-control select2'>"
                    regCombo += " <option value=''>Seçim edin</option>"
                    for (var i = 0; i < result.data.length; i++) {
                        regCombo += " <option value='" + result.data[i].id + "'> " + result.data[i].name + " </option>"
                    }
                    regCombo += "</div> </select> "
                    $('#regionContainer').append(regCombo);
                }




                //if (ai > 0) {
          
                    $("#id" + res[ai] + "").val(res[ai+1]).change();
                //}
                    ai = ai + 1;

                    $('.select2').select2();
            },
            error: function () {

            }
        });
    }
};

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

