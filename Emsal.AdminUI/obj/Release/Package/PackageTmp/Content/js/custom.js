$(document).ajaxStart(function () {
    $(".loader").css("display", "block");
});

$(document).ajaxComplete(function () {
    $(".loader").css("display", "none");
});




var allowfiletype = ["image/jpeg", "application/pdf", "image/png"];
var ftypes = ".pdf, .jpeg, .jpg, .png ";
var tfilefieldtemplate;
var ffilefieldtemplate;
var filefieldtemplate;

function chosefiles(elem) {
    var requiredfs = 2;
    var totalfs = 0;
    var fiv = 0;
    var flength = elem.files.length;
    var froot = $(elem).parent();
    var filenames = "";


    if (froot.find('.scope').length == 0) {
        if (froot.find('.true').length == 0)
            tfilefieldtemplate = froot.html();
        if (froot.find('.false').length == 0)
            ffilefieldtemplate = froot.html();
    }

    if (froot.find('.true').length == 0)
        filefieldtemplate = tfilefieldtemplate;
    if (froot.find('.false').length == 0)
        filefieldtemplate = ffilefieldtemplate;

    for (l = 0; l < flength; l++) {
        if ($.inArray(elem.files[l].type, allowfiletype) < 0)
            fiv = 1;

        totalfs = totalfs + parseInt(elem.files[l].size, 10) / 1024;

        filenames = filenames + elem.files[l].name + ' - ' + (Math.round(((parseInt(elem.files[l].size, 10) / 1024)) / 1024 * 100) / 100) + ' mb' + '\n';
    }

    totalfs = (Math.round((totalfs / 1024) * 100) / 100);

    froot.find('.sel').html('<span class="scope" style="font-size: 16px;font-weight: bold;">' + flength + '</span> fayl seçilib, həcmi <span style="font-size: 16px;font-weight: bold;">' + totalfs + '</span> mb <span title="' + filenames + '" style="cursor:pointer;color:#428bca;" class="glyphicon glyphicon-info-sign"></span>');

    $('span').tooltip()

    if (totalfs > requiredfs) {
        alert('Seçilmiş faylların həcmi ' + requiredfs + ' MB-dan az olmalıdır. \n\n Sizin sənədin həcmi ' + totalfs + ' MB');
        froot.html(filefieldtemplate);
    }

    if (fiv == 1) {
        alert('Seçilmiş fayl tipinə icazə verilmir. \n\nQəbul olunan fayl tipləri: ' + ftypes);
        froot.html(filefieldtemplate);
    }

};


var parId = 0;
var name = "";
var description = "";
var camBeOrder = 0;

//if ($('#productCheckEdit').is(':checked')) {
//    $('#productName').val(name);
//    $('#productDescription').val(description);
//} else
//{
//    $('#productName').val('');
//    $('#productDescription').val('');
//}

$('#productCheckEdit').click(function () {
    if (this.checked) {
        $('#productName').val(name);
        $('#productDescription').val(description);
        $('#ProductHeaderGroup').hide();
        if (camBeOrder == 1) {
            $("#canBeOrder").prop("checked", true);
        }
        else {
            $("#canBeOrder").prop("checked", false);
        }

        $.ajax({
            url: '/Home/ProdCatVal',
            type: 'GET',
            data: { productCatalogId: parId },
            success: function (result) {
                $('#ProdCatVal').html(result);
            },
            error: function () {

            }
        });

    }
    else {
        $('#productName').val('');
        $('#productDescription').val('');
        $('#ProductHeaderGroup').show();


        $.ajax({
            url: '/Home/ProdCatVal',
            type: 'GET',
            data: { productCatalogId: 0 },
            success: function (result) {
                $('#ProdCatVal').html(result);
            },
            error: function () {

            }
        });
    }
});

//function checkEdit(elem)
//{
//    if(elem.checked)
//    {
//        $('#productName').val(name);
//        $('#productDescription').val(description);
//    }
//    else
//    {
//        $('#productName').val('');
//        $('#productDescription').val('');
//    }
//}

function chcheck(elem) {
    var ch = $(elem).is(":checked");

    if (ch == true) {
        $(elem).parent().parent().find('.ch').prop('checked', true);

        $(elem).parent().parent().find('.ch').attr("disabled", true);
    }
    else if (ch == false) {
        $(elem).parent().parent().find('.ch').prop('checked', false);
        $(elem).parent().parent().find('.ch').removeAttr("disabled");
    }
}

function setParId(elem, ProductId, ProductName, ProductDescription, cBeOrder) {
    //parId = $(elem).val();
    parId = ProductId;
    name = ProductName;
    description = ProductDescription;
    camBeOrder = cBeOrder;

    $('#ProductHeader').html($(elem).html() + ' &nbsp;&nbsp;<a href="/Home/DeleteProdCat/?&productCatalogId=' + ProductId + '" class="glyphicon glyphicon-remove text-danger"></a>');

    $("#ProductCatalogTree").find('.btn-info').removeClass("btn-info btn-xs");
    $(elem).addClass("btn-info btn-xs");

    ifChecked();

}

$(document).ready(function () {
    ifChecked();
});

function ifChecked()
{
    if ($("#productCheckEdit").is(':checked')) {
        $('#productCheckEdit').trigger('click');
    }
    if ($("#canBeOrder").is(':checked')) {
        $('#canBeOrder').trigger('click');
    }
}

function sendFiles(prodId) {

    var formData = new FormData();
    var len = $('#fup')[0].files.length;
    //alert(len);
    for (i = 0; i < len; i++) {
        formData.append('file', $('#fup')[0].files[i]);
    }

    formData.append('prodId', prodId)

    $.ajax({
        url: '/Home/File',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            location.reload();
        },
        error: function () {
            alert('səhv baş verdi');
        }
    });
}


function AddProductCatalog() {
    var chinput = $('#chinput').val();
    var snd = "";
    var enumCatVal = "";

    for (var i = 0; i < chinput; i++) {

        var chmain = $('input:checkbox[name="chmain' + i + '"]:checked')
               .map(function () {
                   return $(this).val()
               })
               .get();
        if (chmain == '') {
            chmain = 0;
        }

        var chchield = $('input:checkbox[name="chchield' + i + '[]"]:checked')
             .map(function () {
                 return $(this).val()
             })
              .get();

        //if (chmain != '') {
        //    chchield = 0;
        //}

        if (chmain == 0 && chchield != '') {
            chmain = $('input:checkbox[name="chmain' + i + '"]')
           .map(function () {
               return $(this).val()
           })
           .get();
        }

        snd = chmain + ',' + chchield;

        //alert(snd);

        if (snd != '0,' || snd != chmain + ',') {
            if (enumCatVal != '') {
                enumCatVal = enumCatVal + '*' + snd;
            }
            else {
                enumCatVal = snd;
            }
        }

    }

    //alert(enumCatVal);




    productName = $('#productName').val();
    productDescription = $('#productDescription').val();
    //fup = $('#fup').val();

    //$('#fupDanger').html('');

    //if ($.trim(fup).length < 3) {
    //    $('#fupDanger').html('Fayl seçilməyib');
    //    return false;
    //    //e.preventDefault();
    //}

    if ($.trim(productName).length < 3) {
        $('#dproductName').html('Məhsulun adı daxil edilməyib');
        return false;
        //e.preventDefault();
    }
    var isEdit;
    var canBeOrder = 0;
    if (document.getElementById('productCheckEdit').checked) {
        isEdit = true;
    }

    if (document.getElementById('canBeOrder').checked) {
        canBeOrder = 1;
    }

    $.ajax({
        url: '/Home/AddProductCatalog',
        type: 'GET',
        data: { productName: productName, productDescription: productDescription, productCatalogParentID: parId, enumCatVal: enumCatVal, isEdit: isEdit, canBeOrder: canBeOrder },
        success: function (result) {

            sendFiles(result);

            $('#productName').val('');
            $('#productDescription').val('');
            //location.reload();
        },
        error: function () {

        }
    });
}




function checkForm() {
    clearSpan();

    var name = $('#name').val();
    var surname = $('#surname').val();
    var fathername = $('#fathername').val();
    var email = $('#email').val();
    var phone = $('#phone').val();
    var note = $('#note').val();

    if ($.trim(name).length < 3) {
        $('#dname').html('Ad daxil edilməyib');
        return false;
        //e.preventDefault();
    }
    else if ($.trim(surname).length < 3) {
        $('#dsurname').html('Soyad daxil edilməyib');
        return false;
    }
    else if ($.trim(fathername).length < 3) {
        $('#dfathername').html('Ata adı daxil edilməyib');
        return false;
    }
    else if ($.trim(email).length <= 5) {
        $('#demail').html('E-poçt daxil edilməyib');
        return false;
    }
    else if (!validateEmail(email)) {
        $('#demail').html('E-poçt ünvanı düzgün deyil');
        return false;
    }

    else if ($.trim(phone).length < 3) {
        $('#dphone').html('Telefon daxil edilməyib');
        return false;
    }
    else if ($.trim(note).length < 3) {
        $('#dnote').html('Qeyd daxil edilməyib');
        return false;
    }
    else {
        //('#formsuccess').addClasss("bg-green label");
        $('#formsuccess').html('Qeydiyyat uğurla tamamlandı.');
        clearForm();
    }
};

function clearForm() {
    $('#name').val('');
    $('#surname').val('');
    $('#fathername').val('');
    $('#email').val('');
    $('#phone').val('');
    $('#note').val('');
};

function clearSpan() {
    $('#dname').html('');
    $('#dsurname').html('');
    $('#dfathername').html('');
    $('#demail').html('');
    $('#dphone').html('');
    $('#dnote').html('');
    $('#formsuccess').html('');
    //('#formsuccess').removeClass("bg-green label");
};

function validateEmail(sEmail) {
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (filter.test(sEmail)) {
        return true;
    }
    else {
        return false;
    }
}



function chcheckList(elem) {
    var ch = $(elem).is(":checked");

    if (ch == true) {
        $(elem).parent().parent().parent().find('.ch').prop('checked', true);
    }
    else if (ch == false) {
        $(elem).parent().parent().parent().find('.ch').prop('checked', false);
    }
    getCheckedList();
}



$('.ch').click(function () {
    if (this.checked) {
        getCheckedList();
    }
    else {
        getCheckedList();
    }
});

function getCheckedList() {
    var checkedList = new Array();

    var n = jQuery(".ch:checked").length;
    if (n > 0) {
        jQuery(".ch:checked").each(function () {
            checkedList.push($(this).val());
        });
    }

    $('#checkedList').val(checkedList);


    if ($('#checkedList').val() == "") {
        $('#chall').hide();
        $('#allCheck').prop('checked', false);
    } else {
        $('#chall').show();
    }
};



function getAttachmentFile(Id) {
    $('#getImageMessage').html('');
    $("#getImage").modal('show');
    $("#getImage").modal({ backdrop: 'static', keyboard: false });

    $.ajax({
        url: '/ProductionDocument/GetFile?Id=' + Id,
        type: 'GET',
        success: function (result) {
            $('#getImageMessage').html(result);
        },
        error: function () {
        }
    })
}



function DemandProductionSearch(elem, value) {
    $.ajax({
        url: '/DemandProduction/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}


function DemandProductionSearchwd(elem, value) {
    $.ajax({
        url: '/DemandProduction/Indexwd?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function DemandProductDetailInfoForAccounting(elem, value) {
    $.ajax({
        url: '/DemandProduction/DemandProductDetailInfoForAccounting?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function OfferProductionSearch(elem, value) {
    $.ajax({
        url: '/OfferProduction/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function OfferProductionSearchwd(elem, value) {
    $.ajax({
        url: '/OfferProduction/Indexwd?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function PotentialProductionSearch(elem, value) {
    $.ajax({
        url: '/PotentialProduction/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}


function SearchWP(contaction, elem, value) {
    $.ajax({
        url: contaction+'?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}




function GetProductCatalog(elem) {
    $(elem).parent().nextAll().remove();
    pId = $(elem).val();


    if (pId == "") {
        GetUserInfoBy(0, elem);
    }

    if (pId == 0) {
        location.reload();
    }
    if (pId > 0) {
        //$('#puserMenu').html('');
        $.ajax({
            url: '/Report/ProductCatalog?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                //$('#puserMenu').html(result);
                if (result == "") {
                    DemandOfferProduct(pId);
                }

                $(elem).parent().parent().append(result);

                $('.select2').select2();
            },
            error: function () {

            }
        });
    }
}

function DemandOfferProduct(auid) {
    $.ajax({
        url: '/Report/DemandOfferProduct?prodId=' + auid,
        type: 'GET',
        success: function (result) {
            $('#report').html(result);
        },
        error: function () {

        }
    });
}


