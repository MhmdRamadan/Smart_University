(function ($) {
	"use strict";

	jQuery(document).ready(function ($) {

		/* 
		=================================================================
		01 - Main Slider JS
		=================================================================	
		*/

		$(".droome-slide").owlCarousel({
			animateOut: 'fadeOutLeft',
			animateIn: 'fadeIn',
			items: 1,
			nav: true,
			dots: false,
			autoplayTimeout: 7000,
			autoplaySpeed: 2000,
			autoplay: true,
			loop: true,
			navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"],
			mouseDrag: true,
			touchDrag: true,
		});

		/* 
		=================================================================
		02 - Special Slider JS
		=================================================================	
		*/

		$(".special-slider").owlCarousel({
			animateOut: 'fadeOutLeft',
			animateIn: 'fadeIn',
			items: 1,
			nav: false,
			dots: true,
			autoplayTimeout: 7000,
			autoplaySpeed: 5000,
			autoplay: true,
			loop: true,
			mouseDrag: true,
			touchDrag: true,
		});


		/* 
		=================================================================
		03 - Team Slider JS
		=================================================================	
		*/

		$(".team-slider").owlCarousel({
			nav: false,
			dots: true,
			autoplay: true,
			margin: 30,
			loop: true,
			mouseDrag: true,
			touchDrag: true,
			responsive: {
				0: {
					items: 1
				},
				480: {
					items: 1
				},
				600: {
					items: 2
				},
				1000: {
					items: 4
				},
				1200: {
					items: 4
				}
			}
		});

		/* 
		=================================================================
		04 - Food Menu Slider JS
		=================================================================	
		*/

		$(".food-menu-slider").owlCarousel({
			nav: false,
			dots: true,
			autoplay: true,
			margin: 20,
			loop: true,
			mouseDrag: true,
			touchDrag: true,
			responsive: {
				0: {
					items: 1
				},
				480: {
					items: 1
				},
				600: {
					items: 1
				},
                680: {
					items: 2
				},
				1000: {
					items: 2
				},
				1200: {
					items: 1
				}
			}
		});

		/* 
		=================================================================
		05 - Food Menu Turn JS
		=================================================================	
		*/
		$('.pages').turn({
			duration: 1500,
			width: 638,
			height: 455,
			//  acceleration: true,
			//  display: 'single',
			// autoCenter: true,
			turnCorners: "bl,br",
			elevation: 300,
			when: {
				turned: function (e, page) {
					console.log('Current view: ', $(this).turn('view'));
				}
			}
		});


	});


}(jQuery));

