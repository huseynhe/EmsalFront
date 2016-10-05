$(function () {
   
    $("#sendInfo").on("click", function (e) {
        if ($("#name").val() === "" || $("#surname").val() === "" || $("#username").val() === "" ||
            $("#fathername").val() === "" || $("#password").val() === "" ||
            $("#email").val() === "" || $("#birthdate").val() == "" ||
            $("#country").val() == "Ölke Seçin" ||
            $("#birthdate").val() == "" ||
            $("#city").val() == "Şəhər Seçin" ||
            $("#Gender").val() === "Cinsi Seçin" || $("#managerName").val()== "") {

            e.preventDefault();

            $("form p").html("");
            if ($("#managerName").val() == "") {
                $("managerNameAlert").html("Menecer adı Daxil Edilmemişdir")
            }
            if ($("#name").val() === "") {
                $("#nameAlert").html("Ad Daxil Edilmemişdir");
            }
            if ($("#surname").val() === "") {
                $("#surnameAlert").html("Soyad Daxil Edilmemişdir");
            }
            if ($("#username").val() === "") {
                $("#usernameAlert").html("İstifadəçi Adı Daxil Edilmemişdir");
            }
            if ($("#fathername").val() === "") {
                $("#fatherNameAlert").html("Ata adı Daxil Edilmemişdir");
            }
            if ($("#password").val() === "") {
                $("#passwordAlert").html("Şifrə Daxil Edilmemişdir");
            }
            if ($("#email").val() === "") {
                $("#emailAlert").html("Email Daxil Edilmemişdir");
            }
            if ($("#birthdate").val() == "") {
                $("#dateAlert").html("Doğum tarixi Daxil Edilmemişdir");
            }
            if ($("#country").val() == "Ölke Seçin") {
                $("#countryAlert").html("Ölke  Daxil Edilmemişdir");
            }
            if ($("#city").val() == "Şəhər Seçin") {
                $("#cityAlert").html("Şeher  Daxil Edilmemişdir");
            }
            if ($("#Gender").val() === "Cinsi Seçin") {
                $("#genderAlert").html("Cinsi  Daxil Edilmemişdir");
            }
        }
    })
})


