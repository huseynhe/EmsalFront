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


var valu;

function GetAdminUnit(elem) {
    $(elem).parent().nextAll().remove();
    pId = $(elem).val();

    valu = 0;
    if (pId == '') {
        var name = $(elem).attr('name');
        var i = name.substring(5, name.length - 1);
        var val = 0;

        for (var d = 0; d < i; d++) {
            val = $('select[name="adId[' + d + ']"]').val();

            if (val != undefined) {
                valu = $('select[name="adId[' + d + ']"]').val();
            }
        }
        pId = valu;

    }

    if (pId == "") {
        GetUserInfoBy(0, elem);
    }

    if (pId > 0) {
        //$('#puserMenu').html('');
        $.ajax({
            url: '/Home/AdminUnit?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                //if (result == "")
                //{
                GetUserInfoBy(pId, elem);
                //}
               
                    $(elem).parent().parent().append(result);

                $('.select2').select2();
            },
            error: function () {

            }
        });
    }
}


var si = 0;
var s = 0;
var ad = " asc";
var osort = "";
var gsort = "";

function GetDefaultSortIcon()
{
    $('.sortingpu').html('<span class="glyphicon glyphicon-sort pull-right"></span>');
}


$( document ).ready(function() {
    GetDefaultSortIcon();

    var href;
    var curpathname = window.location.pathname;

    jQuery('.main_menu').find('a').each(function () {
        href = $(this).attr('href');
        if (href == curpathname)
        {
            $(this).addClass('active');
        }
    });
});

function GetUserInfo(elem, sort) {
    GetDefaultSortIcon();
    if (si == 0) {
        osort = sort;
        si = 1;
    }

    if (osort != sort) {
        ad = " asc";
        s = 0;
    }

    osort = sort;
    gsort=sort + ad;
    GetUserInfoSort(elem, gsort);

    if (s == 0) {
        $(elem).html('<span class="glyphicon glyphicon-sort-by-alphabet"></span>');
        s = 1;
        ad = " desc";
    }
    else if (s == 1) {
        $(elem).html('<span class="glyphicon glyphicon-sort-by-alphabet-alt"></span>');
        s = 0;
        ad = " asc";
    }
}




function GetUserInfoSort(elem, sort) {
    $.ajax({
        url: '/Home/UserInfo?sort=' + sort,
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            $(elem).parent().parent().parent().parent().parent().find('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}



function GetUserInfoBy(addressId, elem) {

    //if (addressId > 0) {
        $.ajax({
            url: '/Home/UserInfo?addressId=' + addressId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $(elem).parent().parent().parent().find('#AjaxPaginationList').html(result);

                if (valu > 0) {
                    $(elem).parent().remove();
                }

            },
            error: function () {

            }
        });
    //}
}

function UserInfoSearch(elem, value)
{
    $.ajax({
        url: '/Home/UserInfo?' + value + '=' + $(elem).val(),
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            $(elem).parent().parent().parent().parent().parent().find('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

var oldpId = 0;
function GetProductCatalog(elem, pId, isMain) {

    if (isMain == 1 && oldpId == pId) {
        $('.resp').html('');
        pId = 0;
    }
    else {
        if (isMain == 1) {
            $('.resp').html('');
        }
        $.ajax({
            url: '/Home/ProductCatalog?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                if (result.length < 10) {
                    GetAnnouncement(pId);
                }
                //$('.resp').html('');
                $(elem).parent().parent().find('.resp').html(result);
            },
            error: function () {

            }
        });
    }

    oldpId = pId;
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

function searchAnnouncement(elem) {
    var value = $(elem).val();

    $.ajax({
        url: '/Home/SearchAnnouncement?value=' + value,
        type: 'GET',
        success: function (result) {
            $('#searchAnnouncementResult').html(result);
        },
        error: function () {

        }
    });
}




var oldopId = 0;
function GetHomeOffer(elem, pId, isMain) {

    if (isMain == 1 && oldopId == pId) {
        $('.resp').html('');
        pId = 0;
    }
    else {
        if (isMain == 1) {
            $('.resp').html('');
        }
        $.ajax({
            url: '/OfferHome/ProductCatalog?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                if (result.length < 10) {
                    GetOfferProduction(pId);
                }
                //$('.resp').html('');
                $(elem).parent().parent().find('.resp').html(result);
            },
            error: function () {

            }
        });
    }

    oldopId = pId;
};


function GetOfferProduction(productId) {
    $('#responceAnnouncement').html('');
    $.ajax({
        url: '/OfferHome/OfferProduction?productId=' + productId,
        type: 'GET',
        success: function (result) {
            $('#responceAnnouncement').html(result);
        },
        error: function () {

        }
    });
};
