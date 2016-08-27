var pId;

function GetProductCatalog(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();

    $.ajax({
        url: '/OfferProduction/ProductCatalog?pId=' + pId,
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            $('#productId').val(pId);
            $(elem).parent().parent().append(result);

            //if (result == "")
            //{
            //    $('#calendarMainDiv').show();
            //    GetUnitofmeasurement(pId);
            //}
            getChooseFileTemplate(pId);
            $('.select2').select2();
        },
        error: function () {

        }
    });
};


var Unitofmeasurementresult;
function GetUnitofmeasurement(pId) {
    $.ajax({
        url: '/OfferProduction/Unitofmeasurement?pId=' + pId,
        type: 'GET',
        success: function (result) {
            Unitofmeasurementresult = result;

            return result;
        },
        error: function () {

        }
    });
};


function GetAdminUnit(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();
    //var dd = 0;
    //while (dd < 4) {
        
    //    dd++;
    //    alert(dd);
    //}

    if (pId == '') {
        var name = $(elem).attr('name');
        var i = name.substring(5, name.length - 1);
        var valu = 0;
        var val = 0;

        for (var d = 0; d <i; d++)
        {
            val = $('select[name="adId[' + d + ']"]').val();

            if (val != undefined) {
                valu = $('select[name="adId[' + d + ']"]').val();
            }
        }

        $('#addressId').val(valu);
    }

    if (pId > 0) {
        $.ajax({
            url: '/OfferProduction/AdminUnit?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $('#addressId').val(pId);
                $(elem).parent().parent().append(result);

                $('.select2').select2();
            },
            error: function () {

            }
        });
    }
}


function sendFiles() {
    documentType = $('#documentTypes').val();
    $('#documentTypesdg').html('');

    if (documentType < 1) {
        $('#documentTypesdg').html('Sənəd növü seçilməyib');
        return false;
        //e.preventDefault();
    }

    var formData = new FormData();
    var len = $('#fup')[0].files.length;
    //alert(len);
    for (i = 0; i < len; i++) {
        formData.append('file', $('#fup')[0].files[i]);
    }

    formData.append('documentType', documentType)

    $.ajax({
        url: '/OfferProduction/File',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            $('#documentTypes').val('');
            $('#filefieldtemplate').html(filefieldtemplate);

            $('#selectedDocuments').html('');
            getSelectedDocuments();
            getChooseFileTemplate(pId);
        },
        error: function () {
            alert('səhv baş verdi');
        }
    });
}


function getSelectedDocuments() {

    $.ajax({
        url: '/OfferProduction/SelectedDocuments',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#selectedDocuments').html(result);
        },
        error: function () {

        }
    });
}

function getChooseFileTemplate(pId) {

    $.ajax({
        url: '/OfferProduction/ChooseFileTemplate?&pId=' + pId,
        type: 'GET',
        data: {},
        success: function (result) {
            $('#chooseFileTemplate').html(result);
            $('#btnUploadFile').addClass('disabled');
        },
        error: function () {
            location.reload();
        }
    });
}

function deleteSelectedDocument(id) {
    $.ajax({
        url: '/OfferProduction/DeleteSelectedDocument?&id=' + id,
        type: 'GET',
        data: {},
        success: function (result) {
            getSelectedDocuments();
            getChooseFileTemplate(pId);
        },
        error: function () {

        }
    });
}

function deleteSelectedOfferProduct(id) {
    $.ajax({
        url: '/OfferProduction/DeleteSelectedOfferProduct?&id=' + id,
        type: 'GET',
        data: {},
        success: function (result) {
            getSelectedProducts();
        },
        error: function () {

        }
    });
}

function getSelectedProducts() {
    $('#selectedProducts').html('');
    $.ajax({
        url: '/OfferProduction/SelectedProducts',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#selectedProducts').html(result);
        },
        error: function () {

        }
    });
}