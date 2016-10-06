
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


function OfferStateSearch(elem, value) {
    $.ajax({
        url: '/OfferState/Index?' + value + '=' + $(elem).val(),
        type: 'GET',
        success: function (result) {
            $('#AjaxPaginationList').html(result);
        },
        error: function () {

        }
    });
}

function PotentialClientStateSearch(elem, value) {
    $.ajax({
        url: '/PotentialClientState/Index?' + value + '=' + $(elem).val(),
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