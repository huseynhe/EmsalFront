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
    $('#selectedProducts').html('');
    $.ajax({
        url: '/DemandProduction/SelectedProducts',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#selectedProducts').html(result);
        },
        error: function () {

        }
    });
}