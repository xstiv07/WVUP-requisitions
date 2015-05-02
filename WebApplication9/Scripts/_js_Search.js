$(document).ready(function () {
    $('form').submit(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var resultsDiv = $('#results');
        var errorMsg = 'Something failed, please try again';

        $.ajax({
            url: this.action,
            type: "GET",
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
            data: $(this).serialize(),
            success: function (data) {
                resultsDiv.html(data);
                $("#search-form").slideToggle('slow');
            },
            complete: function () {
                $('.spinner').fadeOut('slow');
            }
        })
    })
});