

$(function () {

    $("#contactForm input,#contactForm textarea").jqBootstrapValidation({
        preventSubmit: true,
        submitError: function ($form, event, errors) {
            // additional error messages or events
        },
        submitSuccess: function ($form, event) {
            event.preventDefault(); // prevent default submit behaviour

            // temporate success in waiting response from server
            $('#success').html("<div class='alert alert-success'>");
            $('#success > .alert-success')
                .html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
                .append("</button>");
            $('#success > .alert-success')
                .append("<strong>" + window.Js_ThankForYourMessage + "</strong>");
            $('#success > .alert-success')
                .append('</div>');

            $('#contactForm button.btn')[0].disabled = true;


            // get values from FORM
            var name = $("input#name").val();
            var email = $("input#email").val();
            var phone = $("input#phone").val();
            var message = $("textarea#message").val();
            var firstName = name; // For Success/Failure Message
            // Check for white space in name for Success/Fail message
            if (firstName.indexOf(' ') >= 0) {
                firstName = name.split(' ').slice(0, -1).join(' ');
            }
            $.ajax({
                url: "/Contact/ContactForm",
                type: "POST",
                data: {
                    FullName: name,
                    Phone: phone,
                    Email: email,
                    Message: message
                },
                cache: false,
                success: function (response) {

                    if (response != null && response.success) {
                        //alert(response.responseText);

                        // Success message =============================================================================================
                        if ($('#success').find('.alert').length === '0') {

                            $('#success').html("<div class='alert alert-success'>");
                            $('#success > .alert-success')
                                .html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
                                .append("</button>");
                            $('#success > .alert-success')
                                .append("<strong>" + window.Js_ThankForYourMessage + "</strong>");
                            $('#success > .alert-success')
                                .append('</div>');
                        }

                        $('#contactForm button.btn')[0].disabled = false;

                        //clear all fields
                        $('#contactForm').trigger("reset");

                    } else {
                        // DoSomethingElse()
                        //alert(response.responseText);
                        if ($('#success').find('.alert').length !== '0')
                            $('#success').empty();

                        // Fail message
                        $('#success').html("<div class='alert alert-danger'>");
                        $('#success > .alert-danger').html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
                            .append("</button>");
                        $('#success > .alert-danger').append("<strong>" + response.responseText + "</strong>");
                        $('#success > .alert-danger').append('</div>');
                        //clear all fields
                        $('#contactForm').trigger("reset");

                        $('#contactForm button.btn')[0].disabled = false;
                    }

                },
                error: function () {
                    // If the server sends some status code different than 200, the error callback is executed:
                    // Fail message
                    $('#success').html("<div class='alert alert-danger'>");
                    $('#success > .alert-danger').html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
                        .append("</button>");
                    $('#success > .alert-danger').append("<strong>" + window.Js_ServerBusyPleaseTryLater + "</strong>");
                    $('#success > .alert-danger').append('</div>');
                    //clear all fields
                    $('#contactForm').trigger("reset");

                    $('#contactForm button.btn')[0].disabled = false;
                },
            })
        },
        filter: function () {
            return $(this).is(":visible");
        },
    });

    $("a[data-toggle=\"tab\"]").click(function (e) {
        e.preventDefault();
        $(this).tab("show");
    });
});


/*When clicking on Full hide fail/success boxes */
$('#name').focus(function () {
    $('#success').html('');
});
