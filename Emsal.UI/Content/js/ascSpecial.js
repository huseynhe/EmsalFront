$(function () {

    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
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
        
        $("#ReceivedMessagesMain").css("display", "block");
        $("#SentMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "block");
        $(".mesajMain").css("display", "block");
        $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
        $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        $("#SentMessagesSortedForDateAsc").css("display", "none");
        $("#SentMessagesSortedForDateDes").css("display", "none");
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#user_mes").click();

    })

    $("#SentMessages").click(function () {
        $("#SentMessagesMain").css("display", "block");
        $("#ReceivedMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "block");
        $("#InboxMessagesSelect").css("display", "none");
        $(".mesajMain").css("display", "block");
        $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
        $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        $("#SentMessagesSortedForDateAsc").css("display", "none");
        $("#SentMessagesSortedForDateDes").css("display", "none");
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#user_mes").click();

    })

   

    $("#personalEmail").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "block");
        $("#currentPasswordBody").css("display", "none");
        $(".mesajMain").css("display", "none");

    })
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $(".mesajMain").css("display", "none");

    })

    $("#currentPassword").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "block");
        $(".mesajMain").css("display", "none");

    })

    $("#updateEmailAction").click(function () {
        $.ajax({
            url: "/Ordinary/UpdateEmail",
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
            url: "/ASCSpecialSummary/CheckPassword",
            type: "Get",
            data: { password: $("#oldPassword").val() },
            success: function (result) {
                if (result === "true") {
                    if ($("#newPassword1").val() == $("#newPassword2").val()) {
                        if ($("#newPassword1").val().length >= 8) {
                            $.ajax({
                                url: "/ASCSpecialSummary/ChangePassword",
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
                            $("#wrongPassword").html("Şifrəniz 8 rəqəmdən böyük olmalıdır");
                        }
                    }
                    else {
                        $("#wrongPassword").html("Təkrar şifrə yeni şifre ilə eyni deyil");
                    }

                }
                else {
                    $("#wrongPassword").html("Şifrəniz möcud şifrə ilə eyni deyil");
                }
            },
            error: function (e) {
                return e;
            }
        })
    })

})
