$(function(){
    
    $("#potentialClients").on("click", function () {
        $(".PotentialClientMain").css("display", "block");
        $(".PotentialSellerMain").css("display", "none");
    })
    $("#potentialSellers").on("click", function () {
        $(".PotentialClientMain").css("display", "none");
        $(".PotentialSellerMain").css("display", "block");
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
        $("#userInfoos").css("display", "none");
        $("#persInfHead").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#mesagessMain").css("display", "block");

    })

    $("#SentMessages").click(function () {
        $("#userInfoos").css("display", "none");
        $("#persInfHead").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#mesagessMain").css("display", "block");
       
    })

    $("#InboxMessagesSelect").on("change", function () {
        if ($("#InboxMessagesSelect").val() == "1") {
            $(".PotentialClientMain").css("display", "none");
            $(".PotentialSellerMain").css("display", "none");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "block");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $("#SentMessagesSelect").css("display", "none");
            $("#InboxMessagesSelect").css("display", "block");
            $(".mesajMain").css("display", "block");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#SentMessagesSortedForDateDes").css("display", "none");
            $("#createdUsers").css("display", "none");

        }
        if ($("#InboxMessagesSelect").val() == "2") {
            $(".PotentialClientMain").css("display", "none");
            $(".PotentialSellerMain").css("display", "none");
            $("#ReceivedMessagesSortedForDateDes").css("display", "block");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $("#SentMessagesSelect").css("display", "none");
            $("#InboxMessagesSelect").css("display", "block");
            $(".mesajMain").css("display", "block");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#SentMessagesSortedForDateDes").css("display", "none");
            $("#createdUsers").css("display", "none");

        }

    })

    $("#SentMessagesSelect").on("change", function () {
        if ($("#SentMessagesSelect").val() == "1") {
            $(".PotentialClientMain").css("display", "none");
            $(".PotentialSellerMain").css("display", "none");
            $("#SentMessagesSortedForDateAsc").css("display", "block");
            $("#SentMessagesSortedForDateDes").css("display", "none");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $(".mesajMain").css("display", "block");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");

        }
        if ($("#SentMessagesSelect").val() == "2") {
            $(".PotentialClientMain").css("display", "none");
            $(".PotentialSellerMain").css("display", "none");
            $("#SentMessagesSortedForDateDes").css("display", "block");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $(".mesajMain").css("display", "block");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        }
    }
    );
    $("#created").click(function () {
        $("#createdUsers").css("display", "block");
        $("#userInfoos").css("display", "none");
        $("#persInfHead").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");

    })
    $("#personalEmail").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "block");
        $("#currentPasswordBody").css("display", "none");
        $("#createdUsers").css("display", "none");

    })
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
        $("#createdUsers").css("display", "none");
        $("#userInfoos").css("display", "block");
        $("#persInfHead").css("display", "block");
        $("#createdUsers").css("display", "none");

    })

    $("#currentPassword").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "block");
        $("#createdUsers").css("display", "none");

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
            url: "/AsanXidmetSpecialSummary/CheckPassword",
            type: "Get",
            data: { password: $("#oldPassword").val() },
            success: function (result) {
                if (result === "true") {
                    if ($("#newPassword1").val() == $("#newPassword2").val()) {
                        if ($("#newPassword1").val().length >= 8) {
                            $.ajax({
                                url: "/AsanXidmetSpecialSummary/ChangePassword",
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
                            $("#wrongPassword").html("Şifrəniz 8 rəqəmden böyük olmalıdır");
                        }
                    }
                    else {
                        $("#wrongPassword").html("Təkrar şifrə yeni şifrə ilə eyni deyil");
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
