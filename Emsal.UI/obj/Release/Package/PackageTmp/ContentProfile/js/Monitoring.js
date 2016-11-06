
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


function OfferMonitoringSearch(elem, value) {
    $.ajax({
        url: '/OfferMonitoring/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function OfferMonitoringContractSearch(elem, value) {
    $.ajax({
        url: '/OfferMonitoring/Contract?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}


function PotentialClientMonitoringSearch(elem, value) {
    $.ajax({
        url: '/PotentialClientMonitoring/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

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

var attObj;
function GetAttachFileTemp(elem, pid) {
    attObj = elem;
    $('.attachFileField').html('');
    $('#personId').val(pid);

    $.ajax({
        url: '/OfferMonitoring/FileTemplate?pid='+pid,
        type: 'GET',
        success: function (result) {
            $(attObj).parent().parent().find('.attachFileField').html(result);
        },
        error: function () {

        }
    });
}


function DeleteContractById(id) {
    var pid = $('#personId').val();

    $.ajax({
        url: '/OfferMonitoring/DeleteContract?id=' + id,
        type: 'GET',
        success: function (result) {
            GetAttachFileTemp(attObj, pid);
        },
        error: function () {

        }
    });
}


function sendFiles() {
    var formData = new FormData();
    var len = $('#fup')[0].files.length;

    for (i = 0; i < len; i++) {
        formData.append('file', $('#fup')[0].files[i]);
    }

    //formData.append('documentType', documentType)
    if (i == 0)
    {
        alert('fayl seçilməyib');
        return false;
    }

    var pid=$('#personId').val();
    formData.append('pid', pid)
    $.ajax({
        url: '/OfferMonitoring/File',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            GetAttachFileTemp(attObj, pid);
        },
        error: function () {
            alert('səhv baş verdi');
        }
    });
}