$(document).ready(function() {

	$("#per_inf").click(function(){
		$(".per_information").toggle();
		return false;
	});
	
	
	$("#linkToSpecialSummaryFromProfile").on("click", function () {
	    window.location.href = "/SpecialSummary/Index";
	    return false;
	})
	$("#y_tesdiq").click(function(){
		$(".yayinda_olan").css("display", "none");
		$(".yayinda_olmayan").css("display", "none");
		$(".yayimdan_cixan").css("display", "none");
		$(".yayinda_tesdiq").css("display", "block");
	});
	
	$("#y_olan").click(function(){
		$(".yayinda_tesdiq").css("display", "none");
		$(".yayinda_olmayan").css("display", "none");
		$(".yayimdan_cixan").css("display", "none");
		$(".yayinda_olan").css("display", "block");
	});
	
	$("#y_olmayan").click(function(){
		$(".yayinda_tesdiq").css("display", "none");
		$(".yayinda_olan").css("display", "none");
		$(".yayimdan_cixan").css("display", "none");
		$(".yayinda_olmayan").css("display", "block");
	});
	
	$("#y_cixan").click(function(){
		$(".yayinda_tesdiq").css("display", "none");
		$(".yayinda_olan").css("display", "none");
		$(".yayinda_olmayan").css("display", "none");
		$(".yayimdan_cixan").css("display", "block");
	});
	
	
	
	
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
	
	
	


var sampleJson1 = {
ToolTipPosition: "bottom",
data: [{ order: 1, Text: "Foo", ToolTipText: "Step1-Foo", highlighted: true },
    { order: 2, Text: "Bar", ToolTipText: "Step2-Bar", highlighted: true },
    { order: 3, Text: "Baz", ToolTipText: "Step3-Baz", highlighted: false },
    { order: 4, Text: "Quux", ToolTipText: "Step4-Quux", highlighted: false }]
};    

//Invoking the plugin
$(document).ready(function () {
        //$("#tracker1").progressTracker(sampleJson1);
    });
	
	
	
	
	
});