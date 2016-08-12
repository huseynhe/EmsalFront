var allowfiletype = ["image/jpeg", "application/pdf"];
var ftypes = ".pdf, .jpeg, .jpg";
var tfilefieldtemplate;
var ffilefieldtemplate;
var filefieldtemplate;

function chosefiles(elem) {
    var requiredfs = 2;
    var totalfs = 0;
    var fiv=0;
    var flength = elem.files.length;
    var froot = $(elem).parent();
    var filenames="";


    if (froot.find('.scope').length == 0)
    {
        if (froot.find('.true').length == 0)
            tfilefieldtemplate=froot.html();
        if (froot.find('.false').length == 0)
            ffilefieldtemplate = froot.html();
    }
       
    if (froot.find('.true').length == 0)
        filefieldtemplate=tfilefieldtemplate;
    if (froot.find('.false').length == 0)
        filefieldtemplate=ffilefieldtemplate;

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

    if(fiv==1)
    {
        alert('Seçilmiş fayl tipinə icazə verilmir. \n\nQəbul olunan fayl tipləri: ' + ftypes);
        froot.html(filefieldtemplate);
    }

};



var parId = 0;

function setParId(elem) {
    parId = $(elem).val();
}

function AddProductCatalog() {
    productName = $('#productName').val();
    productDescription = $('#productDescription').val();

    $.ajax({
        url: '/Home/AddProductCatalog',
        type: 'GET',
        data: { productName: productName, productDescription: productDescription, productCatalogParentID: parId },
        success: function (result) {
            $('#productName').val('');
            $('#productDescription').val('');
            location.reload();
        },
        error: function () {

        }
    });
}


$(document).ready(function () {
    $("a").tooltip();
});




function GetAdminUnit(pId) {
    if (pId > 0) {
        $('#puserMenu').html('');
        $.ajax({
            url: '/Home/AdminUnit?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $('#puserMenu').html(result);
            },
            error: function () {

            }
        });
    }
}

function GetUserInfoBy(addressId, elem) {

    if (addressId > 0) {
        $.ajax({
            url: '/Home/UserInfoBy?addressId=' + addressId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $(elem).parent().parent().parent().parent().find('#AjaxPaginationList').html(result);
            },
            error: function () {

            }
        });
    }
}


function GetProductCatalog(elem, pId) {
    $.ajax({
        url: '/Home/ProductCatalog?pId=' + pId,
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            if (result.length < 10)
            {
                GetAnnouncement(pId);
            }
            //$('.resp').html('');
            $(elem).parent().parent().find('.resp').html(result);
        },
        error: function () {

        }
    });
};


function GetAnnouncement(productId) {
    $('#responceAnnouncement').html('');
    $.ajax({
        url: '/Home/Announcement?productId=' + productId,
        type: 'GET',
        success: function (result) {
            $('#responceAnnouncement').html(result);
        },
        error: function () {

        }
    });
};