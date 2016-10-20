$(function () {
    $("#per_inf").click(function () {
        $(".per_information").toggle();
        return false;
    });

    $("#per_mes").click(function () {
        $(".messages").toggle();
        return false;
    });



    $("#personalEmail").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "block");
        $("#currentPasswordBody").css("display", "none");
        $("#personalPhoneMain").css("display", "none");

    })
    $("#personalPhone").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#personalPhoneMain").css("display", "block");
    })
    
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#personalPhoneMain").css("display", "none");

    })

    $("#currentPassword").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "block");
    })

    $("#r_Offer").click(function () {
        $(".yayinda_olan").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".yayinda_tesdiq").css("display", "none");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "block");
        $(".reEdited").css("display", "none");

    })


    $("#y_tesdiq").click(function () {
        $(".yayinda_olan").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".yayinda_tesdiq").css("display", "block");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "none");


    });

    $("#y_olan").click(function () {
        $(".yayinda_tesdiq").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".yayinda_olan").css("display", "block");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "none");

    });

    $("#y_olmayan").click(function () {
        $(".yayinda_tesdiq").css("display", "none");
        $(".yayinda_olan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".yayinda_olmayan").css("display", "block");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "none");

    });

    $("#y_cixan").click(function () {
        $(".yayinda_tesdiq").css("display", "none");
        $(".yayinda_olan").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "block");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "none");

    });

    $("#t_lenen").click(function () {
        $(".yayinda_tesdiq").css("display", "none");
        $(".yayinda_olan").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "block");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "none");

    });
    $("#reEditedOffers").click(function () {
        $(".yayinda_tesdiq").css("display", "none");
        $(".yayinda_olan").css("display", "none");
        $(".yayinda_olmayan").css("display", "none");
        $(".yayimdan_cixan").css("display", "none");
        $(".mesajMain").css("display", "none");
        $(".tesdiqlenen").css("display", "none");
        $(".rejected").css("display", "none");
        $(".reEdited").css("display", "block");
    })



    function convertTodate(elem) {
        if (elem != null) {
            if (elem.endDate === undefined) {
                if (elem.birtday === undefined) {
                    date = new Date(elem.createdDate * 1000);
                }
                else {
                    date = new Date(elem.birtday * 1000);
                }
            }
            else {
                date = new Date(elem.endDate * 1000);
            }
        }
        else {
            date = new Date();
        }
       
        var month = date.getUTCMonth() + 1;
        var day = date.getUTCDate();
        var year = date.getUTCFullYear();
        endDate = day + "/" + month + "/" + year;
        
        return endDate;
    }
    function convertToMessageDate(elem) {
        var year = Math.floor(elem.createdDate / (10000000000000));
        var month = Math.floor(elem.createdDate / (100000000000) - year *100);
        var day = Math.floor(elem.createdDate / (1000000000) - Math.floor(elem.createdDate / (100000000000) - year * 100) *100 - year*10000);
        endDate = day + "/" + month + "/" + year;

        return endDate;
    }

   

    $.ajax({
        url: "/SpecialSummary/GetUserById",
        type: "Get",
        data: { Id: 1 },
        success: function (data) {
            $(".adSoyad").html(data.ForeignOrganisation.name);
            $(".userNameEkle").html(data.User.Username);
            $(".education").html(data.EducationLevel);
            $(".job").html(data.Job);
            //convert int to date
            var date = convertTodate(data.Person);
            ////////////////////////////
            $(".birthday").html(date);
            if (data.Person != null) {
                $(".aDbbSoyad").html(data.Person.Name + "," + data.Person.Surname)
                $(".gender").html(data.Person.gender);
                $("#upGender").val(data.Person.gender);
                $("#upName").val(data.Person.Name);
                $("#upSurname").val(data.Person.Surname);
            }
            
            $("#upUserName").val(data.User.Username);
            
            localStorage.setItem("name", $("#upName").val());
            
            $("#upBirthDate").val(date);
           
            $("#upJob").val(data.Job);

            $("#currentEmail").html(data.User.Email);
            $("#updateEmail").val($("#currentEmail").html());
        },
        error: function (e) {
            return e;
        }
    })
    var name = localStorage.getItem("name")

    
            $("#changePassword").click(function () {
                $.ajax({
                    url: "/Ordinary/CheckPassword",
                    type: "Get",
                    data: { password: $("#oldPassword").val() },
                    success: function (result) {
                        if (result === "true") {
                            if ($("#newPassword1").val() == $("#newPassword2").val()) {
                                if ($("#newPassword1").val().length >= 8) {
                                    $.ajax({
                                        url: "/Ordinary/ChangePassword",
                                        type: "Get",
                                        data: { password: $("#newPassword1").val()},
                                        success: function (item) {
                                            if (item == "governmentOrganisationSpecialSummary") {
                                                window.location.href = "/GovernmentOrganisationSpecialSummary/Index";
                                            }
                                            else {
                                                window.location.href = "/SpecialSummary/Index";
                                            }
                                        },
                                        error: function (e) {
                                            return e;
                                        }
                                    })
                                }

                                else {
                                    $("#wrongPassword").html("Şifreniz 8 reqemden böyük olmalıdır");
                                }
                            }
                            else {
                                $("#wrongPassword").html("Tekrar şifre yeni şifre ile eyni deyil");
                            }

                        }
                        else {
                            $("#wrongPassword").html("Şifreniz möcud şifre ile eyni deyil");
                        }
                    },
                    error: function (e) {
                        return e;
                    }
                })
        //    })
        })



            $("#updateEmailAction").click(function () {
                $.ajax({
                    url: "/Ordinary/UpdateEmail",
                    type: "Get",
                    data: {email: $("#updateEmail").val() },
                    success: function (data) {
                        location.reload();
                    },
                    error: function (e) {
                        return e;
                    }
                })
  

        })


    
            $("#changeDetails").on("click", function () {
                $.ajax({
                    url: "/Ordinary/UpdateUser",
                    type: "Get",
                    data: {userName: $("#upUserName").val(), gender: $("#upGender").val(), educationId: $("#upEducation").val(), jobId: $("#upJobb").val(), email: $("#updateEmail").val() },
                    success: function (data) {
                        if (data == "governmentOrganisationSpecialSummary") {
                            window.location.href = "/GovernmentOrganisationSpecialSummary/Index";
                        }
                        else {
                            window.location.href = "/SpecialSummary/Index";
                        }
                    },
                    error: function (e) {
                        return e;
                    }
                })
     
            })
})