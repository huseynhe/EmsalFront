$(document).ready(function () { firstRun(true); });

function enableDisable(state) {
    $("#name").attr('readonly', state);
    $("#surname").attr('readonly', state);
    $("#fatherName").attr('readonly', state); 
}

function firstRun(state) {
    $("#UserName").attr('readonly', state);
    $("#Name").attr('readonly', state);
    $("#Email").attr('readonly', state);
    $("#Password").attr('readonly', state);

    $("#name").attr('readonly', state);
    $("#surname").attr('readonly', state);
    $("#fatherName").attr('readonly', state);
    $("#ManagerEmail").attr('readonly', state);
    $("#workPhonePrefix").attr('disabled', state);
    $("#ManagerWorkPhone").attr('readonly', state);
    $("#mobilePhonePrefix").attr('disabled', state);
    $("#ManagerMobilePhone").attr('readonly', state);
    $("#birtday").attr('readonly', state);
    $("#Gender").attr('disabled', state);
    $("#education").attr('disabled', state);
    $("#Job").attr('disabled', state);



}


function check4voen() {
    var voen = $("#Voen").val();

    if (voen.length > 0) {
        $.ajax({
            url: '/GovernmentOrganisation/CheckForVoen?voen=' + voen,
            type: 'POST',
            datatype: 'json',
            data: 'data',
            success: function (result) {
                if (result.data == null) {
                    alert('VÖEN doğru deyil');
                } else {
                    //if (result.data.length > 0) {
                    if (result.data.Name != null) {
                        $("#Name").val(result.data.Name);

                        enableDisable(true);

                        //$('#gender').attr('disabled', true);
                        //$("#birtday").attr('disabled', true);
                    }
                    else {

                        enableDisable(false);
                        //$('#gender').attr('disabled', false);
                        //$("#birtday").attr('disabled', false);
                    }
                }
            },
            error: function () {

            }
        });

        firstRun(false);
    }
    else { firstRun(true) }
}


function check() {
    var pin = $("#pin").val();

    if (pin.length > 0) {
        $.ajax({
            url: '/GovernmentOrganisation/Check?pId=' + pin,
            type: 'POST',
            datatype: 'json',
            data: 'data',
            success: function (result) {
                if (result.data == null) {
                    alert('FİN doğru deyil');
                } else {
                    //if (result.data.length > 0) {
                    if (result.data.ManagerName != null) {
                        $("#name").val(result.data.ManagerName);
                        $("#surname").val(result.data.Surname);
                        $("#fatherName").val(result.data.FatherName);
                        $('#Gender').val(result.data.Gender).change();
                        $("#birtday").val(result.data.Birthday);

                        enableDisable(true);

                        //$('#gender').attr('disabled', true);
                        //$("#birtday").attr('disabled', true);
                    }
                    else {
                        $("#birtday").val(result.bDate);

                        enableDisable(false);
                        //$('#gender').attr('disabled', false);
                        //$("#birtday").attr('disabled', false);
                    }
                }
            },
            error: function () {

            }
        });

        firstRun(false);
    }
    else {  firstRun (true)}
}