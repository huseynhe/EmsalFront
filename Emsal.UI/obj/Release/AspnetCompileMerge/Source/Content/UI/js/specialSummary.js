$(function () {

    function convertTodate(elem) {
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

    function sortArray(arrList) {
        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {
            for (var j = i + 1; j < arr.length; j++) {
                if (arr[i].total_price < arr[j].total_price) {
                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }

    function sortArrayDes(arrList) {
        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {
            for (var j = i + 1; j < arr.length; j++) {
                if (arr[i].total_price > arr[j].total_price) {
                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }


    function sortArrayForDueDateDes(arrList) {

        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {

            for (var j = i + 1; j < arr.length; j++) {

                if (arr[i].endDate < arr[j].endDate) {


                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }

    function sortArrayForDueDateAsc(arrList) {

        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {

            for (var j = i + 1; j < arr.length; j++) {

                if (arr[i].endDate > arr[j].endDate) {


                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }

    function sortMessagesForDueDateDes(arrList) {
        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {

            for (var j = i + 1; j < arr.length; j++) {

                if (arr[i].createdDate > arr[j].createdDate) {


                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }

    function sortMessagesForDueDateAsc(arrList) {
        var arr = [];
        arrList.map(function (item) {
            arr.push(item);
        })
        for (var i = 0; i < arr.length; i++) {

            for (var j = i + 1; j < arr.length; j++) {

                if (arr[i].createdDate < arr[j].createdDate) {


                    var c = arr[j];
                    arr[j] = arr[i];
                    arr[i] = c;
                }
            }
        }

        return arr;
    }

    $.ajax({
        url: "/SpecialSummary/UpdateOnAirOffers",
        type: "Get",
        data: { UserID: 1 },
        error: function (e) {
            console.log(e);
        }
    })

   
    $.ajax({
        url: "/SpecialSummary/GetUserById",
        type: "Get",
        data: { Id: 1 },
        success: function (data) {
            $(".adSoyad").html(data.Person.Name + " " + data.Person.Surname);
            $(".userNameEkle").html(data.User.Username);
            $(".education").html(data.EducationLevel);
            $(".job").html(data.Job);
            //convert int to date
            var date = convertTodate(data.Person);
            ////////////////////////////
            $(".birthday").html(date);
            $(".gender").html(data.Person.gender);
            $("#upUserName").val(data.User.Username);
            $("#upName").val(data.Person.Name);
            localStorage.setItem("name", $("#upName").val());
            $("#upSurname").val(data.Person.Surname);
            $("#upBirthDate").val(date);
            $("#upGender").val(data.Person.gender);
            $("#upJob").val(data.Job);

            $("#currentEmail").html(data.User.Email);
            $("#updateEmail").val($("#currentEmail").html());
        },
        error: function (e) {
            return e;
        }
    })
    var name = localStorage.getItem("name")

    //var enumUserType = function () {
    //    return $.ajax({
    //        url: "/SpecialSummary/GetRoleByName",
    //        type: "Get",
    //        data: { name: "fizikişexs" }
    //    })
    //}

    //Promise.resolve(enumUserType())
        //.then(function (userTypeId) {
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
        //    })
        })



    //Promise.resolve(enumUserType())
    //    .then(function(userType){
            $("#updateEmailAction").click(function () {
                $.ajax({
                    url: "/SpecialSummary/UpdateEmail",
                    type: "Get",
                    data: {email: $("#updateEmail").val() },
                    success: function (data) {
                        location.reload();
                    },
                    error: function (e) {
                        return e;
                    }
                })
        //    })

        })


    //Promise.resolve(enumUserType())
    //    .then(function (userType) {
            $("#changeDetails").on("click", function () {
                $.ajax({
                    url: "/SpecialSummary/UpdateUser",
                    type: "Get",
                    data: {userName: $("#upUserName").val(), gender: $("#upGender").val(), educationId: $("#upEducation").val(), jobId: $("#upJob").val(), email: $("#updateEmail").val() },
                    success: function (data) {
                        location.reload();
                    },
                    error: function (e) {
                        return e;
                    }
                })
        //    })
        })

    //on air offers to the view
    $("#y_olan").on("click", function () {
        $("#onAirSortedForDateAsc").css("display", "none");
        $("#onAirSortedForDateDes").css("display", "none");
        $("#onAirSortedForPriceAsc").css("display", "none");
        $("#onAirSortedForPriceDes").css("display", "none");

        $("#yOlanTeklifler").css("display", "block");
    })

    //on air offers sorted for price asc

    $("#teklif").on("change", function () {
        if ($("#teklif").val() === "4") {
            $("#onAirSortedForDateAsc").css("display", "none");
            $("#onAirSortedForDateDes").css("display", "none");
            $("#onAirSortedForPriceDes").css("display", "none");
            $("#onAirSortedForPriceAsc").css("display", "block");
            $("#yOlanTeklifler").css("display", "none");
        }


    })

    //on air offers sorted for price desc

    $("#teklif").on("change", function () {
        if ($("#teklif").val() === "5") {
            $("#onAirSortedForDateAsc").css("display", "none");
            $("#onAirSortedForDateDes").css("display", "none");
            $("#onAirSortedForPriceAsc").css("display", "none");
            $("#onAirSortedForPriceDes").css("display", "block");
            $("#yOlanTeklifler").css("display", "none");
        }


    })

    //on air offers sorted for due date asc

    $("#teklif").on("change", function () {
        if ($("#teklif").val() === "2") {
            $("#onAirSortedForDateAsc").css("display", "block");
            $("#yOlanTeklifler").css("display", "none");
            $("#onAirSortedForPriceAsc").css("display", "none");
            $("#onAirSortedForPriceDes").css("display", "none");
            $("#onAirSortedForDateDes").css("display", "none");
        }

    })

    //on air offer sorted for due date des

    $("#teklif").on("change", function () {
        if ($("#teklif").val() === "3") {
            $("#onAirSortedForDateAsc").css("display", "none");
            $("#onAirSortedForDateDes").css("display", "block");
            $("#yOlanTeklifler").css("display", "none");
            $("#onAirSortedForPriceDes").css("display", "none");
            $("#onAirSortedForPriceAsc").css("display", "none");

        }

    })

    //bildirişe göre
    $("#teklif").on("change", function () {
        if ($("#teklif").val() === "1") {
            $("#onAirSortedForDateAsc").css("display", "none");
            $("#onAirSortedForDateDes").css("display", "none");
            $("#yOlanTeklifler").css("display", "block");
            $("#onAirSortedForPriceAsc").css("display", "none");
            $("#onAirSortedForPriceDes").css("display", "none");


        }
    })


    //off air offers to the view
    $("#y_cixan").on("click", function () {
        $("#yCixanTeklifler").css("display", "block");
        $("#offAirSortedForDateAsc").css("display", "none");
        $("#offAirSortedForDateDes").css("display", "none");
        $("#offAirSortedForPriceAsc").css("display", "none");
        $("#offAirSortedForPriceDes").css("display", "none");
    })
  
    //off air offers sorted for price asc

    $("#deprOffer").on("change", function () {
        if ($("#deprOffer").val() === "4") {
            $("#yCixanTeklifler").css("display", "none");
            $("#offAirSortedForDateAsc").css("display", "none");
            $("#offAirSortedForDateDes").css("display", "none");
            $("#offAirSortedForPriceAsc").css("display", "block");
            $("#offAirSortedForPriceDes").css("display", "none");
        }
    })

    //off air offers sorted for price desc

    $("#deprOffer").on("change", function () {
        if ($("#deprOffer").val() === "5") {
            $("#yCixanTeklifler").css("display", "none");
            $("#offAirSortedForDateAsc").css("display", "none");
            $("#offAirSortedForDateDes").css("display", "none");
            $("#offAirSortedForPriceAsc").css("display", "none");
            $("#offAirSortedForPriceDes").css("display", "block");
        }
    })

    //off air offers sorted for due date asc

    $("#deprOffer").on("change", function () {
        if ($("#deprOffer").val() === "2") {
            $("#yCixanTeklifler").css("display", "none");
            $("#offAirSortedForDateAsc").css("display", "block");
            $("#offAirSortedForDateDes").css("display", "none");
            $("#offAirSortedForPriceAsc").css("display", "none");
            $("#offAirSortedForPriceDes").css("display", "none");
        }

    })

    //off air offer sorted for due date des

    $("#deprOffer").on("change", function () {
        if ($("#deprOffer").val() === "3") {
            $("#yCixanTeklifler").css("display", "none");
            $("#offAirSortedForDateAsc").css("display", "none");
            $("#offAirSortedForDateDes").css("display", "block");
            $("#offAirSortedForPriceAsc").css("display", "none");
            $("#offAirSortedForPriceDes").css("display", "none");
        }

    })

    //bildirişe göre
    $("#deprOffer").on("change", function () {
        if ($("#deprOffer").val() === "1") {
            $("#yCixanTeklifler").css("display", "none");
            $("#offAirSortedForDateAsc").css("display", "none");
            $("#offAirSortedForDateDes").css("display", "none");
            $("#offAirSortedForPriceAsc").css("display", "block");
            $("#offAirSortedForPriceDes").css("display", "none");
        }
    })

    //not confirmed potential productions to view
    $("#y_olmayan").on("click", function () {
        $("#tesdiqlenmemisPotensial").css("display", "block");
        $("#notConfirmedPotentialForPriceAsc").css("display", "none");
        $("#notconfirmedPotentialForPriceDes").css("display", "none");
    })

    //not confirmed potential productions sorted for price asc
    $("#notConfirmedPotential").on("change", function () {
        if ($("#notConfirmedPotential").val() === "2") {
            $("#tesdiqlenmemisPotensial").css("display", "none");
            $("#notConfirmedPotentialForPriceAsc").css("display", "block");
            $("#notconfirmedPotentialForPriceDes").css("display", "none");
        }


    })

    //not confirmed potential productions sorted for price des
    $("#notConfirmedPotential").on("change", function () {
        if ($("#notConfirmedPotential").val() === "3") {
            $("#tesdiqlenmemisPotensial").css("display", "none");
            $("#notConfirmedPotentialForPriceAsc").css("display", "none");
            $("#notConfirmedPotentialForPriceDes").css("display", "block");
        }


    })

    //bildirişe göre
    $("#notConfirmedPotential").on("change", function () {
        if ($("#notConfirmedPotential").val() === "1") {
            $("#tesdiqlenmemisPotensial").css("display", "block");
            $("#notconfirmedPotentialForPriceAsc").css("display", "none");
            $("#notconfirmedPotentialForPriceDes").css("display", "none");
        }
    })

    //confirmed potential productions to view
    $("#y_tesdiq").on("click", function () {
        $("#tesdiqlenmisPotensial").css("display", "block");
        $("#confirmedPotentialForPriceAsc").css("display", "none");
        $("#confirmedPotentialForPriceDes").css("display", "none");
    })

    //confirmed potential productions sorted for price asc
    $("#confirmedPotential").on("change", function () {
        if ($("#confirmedPotential").val() === "2") {
            $("#tesdiqlenmisPotensial").css("display", "none");
            $("#confirmedPotentialForPriceAsc").css("display", "block");
            $("#confirmedPotentialForPriceDes").css("display", "none");
        }
    })


    //confirmed potential productions sorted for price des

    $("#confirmedPotential").on("change", function () {
        if ($("#confirmedPotential").val() === "3") {
            $("#tesdiqlenmisPotensial").css("display", "none");
            $("#confirmedPotentialForPriceAsc").css("display", "none");
            $("#confirmedPotentialForPriceDes").css("display", "block");
        }
    })

    //bildirişe göre

    $("#confirmedPotential").on("change", function () {
        if ($("#confirmedPotential").val() === "1") {
            $("#tesdiqlenmisPotensial").css("display", "block");
            $("#confirmedPotentialForPriceAsc").css("display", "none");
            $("#confirmedPotentialForPriceDes").css("display", "none");
        }
    })


    //sent messages to view

    $("#sentMessages").on("click", function () {
        $("#sentMessagesSelect").css("display", "block");
        $("#inboxMessagesSelect").css("display", "none");
        $.ajax({
            url: "/SpecialSummary/GetSentMessagesByFromUserId",
            type: "Get",
            data: { userId: 1 },
            success: function (messages) {
                $(".mesajMain").css("display", "block");
                $(".yayinda_olan").css("display", "none");
                $(".yayinda_tesdiq").css("display", "none");
                $(".yayinda_olmayan").css("display", "none");
                $(".yayimdan_cixan").css("display", "none");
                $("#receivedMessages").html("");
                if (messages.length == 0) {
                    $("#messageMain").html("Heç ne tapılmadı");
                }
                else {
                    $("#messageMain").html(messages.length + " " + "mesaj tapıldı");
                    messages.map(function (message) {
                        var createdDate = convertToMessageDate(message);
                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.toUserID },
                            success: function (user) {
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                                    user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                              "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                                      "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                                   "</ul></div></td></tr></table></div>")
                            },
                            error: function (e) {
                                return e;
                            }
                        })



                    })

                }



            },
            error: function () {

            }

        })
    })

    //sent messages sorted for date des 
    $("#sentMessagesSelect").on("change", function () {
        if ($("#sentMessagesSelect").val() == "2") {
            $.ajax({
                url: "/SpecialSummary/GetSentMessagesByFromUserId",
                type: "Get",
                data: { userId: 1 },
                success: function (messages) {
                    $(".mesajMain").css("display", "block");
                    $(".yayinda_olan").css("display", "none");
                    $(".yayinda_tesdiq").css("display", "none");
                    $(".yayinda_olmayan").css("display", "none");
                    $(".yayimdan_cixan").css("display", "none");
                    $("#receivedMessages").html("");

                    var sortedMessages = sortMessagesForDueDateAsc(messages);
                    sortedMessages.map(function (message) {

                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.toUserID },
                            success: function (user) {

                                var createdDate = convertToMessageDate(message);
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                        user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                  "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                          "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                       "</ul></div></td></tr></table></div>")

                            },
                            error: function (e) {
                                return e;
                            }
                        })



                    })



                },
                error: function () {

                }

            })

        }
    })

    //sent messages sorted for date asc
    $("#sentMessagesSelect").on("change", function () {
        if ($("#sentMessagesSelect").val() == "1") {
            $.ajax({
                url: "/SpecialSummary/GetSentMessagesByFromUserId",
                type: "Get",
                data: { userId: 1 },
                success: function (messages) {
                    $(".mesajMain").css("display", "block");
                    $(".yayinda_olan").css("display", "none");
                    $(".yayinda_tesdiq").css("display", "none");
                    $(".yayinda_olmayan").css("display", "none");
                    $(".yayimdan_cixan").css("display", "none");
                    $("#receivedMessages").html("");
                    var sortedMessages = sortMessagesForDueDateDes(messages);
                    sortedMessages.map(function (message) {

                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.toUserID },
                            success: function (user) {

                                var createdDate = convertToMessageDate(message);
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                        user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                  "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                          "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                       "</ul></div></td></tr></table></div>")

                            },
                            error: function (e) {
                                return e;
                            }
                        })
                    })

                },
                error: function () {

                }

            })

        }
    })



    //received messages to view
    $("#inbox").on("click", function () {
        $("#sentMessagesSelect").css("display", "none");
        $("#inboxMessagesSelect").css("display", "block");
        $.ajax({
            url: "/SpecialSummary/GetInboxMessagesByToUserId",
            type: "Get",
            data: { userId: 1 },
            success: function (messages) {
                $(".mesajMain").css("display", "block");
                $(".yayinda_olan").css("display", "none");
                $(".yayinda_tesdiq").css("display", "none");
                $(".yayinda_olmayan").css("display", "none");
                $(".yayimdan_cixan").css("display", "none");
                $("#receivedMessages").html("");
                if (messages.length == 0) {
                    $("#messageMain").html("Heç ne tapılmadı");
                }
                else {
                    $("#messageMain").html(messages.length + " " + "mesaj tapıldı");
                    messages.map(function (message) {
                        var createdDate = convertToMessageDate(message);
                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.fromUserID },
                            success: function (user) {
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                                    user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                              "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                                      "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                                   "</ul></div></td></tr></table></div>")
                            },
                            error: function (e) {
                                return e;
                            }
                        })

                    })

                }


            },
            error: function () {

            }

        })
    })

    //received messages sorted for date des
    $("#inboxMessagesSelect").on("change", function () {
        if ($("#inboxMessagesSelect").val() == "2") {
            $.ajax({
                url: "/SpecialSummary/GetInboxMessagesByToUserId",
                type: "Get",
                data: { userId: 1 },
                success: function (messages) {
                    $(".mesajMain").css("display", "block");
                    $(".yayinda_olan").css("display", "none");
                    $(".yayinda_tesdiq").css("display", "none");
                    $(".yayinda_olmayan").css("display", "none");
                    $(".yayimdan_cixan").css("display", "none");
                    $("#receivedMessages").html("");
                    var sortedMessages = sortMessagesForDueDateAsc(messages);
                    sortedMessages.map(function (message) {

                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.fromUserID },
                            success: function (user) {

                                var createdDate = convertToMessageDate(message);
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                        user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                  "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                          "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                       "</ul></div></td></tr></table></div>")

                            },
                            error: function (e) {
                                return e;
                            }
                        })
                    })

                },
                error: function () {

                }

            })

        }

    })


    //received messaged sorted for date asc

    $("#inboxMessagesSelect").on("change", function () {
        if ($("#inboxMessagesSelect").val() == "1") {
            $.ajax({
                url: "/SpecialSummary/GetInboxMessagesByToUserId",
                type: "Get",
                data: { userId: 1 },
                success: function (messages) {
                    $(".mesajMain").css("display", "block");
                    $(".yayinda_olan").css("display", "none");
                    $(".yayinda_tesdiq").css("display", "none");
                    $(".yayinda_olmayan").css("display", "none");
                    $(".yayimdan_cixan").css("display", "none");
                    $("#receivedMessages").html("");
                    var sortedMessages = sortMessagesForDueDateDes(messages);
                    sortedMessages.map(function (message) {

                        $.ajax({
                            url: "/SpecialSummary/GetUserById",
                            type: "Get",
                            data: { Id: message.fromUserID },
                            success: function (user) {

                                var createdDate = convertToMessageDate(message);
                                $("#receivedMessages").append("<div class='white_fon'><table class='responcive'><tr><td colspan='2'><span><a href = ''>" +
                        user.User.Username + "</a><span></td></tr><tr><td><img src='http://emsal.az/staticFiles/male.png' style = 'max-width:100px;max-height:77px'></td><td>" +
                  "<p>" + message.message + "</p><p>Mesajın tarixi: " + createdDate + "</p></td><td class='text-right'><div class='btn-group'><button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>Əməliyyatlar <span class='caret'></span></button><ul class='dropdown-menu'>" +
                          "<li><a href='/SpecialSummary/DeleteComMessage?Id=" + message.Id + "'>Sil</a></li></li>" +
                       "</ul></div></td></tr></table></div>")

                            },
                            error: function (e) {
                                return e;
                            }
                        })
                    })

                },
                error: function () {

                }

            })

        }

    })



})