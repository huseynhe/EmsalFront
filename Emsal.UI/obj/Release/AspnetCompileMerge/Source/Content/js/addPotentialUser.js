$(function () {
    $("#country").on("change", function () {
        var cities = function () {
            return $.ajax({
                url: "/SignUp/GetAdminUnitsByParentId",
                type: "Get",
                data: { Id: $("#country").val() }
            })
        }

        Promise.resolve(cities())
            .then(function (resultArray) {
                $("#city").html("");
                $("#village").html(" ");
                $("#throughfare").html("");
                resultArray.map(function (item) {
                    $("#city").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })

                var villages = function () {
                    return $.ajax({
                        url: "/SignUp/GetAdminUnitsByParentId",
                        type: "Get",
                        data: { Id: $("#city").val() }
                    })
                }

                Promise.resolve(villages())
               .then(function (resultArray) {
                   $("#village").html("");
                   resultArray.map(function (item) {
                       $("#village").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                   })
               })
               .catch(function (err) {
                   console.log(err.message);
               });
            })
            .catch(function (err) {
                console.log(err.message);
            });

    })
    $("#city").on("change", function () {
        var villages = function () {
            return $.ajax({
                url: "/SignUp/GetAdminUnitsByParentId",
                type: "Get",
                data: { Id: $("#city").val() }
            })
        }

        Promise.resolve(villages())
            .then(function (resultArray) {
                $("#village").html(" ");
                resultArray.map(function (item) {
                    $("#village").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })
            })
            .catch(function (err) {
                console.log(err.message);
            });
    })
    $("#village").on("change", function () {
        var throughfares = function () {
            return $.ajax({
                url: "/SignUp/GetThroughfaresByAdminUnitId",
                type: "Get",
                data: { Id: $("#village").val() }
            })
        }

        Promise.resolve(throughfares())
            .then(function (resultArray) {
                $("#throughfare").html("");
                resultArray.map(function (item) {
                    $("#throughfare").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })
            })
            .catch(function (err) {
                console.log(err.message);
            });
    })

    $("#managerCountry").on("change", function () {
        var cities = function () {
            return $.ajax({
                url: "/SignUp/GetAdminUnitsByParentId",
                type: "Get",
                data: { Id: $("#managerCountry").val() }
            })
        }

        Promise.resolve(cities())
            .then(function (resultArray) {
                $("#managerCity").html("");
                $("#managerVillage").html(" ");
                $("#managerThroughfare").html("");
                resultArray.map(function (item) {
                    $("#managerCity").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })

                var villages = function () {
                    return $.ajax({
                        url: "/SignUp/GetAdminUnitsByParentId",
                        type: "Get",
                        data: { Id: $("#managerCity").val() }
                    })
                }

                Promise.resolve(villages())
               .then(function (resultArray) {
                   $("#managerVillage").html("");
                   resultArray.map(function (item) {
                       $("#managerVillage").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                   })
               })
               .catch(function (err) {
                   console.log(err.message);
               });
            })
            .catch(function (err) {
                console.log(err.message);
            });

    })
    $("#managerCity").on("change", function () {
        var villages = function () {
            return $.ajax({
                url: "/SignUp/GetAdminUnitsByParentId",
                type: "Get",
                data: { Id: $("#managerCity").val() }
            })
        }

        Promise.resolve(villages())
            .then(function (resultArray) {
                $("#managerVillage").html(" ");
                resultArray.map(function (item) {
                    $("#managerVillage").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })
            })
            .catch(function (err) {
                console.log(err.message);
            });
    })
    $("#managerVillage").on("change", function () {
        var throughfares = function () {
            return $.ajax({
                url: "/SignUp/GetThroughfaresByAdminUnitId",
                type: "Get",
                data: { Id: $("#managerVillage").val() }
            })
        }

        Promise.resolve(throughfares())
            .then(function (resultArray) {
                $("#managerThroughfare").html("");
                resultArray.map(function (item) {
                    $("#managerThroughfare").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })
            })
            .catch(function (err) {
                console.log(err.message);
            });
    })

    $("#sendInfo").on("click", function (e) {
        if ($("#name").val() === "" || $("#surname").val() === "" || $("#username").val() === "" ||
            $("#fathername").val() === "" || $("#pin").val() == 0 || $("#password").val() === "" ||
            $("#email").val() === "" || $("#birthdate").val() == "" ||
            $("#country").val() == "Ölke Seçin" ||
            $("#birthdate").val() == "" ||
            $("#city").val() == "Şəhər Seçin" ||
            $("#education").val() == "Tehsil Seçin" || $("#Job").val() == "İş seçin" || $("#Gender").val() === "Cinsiyyet Seçin") {

            e.preventDefault();

            $("form p").html("");

            if ($("#name").val() === "") {
                $("#nameAlert").html("Ad Daxil Edilmemişdir");
            }
            if ($("#surname").val() === "") {
                $("#surnameAlert").html("Soyad Daxil Edilmemişdir");
            }
            if ($("#username").val() === "") {
                $("#usernameAlert").html("İstifadeçi adı Daxil Edilmemişdir");
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
            if ($("#education").val() == "Tehsil Seçin") {
                $("#educationAlert").html("Tehsil melumatı  Daxil Edilmemişdir");
            }
            if ($("#Job").val() == "İş seçin") {
                $("#jobAlert").html("İş melumatı  Daxil Edilmemişdir");
            }
            if ($("#Gender").val() === "Cinsiyyet Seçin") {
                $("#genderAlert").html("Cinsiyyet  Daxil Edilmemişdir");
            }
        }
    })

    var cities = function () {
        return $.ajax({
            url: "/SignUp/GetAdminUnitsByParentId",
            type: "Get",
            data: { Id: $("#country").val() }
        })
    }

    Promise.resolve(cities())
        .then(function (resultArray) {
            console.log(resultArray);
            resultArray.map(function (item) {
                $("#city").append("<option value =" + item.Id + ">" + item.Name + "</option>");
            })

            var villages = function () {
                return $.ajax({
                    url: "/SignUp/GetAdminUnitsByParentId",
                    type: "Get",
                    data: { Id: $("#city").val() }
                })
            }

            Promise.resolve(villages())
        .then(function (resultArray) {
            resultArray.map(function (item) {
                $("#village").append("<option value =" + item.Id + ">" + item.Name + "</option>");
            })
        })
        .catch(function (err) {
            console.log(err.message);
        });

            var throughfares = function () {
                return $.ajax({
                    url: "/SignUp/GetAdminUnitsByParentId",
                    type: "Get",
                    data: { Id: $("#village").val() }
                })
            }

            Promise.resolve(throughfares())
       .then(function (resultArray) {
           console.log(resultArray);
           resultArray.map(function (item) {
               $("#throughfare").append("<option value =" + item.Id + ">" + item.Name + "</option>");
           })
       })
       .catch(function (err) {
           console.log(err.message);
       });
        })
        .catch(function (err) {
            console.log(err.message);
        });

    var managerCities = function () {
        return $.ajax({
            url: "/SignUp/GetAdminUnitsByParentId",
            type: "Get",
            data: { Id: $("#managerCountry").val() }
        })
    }

    Promise.resolve(managerCities())
        .then(function (resultArray) {
            resultArray.map(function (item) {
                $("#managerCity").append("<option value =" + item.Id + ">" + item.Name + "</option>");
            })

            var managerVillages = function () {
                return $.ajax({
                    url: "/SignUp/GetAdminUnitsByParentId",
                    type: "Get",
                    data: { Id: $("#managerCity").val() }
                })
            }

            Promise.resolve(managerVillages())
            .then(function (resultArray) {
                resultArray.map(function (item) {
                    $("#managerVillage").append("<option value =" + item.Id + ">" + item.Name + "</option>");
                })
            })
            .catch(function (err) {
                console.log(err.message);
            });

            var managerThroughfares = function () {
                return $.ajax({
                    url: "/SignUp/GetAdminUnitsByParentId",
                    type: "Get",
                    data: { Id: $("#village").val() }
                })
            }

            Promise.resolve(managerThroughfares())
           .then(function (resultArray) {
               console.log(resultArray);
               resultArray.map(function (item) {
                   $("#managerThroughfare").append("<option value =" + item.Id + ">" + item.Name + "</option>");
               })
           })
           .catch(function (err) {
               console.log(err.message);
           });
        })
        .catch(function (err) {
            console.log(err.message);
        });
    
})


