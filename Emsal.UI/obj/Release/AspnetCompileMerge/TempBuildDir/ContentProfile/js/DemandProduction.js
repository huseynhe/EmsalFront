var pId;

function GetProductCatalog(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();

    $.ajax({
        url: '/DemandProduction/ProductCatalog?pId=' + pId,
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            $('#productId').val(pId);
            $(elem).parent().parent().append(result);

            getChooseFileTemplate(pId);
            callSelect2();
        },
        error: function () {

        }
    });
};


var Unitofmeasurementresult;
function GetUnitofmeasurement(pId) {
    $.ajax({
        url: '/DemandProduction/Unitofmeasurement?pId=' + pId,
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

    if (pId > 0) {
        $.ajax({
            url: '/DemandProduction/AdminUnit?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $('#addressId').val(pId);
                $(elem).parent().parent().append(result);

                callSelect2();
            },
            error: function () {

            }
        });
    }
}

function callSelect2()
{
    $('.select2').select2();
}

function GetForeignOrganization(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();

        var valu = 0;
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

        //$('#addressId').val(valu);
        pId = valu;
    }

    if (pId > 0) {
        $.ajax({
            url: '/DemandProduction/ForeignOrganization?pId=' + pId,
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $('#addressId').val(pId);
                ofopid = pId;
                $(elem).parent().parent().append(result);

                if (valu > 0) {
                    $(elem).parent().remove();
                }

                callSelect2();
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
        url: '/DemandProduction/File',
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
        url: '/DemandProduction/SelectedDocuments',
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
        url: '/DemandProduction/ChooseFileTemplate?&pId=' + pId,
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
        url: '/DemandProduction/DeleteSelectedDocument?&id=' + id,
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


function deleteProductionCalendar(id, dId) {
    $.ajax({
        url: '/DemandProduction/DeleteProductionCalendar?&id=' + id,
        type: 'GET',
        data: {},
        success: function (result) {
            getProductionCalendar(dId);
            getSelectedProducts();
        },
        error: function () {

        }
    });
}


function getProductionCalendar(dId) {
    $.ajax({
        url: '/DemandProduction/ProductionCalendar?dId='+dId,
        type: 'GET',
        data: {},
        success: function (result) {
            $('#productionCalendar_' + dId).html(result);
        },
        error: function () {

        }
    });
}


function deleteSelectedDemandProduct(id) {
    $.ajax({
        url: '/DemandProduction/DeleteSelectedDemandProduct?&id=' + id,
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
    //$('#selectedProducts').html('');
    $.ajax({
        url: '/DemandProduction/SelectedProducts?noButton=false',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#selectedProducts').html(result);
        },
        error: function () {

        }
    });
}