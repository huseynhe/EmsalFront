$(document).ready(function() {

    //$("#per_inf").click(function () {
    //    $(".per_information").toggle();
    //    return false;
    //});

    //$("#per_mes").click(function () {
    //    $(".messages").toggle();
    //    return false;
    //});
	


    //$("#personalEmail").click(function () {
    //    $("#personalInfos").css("display", "none");
    //    $("#personalEmail2").css("display", "block");
    //    $("#currentPasswordBody").css("display", "none");
    //})
    //$("#personalInfoButton").click(function () {
    //    $("#personalInfos").css("display", "block");
    //    $("#personalEmail2").css("display", "none");
    //    $("#currentPasswordBody").css("display", "none");
    //})

    //$("#currentPassword").click(function () {
    //    $("#personalInfos").css("display", "none");
    //    $("#personalEmail2").css("display", "none");
    //    $("#currentPasswordBody").css("display", "block");
    //})

    //$("#r_Offer").click(function () {
    //    $(".yayinda_olan").css("display", "none");
    //    $(".yayinda_olmayan").css("display", "none");
    //    $(".yayimdan_cixan").css("display", "none");
    //    $(".yayinda_tesdiq").css("display", "none");
    //    $(".mesajMain").css("display", "none");
    //    $(".tesdiqlenen").css("display", "none");
    //    $(".rejected").css("display", "block");
    //    $(".reEdited").css("display", "none");

    //})
  

	//$("#y_tesdiq").click(function(){
	//	$(".yayinda_olan").css("display", "none");
	//	$(".yayinda_olmayan").css("display", "none");
	//	$(".yayimdan_cixan").css("display", "none");
	//	$(".yayinda_tesdiq").css("display", "block");
	//	$(".mesajMain").css("display", "none");
	//	$(".tesdiqlenen").css("display", "none");
	//	$(".rejected").css("display", "none");
	//	$(".reEdited").css("display", "none");


	//});
	
	//$("#y_olan").click(function(){
	//	$(".yayinda_tesdiq").css("display", "none");
	//	$(".yayinda_olmayan").css("display", "none");
	//	$(".yayimdan_cixan").css("display", "none");
	//	$(".yayinda_olan").css("display", "block");
	//	$(".mesajMain").css("display", "none");
	//	$(".tesdiqlenen").css("display", "none");
	//	$(".rejected").css("display", "none");
	//	$(".reEdited").css("display", "none");

	//});
	
	//$("#y_olmayan").click(function(){
	//	$(".yayinda_tesdiq").css("display", "none");
	//	$(".yayinda_olan").css("display", "none");
	//	$(".yayimdan_cixan").css("display", "none");
	//	$(".yayinda_olmayan").css("display", "block");
	//	$(".mesajMain").css("display", "none");
	//	$(".tesdiqlenen").css("display", "none");
	//	$(".rejected").css("display", "none");
	//	$(".reEdited").css("display", "none");

	//});
	
	//$("#y_cixan").click(function(){
	//	$(".yayinda_tesdiq").css("display", "none");
	//	$(".yayinda_olan").css("display", "none");
	//	$(".yayinda_olmayan").css("display", "none");
	//	$(".yayimdan_cixan").css("display", "block");
	//	$(".mesajMain").css("display", "none");
	//	$(".tesdiqlenen").css("display", "none");
	//	$(".rejected").css("display", "none");
	//	$(".reEdited").css("display", "none");

	//});
	
	//$("#t_lenen").click(function () {
	//    $(".yayinda_tesdiq").css("display", "none");
	//    $(".yayinda_olan").css("display", "none");
	//    $(".yayinda_olmayan").css("display", "none");
	//    $(".yayimdan_cixan").css("display", "none");
	//    $(".mesajMain").css("display", "none");
	//    $(".tesdiqlenen").css("display", "block");
	//    $(".rejected").css("display", "none");
	//    $(".reEdited").css("display", "none");

	//});
	//$("#reEditedOffers").click(function () {
	//    $(".yayinda_tesdiq").css("display", "none");
	//    $(".yayinda_olan").css("display", "none");
	//    $(".yayinda_olmayan").css("display", "none");
	//    $(".yayimdan_cixan").css("display", "none");
	//    $(".mesajMain").css("display", "none");
	//    $(".tesdiqlenen").css("display", "none");
	//    $(".rejected").css("display", "none");
	//    $(".reEdited").css("display", "block");
	//})
	
	$(function () {
		$('[data-toggle="tooltip"]').tooltip()
	})
	
	var count = 1;
	$('.panel-title .collapsed').click(function(){
		count++;
		if(count%2 == 0) {
			$('.panel-title span').addClass('glyphicon-menu-down').removeClass('glyphicon-menu-up');
		} else {
			$('.panel-title span').addClass('glyphicon-menu-up').removeClass('glyphicon-menu-down');
		}
	});
	
	$(".nav-tabs li:nth-child(5) a").click(function(){
		$('#headingOne .collapsed').click();
		$(".panel-body p:firstChild a").addClass("active");
	});
	
	
	$("#checkAll").click(function () {
    $(".check").prop('checked', $(this).prop('checked'));
	});


var sampleJson1 = {
ToolTipPosition: "bottom",
data: [{ order: 1, Text: "Foo", ToolTipText: "Step1-Foo", highlighted: true },
    { order: 2, Text: "Bar", ToolTipText: "Step2-Bar", highlighted: true },
    { order: 3, Text: "Baz", ToolTipText: "Step3-Baz", highlighted: false },
    { order: 4, Text: "Quux", ToolTipText: "Step4-Quux", highlighted: false }]
};    

//Invoking the plugin
$(document).ready(function () {
       // $("#tracker1").progressTracker(sampleJson1);
    });
	
	   /* for the google maps scripts*/

	//$(window).on("load", function () {
	//    google.maps.event.addDomListener(window, 'load', initialize());
	//})
	//$("#olke").on("change", function () {
	//    google.maps.event.addDomListener(window, 'load', initialize());
	//})
	//$("#seher").on("change", function () {
	//    google.maps.event.addDomListener(window, 'load', initialize(40, 49));
	//})
	//$("#village").on("change", function () {
	//    google.maps.event.addDomListener(window, 'load', initialize(40.5, 50));
	//})
	
	/***********************************************/
	/* progressbar(without stages) in addDetail ui script*/
	//$(function () {
	//    $("#progressbar").progressbar({
	//        value: 2
	//    })
	//});
    /*****************************************/

    /* expiry date script in addDetail */
	//$(function () {
	//    $('#expiryDate').datepicker();
	//})
    /******************************************/

    /* show phone number on the screen or not radio and checkboxes scripts*/
	$("input:radio[id='notShowPhone']").change(
               function () {
                   if ($(this).is(':checked')) {
                       $("#elaqe").html("<div><div class = 'row' style = 'margin-bottom:25px'><div class = 'col-xs-4 col-xs-offset-4'><img src = 'img/mektub.png' style = 'width:50px;height:50px;' class = 'img-responsive'></div></div><div><p style='text-align:center'>Bu seçimi etdiyiniz zaman, elanınıza baxanlar sizinlə sadəcə sistem üzərinden mesajlaşaraq elaqe saxlaya bilərlər. Elanda heçbir telefon nömrəniz görünməz.</p></div></div>");
                   }

               })
	$("input:radio[id='showPhone']").change(
        function () {
            if ($(this).is(':checked')) {
                $("#elaqe").html("<div class='row'><div class='col-xs-6'><p><b>Adı Soyadı</b></p></div><div class='col-xs-6'><p>Əhmədov Məmməd</p></div></div><div class='row'><div class='col-xs-6'><p><b>Ev Telefonu</b></p></div><div class='col-xs-6'><p>123456</p></div></div><div class='row'><div class='col-xs-6'><p><b>İş Telefonu</b></p></div><div class='col-xs-6'><p>7894564</p></div></div><div class='row'><div class='col-xs-6'><p><b>Mobil</b></p></div><div class='col-xs-6'><p>0519425645</p></div></div><a href='#''>Məlumatlarda düzəliş et</a>");
            }

        })

	$("#noMessage").change(
        function () {
            if (this.checked) {
                document.getElementById("notShowPhone").disabled = true;
                $("#disabled").css("color", "#C2B3B9");
                $("#elaqe").html("<div class='row'><div class='col-xs-6'><p><b>Adı Soyadı</b></p></div><div class='col-xs-6'><p>Əhmədov Məmməd</p></div></div><div class='row'><div class='col-xs-6'><p><b>Ev Telefonu</b></p></div><div class='col-xs-6'><p>123456</p></div></div><div class='row'><div class='col-xs-6'><p><b>İş Telefonu</b></p></div><div class='col-xs-6'><p>7894564</p></div></div><div class='row'><div class='col-xs-6'><p><b>Mobil</b></p></div><div class='col-xs-6'><p>0519425645</p></div></div><a href='#''>Məlumatlarda düzəliş et</a>");
            }
            else {
                document.getElementById("notShowPhone").disabled = false;
                $("#disabled").css("color", "black");
            }
        })

    /*******************************************************************/

    /******************/

    /*addreview height arranged*/

	/*$("#addReview").css("height", $(window).height());*/

    /*************/

	function GoToHomePage() {
	    window.location = '/';
	}
});