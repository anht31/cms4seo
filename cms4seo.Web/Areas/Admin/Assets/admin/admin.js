/* Project  ========================================================

Project:	admin script
Version:	0.0.1
Last change:  22 apr 2016

==================================================================== */



/* SetCulture =============================================================*/

function SetCulture(culture) {
    setCookie("_culture", culture, 365);

    // reload page
    location.reload();
}


/* admin - product - edit =============================================================*/




$(function () {
    function split(val) {
        return val.split(/,\s*/);
    }

    function extractLast(term) {
        return split(term).pop();
    }


    // ArticleTags =======================================================================

    var availableArticleTags;

    $("#SelectedArticleTag")
        // don't navigate away from the field on tab when selecting an item
        .bind("keydown",
            function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
        .autocomplete({
            minLength: 0,
            source: function (request, response) {
                if (availableArticleTags) {
                    response($.ui.autocomplete.filter(availableArticleTags, extractLast(request.term)));
                    return;
                }

                //console.log($('#Tags:first').data("value"));

                $.getJSON("/admin/" + $("#SelectedArticleTag:first").data("value") + "/",
                    request,
                    function (data) {
                        availableArticleTags = data;
                        response($.ui.autocomplete.filter(availableArticleTags, extractLast(request.term)));
                    });
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ").slice(0, -2);
                return false;
            }
        });


    // ProductTags =======================================================================

    var availableProductTags;

    $("#SelectedProductTag")
        // don't navigate away from the field on tab when selecting an item
        .bind("keydown",
            function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
        .autocomplete({
            minLength: 0,
            source: function (request, response) {
                if (availableProductTags) {
                    response($.ui.autocomplete.filter(availableProductTags, extractLast(request.term)));
                    return;
                }

                //console.log($('#Tags:first').data("value"));

                $.getJSON("/admin/" + $("#SelectedProductTag:first").data("value") + "/",
                    request,
                    function (data) {
                        availableProductTags = data;
                        response($.ui.autocomplete.filter(availableProductTags, extractLast(request.term)));
                    });
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ").slice(0, -2);
                return false;
            }
        });



    // Tags =======================================================================

    var availableTags;

    $("#Tags")
        // don't navigate away from the field on tab when selecting an item
        .bind("keydown",
            function (event) {
                if (event.keyCode === $.ui.keyCode.TAB &&
                    $(this).autocomplete("instance").menu.active) {
                    event.preventDefault();
                }
            })
        .autocomplete({
            minLength: 0,
            source: function (request, response) {
                if (availableTags) {
                    response($.ui.autocomplete.filter(availableTags, extractLast(request.term)));
                    return;
                }

                //console.log($('#Tags:first').data("value"));

                $.getJSON("/admin/" + $("#Tags:first").data("value") + "/",
                    request,
                    function (data) {
                        availableTags = data;
                        response($.ui.autocomplete.filter(availableTags, extractLast(request.term)));
                    });
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ").slice(0, -2);
                return false;
            }
        });


    $(".datepicker").datepicker();


    // bypass-error-validation-date-in-vn =============================================================
    //jQuery.validator.methods.date = function (value, element) {
    //    var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
    //    if (isChrome) {
    //        var d = new Date();
    //        return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
    //    } else {
    //        return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
    //    }
    //};


});

/* product - del =============================================================*/

$("a[data-action=delete]")
    .click(function (e) {

        e.preventDefault();

        var value = this.getAttribute("data-value");
        var id = this.getAttribute("data-id");

        var href = this.href;


        bootbox.confirm("Are you sure you want to delete " + value + "?",
            function (result) {
                if (result) {

                    // prevent duplication click.            
                    $("a[data-action=delete]#" + id).attr("disabled", true);

                    var token = $("input[name=__RequestVerificationToken]").val();

                    $.post(href,
                        {
                            __RequestVerificationToken: token
                        },
                        function (response) {
                            location.reload();
                            //$('#admin-body').html(response);
                            //$('#page-body').load(reloadHref);
                        });
                }
                
            });


    });


/* search optimize =============================================================*/

$("a[data-action=confirm]")
    .click(function (e) {

        e.preventDefault();


        var value = this.getAttribute("data-value");
        var id = this.getAttribute("data-id");

        var href = this.href;

        if (this.getAttribute("disabled") === "disabled")
            return;


        bootbox.confirm("Are you sure you want to " + value + "?",
            function (result) {
                if (result) {

                    // prevent duplication click.            
                    $("a[data-action=confirm]#" + id).attr("disabled", true);

                    

                    location = href;

                    //var token = $("input[name=__RequestVerificationToken]").val();
                    //$.post(href,
                    //    {
                    //        __RequestVerificationToken: token
                    //    },
                    //    function (response) {
                    //        //location.reload();
                    //        //$('#admin-body').html(response);
                    //        //$('#page-body').load(reloadHref);
                    //    });


                    $("a[data-action=confirm]#" + id).attr("disabled", false);
                }
                
            });


        

    });


/* index - hit-counter ======================================================== */

//$(function () {

//    // Declare a proxy to reference the hub.
//    var counterHub = $.connection.counterHub;

//    // register online user at the very beginning of the page
//    // Start the connection.
//    $.connection.hub.start()
//        .done(function () {

//            //affter connected to hub

//            // Call the Send method on the hub.
//            //console.log("counterHub.server.send()");
//        });

//    // Create a function that the hub can call to recalculate online users.
//    counterHub.client.updateCounter = function (message) {

//        // Add the message to the page.
//        $("#counter").text(message);
//    };
//});

// signalR test ================================================================

$(window)
    .bind("storage",
        function (e) {
            var sharedData = localStorage.getItem("sharedKey");
            if (sharedData !== null)
                console.log(
                    'A tab called localStorage.setItem("sharedData",' + sharedData + ")"
                );
        });




// message setup =========================================================

function LobiboxMessage(type, message) {
    Lobibox.notify(type, {
        size: 'mini',
        sound: false,
        //soundPath: '/Areas/Admin/Assets/lobibox/sounds/',
        delayIndicator: false,
        msg: message,
        iconSource: 'fontAwesome',
        position: "bottom left"
    });
}

// modal upload -> clear all child ===========================================

var RemoveChilden = function (id) {
    var myNode = document.getElementById(id);
    while (myNode.firstChild) {
        myNode.removeChild(myNode.firstChild);
    }
}

// admin -> table ===========================================
$(document)
.ready(function () {
    $('#dataTables-adv')
        .DataTable({
            responsive: true,
            "pageLength": 100,

            //"language": {
            //    "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Vietnamese.json"
            //}

            "language": {
                "url": "/Areas/Admin/Assets/dataTables/i18n/" 
                    + window.dataTableLanguage
                    + ".json"
            }

        });

    //console.log(resources.js_Hello);
});



// helper -> contains =========================================================

//String.prototype.contains = function (it) { return this.indexOf(it) != -1; };


/* admin - numberSeparator ======================================================== */

$(document)
    .ready(function () {
        $('input.numberPrice')
            .keyup(function (event) {
                // skip for arrow keys
                if (event.which >= 37 && event.which <= 40) {
                    event.preventDefault();
                }
                var $this = $(this);

                var strNum;
                // check culture is English
                if (document.documentElement.lang.indexOf("en") !== -1) {
                    strNum = $this.val().replace(/\,/g, "");
                } else {
                    strNum = $this.val().replace(/\./g, "");
                }
                
                if (strNum !== "" && strNum !== 'NaN') {
                    var intResult = parseInt(strNum);
                    var result = intResult.toLocaleString(document.documentElement.lang);
                    $this.val(result);
                } else {
                    $this.val("");
                }
            });

    });


/* admin - counter description character 155 ======================================================== */


$('.counterCharacter').keyup(updateCountCharacter);
$('.counterCharacter').keydown(updateCountCharacter);

function updateCountCharacter() {
    var cs = 155 - $(this).val().length;
    if (cs >= 0) {
        $('.remainCharacters').text(resources.js_Remaining + cs);
    }
    else {
        $('.remainCharacters').text(resources.js_Exceeded + -cs);
    }
}

$(document).ready(function () {
    $(".counterCharacter").change(updateCountCharacter).change();
    //$.proxy(updateCount, $('.counterCharacter'))();
});


/* admin - counter meta title ======================================================== */

$('.counterMetaTitle').keyup(updateCountMetaTitle);
$('.counterMetaTitle').keydown(updateCountMetaTitle);

function updateCountMetaTitle() {
    var cs = 62 - $(this).val().length;
    if (cs >= 0) {
        $('.remainMetaTitle').text(resources.js_Remaining + cs);
    }
    else {
        $('.remainMetaTitle').text(resources.js_Exceeded + -cs);
    }
}

$(document).ready(function () {
    $(".counterMetaTitle").change(updateCountMetaTitle).change();
    //$.proxy(updateCount, $('.counterMetaTitle'))();
});


/* admin - counter meta description ======================================================== */

$('.counterMetaDescription').keyup(updateMetaDescription);
$('.counterMetaDescription').keydown(updateMetaDescription);

function updateMetaDescription() {
    var cs = 155 - $(this).val().length;
    if (cs > 0) {
        $('.remainMetaDescription').text(resources.js_Remaining + cs);
    }
    else {
        $('.remainMetaDescription').text(resources.js_Exceeded + -cs);
    }
}

$(document).ready(function () {
    $(".counterMetaDescription").change(updateMetaDescription).change();
    //$.proxy(updateCount, $('.counterMetaDescription'))();
});



/* admin - counter meta keyword ======================================================== */

$('.counterMetaKeyword').keyup(updateMetaKeyword);
$('.counterMetaKeyword').keydown(updateMetaKeyword);

function updateMetaKeyword() {
    var cs = 155 - $(this).val().length;
    if (cs > 0) {
        $('.remainMetaKeyword').text(resources.js_Remaining + cs);
    }
    else {
        $('.remainMetaKeyword').text(resources.js_Exceeded + -cs);
    }
}

$(document).ready(function () {
    $(".counterMetaKeyword").change(updateMetaKeyword).change();
    //$.proxy(updateCount, $('.counterMetaKeyword'))();
});



/* admin - aside photo ======================================================== */

$(window).on('scroll', function (event) {
    var scrollValue = $(window).scrollTop();
    if (scrollValue > 70) {
        $('.aside-upload-photo').addClass('fixed-top-aside-right');
    } else {
        $('.aside-upload-photo').removeClass('fixed-top-aside-right');
    }
});


/* Mobile Management ======================================================== */

// trigger event of input type file in dmUploader
$("#uploadMobile").click(function() {
    $('#drag-and-drop-zone input').trigger('click');
});



/* ==============================================
Plugin Active
=============================================== */

window.addEventListener('load',function(){
    jQuery(".waiting-reload-plugins").click(function(){
        console.log("button press");
        $('.se-pre-con').addClass('active');
    });
});

var cPlugin = (function ($) {

    function toggle(item, area, zone) {
        //if ($(item).is(':checked')) {
        //    console.log("Active");
        //} else {
        //    console.log("Deactivate");
        //}

        var active = $(item).is(':checked');

        var params = "?active=" + active + "&area=" + area + "&zone=" + zone;

        window.location = "/Admin/Plugins/Toggle/" + params;
    }

    return {
        Toggle: toggle
    }

})(jQuery);

/* ==============================================
Plugin Active
=============================================== */