var pId;

function GetProductCatalog(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();

    $.ajax({
        url: '/PotentialClient/ProductCatalog?pId=' + pId,
        type: 'GET',
        //data: { "pId": appId},
        success: function (result) {
            $('#productId').val(pId);
            $(elem).parent().parent().append(result);

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
            url: '/PotentialClient/AdminUnit?pId=' + pId,
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

function GetAdminUnitFU(elem) {
    pId = $(elem).val();
    $(elem).parent().nextAll().remove();

    if (pId > 0) {
        $.ajax({
            url: '/PotentialClient/AdminUnit?pId=' + pId + '&status=1',
            type: 'GET',
            //data: { "pId": appId},
            success: function (result) {
                $('#addressIdFU').val(pId);
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
        url: '/PotentialClient/File',
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
        url: '/PotentialClient/SelectedDocuments',
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
        url: '/PotentialClient/ChooseFileTemplate?&pId=' + pId,
        type: 'GET',
        data: {},
        success: function (result) {
            $('#chooseFileTemplate').html(result);
            $('#btnUploadFile').addClass('disabled');
        },
        error: function () {

        }
    });
}

function deleteSelectedDocument(id) {
    $.ajax({
        url: '/PotentialClient/DeleteSelectedDocument?&id=' + id,
        type: 'GET',
        data: {},
        success: function (result) {
            getSelectedDocuments();
            getChooseFileTProemplate(pId);
        },
        error: function () {

        }
    });
}

function deleteSelectedPotentialProduct(id) {
    $.ajax({
        url: '/PotentialClient/DeleteSelectedPotentialProduct?&id=' + id,
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
        url: '/PotentialClient/SelectedProducts',
        type: 'GET',
        data: {},
        success: function (result) {
            $('#selectedProducts').html(result);
        },
        error: function () {

        }
    });
}

function finvoennull(v) {
    $('#' + v).val('');

    if (v == "FIN") {
        elem = $('#name');
        $(elem).attr('disabled', false);
        $(elem).val('');

        elem = $('#surname');
        $(elem).attr('disabled', false);
        $(elem).val('');

        elem = $('#fathername');
        $(elem).attr('disabled', false);
        $(elem).val('');
    }


    if (v == "VOEN") {
        elem = $('#legalPName');
        $(elem).attr('disabled', false);
        $(elem).val('');

        elem = $('#legalPSurname');
        $(elem).attr('disabled', false);
        $(elem).val('');

        elem = $('#legalPFathername');
        $(elem).attr('disabled', false);
        $(elem).val('');

        elem = $('#legalLame');
        $(elem).attr('disabled', false);
        $(elem).val('');
    }
}



function getPhysicalPerson() {
    fin = $('#FIN').val();

    if ($.trim(fin).length < 3) {
        alert('FİN daxil edilməyib');
        $('#FIN').focus();
        return false;
    }

    var elem;

    $.ajax({
        url: '/PotentialClient/GetPhysicalPerson?fin=' + fin,
        type: 'GET',
        success: function (result) {
            elem = $('#name');
            $(elem).val(result.Name);
            if (result.Name != null)
                $(elem).attr('disabled', true);
            else {
                $(elem).attr('disabled', false);
                alert("Qeyd mövcud deyil");
            }

            elem = $('#surname');
            $(elem).val(result.Surname);
            if (result.Surname != null)
                $(elem).attr('disabled', true);
            else
                $(elem).attr('disabled', false);

            elem = $('#fathername');
            $(elem).val(result.FatherName);
            if (result.FatherName != null)
                $(elem).attr('disabled', true);
            else
                $(elem).attr('disabled', false);

        },
        error: function () {

        }
    });

}

function getLegalPerson() {
    voen = $('#VOEN').val();

    if ($.trim(voen).length < 3) {
        alert('VÖEN daxil edilməyib');
        $('#VOEN').focus();
        return false;
    }

    var elem;

    $.ajax({
        url: '/PotentialClient/GetLegalPerson?voen=' + voen,
        type: 'GET',
        success: function (result) {
            elem = $('#legalPName');
            $(elem).val(result.Name);
            if (result.Name != null)
                $(elem).attr('disabled', true);
            else {
                $(elem).attr('disabled', false);
                alert("Qeyd mövcud deyil");
            }

            elem = $('#legalPSurname');
            $(elem).val(result.Surname);
            if (result.Surname != null)
                $(elem).attr('disabled', true);
            else
                $(elem).attr('disabled', false);

            elem = $('#legalPFathername');
            $(elem).val(result.FatherName);
            if (result.FatherName != null)
                $(elem).attr('disabled', true);
            else
                $(elem).attr('disabled', false);

            elem = $('#legalLame');
            $(elem).val(result.PinNumber);
            if (result.FatherName != null)
                $(elem).attr('disabled', true);
            else
                $(elem).attr('disabled', false);
        },
        error: function () {

        }
    });

}