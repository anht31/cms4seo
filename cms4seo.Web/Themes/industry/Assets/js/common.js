$(".LinkToBottom")
    .click(function () {
        $(".p-offcanvas").removeClass("active-left");
        $(".p-offcanvas").removeClass("active-right");
        $("html, body").animate({ scrollTop: $(document).height() }, "slow");
        return false;
    });


/* aside - flipclock ======================================================== */


$(document)
    .ready(function () {

        var counter = $('.clock:first').data("value");

        // Instantiate a counter
        clock = new FlipClock($('.clock'),
            counter,
            {
                clockFace: 'Counter',
                minimumDigits: 7
            });
    });

/* index - hitcounter ======================================================== */

//$(function () {

//    // Declare a proxy to reference the hub.
//    var counterHub = $.connection.counterHub;

//    // register online user at the very begining of the page
//    // Start the connection.
//    $.connection.hub.start()
//        .done(function () {

//            //affter connected to hub

//            // Call the Send method on the hub.
//            //console.log("counterHub.server.send()");
//        });

//    // Create a function that the hub can call to recalculate online users.
//    counterHub.client.updatecounter = function (message) {

//        var values = message.toString().split("|");

//        // online
//        // Add the message to the page.
//        $("#counter").text(values[0]);

//        // Or set it to a specific value
//        clock.setValue(values[1]);
//        //$('#session-counter').data("value", values[1]);
//    };
//});


/* Accordion with plus/minus icon ============================================ */

//$(document).ready(function () {

//    var accordionId = $(".toggle-accordion").attr("accordion-id");

//    $(".toggle-accordion").on("click", function () {

//        numPanelOpen = $(accordionId + ' .collapse.in').length;

//        $(this).toggleClass("active");

//        //if (numPanelOpen == 0) {


//        if (numPanelOpen == 0) {
//            openAllPanels(accordionId);
//        } else {
//            closeAllPanels(accordionId);
//        }
//    });

//    openAllPanels = function (aId) {
//        console.log("setAllPanelOpen");
//        $(aId + ' .panel-collapse:not(".in")').collapse('show');
//    };
//    closeAllPanels = function (aId) {
//        console.log("setAllPanelclose");
//        $(aId + ' .panel-collapse.in').collapse('hide');
//    };


//    if ($(window).width() > 992) {
//        openAllPanels(accordionId);
//        $(".toggle-accordion").addClass('active');
//    } else {
//        closeAllPanels(accordionId);
//        $(".toggle-accordion").removeClass('active');
//    }

//});



/* Brand ======================================================== */

if ($(".sectionBrandSlick").length) {
    /*FOUND*/


    $('.sectionBrandSlick')
        .slick({
            dots: false,
            infinite: true,
            autoplay: true,
            autoplaySpeed: 2000,
            arrows: false,
            speed: 2000,
            slidesToShow: 8,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 6,
                        slidesToScroll: 3,
                        infinite: true
                        //dots: true
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 4,
                        slidesToScroll: 3
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 2
                    }
                }
            ]
        });

}


/* product - system -> width > 768 will be run ==================================== */


//$(document).ready(function () {

//    if ($(window).width() > 768) {

//        /* product - system ======================================================== */


//        $('.responsive').slick({
//            dots: false,
//            rtl: false,
//            //dots: false,
//            infinite: true,
//            autoplay: true,
//            autoplaySpeed: 5000,
//            speed: 100,
//            slidesToShow: 4,
//            slidesToScroll: 3,
//            //prevArrow: "<img class='a-left control-c prev slick-prev' src='~/Content/slick/prev.png'>",
//            //nextArrow: "<img class='a-right control-c next slick-next' src='~/Content/slick/next.png'>",
//            responsive: [
//                //{
//                //    breakpoint: 1400,
//                //    settings: {
//                //        slidesToShow: 5,
//                //        slidesToScroll: 5,
//                //        infinite: true,
//                //        dots: true
//                //    }
//                //},
//              {
//                  breakpoint: 1024,
//                  settings: {
//                      slidesToShow: 4,
//                      slidesToScroll: 4,
//                      infinite: true,
//                      dots: true
//                  }
//              },
//              {
//                  breakpoint: 768,
//                  settings: {
//                      slidesToShow: 3,
//                      slidesToScroll: 3
//                  }
//              },
//              {
//                  breakpoint: 480,
//                  settings: {
//                      slidesToShow: 2,
//                      slidesToScroll: 2
//                  }
//              }
//            ]
//        });



//        /* product - slick rtl ======================================================== */

//        $('.responsive-rtl').slick({
//            dots: false,
//            rtl: true,
//            //dots: false,
//            infinite: true,
//            autoplay: true,
//            autoplaySpeed: 5000,
//            speed: 100,
//            slidesToShow: 4,
//            slidesToScroll: 3,
//            responsive: [
//            //{
//            //    breakpoint: 1400,
//            //    settings: {
//            //        slidesToShow: 5,
//            //        slidesToScroll: 5,
//            //        infinite: true,
//            //        dots: true
//            //    }
//            //},
//              {
//                  breakpoint: 1024,
//                  settings: {
//                      slidesToShow: 4,
//                      slidesToScroll: 4,
//                      infinite: true,
//                      dots: true
//                  }
//              },
//              {
//                  breakpoint: 768,
//                  settings: {
//                      slidesToShow: 3,
//                      slidesToScroll: 3
//                  }
//              },
//              {
//                  breakpoint: 480,
//                  settings: {
//                      slidesToShow: 2,
//                      slidesToScroll: 2
//                  }
//              }
//            ]
//        });



//    }
//});





// Dropdown clickable ===========================================================
// Bootstrap 4 - Keeping Parent of Dropdown a clickable link


jQuery(function($) {
    if ($(window).width() > 769) {
        $('.navbar .dropdown').hover(function() {
            $(this).find('.dropdown-menu').first().stop(true, true).delay(250).slideDown();

        }, function() {
            $(this).find('.dropdown-menu').first().stop(true, true).delay(100).slideUp();

        });

        $('.navbar .dropdown > a').click(function() {
            location.href = this.href;
        });

    }
});