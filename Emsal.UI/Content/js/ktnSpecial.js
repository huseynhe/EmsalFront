$(function () {

    $("#created").click(function () {
        $("#createdUsers").css("display", "block");
        $("#userInfoos").css("display", "none");
        $("#persInfHead").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $(".mesajlar").css("display", "none");
    })
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#createdUsers").css("display", "none");
        $("#userInfoos").css("display", "block");
        $("#persInfHead").css("display", "block");
    })
    $("#user_inf").click(function () {
        $(".per_information").toggle();
        return false;
    });

    $("#user_mes").click(function () {
        $(".messages").toggle();
        return false;
    });

    $("#Inbox").click(function () {
        $("#SentMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $(".mesajlar").css("display", "block");
        $(".mesageCome").css("display", "block");
        $("#createdUsers").css("display", "none");
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $(".messageSentt").css("display", "none");

        $("#user_mes").click();
    })

    $("#SentMessages").click(function () {
        $("#SentMessagesMain").css("display", "block");
        $("#ReceivedMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "block");
        $("#InboxMessagesSelect").css("display", "none");
        $(".messageSentt").css("display", "block");
        $("#createdUsers").css("display", "none");
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $(".mesajlar").css("display", "block");
        $(".mesageCome").css("display", "none");

        $("#user_mes").click();
    })


    $("#personalEmail").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "block");
        $("#currentPasswordBody").css("display", "none");
        $("#createdUsers").css("display", "none");
        $(".mesajlar").css("display", "none");
    })
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#createdUsers").css("display", "none");
        $(".mesajlar").css("display", "none");

    })

    $("#currentPassword").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "block");
        $("#createdUsers").css("display", "none");
        $(".mesajlar").css("display", "none");
    })

    $("#updateEmailAction").click(function () {
        $.ajax({
            url: "/SpecialSummary/UpdateEmail",
            type: "Get",
            data: { email: $("#updateEmail").val() },
            success: function (data) {
                location.reload();
            },
            error: function (e) {
                console.log(e);
            }
        })
    })
    $("#changePassword").click(function () {
        $.ajax({
            url: "/KTNSpecialSummary/CheckPassword",
            type: "Get",
            data: { password: $("#oldPassword").val() },
            success: function (result) {
                if (result === "true") {
                    if ($("#newPassword1").val() == $("#newPassword2").val()) {
                        if ($("#newPassword1").val().length >= 8) {
                            $.ajax({
                                url: "/KTNSpecialSummary/ChangePassword",
                                type: "Get",
                                data: { password: $("#newPassword1").val() },
                                success: function (item) {
                                    location.reload();
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
    })

})
