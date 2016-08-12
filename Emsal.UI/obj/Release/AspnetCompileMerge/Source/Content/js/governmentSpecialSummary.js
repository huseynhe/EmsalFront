$(function () {
    $("#contracts").on("click", function () {
        $(".ContractMain").css("display", "block");
        $(".OrderMain").css("display", "none");
        $(".ExpiredOrderMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "none");
        $(".mesajMain").css("display", "none");
    })
    $("#orders").on("click", function () {
        $(".ContractMain").css("display", "none");
        $(".OrderMain").css("display", "block");
        $(".ExpiredOrderMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "none");
        $(".mesajMain").css("display", "none");
    })
    $("#expiredOrders").on("click", function () {
        $(".ContractMain").css("display", "none");
        $(".OrderMain").css("display", "none");
        $(".ExpiredOrderMain").css("display", "block");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "none");
        $(".mesajMain").css("display", "none");
    })



    //user tablosu


    $("#user_inf").click(function () {
        $(".per_information").toggle();
        return false;
    });

    $("#user_mes").click(function () {
        $(".messages").toggle();
        return false;
    });



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
    var enumUserType = function () {
        return $.ajax({
            url: "/SpecialSummary/GetEnumValueByName",
            type: "Get",
            data: { name: "dövletteşkilati"}
        })
    }
    Promise.resolve(enumUserType())
        .then(function (userTypeId) {
            $("#updateEmailAction").click(function () {
                $.ajax({
                    url: "/SpecialSummary/UpdateEmail",
                    type: "Get",
                    data: { email: $("#updateEmail").val() },
                    success: function (data) {
                        location.reload();
                    },
                    error: function (e) {
                        return e;
                    }
                })
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
                                data: { password: $("#newPassword1").val()},
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


    $("#Inbox").click(function () {
        $("#ReceivedMessagesMain").css("display", "block");
        $("#SentMessagesMain").css("display", "none");
        $("#SentMessagesSelect").css("display", "none");
        $("#InboxMessagesSelect").css("display", "block");
        $(".mesajMain").css("display", "block");
        $(".ContractMain").css("display", "none");
        $(".OrderMain").css("display", "none");
        $(".ExpiredOrderMain").css("display", "none");
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
        $(".ContractMain").css("display", "none");
        $(".OrderMain").css("display", "none");
        $(".ExpiredOrderMain").css("display", "none")
        $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
        $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        $("#SentMessagesSortedForDateAsc").css("display", "none");
        $("#SentMessagesSortedForDateDes").css("display", "none");
    })

    $("#InboxMessagesSelect").on("change", function () {
        if ($("#InboxMessagesSelect").val() == "1")
        {
            $("#ReceivedMessagesSortedForDateAsc").css("display", "block");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $("#SentMessagesSelect").css("display", "none");
            $("#InboxMessagesSelect").css("display", "block");
            $(".mesajMain").css("display", "block");
            $(".ContractMain").css("display", "none");
            $(".OrderMain").css("display", "none");
            $(".ExpiredOrderMain").css("display", "none");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#SentMessagesSortedForDateDes").css("display", "none");
        }
        if($("#InboxMessagesSelect").val() == "2")
        {
            $("#ReceivedMessagesSortedForDateDes").css("display", "block");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $("#SentMessagesSelect").css("display", "none");
            $("#InboxMessagesSelect").css("display", "block");
            $(".mesajMain").css("display", "block");
            $(".ContractMain").css("display", "none");
            $(".OrderMain").css("display", "none");
            $(".ExpiredOrderMain").css("display", "none");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#SentMessagesSortedForDateDes").css("display", "none");
        }

    })

    $("#SentMessagesSelect").on("change", function ()
    {
        if ($("#SentMessagesSelect").val() == "1") {
            $("#SentMessagesSortedForDateAsc").css("display", "block");
            $("#SentMessagesSortedForDateDes").css("display", "none");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $(".mesajMain").css("display", "block");
            $(".ContractMain").css("display", "none");
            $(".OrderMain").css("display", "none");
            $(".ExpiredOrderMain").css("display", "none");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");

        }
        if ($("#SentMessagesSelect").val() == "2") {
            $("#SentMessagesSortedForDateDes").css("display", "block");
            $("#SentMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesSortedForDateAsc").css("display", "none");
            $("#ReceivedMessagesMain").css("display", "none");
            $("#SentMessagesMain").css("display", "none");
            $(".mesajMain").css("display", "block");
            $(".ContractMain").css("display", "none");
            $(".OrderMain").css("display", "none");
            $(".ExpiredOrderMain").css("display", "none");
            $("#ReceivedMessagesSortedForDateDes").css("display", "none");
        }
    }
    );

})