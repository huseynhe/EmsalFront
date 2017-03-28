$(document).ajaxStart(function () {
    $(".loader").css("display", "block");
});

$(document).ajaxComplete(function () {
    $(".loader").css("display", "none");
});

function StatisticsSearch(elem, controller, action, param) {
    $.ajax({
        url: '/' + controller + '/' + action + '?' + param + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function resetStaticVariables() {
    var str = window.location.pathname;
    var res = str.split("/");

    if (res[1] != 'PotentialClient') {
        $.ajax({
            url: '/PotentialClient/ResetStaticVariables',
            type: 'GET',
            success: function (result) {
            },
            error: function () {

            }
        });
    }

    if (res[1] != 'DemandProduction') {
        $.ajax({
            url: '/DemandProduction/ResetStaticVariables',
            type: 'GET',
            success: function (result) {
            },
            error: function () {

            }
        });
    }
}
$(document).ready(function () {
    resetStaticVariables();

    $("#rootId").trigger("click");

    $("a").tooltip();

    GetDefaultSortIcon();

    var href;
    var curpathname = window.location.pathname;

    jQuery('.main_menu').find('a').each(function () {
        href = $(this).attr('href');
        if (href == curpathname) {
            $(this).addClass('active');
        }
    });
});

var ri = 0;

function OfferHomeSearch(elem, param) {
    if (ri == 0 || ri == 2) {

        var value = $(elem).val();
        ri = 1;

        $.ajax({
            url: '/OfferHome/OfferProduction?' + param + '=' + $(elem).val(),
            type: 'GET',
            success: function (result) {
                $('#AjaxPaginationList').html(result);

                ri = 2;
                if ($(elem).val() != value) {
                    OfferHomeSearch(elem, param);
                }
            },
            error: function () {

            }
        });
    }
}

function OfferHomeSearchFV() {
    var value = $('#finvoen').val();
    $.ajax({
        url: '/OfferHome/OfferProduction?fv=' + value,
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

var allowfiletype = ["image/jpeg", "application/pdf"];
var ftypes = ".pdf, .jpeg, .jpg";
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


var valu;
var orid = 0;

function GetAdminUnit(elem) {
    $(elem).parent().nextAll().remove();
    pId = $(elem).val();
    var rId = $('#rId').val();

    if (orid > 0 && rId != orid) {
        $('#AjaxPaginationList').html('');
        pId = -1;
    }
    orid = rId;

    valu = 0;

    if (pId == '' || pId=='-1') {
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

        if (pId == 0) {
            pId = -1;
        }
    }

    //if (pId == "") {
    //    GetUserInfoBy(0, elem);
    //}
    var pIdz = 0;

    if (pId == undefined)
    {
        pId = 0;
    }

    //if (pId >= -1) {
        //$('#puserMenu').html('');

        
        pIdz = pId;
        if (pId == -1) {
            pIdz = 0;
        }

        $.ajax({
            url: '/Home/AdminUnit?pId=' + pIdz + '&rId=' + rId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                //if (result == "")
                //{

                //}
                if (pId <= 0) {
                    $('#puserMenu').html(result);
                } else {
                    $(elem).parent().parent().append(result);
                }
                GetUserInfoBy(pId, elem);

                $('.select2').select2();
            },
            error: function () {

            }
        });
    //} else {
    //    GetUserInfoBy(pId, elem);
    //}
}


var si = 0;
var s = 0;
var ad = " asc";
var osort = "";
var gsort = "";

function GetDefaultSortIcon() {
    $('.sortingpu').html('<span class="glyphicon glyphicon-sort pull-right"></span>');
}


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
    gsort = sort + ad;
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
            //$(elem).parent().parent().parent().find('#AjaxPaginationList').html(result);
            $('#AjaxPaginationList').html(result);
            if (valu > 0) {
                $(elem).parent().remove();
            }

        },
        error: function () {

        }
    });
    //}
}


function UserInfoSearch(elem, param) {

    if (ri == 0 || ri == 2) {

        var value = $(elem).val();
        ri = 1;

        if (value == '15' && param == 'rId') {
            $('#userListHeader').html('POTENSİAL İSTEHSALÇILAR');
        }

        else if (value == '11' && param == 'rId') {
            $('#userListHeader').html('İDXALÇILAR');
        }


        $.ajax({
            url: '/Home/UserInfo?' + param + '=' + $(elem).val(),
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $(elem).parent().parent().parent().parent().parent().find('#AjaxPaginationList').html(result);

                ri = 2;
                if ($(elem).val() != value) {
                    UserInfoSearch(elem, param);
                }

                var au = $('select[name="adId[0]"]');

                GetAdminUnit(au);

            },
            error: function () {

            }
        });
    }
}

var oldpId = 0;
function GetProductCatalog(elem, pId) {
    var lStatus = $(elem).parent().find('.lstatus');

    if (lStatus.val() == pId) {
        lStatus.val(1);
        $(elem).parent().find('.resp').html('');
    }
    else {
        if (pId == -1) {
            pId = 0;
        }
        $.ajax({
            url: '/Home/ProductCatalog?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                //if (result.length < 10) {
                    GetAnnouncement(pId);
                //}
                //$('.resp').html('');
                $(elem).parent().parent().find('.resp').html(result);
            },
            error: function () {

            }
        });
    }

    if (lStatus.val() < 1)
        lStatus.val(pId);

    if (lStatus.val() == 1)
        lStatus.val(0);

    oldpId = pId;
};


function GetAnnouncement(productId) {
    $('#responceAnnouncement').html('');
    $.ajax({
        url: '/Home/Announcement?productId=' + productId,
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
};


function searchAnnouncement(elem) {
    if (ri == 0 || ri == 2) {

        var value = $(elem).val();
        ri = 1;

        $.ajax({
            url: '/Home/SearchAnnouncement?value=' + value,
            type: 'GET',
            success: function (result) {
                $('#searchAnnouncementResult').html(result);
                ri = 2;
                if ($(elem).val() != value) {
                    searchAnnouncement(elem);
                }
            },
            error: function () {

            }
        });
    }
}



var oldopId = 0;
function GetHomeOffer(elem, pId) {
    var lStatus = $(elem).parent().find('.lstatus');

    if (lStatus.val() == pId) {
        lStatus.val(1);
        $(elem).parent().find('.resp').html('');
    }
    else {
        if (pId == -1) {
            pId = 0;
        }
        $.ajax({
            url: '/OfferHome/ProductCatalog?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                //if (result.length < 10) {
                    GetOfferProduction(pId);
                //}
                //$('.resp').html('');
                $(elem).parent().parent().find('.resp').html(result);
            },
            error: function () {

            }
        });
    }

    if (lStatus.val() < 1)
        lStatus.val(pId);

    if (lStatus.val() == 1)
        lStatus.val(0);
};


function GetOfferProduction(productId) {
    $('#AjaxPaginationList').html('');
    $.ajax({
        url: '/OfferHome/OfferProduction?productId=' + productId,
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
};

