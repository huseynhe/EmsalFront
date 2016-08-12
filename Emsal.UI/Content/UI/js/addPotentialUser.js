$(function () {

    $("#sendInfo").on("click", function (e) {
        if ($("#name").val() === "" || $("#surname").val() === "" || $("#username").val() === "" ||
            $("#fathername").val() === "" || $("#pin").val() == 0 || $("#password").val() === "" ||
            $("#email").val() === "" || $("#birthdate").val() == "" ||
            $("#country").val() == "Ölke Seçin" ||
            $("#birthdate").val() == "" ||
            $("#city").val() == "Şəhər Seçin" ||
            $("#education").val() == "Təhsil Seçin" || $("#Job").val() == "İş seçin" || $("#Gender").val() === "Cinsi Seçin") {

            e.preventDefault();

            $("form p").html("");

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
            if ($("#pin").val() == 0) {
                $("#pinAlert").html("Fin nömre Daxil Edilmemişdir");
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
            if ($("#education").val() == "Təhsil Seçin") {
                $("#educationAlert").html("Təhsil melumatı  Daxil Edilmemişdir");
            }
            if ($("#Job").val() == "İş seçin") {
                $("#jobAlert").html("İş melumatı  Daxil Edilmemişdir");
            }
            if ($("#Gender").val() === "Cinsi Seçin") {
                $("#genderAlert").html("Cinsi  Daxil Edilmemişdir");
            }
        }
    })

   
    
})


