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
        $(".PotentialClientMain").css("display", "none");
        $(".PotentialSellerMain").css("display", "none");
        $("#ReceivedMessagesMain").css("display", "block");
        $("#SentMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "block");
        $(".mesajMain").css("display", "block");
        $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
        $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        $("#SentMessagesSortedForDateAsc").css("display", "none");
        $("#SentMessagesSortedForDateDes").css("display", "none");
    })

    $("#SentMessages").click(function () {
        $("#SentMessagesMain").css("display", "block");
        $("#ReceivedMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "block");
        $("#InboxMessagesSelect").css("display", "none");
        $(".mesajMain").css("display", "block");
        $(".PotentialClientMain").css("display", "none");
        $(".PotentialSellerMain").css("display", "none");
        $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
        $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        $("#SentMessagesSortedForDateAsc").css("display", "none");
        $("#SentMessagesSortedForDateDes").css("display", "none");
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

    $("#personalEmail").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "block");
        $("#currentPasswordBody").css("display", "none");
    })
    $("#personalInfoButton").click(function () {
        $("#personalInfos").css("display", "block");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "none");
    })

    $("#currentPassword").click(function () {
        $("#personalInfos").css("display", "none");
        $("#personalEmail2").css("display", "none");
        $("#currentPasswordBody").css("display", "block");
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
            url: "/SpecialSummary/CheckPassword",
            type: "Get",
            data: { password: $("#oldPassword").val() },
            success: function (result) {
                if (result === "true") {
                    if ($("#newPassword1").val() == $("#newPassword2").val()) {
                        if ($("#newPassword1").val().length >= 8) {
                            $.ajax({
                                url: "/SpecialSummary/ChangePassword",
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
