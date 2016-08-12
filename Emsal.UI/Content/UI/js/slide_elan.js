$(document).ready(function() {
 
 var owl = $("#owl-demo");
 
  owl.owlCarousel({
      items : 1, //10 items above 1000px browser width
      itemsDesktop : [1200,4], //5 items between 1000px and 901px
      itemsDesktopSmall : [900,2], // betweem 900px and 601px
      itemsTablet: [400,1], //1 items between 600 and 0
      itemsMobile : false // itemsMobile disabled - inherit from itemsTablet option
  });
	owl.trigger('owl.play', 3000);
	$(".owl-pagination").css("display", "none");
	$(".owl-wrapper").css("top", "1px");
  // Custom Navigation Events
 /*  $(".next").click(function(){
    owl.trigger('owl.next');
  })
  $(".prev").click(function(){
    owl.trigger('owl.prev');
  })
  $(".play").click(function(){
    owl.trigger('owl.play',2000); //owl.play event accept autoPlay speed as second parameter
  })
  $(".stop").click(function(){
    owl.trigger('owl.stop');
  }) */
 
    
	 $("[data-toggle=dropdown]").click(function() {
		$(".dropdown-menu").slideToggle(1000);
		//$(".categoria_in1").css("margin-top", "-100px");
	}); 
	




  $('#inlineRadio1').click(function() {
        $('#exampleInputText4').attr("disabled", true);
		$('.form-group select').attr("disabled", true);
		$('#exampleInputText1').attr("disabled", false);  
		$('#exampleInputText2').attr("disabled", false); 
		$('#exampleInputText3').attr("disabled", false); 		
    });
	
	$('#inlineRadio2').click(function() {
        $('#exampleInputText1').attr("disabled", true);  
		$('#exampleInputText2').attr("disabled", true); 
		$('#exampleInputText3').attr("disabled", true);  
		$('#exampleInputText4').attr("disabled", false);
		$('.form-group select').attr("disabled", false);
    });

	
	var count1 = -1;
					
	$("#more").click(function(){
		count1++;
		if(count1 % 2 == 0) {
			$(".more_seach").slideDown();
			
		} else {
			$(".more_seach").slideUp();
		}
	});
			
			

	
	$(".categoria_left ul li").mouseover(function(){
      
		$(".categoria_block").css("display", "block");
		$(".categoria_in2").css("display", "none");
    });
	$(".categoria_block").mouseover(function(){
      
		$(".categoria_block").css("display", "block");
		$(".categoria_in2").css("display", "none");
    });
	
	$(".categoria_block").mouseout(function(){
      
		$(".categoria_block").css("display", "none");
		$(".categoria_in2").css("display", "block");
    });
    $(".categoria_left ul li").mouseout(function(){
      
		$(".categoria_block").css("display", "none");
		$(".categoria_in2").css("display", "block");
    });
 

	/* $(window).resize(function(){
		if($(window).width() < 1000) {
		 $(".left-frame_in").hide();
		 }
		
		 else {
		 $(".left-frame_in").show();
		 }
	}); */
	
	
	$(window).resize(function() {
		if (window.innerWidth < 900) {

		$('.right_below:parent').each(function () {
			$(this).insertBefore($(this).prev('.left_above'));
		});
		
		$('.right_below:parent').each(function () {
			$(this).insertBefore($(this).prev('.left_above'));
		});

		} else if (window.innerWidth > 900) {

			// Change back to original .LatestNews

		}
	}).resize();
	
	
});