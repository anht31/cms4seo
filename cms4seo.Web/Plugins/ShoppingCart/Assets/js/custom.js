/*
 * Custom Script for Plugin
 * Name: ShoppingCart
 *
 */



// use animate.css by script =======================================================

function animateCSS(element, animationName, callback) {
    var node = document.querySelector(element);
    node.classList.add('animated', animationName);

    function handleAnimationEnd() {
        node.classList.remove('animated', animationName);
        node.removeEventListener('animationend', handleAnimationEnd);

        if (typeof callback === 'function') callback();
    }

    node.addEventListener('animationend', handleAnimationEnd);
}



// shoping cart =======================================================

function AddToMyCart(productId) {

    var quality = $('#numberOfProduct-' + productId).val();
    var result;

    $.getJSON("/Plugins/Cart/AddToCart?productId=" + productId + "&quality=" + quality
        , function (data) {
            console.log("first success");
            result = data;
            $('div#cart-status').text(data.qualityTotal);
        })
    .done(function () {
        animateCSS("#shoping-cart", "bounce");
        console.log("second success");
    })
    .fail(function () {
        console.log("error");
    })
    .always(function () {
        console.log("complete");
    });


}



function PlusMyCart(productId) {

    //var quality = $('#numberOfProduct-' + productId).val();

    $.getJSON("/Plugins/Cart/UpdateCart?productId=" + productId + "&quality=" + "1"
        , function (data) {
            //console.log("first success");

            //$('label#line-quality-' + productId).text(data.lineQuality);

            $('div#cart-status').text(data.qualityTotal);

            // currentCulture set by selectGlobalize.js (in html attr)
            $('#table-cart-detail').load("/Plugins/Cart/TableCartDetail");
        })
        .done(function () {
            animateCSS("#shoping-cart", "bounce");
        })
        .fail(function () {
            console.log("error");
        })
        .always(function () {
            console.log("complete");
        });


}


function MinusMyCart(productId) {

    var quality = $('label#line-quality-' + productId).text();

    // delete row
    if (quality === "1") {
        document.getElementById('button-minus-' + productId).disabled = true;
        //$('#row-' + productId).fadeOut('slow');
    }

    $.getJSON("/Plugins/Cart/UpdateCart?productId=" + productId + "&quality=" + "-1"
        , function (data) {
            //console.log("first success");

            //$('label#line-quality-' + productId).text(data.lineQuality);

            $('div#cart-status').text(data.qualityTotal);

            $('#table-cart-detail').load("/Plugins/Cart/TableCartDetail");
        })
        .done(function () {
            animateCSS("#shoping-cart", "bounce");
        })
        .fail(function () {
            console.log("error");
        })
        .always(function () {
            console.log("complete");
        });


}


function RemoveMyCart(productId) {



    $.getJSON("/Plugins/Cart/RemoveFromCart?productId=" + productId
        , function (data) {
            //console.log("first success");


            $('div#cart-status').text(data.qualityTotal);

            $('#table-cart-detail').load("/Plugins/Cart/TableCartDetail");
        })
        .done(function () {
            animateCSS("#shoping-cart", "bounce");
        })
        .fail(function () {
            console.log("error");
        })
        .always(function () {
            console.log("complete");
        });


}

function Submit(productId) {


}



//checkoutform ================

var validationMessage = "";
if (document.documentElement.lang == "vi") {
    validationMessage = "Vui lòng điền vào trường này.";
    validationEmailMessage = "Vui lòng điền vào trường này. (kiểm tra xem có thiếu \u0040 hay không).";

} else {
    validationMessage = "Please enter this field.";
    validationEmailMessage = "Please enter this field (check if there is a missing \u0040).";
}

function SubmitCheckOut(element) {

    if ($('#checkoutForm')[0].checkValidity() === false) {

        $('#checkoutForm input[type=text]')
            .on('change invalid',
                function () {
                    var textfield = $(this).get(0);

                    // 'setCustomValidity not only sets the message, but also marks
                    // the field as invalid. In order to see whether the field really is
                    // invalid, we have to remove the message first
                    textfield.setCustomValidity('');

                    if (!textfield.validity.valid) {
                        textfield.setCustomValidity(validationMessage);
                    }
                });


        $('#checkoutForm input[type=email]')
            .on('change invalid',
                function () {
                    var textfield = $(this).get(0);

                    textfield.setCustomValidity('');

                    if (!textfield.validity.valid) {
                        textfield.setCustomValidity(validationEmailMessage);
                    }
                });


        return;
    }


    element.disabled = true;
    var loadingText = '<i class="fa fa-circle-o-notch fa-spin"></i> loading...';
    $(element).html(loadingText);
    document.getElementById("checkoutForm").submit();
}
