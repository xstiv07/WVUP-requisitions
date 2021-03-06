﻿var errorMsg = 'Something failed, please try again';
$(document).ready(function () {
    $('#quick-view').click(function (e) {
        $.ajax({
            type: "GET",
            url: '/Account/Register',
            beforeSend: function () {
                $('.spinner').fadeIn('slow')
            },
            error: function (xhr, status, error) {
                $('.spinner').fadeOut('slow')
                $('.ajax-message').text(errorMsg);
                $('.ajax-message').css('backgroundColor', '#F1A9A0');
                $('.ajax-message').css('color', 'white');
                $('.ajax-message').fadeIn('slow').delay(5000).fadeOut('slow');
            },
            success: function (content) {
                $("#qv-pop-up").html(content);
            },
            complete: function () {
                $('.spinner').fadeOut('slow');
                $('#qv-pop-up').slideDown("slow");
            }
        });
    });
    $('#close').click(function () {
        console.log("here")
        $('#qv-pop-up').slideUp("slow")
    });

    $("#submitRegister").click(function () {
        if (!$("#register").valid()) {
            return false;
        }
    });
});