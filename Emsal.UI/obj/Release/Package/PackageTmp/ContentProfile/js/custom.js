var allowfiletype = ["image/jpeg", "image/png", "application/pdf"];
var ftypes = ".pdf, .jpeg, .jpg, .png";
var tfilefieldtemplate;
var ffilefieldtemplate;
var filefieldtemplate;

function chosefiles(elem) {
    //var requiredfs = 2;
    var requiredfs = (Math.round((($('#uploadFileSize').val() / 1024) / 1024) * 100) / 100);

    var totalfs = 0;
    var fiv = 0;
    var flength = elem.files.length;
    var froot = $(elem).parent();
    var filenames = "";

    //alert(requiredfs);
    $('#btnUploadFile').removeClass('disabled');

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
        $('#btnUploadFile').addClass('disabled');
        alert('Seçilmiş faylların həcmi ' + requiredfs + ' MB-dan az olmalıdır. \n\n Sizin sənədin həcmi ' + totalfs + ' MB');
        froot.html(filefieldtemplate);
    }

    if (fiv == 1) {
        $('#btnUploadFile').addClass('disabled');
        alert('Seçilmiş fayl tipinə icazə verilmir. \n\nQəbul olunan fayl tipləri: ' + ftypes);
        froot.html(filefieldtemplate);
    }

};


function chcheckMonth(elem) {
    var ch = $(elem).is(":checked");

    if (ch == true) {
        $(elem).parent().parent().parent().parent().find('.ch').prop('checked', true);
        //$(elem).parent().parent().parent().parent().find('.ch').attr("disabled", true);
    }
    else if (ch == false) {
        $(elem).parent().parent().parent().parent().find('.ch').prop('checked', false);
        //$(elem).parent().parent().parent().parent().find('.ch').removeAttr("disabled");
    }

    getAllCheckedMonth();
}

function getAllCheckedMonth() {
    var selectedMonth = new Array();
    var selectedMonthName = new Array();
    var selectedMonthNameUI = new Array();

    var n = jQuery(".ch:checked").length;
    if (n > 0) {
        jQuery(".ch:checked").each(function () {
            selectedMonth.push($(this).val());
            selectedMonthName.push($(this).parent().find('.chn').html());
            selectedMonthNameUI.push('<span style="margin-top: 5px;" class="btn btn-default btn-sm">' + $(this).parent().find('.chn').html() + '</span>&nbsp;&nbsp;');
        });
    }

    $('#selectedMonth').val(selectedMonth);
    $('.chMonthName').html(selectedMonthNameUI);

};

jQuery(".ch").click(function () {
    getAllCheckedMonth()
});

setTimeout(function () {
    $('#checkAll').trigger('click');

    getAllCheckedMonth();
}, 1000);


$('.signpdf').hide();
$('.btnsubmit').hide();

$('#confirmList').click(function () {
    if (this.checked) {
        $('#mainf').hide();
        $('.signpdf').show();
        $('.btnsubmit').show();
        $('.btnadd').hide();
    }
    else {
        $('#mainf').show();
        $('.signpdf').hide();
        $('.btnsubmit').hide();
        $('.btnadd').show();
        //location.reload();
    }
});
