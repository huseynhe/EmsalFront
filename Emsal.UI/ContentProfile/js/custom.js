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


function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}

$("#startDate").change(function () {
    alert('Change!');
});


function Getotherdate(elem, status, form) {
    if (form == 'o')
    {
        form = 'OfferProduction';
    }
    if (form == 'd') {
        form = 'DemandProduction';
    }
    if (status == 'y') {
        var year = $(elem).val();
        daySelect = '<option value="">Seçim edin</option>';
        for (var i = year; i < parseInt(year) + 2; i++) {
            daySelect = daySelect + '<option value="' + i + '">' + i + '</option>';
        }
        $('#endDateYear').html(daySelect);
    }

    var startDateYear=$('#startDateYear').val();
    var startDateMonth=$('#startDateMonth').val();
    var endDateYear=$('#endDateYear').val();
    var endDateMonth = $('#endDateMonth').val();
    var productId = $('#productId').val();

    if (startDateYear != "" && startDateMonth != "" && endDateYear != "" && endDateMonth!="")
    {
        $.ajax({
            url: '/'+form+'/Getmonth?id='+productId+'&startDateYear=' + startDateYear + '&startDateMonth=' + startDateMonth + '&endDateYear=' + endDateYear + '&endDateMonth=' + endDateMonth,
            type: 'GET',
            success: function (result) {
                $('#getmonthresult').html(result);
            },
            error: function () {

            }
        });
    }
    else {
        $('#getmonthresult').html('');
    }
}

var shippingScheduleStatus;

$(".shippingSchedule").click(function () {
    $(this).parent().find('.active').removeClass("active");
    var value = $(this).find('.hinp').val();

    if (value != shippingScheduleStatus)
    {
    $("#endDateMonth").change();
    }
    shippingScheduleStatus = value;
    $('#getmonthresult').show();
});

//function asd() {
//    $(".size").keyup(function (event) {
//        alert("Handler for .keyup() called.");
//    })
//};

function getSizeValue(elem)
{
    var selMonthCount = jQuery(".size").length;
    var ts = 0;
    var hm = 0;
    var tv = 0;

    if (selMonthCount > 0) {
        jQuery(".size").each(function () {
            hm = $(this).parent().parent().parent().parent().find(".howMany").val();
            tv = $(this).val();
            if (tv == "")
            {
                tv=0;
            }
            ts = ts + (parseFloat(tv) * hm);
        })

        ts = (Math.round(ts * 1000) / 1000);

        $("#totalSize").html("Toplam miqdarı (həcmi): "+ts);
    }
}

function isWeekday(year, month, day) {
    //var day = new Date(year, month, day).getDay();
    //return day != 0 && day != 6;

    var date = new Date(year, month - 1, day);
    var weekday = new Array("sunday", "monday", "tuesday", "wednesday",
                        "thursday", "friday", "saturday");

    return weekday[date.getDay()];
}

function getWeekdayInMonth(month, year, elem) {
    var weekday = $(elem).val();
    var days = daysInMonth(month, year);

    var weekdays = new Array(7);
    weekdays[1] = "monday";
    weekdays[2] = "tuesday";
    weekdays[3] = "wednesday";
    weekdays[4] = "thursday";
    weekdays[5] = "friday";
    weekdays[6] = "saturday";
    weekdays[7] = "sunday";

    var cweekdays = 0;
    for (var i = 0; i < days; i++) {
        if (isWeekday(year, month, i + 1) == weekdays[weekday]) cweekdays++;
    }

    if (cweekdays == 0)
    {
        cweekdays = 1;
    }

    $(elem).parent().parent().find(".howMany").val(cweekdays);
    $(elem).parent().parent().find(".size").val($(elem).parent().parent().find(".hsize").val() / cweekdays);

    var ths;
    ths = $(elem).parent().parent().find(".size");
    getSizeValue(ths);
}


function getDayName() {
    var year = 2016;
    var month = 8;
    var day = 16;

    var date = new Date(year, month - 1, day);
    var weekday = new Array("sunday", "monday", "tuesday", "wednesday",
                        "thursday", "friday", "saturday");

    return weekday[date.getDay()];
}


function getAllCheckedMonth() {
    var selectedMonth = new Array();
    var selectedMonthName = new Array();
    var selectedMonthNameUI = new Array();
    var osize = 0;

    //if ($('#startDate').val() == "")
    //{
    //    alert('Başlama tarixi xanası məcburidir.');
    //return false;
    //}

    //if ($('#endDate').val() == "") {
    //    alert('Bitmə tarixi xanası məcburidir.');
    //    return false;
    //}


    //var startDate = $("#startDate").val();
    //var endDate = $("#endDate").val();
    //var sDate = new Date(startDate);
    //var eDate = new Date(endDate);

    //alert(sDate);
    //alert(eDate);
    $("#totalSize").html("");

    if ($("#osize").val() != undefined)
    {
    osize = $("#osize").val();
    }

    var n = jQuery(".ch:checked").length;
    var nc = 0;
    var ncv;
    var ncvn;
    var dInMonth;
    var daySelect;
    var hourSelect;
    var yearCalendar;
    var Unitofmeasurementresultn="";
    if (n > 0) {        
        jQuery(".ch:checked").each(function () {
            Unitofmeasurementresultn = Unitofmeasurementresult;
            //if (nc > 0) {
                ncv = '5555]';
                ncvn = nc+']';

                for (i = 0; i < 15; i++) {
                    Unitofmeasurementresultn = Unitofmeasurementresultn.replace(ncv, ncvn);
                }
            //}
                var str = $(this).parent().parent().find('.rub').val();
                yearCalendar = str.substring(1, str.length);
            monthString = $(this).parent().parent().find('.monthName').val();
            var dat = new Date('1 ' + monthString + ' ' + yearCalendar);
            dInMonth = daysInMonth(dat.getMonth() + 1, yearCalendar);

            howMany = 1;
            month = "";
            year = "";
            howManySelect = "";
            daySelect = "";
            hourSelect = "";

            if (shippingScheduleStatus != "gunluk" && shippingScheduleStatus != "heftelik") {
                for (var i = 1; i <= dInMonth; i++) {
                    daySelect = daySelect + '<option value="' + i + '">' + i + '</option>';
                }

                daySelect = '<div class="col-md-3"><label class="control-label">Gün</label><select required class="form-control select2" name="day[' + nc + ']"><option value="">Seçin</option>' + daySelect + '</select><span class="field-validation-valid text-danger" data-valmsg-for="day[' + nc + ']" data-valmsg-replace="true"></span></div>';
            }

            if (shippingScheduleStatus == "heftelik") {
                dInMonth = 7;

                for (var i = 1; i <= dInMonth; i++) {
                    daySelect = daySelect + '<option value="' + i + '">' + i + '</option>';
                }

                daySelect = '<div class="col-md-3"><label class="control-label">Gün</label><select onchange="getWeekdayInMonth(' + (dat.getMonth() + 1) + ', ' + yearCalendar + ', this)" required class="form-control select2" name="day[' + nc + ']"><option value="">Seçin</option>' + daySelect + '</select><span class="field-validation-valid text-danger" data-valmsg-for="day[' + nc + ']" data-valmsg-replace="true"></span></div>';
            }


            if (shippingScheduleStatus == "gunluk") {
                howMany = dInMonth;
            }
            for (var i = 1; i <= 24; i++) {
                hourSelect = hourSelect + '<option value="' + i + '">' + i + ':00</option>';
            }


            hourSelect = '<div class="col-md-3"><label class="control-label">Saat</label><select required class="form-control select2" name="hour[' + nc + ']"><option value="">Seçin</option>' + hourSelect + '</select><span class="field-validation-valid text-danger" data-valmsg-for="hour[' + nc + ']" data-valmsg-replace="true"></span></div>';

            month = '<input hidden name="month[' + nc + ']" value="' + $(this).parent().find('.ch').val() + '">';
            year = '<input hidden name="year[' + nc + ']" value="' + yearCalendar + '">';
            howManySelect = '<input hidden name="howMany[' + nc + ']" class="howMany" value="' + howMany + '">';

            var ts;
            if (n > 0 && osize > 0) {
                ts = Math.round(((osize / n) / howMany)*1000)/1000;
            }

            Unitofmeasurementresultn = Unitofmeasurementresultn.replace('inpsize', 'value="' + ts + '"');
            Unitofmeasurementresultn = Unitofmeasurementresultn.replace('hinpsize', 'value="' + ts + '"');

                selectedMonth.push($(this));
                selectedMonthName.push($(this).parent().find('.chn').html());
                selectedMonthNameUI.push('<div><div class="col-md-6"><label class="control-label">Ay</label><h3 style="margin-top: 5px;" class="text-danger text-bold">' + $(this).parent().find('.chn').html() + '</h3></div>' + daySelect + hourSelect + month + year + howManySelect + '<span class="clearfix"></span><br><span>' + Unitofmeasurementresultn + '</span><span class="clearfix"></span></div><hr><br>');

                nc = nc + 1;
        });
    }

    $('#selectedMonth').val(selectedMonth);
    $('.chMonthName').html(selectedMonthNameUI);

    $('.select2').select2();
};

jQuery(".ch").click(function () {
    var rub;
    var year;
    //if (shippingScheduleStatus == "illik") {
    //    var ths = this;
    //    jQuery(".ch:checked").attr('checked', false);
    //    $(this).prop('checked', true);
    //}


    if (shippingScheduleStatus == "illik") {
        rub = $(this).parent().parent().find('.rub').val();
        year = rub.substring(1, rub.length);
        $(this).parent().parent().parent().parent().parent().find('.year' + year + ':checked').attr('checked', false);
        $(this).prop('checked', true);
    }


    if (shippingScheduleStatus == "rubluk") {
        rub = $(this).parent().parent().find('.rub').val();

        $(this).parent().parent().parent().parent().parent().find('.rub' + rub + ':checked').attr('checked', false);
        $(this).prop('checked', true);
    }

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


